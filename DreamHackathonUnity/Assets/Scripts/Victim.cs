using UnityEngine;
using System.Collections;

public class Victim : MonoBehaviour
{
	void OnNetworkInstantiate(NetworkMessageInfo in_info)
	{
		if (Network.isClient)
		{
			Debug.Log("Disabling cameras");
			foreach (var cam in GetComponentsInChildren<Camera>())
			{
				cam.enabled = false;
			}
		}

		var game = GameObject.FindGameObjectWithTag("GameController").GetComponent<Game>();
		game.OnVictimSpawned(this);
	}
}
