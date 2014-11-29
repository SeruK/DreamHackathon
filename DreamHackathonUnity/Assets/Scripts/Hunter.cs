using UnityEngine;
using System.Collections;

public class Hunter : MonoBehaviour
{
	public Camera Cam;
	public Transform Model;

	private Vector3 modelOffset;

	void OnNetworkInstantiate(NetworkMessageInfo in_info)
	{
		if (!Network.isClient)
		{
			Debug.Log("Disabling camera");
			if (Cam == null)
			{
				Debug.LogError("Cam was null?!");
			}
			else Cam.enabled = false;

			var listener = GetComponentInChildren<AudioListener>();
			listener.enabled = false;

			var motor = GetComponent<CharacterMotor>();
			motor.enabled = false;
		}

		modelOffset = Model.transform.position - transform.position;
		Model.transform.parent = null;

		var go = GameObject.FindGameObjectWithTag("GameController");
		if (go == null)
		{
			Debug.LogError("Couldn't find GameController!");
			return;
		}
		var game = go.GetComponent<Game>();
		if (game == null)
		{
			Debug.LogError("GameController did not have Game script!");
			return;
		}
		game.OnHunterSpawned(this);
	}

	public void SetPlayerIndex(uint in_index)
	{
		gameObject.name = "Hunter " + in_index;
		foreach (var clook in GetComponentsInChildren<ControllerLook>())
		{
			clook.controller = (int)in_index;
		}
		
		var input = GetComponentInChildren<ControllerFPSInput>();
		input.Controller = (int)in_index;

		if (in_index == 0)      Cam.rect = new Rect(0.0f, 0.5f, 0.5f, 0.5f);
		else if (in_index == 1) Cam.rect = new Rect(0.5f, 0.0f, 0.5f, 0.5f);
		else if (in_index == 2) Cam.rect = new Rect(0.0f, 0.0f, 0.5f, 0.5f);
		else if (in_index == 3) Cam.rect = new Rect(0.5f, 0.5f, 0.5f, 0.5f);

		int layer = 9 + (int)in_index;
		Cam.cullingMask &= ~(1 << layer);
		SetPhysicsLayerRecursive(Model.gameObject, layer);
	}

	void SetPhysicsLayerRecursive(GameObject in_object, int in_layer)
	{
		in_object.layer = in_layer;
		for (int i = 0; i < in_object.transform.childCount; ++i)
		{
			var child = in_object.transform.GetChild(i);
			SetPhysicsLayerRecursive(child.gameObject, in_layer);
		}
	}

	void OnDestroy()
	{
		if (Model)
		{
			Destroy(Model);
		}
	}

	void Update()
	{
		if (Model)
		{
			var trans = Model.transform;
			trans.position = Vector3.Lerp(trans.position, transform.position + modelOffset, 0.1f);
			trans.rotation = Quaternion.Slerp(trans.rotation, transform.rotation, 0.1f);
		}
	}
}
