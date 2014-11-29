using UnityEngine;
using System.Collections;

public class Hunter : MonoBehaviour
{
	public Camera Cam;
	public Transform Model;

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
		}

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
		if (Model) Model.transform.parent = null;
		game.OnHunterSpawned(this);
	}

	void OnDestroy()
	{
		if (Model) Destroy(Model);
	}

	void Update()
	{
		if (Model)
		{
			Model.transform.position = Vector3.Lerp(Model.transform.position, transform.position, 0.1f);
			Model.transform.rotation = Quaternion.Slerp(Model.transform.rotation, transform.rotation, 0.1f);
		}
	}
}
