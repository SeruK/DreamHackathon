using UnityEngine;
using System.Collections;

public class ControllerInput
{
	// Might not be correct with this ver of Tattie Bogle
	public enum Button
	{
		A = 16,
		B = 17,
		X = 18,
		Y = 19,
		Start = 9,
		Back = 10,
		BumperLeft = 14,
		BumperRight = 15,
		DpadUp = 5,
		DpadDown = 6,
		DpadLeft = 7,
		DpadRight = 8
	}

	public enum Range
	{
//		LeftTrigger,
//		RightTrigger,
		LeftStick,
		RightStick,
//		Dpad
	}

	public enum Axis
	{
		X,
		Y
	}

	public static bool GetButton(uint in_controller, Button in_button)
	{
		return Input.GetKey(GetKeyCode(in_controller, in_button));
	}

	public static bool GetButtonDown(uint in_controller, Button in_button)
	{
		return Input.GetKeyDown(GetKeyCode(in_controller, in_button));
	}

	public static bool GetButtonUp(uint in_controller, Button in_button)
	{
		return Input.GetKeyUp(GetKeyCode(in_controller, in_button));
	}

	public static float GetAxis(uint in_controller, Range in_range, Axis in_axis)
	{
		return Input.GetAxis(GetRangeString(in_controller, in_range, in_axis));
	}

	public static Vector2 GetRange(uint in_controller, Range in_range)
	{
		return new Vector2(GetAxis(in_controller, in_range, Axis.X),
		                   GetAxis(in_controller, in_range, Axis.Y));
	}

	private static KeyCode GetKeyCode(uint in_controller, Button in_button)
	{
		string str = GetControllerName(in_controller) + "Button" + (int)in_button;
		return (KeyCode)System.Enum.Parse(typeof(KeyCode), str);
	}

	private static string GetControllerName(uint in_controller)
	{
		return "Joystick" + (in_controller + 1);
	}

	public static string GetButtonName(Button in_button)
	{
		return System.Enum.GetName(typeof(Button), in_button);
	}

	public static string GetRangeName(Range in_range)
	{
		return System.Enum.GetName(typeof(Range), in_range);
	}

	private static string GetRangeString(uint in_controller, Range in_range, Axis in_axis)
	{
		string axis = in_axis == Axis.X ? "XAxis" : "YAxis";

		return GetInternalRangeName(in_range) + "_" + axis + "_" + (in_controller + 1);
	}

	private static string GetInternalRangeName(Range in_range)
	{
		switch (in_range)
		{
			case Range.LeftStick:
			{
				return "L";
			}
			case Range.RightStick:
			{
				return "R";
			}
//			case Range.LeftTrigger:
//			{
//				return "TriggersL";
//			}
//			case Range.RightTrigger:
//			{
//				return "TriggersR";
//			}
//			case Range.Dpad:
//			{
//				return "DPad";
//			}

			default: break;
		}

		throw new System.Exception("Shouldn't come here!");
	}

	private ControllerInput()
	{
	}
}
