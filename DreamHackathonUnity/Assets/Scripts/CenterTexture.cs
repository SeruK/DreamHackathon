using UnityEngine;
using System.Collections;

public class CenterTexture : MonoBehaviour
{
	public GUITexture texture;

	void OnEnable()
	{
		texture.pixelInset = new Rect(Screen.width / 2.0f + texture.texture.width / 2.0f,
		                              -Screen.height / 2.0f + texture.texture.height / 2.0f,
		                              0.0f, 0.0f);
	}
}
