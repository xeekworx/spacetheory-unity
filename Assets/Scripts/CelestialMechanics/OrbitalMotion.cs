using System.Collections;
using UnityEngine;

// ----------------------------------------------------------------------------
// Thanks to: Board To Bits Games
// Original Source: https://github.com/boardtobits/ellipse-orbit
// YouTube Episode: https://www.youtube.com/watch?v=lKfqi52PqHk
// ----------------------------------------------------------------------------

[ExecuteInEditMode]
public class OrbitalMotion : MonoBehaviour
{
    public GameObject OrbitingObject => gameObject;
    public GameObject parent;
    public OrbitalPoint orbitPath;

    [Range(0.00F, 1.00F)]
    public float orbitProgress = 0f;

    //[Min(0.5F)] // Min is fixed in Unity 2020.1.0a7+
    [Range(0.5F, 1000.0F)]
    public float orbitPeriod = 3f;

    private bool orbitActive = true;

    private bool orbitCoRoutineRunning = false;

    private LineRenderer orbitLineRenderer;

    [Range(6, 256)]
    public int orbitPathSegments = 32;

    //[Range(0.001F, 0.001F)]
    public float gravitationalEffect = 0.00f;

    private Vector3 lastPosition = Vector3.zero; // For speed calculation
    public float speed = 0;

    private void FixedUpdate()
    {
        speed = (transform.position - lastPosition).magnitude;
        lastPosition = transform.position;
    }

    // Use this for initialization
    void Start()
    {
        // Initialize the line renderer if it exists:
        orbitLineRenderer = GetComponent<LineRenderer>();
        CalculateEllipseForLineRenderer();

        // If I don't have an orbiting body don't continue:
        if (OrbitingObject == null)
        {
            orbitActive = false;
            return;
        }

        // Set initial orbit position if possible:
        SetOrbitingObjectPosition();

        // If playing and not just stopped in the editor, run the animation:
        if (Application.isPlaying) StartCoroutine(AnimateOrbit());
    }

    void SetOrbitingObjectPosition()
    {
        if (OrbitingObject != null)
        {
            Vector3 orbitPos = orbitPath.Evaluate(orbitProgress);
            OrbitingObject.transform.position = orbitPos
                // Add the position to the parent's position if the parent exists:
                + (parent != null ? parent.transform.position : new Vector3());
        }
    }

    IEnumerator AnimateOrbit()
    {
        orbitCoRoutineRunning = true;

        float lastOrbitPeriod = orbitPeriod;
        if (orbitPeriod < 0.5f) orbitPeriod = 0.5f;
        float orbitSpeed = (1f / orbitPeriod);

        while (orbitActive)
        {
            float currentDistance = Vector3.Distance(OrbitingObject.transform.position, parent.transform.position) * gravitationalEffect;

            // Avoid calculating orbitSpeed every frame and only recalculate when 
            // orbitPeriod has changed:
            if (orbitPeriod != lastOrbitPeriod)
            {
                orbitSpeed = 1f / orbitPeriod;
                lastOrbitPeriod = orbitPeriod;
            }

            orbitProgress
                += Time.fixedDeltaTime * orbitSpeed
                * (gravitationalEffect > 0F ? orbitSpeed / currentDistance : orbitSpeed);
            orbitProgress %= 1f;
            SetOrbitingObjectPosition();
            CalculateEllipseForLineRenderer();

            yield return new WaitForFixedUpdate();
        }

        orbitCoRoutineRunning = false;
    }

    void CalculateEllipseForLineRenderer()
    {
        // This only works if there's a Line Renderer
        if (orbitLineRenderer != null)
        {
            Vector3[] points = new Vector3[orbitPathSegments + 1];
            for (int i = 0; i < orbitPathSegments; i++)
            {
                Vector3 position3D = orbitPath.Evaluate(i / (float)orbitPathSegments);
                points[i] = position3D
                    // Add the position to the parent's position if the parent exists:
                    + (parent != null ? parent.transform.position : new Vector3());
            }

            // Connect the last segment to the first:
            points[orbitPathSegments] = points[0];

            orbitLineRenderer.positionCount = orbitPathSegments + 1;
            orbitLineRenderer.SetPositions(points);
        }
    }

    void OnValidate()
    {
        if (!Application.isPlaying && orbitLineRenderer != null)
        {
            CalculateEllipseForLineRenderer();
        }

        // If we're playing (not stopped in the Editor) and orbitActive
        // is true, restart the animation coroutine:
        if (Application.isPlaying && orbitActive && !orbitCoRoutineRunning)
        {
            StartCoroutine(AnimateOrbit());
        }
        else
        {
            // Update the orbit position, such as when something changed in the editor:
            SetOrbitingObjectPosition();
        }
    }

}