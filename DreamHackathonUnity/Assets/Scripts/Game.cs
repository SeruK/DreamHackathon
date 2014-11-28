using UnityEngine;
using System.Collections;

public class Game : MonoBehaviour
{
	void Update()
	{

	}

	public delegate void DrawDelegate();

	void Indent(int in_amount, DrawDelegate in_draw)
	{
		GUILayout.BeginHorizontal();
		GUILayout.Space(in_amount * 10.0f);
		if (in_draw != null) in_draw();
		GUILayout.EndHorizontal();
	}

	void OnGUI()
	{
		for (uint controller = 1; controller < 2; ++controller)
		{
			GUILayout.Label("Controller " + (controller + 1));

			foreach (ControllerInput.Range range in System.Enum.GetValues(typeof(ControllerInput.Range)))
			{
				Vector2 val = ControllerInput.GetRange(controller, range);
				GUILayout.Label(ControllerInput.GetRangeName(range) + ": " + val);
           	}
			foreach (ControllerInput.Button button in System.Enum.GetValues(typeof(ControllerInput.Button)))
			{
				string state = "";
				if (ControllerInput.GetButtonDown(controller, button))
				{
					state += " Down";
				}
				if (ControllerInput.GetButton(controller, button))
				{
					state += " Pressed";
				}
				if (ControllerInput.GetButtonUp(controller, button))
				{
					state += " Up";
				}
				Indent(1, () => (GUILayout.Label(ControllerInput.GetButtonName(button) + ":" + state)));
			}

        	GUILayout.Space(10.0f);
		}
	}
}
