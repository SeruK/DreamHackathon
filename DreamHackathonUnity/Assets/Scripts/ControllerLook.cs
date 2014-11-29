using UnityEngine;
using System.Collections;

/// MouseLook rotates the transform based on the mouse delta.
/// Minimum and Maximum values can be used to constrain the possible rotation

/// To make an FPS style character:
/// - Create a capsule.
/// - Add the MouseLook script to the capsule.
///   -> Set the mouse look to use LookX. (You want to only turn character but not tilt it)
/// - Add FPSInputController script to the capsule
///   -> A CharacterMotor and a CharacterController component will be automatically added.

/// - Create a camera. Make the camera a child of the capsule. Reset it's transform.
/// - Add a MouseLook script to the camera.
///   -> Set the mouse look to use LookY. (You want the camera to tilt up and down like a head. The character already turns.)
[AddComponentMenu("Camera-Control/Controller Look")]
public class ControllerLook : MonoBehaviour {

	public enum RotationAxes { XAndY = 0, X = 1, Y = 2 }
	public RotationAxes axes = RotationAxes.XAndY;
	public float sensitivityX = 15F;
	public float sensitivityY = 15F;

	public float minimumX = -360F;
	public float maximumX = 360F;

	public float minimumY = -60F;
	public float maximumY = 60F;
	public ControllerInput.Range range;
	public int controller;

	float rotationY = 0F;

	void Update ()
	{
		if (axes == RotationAxes.XAndY)
		{
			float inputX = ControllerInput.GetAxis((uint)controller, range, ControllerInput.Axis.X);
			float rotationX = transform.localEulerAngles.y + inputX * sensitivityX;

			float inputY = ControllerInput.GetAxis((uint)controller, range, ControllerInput.Axis.Y);
			rotationY += inputY * sensitivityY;
			rotationY = Mathf.Clamp (rotationY, minimumY, maximumY);
			
			transform.localEulerAngles = new Vector3(-rotationY, rotationX, 0);
		}
		else if (axes == RotationAxes.X)
		{
			float inputX = ControllerInput.GetAxis((uint)controller, range, ControllerInput.Axis.X);
			transform.Rotate(0, inputX * sensitivityX, 0);
		}
		else
		{
			float inputY = ControllerInput.GetAxis((uint)controller, range, ControllerInput.Axis.Y);
			rotationY += inputY * sensitivityY;
			rotationY = Mathf.Clamp (rotationY, minimumY, maximumY);
			
			transform.localEulerAngles = new Vector3(-rotationY, transform.localEulerAngles.y, 0);
		}
	}
	
	void Start ()
	{
		// Make the rigid body not change rotation
		if (GetComponent<Rigidbody>())
			GetComponent<Rigidbody>().freezeRotation = true;
	}
}