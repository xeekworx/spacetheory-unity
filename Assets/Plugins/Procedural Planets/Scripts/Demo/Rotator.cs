using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Simple script that rotates a transform over time. Defaults to rotating around y-axis.
/// </summary>
public class Rotator : MonoBehaviour {
    public Vector3 eulerRotation = new Vector3(0f, 1f, 0f);

	void Update () {
        transform.Rotate(eulerRotation * Time.deltaTime);
	}
}
