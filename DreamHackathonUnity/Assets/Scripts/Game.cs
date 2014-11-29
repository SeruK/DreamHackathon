using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Game : MonoBehaviour
{
	public delegate void DrawDelegate();

	public Transform  VictimSpawnPoint;
	public GameObject VictimPrefab;

	public Transform[] HunterSpawnPoints;
	public GameObject  HunterPrefab;

	private List<GameObject> hunterGOs;
	private GameObject victimGO;

//#if UNITY_IPHONE && !UNITY_EDITOR
#if UNITY_IPHONE
	void OnPlayerDisconnected(NetworkPlayer in_player)
	{
		if (in_player == Network.player) return;
		Network.RemoveRPCs(in_player);
		Network.DestroyPlayerObjects(in_player);
		Network.Disconnect();
		Application.LoadLevel(0);
	}
#else
	void OnDisconnectedFromServer(NetworkDisconnection in_info)
	{
		Debug.Log(in_info.ToString());
		Application.LoadLevel(0);
	}
#endif

	/***********************************************/

	void Start()
	{
		hunterGOs = new List<GameObject>();

		if (Network.isServer) SpawnVictim();
		else if (Network.isClient) SpawnHunters();
	}

	void SpawnVictim()
	{
		Network.Instantiate(VictimPrefab, VictimSpawnPoint.position, VictimSpawnPoint.rotation, 0);
	}

	void SpawnHunters()
	{
		for (int i = 0; i < 4; ++i)
		{
			var spawnPoint = HunterSpawnPoints[i % HunterSpawnPoints.Length];
			SpawnHunter(spawnPoint.position, spawnPoint.rotation);
		}
	}

	GameObject SpawnHunter(Vector3 in_pos, Quaternion in_rot)
	{
		return Network.Instantiate(HunterPrefab, in_pos, in_rot, 0) as GameObject;
	}

	/***********************************************/

	public void OnVictimSpawned(Victim in_victim)
	{
		victimGO = in_victim.gameObject;
	}

	public void OnHunterSpawned(Hunter in_hunter)
	{
		int playerIndex = hunterGOs.Count;
		in_hunter.gameObject.name = "Hunter " + playerIndex;
		foreach (var clook in in_hunter.GetComponentsInChildren<ControllerLook>())
		{
			clook.controller = playerIndex;
		}

		var input = in_hunter.GetComponentInChildren<ControllerFPSInput>();
		input.Controller = playerIndex;

		hunterGOs.Add(in_hunter.gameObject);
		var player = hunterGOs.Count;
		if (player == 1)      in_hunter.Cam.rect = new Rect(0.0f, 0.0f, 0.5f, 0.5f);
		else if (player == 2) in_hunter.Cam.rect = new Rect(0.5f, 0.0f, 0.5f, 0.5f);
		else if (player == 3) in_hunter.Cam.rect = new Rect(0.0f, 0.5f, 0.5f, 0.5f);
		else if (player == 4) in_hunter.Cam.rect = new Rect(0.5f, 0.5f, 0.5f, 0.5f);
	}
	
	/***********************************************/

	void Update()
	{
		if (hunterGOs != null && Network.isClient) 
		{
			for (uint i = 0; i < hunterGOs.Count; ++i)
			{
				var hunterGO = hunterGOs[(int)i];
				if (!hunterGO) continue;

				var controller = hunterGO.GetComponent<CharacterController>();
				if (!controller) continue;

				var input = ControllerInput.GetRange(i, ControllerInput.Range.LeftStick);
				float movement = 1 * Time.deltaTime;
				controller.Move(new Vector3(input.x, 0, input.y) * movement);
			}
		}
	}

	/***********************************************/

	void Indent(int in_amount, DrawDelegate in_draw)
	{
		GUILayout.BeginHorizontal();
		GUILayout.Space(in_amount * 10.0f);
		if (in_draw != null) in_draw();
		GUILayout.EndHorizontal();
	}

	void OnGUI()
	{
		if (Network.isServer)
		{
			GUILayout.Label("Hosting at " + Network.player.ipAddress + ":" + Network.player.port);
		}
	}

	void DebugDrawControllers()
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
