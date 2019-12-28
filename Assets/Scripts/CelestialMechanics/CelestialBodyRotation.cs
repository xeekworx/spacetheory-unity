using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CelestialBodyRotation : MonoBehaviour
{
    public GameObject RotatingObject => gameObject;

    [SerializeField]
    public float RotationalSpeed = 460F; // 23.93 for Earth-like speed (think hours in a day)

    [SerializeField]
    private const float SpeedConversionValue = 0.0010869565217391F; // Convert Earth-like speed to get a the desired result (trial and error)

    private void FixedUpdate()
    {
        if (RotatingObject != null)
        {
            float angle = RotationalSpeed * SpeedConversionValue;
            RotatingObject.transform.rotation = RotatingObject.transform.rotation * Quaternion.Euler(0, -(angle * Time.fixedDeltaTime), 0);
        }
    }
}
