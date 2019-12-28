using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

	public float maxVelocity = 1000;
	public float forwardSpeed = 500;
	public float sideSpeed = 250;
	public float rotationSpeed = 3;
	public float brakePower = 25; // 0 - 100
	public float tiltAxis = 45;
    public float maxZoomLevel = 170F;
    public float minZoomLevel = 30F;

	private Rigidbody rb;
	private GameObject spaceShip;
	private GameObject thrusterPrimary;
	private ShipManeuveringThrusters thrusterScript;

	private float tiltAxisStep; // For TiltShip()

	private void Start()
	{
		rb = GetComponent<Rigidbody>();
		spaceShip = GameObject.Find("Space Ship Model");
		thrusterPrimary = GameObject.Find("Thruster Primary");
		thrusterScript = GetComponent<ShipManeuveringThrusters>();
	}

	void FixedUpdate()
	{
		float yAxis = Input.GetAxis("Vertical");
		float xAxis = Input.GetAxis("Horizontal");
		float xMouseAxis = Input.GetAxis("Mouse X");

        Zoom();

		//---------------------------------------------------------------------
		// Thrusting forwards or backwards depending on the vertical or 
		// horizontal axis (WS on the keyboard):
		ThrustForward(yAxis);
		ThrustSide(xAxis);

		//---------------------------------------------------------------------
		// Rotate the ship when the right mouse button is held down using the 
		// mouse axis:
		if (Input.GetMouseButton(1))
		{
			Cursor.visible = false;
			Cursor.lockState = CursorLockMode.Locked;

			Rotate(transform, xMouseAxis * rotationSpeed);
		}
		else
		{
			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.None;
		}

		//---------------------------------------------------------------------
		// Brakes! Cause the ship to come to a stop on all directions, this 
		// helps make space movement more controllable.
		if (Input.GetButton("Brake"))
		{
			BrakeApply(brakePower);
			thrusterScript.ActivateWithVelocity(rb.velocity);
		}
		else
		{
			thrusterScript.Activate(false);
			BrakeRelease();
		}

		//---------------------------------------------------------------------
		// Don't go too fast on any axis:
		ClampVelocity();

		//---------------------------------------------------------------------
		// When strafing left and right (horizontal axis, WS on keyboard), tilt 
		// the space ship (even though this isn't realistic in space):
		if (xAxis != 0)
		{
			spaceShip.transform.Rotate(xAxis * (-tiltAxis) - spaceShip.transform.eulerAngles.x, 0, 0);
		}

	}

	private enum TiltDirection { None, Left, Right };
	private void TiltShip(TiltDirection direction)
	{
		if (direction == TiltDirection.None && tiltAxisStep != 0)
		{
			if (tiltAxisStep < 0) tiltAxisStep += 0.2f;
			if (tiltAxisStep > 0) tiltAxisStep -= 0.2f;
		}
		else if (direction == TiltDirection.Left && tiltAxisStep > -1.0f)
		{
			tiltAxisStep -= 0.1f;
			if (tiltAxisStep < -1.0f) tiltAxisStep = -1.0f;
		}
		else if (direction == TiltDirection.Right && tiltAxisStep < 1.0f)
		{
			tiltAxisStep += 0.1f;
			if (tiltAxisStep > 1.0f) tiltAxisStep = 1.0f;
		}

		spaceShip.transform.Rotate(tiltAxisStep * (-tiltAxis) - spaceShip.transform.eulerAngles.x, 0, 0);
	}

	private void ClampVelocity()
	{
		float x = Mathf.Clamp(rb.velocity.x, -maxVelocity, maxVelocity);
		float z = Mathf.Clamp(rb.velocity.z, -maxVelocity, maxVelocity);

		rb.velocity = new Vector3(x, 0, z);
	}

	private void BrakeApply(float rate)
	{
		float convertedRate = rate / 1000.0f;
		rb.drag = convertedRate;
	}

	private void BrakeRelease()
	{
		rb.drag = 0;

		//foreach (GameObject brakeThruster in brakeThrusters)
		//	brakeThruster.SetActive(false);
	}

	private void ThrustForward(float amount)
	{
		//Vector3 force = transform.forward * Time.fixedDeltaTime * forwardSpeed * amount;
		//rb.AddForce(force);

		rb.AddRelativeForce(Vector3.forward * Time.fixedDeltaTime * forwardSpeed * amount);

		if (amount > 0)
		{
			thrusterPrimary.GetComponent<Thruster>().thrusterLength = 20 + amount;
			thrusterPrimary.GetComponentInChildren<AudioSource>().mute = false;
		}
		else
		{
			thrusterPrimary.GetComponent<Thruster>().thrusterLength = 3.5f;
			thrusterPrimary.GetComponentInChildren<AudioSource>().mute = true;
		}
	}

	private void ThrustSide(float amount)
	{
		//Vector3 force = transform.right * Time.fixedDeltaTime * sideSpeed * amount;
		//rb.AddForce(force);

		rb.AddRelativeForce(Vector3.right * Time.fixedDeltaTime * forwardSpeed * amount);
	}

	private void Rotate(Transform t, float amount)
	{
		t.Rotate(0, amount, 0);
	}

    void Zoom()
    {
        var MainCamera = GameObject.FindGameObjectWithTag("MainCamera")?.GetComponent<Camera>() as Camera;
        if (!MainCamera) return;

        if (Input.GetAxis("Mouse ScrollWheel") > 0 && MainCamera.orthographicSize >= minZoomLevel)
        {
            for (int sensitivityOfScrolling = 3; sensitivityOfScrolling > 0; sensitivityOfScrolling--) MainCamera.orthographicSize--;
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0 && MainCamera.orthographicSize <= maxZoomLevel)
        {
            for (int sensitivityOfScrolling = 3; sensitivityOfScrolling > 0; sensitivityOfScrolling--) MainCamera.orthographicSize++;
        }
    }
}
