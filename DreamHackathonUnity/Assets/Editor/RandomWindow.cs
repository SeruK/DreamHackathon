using UnityEngine;
using UnityEditor;
using System.Collections;

public class RandomWindow : EditorWindow
{
	[MenuItem("Window/Random")]
	static void Init()
	{
		EditorWindow.GetWindow(typeof(RandomWindow));
	}

	bool randomizeRot;
	float minScale;
	float maxScale;

	void Randomize()
	{
		foreach (var go in Selection.gameObjects)
		{
			if (randomizeRot)
			{
				go.transform.Rotate(Vector3.up, Random.Range(0.0f, 360.0f));
			}

			go.transform.localScale = new Vector3(1, 1, 1) * Random.Range(minScale, maxScale);
		}
	}

	void OnGUI()
	{
		GUILayout.BeginHorizontal();
		EditorGUILayout.LabelField(minScale.ToString());
		EditorGUILayout.MinMaxSlider(ref minScale, ref maxScale, 0.1f, 3.0f);
		EditorGUILayout.LabelField(maxScale.ToString());
		GUILayout.EndHorizontal();
		randomizeRot = EditorGUILayout.Toggle("Randomize rotation", randomizeRot);
		if (GUILayout.Button("Randomize"))
		{
			Randomize();
		}
	}
}
