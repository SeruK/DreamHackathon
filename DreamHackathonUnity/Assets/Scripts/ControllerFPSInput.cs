using UnityEngine;
using System.Collections;

public class ControllerFPSInput : MonoBehaviour
{
	[System.Serializable]
	public struct MovementAxis
	{
		public ControllerInput.Range Range;
		public ControllerInput.Axis  Axis;
		public bool Invert;
	}

	public MovementAxis xAxis;
	public MovementAxis zAxis;

	public int Controller;

	public ControllerInput.Button JumpButton = ControllerInput.Button.A;

	private CharacterMotor motor;

	void Awake()
	{
		motor = GetComponent<CharacterMotor>();
	}
	
	void Update()
	{
		var input = new Vector2((xAxis.Invert ? -1.0f : 1.0f) * ControllerInput.GetAxis((uint)Controller, xAxis.Range, xAxis.Axis),
		                        (zAxis.Invert ? -1.0f : 1.0f) * ControllerInput.GetAxis((uint)Controller, zAxis.Range, zAxis.Axis));
		// Get the input vector from keyboard or analog stick
		var directionVector = new Vector3(input.x, 0, input.y);
		
		if (directionVector != Vector3.zero) {
			// Get the length of the directon vector and then normalize it
			// Dividing by the length is cheaper than normalizing when we already have the length anyway
			var directionLength = directionVector.magnitude;
			directionVector = directionVector / directionLength;
			
			// Make sure the length is no bigger than 1
			directionLength = Mathf.Min(1, directionLength);
			
			// Make the input vector more sensitive towards the extremes and less sensitive in the middle
			// This makes it easier to control slow speeds when using analog sticks
			directionLength = directionLength * directionLength;
			
			// Multiply the normalized direction vector by the modified length
			directionVector = directionVector * directionLength;
		}
				
		// Apply the direction to the CharacterMotor
		motor.inputMoveDirection = transform.rotation * directionVector;
		motor.inputJump = ControllerInput.GetButton((uint)Controller, JumpButton);
	}
}
