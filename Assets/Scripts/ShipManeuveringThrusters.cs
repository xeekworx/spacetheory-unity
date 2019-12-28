using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipManeuveringThrusters : MonoBehaviour {

	public enum Thruster { FrontLeft, FrontRight, RearLeft, RearRight };

	private GameObject[] thrusters = new GameObject[Enum.GetNames(typeof(Thruster)).Length];
	private uint[] thrusterActivationCount = new uint[Enum.GetNames(typeof(Thruster)).Length];

	// Use this for initialization
	void Start() {
		thrusters[(int)Thruster.FrontLeft] = GameObject.Find("Thruster FL");
		thrusters[(int)Thruster.FrontRight] = GameObject.Find("Thruster FR");
		thrusters[(int)Thruster.RearLeft] = GameObject.Find("Thruster RL");
		thrusters[(int)Thruster.RearRight] = GameObject.Find("Thruster RR");
	}

	// Update is called once per frame
	void FixedUpdate() {

	}

	public void Activate(bool value)
	{
		foreach (GameObject t in thrusters)
		{
			if(t != null) t.SetActive(value);
		}
	}

	public void Activate(Thruster thruster, bool value, bool priority = false)
	{
		// Basically, it goes like this:
		// + If the thruster exists...
		// |-- If priority is given to the activation, do it now.
		// |-- + Else if no priority is given
		// |-----| Always activate if asked to and increment
		// |-----| Only deactivate if the activation count is zero

		var thrusterObject = thrusters[(int)thruster];
		if (thrusterObject != null)
		{
			if (priority)
			{
				thrusterObject.SetActive(value);
			}
			else
			{
				if (value)
				{
					thrusterActivationCount[(int)thruster]++;
					thrusterObject.SetActive(true);
				}
				else
				{
					if (thrusterActivationCount[(int)thruster] > 0)
					{
						thrusterActivationCount[(int)thruster]--;
					}
					else
					{
						thrusterObject.SetActive(false);
					}
				}
			}
		}
	}

	public void ActivateWithVelocity(Vector3 velocity)
	{
		float forwardSpeed = Vector3.Dot(velocity, transform.forward);
		float rightSpeed = Vector3.Dot(velocity, transform.right);

		bool movingForward = forwardSpeed > 10;
		bool movingBackward = forwardSpeed < -10;
		bool movingRight = rightSpeed > 10;
		bool movingLeft = rightSpeed < -10;

		float whenToApplyMaximumThrust = 150;

		bool[] thrusterControl = new bool[Enum.GetNames(typeof(Thruster)).Length];

		// LONGITUDINALLY CONTROL
		if (movingForward)
		{
			// Only activate based on lateral speed...
			if (movingRight && forwardSpeed < whenToApplyMaximumThrust) thrusterControl[(int)Thruster.FrontRight] = true;
			else if (movingLeft && forwardSpeed < whenToApplyMaximumThrust) thrusterControl[(int)Thruster.FrontLeft] = true;
			else // ...unless we're only going forward
			{
				thrusterControl[(int)Thruster.FrontRight] = true;
				thrusterControl[(int)Thruster.FrontLeft] = true;
			}
		}
		else if (movingBackward)
		{
			// Only activate based on lateral speed...
			if (movingRight && forwardSpeed > -whenToApplyMaximumThrust) thrusterControl[(int)Thruster.RearRight] = true;
			else if (movingLeft && forwardSpeed > -whenToApplyMaximumThrust) thrusterControl[(int)Thruster.RearLeft] = true;
			else // ...unless we're only going backward
			{
				thrusterControl[(int)Thruster.RearRight] = true;
				thrusterControl[(int)Thruster.RearLeft] = true;
			}
		}

		// LATERAL CONTROL
		if (movingRight)
		{
			// Only activate based on longitudinal speed...
			if (movingForward && rightSpeed < whenToApplyMaximumThrust) thrusterControl[(int)Thruster.FrontRight] = true;
			else if (movingBackward && rightSpeed < whenToApplyMaximumThrust) thrusterControl[(int)Thruster.RearRight] = true;
			else // ...unless we're only going right
			{
				thrusterControl[(int)Thruster.FrontRight] = true;
				thrusterControl[(int)Thruster.RearRight] = true;
			}
		}
		else if (movingLeft)
		{
			// Only activate based on longitudinal speed...
			if (movingForward && rightSpeed > -whenToApplyMaximumThrust) thrusterControl[(int)Thruster.FrontLeft] = true;
			else if (movingBackward && rightSpeed > -whenToApplyMaximumThrust) thrusterControl[(int)Thruster.RearLeft] = true;
			else // ...unless we're only going left
			{
				thrusterControl[(int)Thruster.FrontLeft] = true;
				thrusterControl[(int)Thruster.RearLeft] = true;
			}
		}

		// JANEWAY SAYS: "DO IT!"
		for (int i = 0; i < Enum.GetNames(typeof(Thruster)).Length; ++i)
		{
			Activate((Thruster)i, thrusterControl[i], true);
		}
	}
}
