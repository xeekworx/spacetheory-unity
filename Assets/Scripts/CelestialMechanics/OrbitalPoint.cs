using System;
using UnityEngine;

// ----------------------------------------------------------------------------
// Thanks to: Board To Bits Games
// Original Source: https://github.com/boardtobits/ellipse-orbit
// YouTube Episode: https://www.youtube.com/watch?v=Or3fA-UjnwU
// ----------------------------------------------------------------------------

[System.Serializable]
public class OrbitalPoint
{

	public float xAxis;
	public float yPlane;
	public float zAxis;
	public float xOffset;
	public float zOffset;

	private Vector3 Offset => new Vector3(xOffset, 0F, zOffset);

	public OrbitalPoint(float xAxis, float yPlane, float zAxis, Vector3 offset = new Vector3())
	{
		this.xAxis = xAxis;
		this.yPlane = yPlane;
		this.zAxis = zAxis;
		this.xOffset = offset.x;
		this.zOffset = offset.z;
	}

	public Vector3 Evaluate(float orbitPeriod)
	{
		float angle = Mathf.Deg2Rad * 360f * orbitPeriod;
		float x = Mathf.Sin(angle) * xAxis + Offset.x;
		float z = Mathf.Cos(angle) * zAxis + Offset.z;
		return new Vector3(x, yPlane, z);
	}
}