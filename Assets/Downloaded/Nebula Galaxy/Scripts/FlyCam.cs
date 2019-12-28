using UnityEngine;
using System.Collections;

public class FlyCam : MonoBehaviour {

    public float Sensitivity;
    public float Speed;

    public bool YInvert;

    private void FixedUpdate()
    {

        float rotationY = Input.GetAxis("Mouse X") * Sensitivity;
        float rotationX = 0;
        if (YInvert)
        {
            rotationX = -Input.GetAxis("Mouse Y") * Sensitivity;
        }
        else
        {
            rotationX = Input.GetAxis("Mouse Y") * Sensitivity;
        }

        float moveY = Input.GetAxis("Vertical");
        float moveX = Input.GetAxis("Horizontal");

        transform.Rotate(new Vector3(rotationX, rotationY));
        transform.Translate(new Vector3(moveX, 0, moveY));

        if (Input.GetKeyDown(KeyCode.E))
        {

            ScreenCapture.CaptureScreenshot(Time.realtimeSinceStartup + "");

        }

    }

}
