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
