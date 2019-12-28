using UnityEngine;

// ----------------------------------------------------------------------------
// Thanks to: Board To Bits Games
// Original Source: https://github.com/boardtobits/ellipse-orbit
// YouTube Episode: https://www.youtube.com/watch?v=mQKGRoV_jBc
// ----------------------------------------------------------------------------

[RequireComponent(typeof(LineRenderer))]
public class OrbitalEllipseRenderer : MonoBehaviour
{

	LineRenderer lr;

	[Range(6, 256)]
	public int segments = 6;
	public OrbitalPoint ellipse;

	void Awake()
	{
		lr = GetComponent<LineRenderer>();
		CalculateEllipse();
	}

	void CalculateEllipse()
	{
		Vector3[] points = new Vector3[segments + 1];
		for (int i = 0; i < segments; i++)
		{
			Vector3 position3D = ellipse.Evaluate(i / (float)segments);
			points[i] = new Vector3(position3D.x, position3D.y, position3D.z);
		}
		points[segments] = points[0];

		lr.positionCount = segments + 1;
		lr.SetPositions(points);
	}

	void OnValidate()
	{
		if (Application.isPlaying && lr != null)
			CalculateEllipse();
	}
}