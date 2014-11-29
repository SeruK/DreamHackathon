using UnityEngine;
using System.Collections;

public class Lobby : MonoBehaviour
{
	public delegate void DrawDelegate();
	public const int ConnectionPort = 25001;
	
#if UNITY_IPHONE

	private string status;
	
	void OnEnable()
	{
		DontDestroyOnLoad(gameObject);
		SetStatus("Initializing server on port " + ConnectionPort + "...");
		var error = Network.InitializeServer(1, ConnectionPort, false);
		if (error != NetworkConnectionError.NoError)
			SetStatus("Failed to initialized server: " + error);
	}

	void OnServerInitialized()
	{
		SetStatus("Hosting on " + Network.player.ipAddress + ":" + ConnectionPort + "\nWaiting for computer...");
	}

	void OnPlayerConnected(NetworkPlayer in_player)
	{
		if (in_player == Network.player) return;

		SetStatus("Computer connected from " + in_player.ipAddress + "! Starting...");
		StartCoroutine(StartGame());
	}

	void SetStatus(string in_status)
	{
		status = in_status;
		if (!string.IsNullOrEmpty(in_status)) Debug.Log(status);
	}

	IEnumerator StartGame()
	{
		// Just wait a while
		yield return null;
		Network.SetSendingEnabled(0, false);
		Network.isMessageQueueRunning = false;
		Application.LoadLevel(1);
		yield return null;
		yield return null;
		Network.isMessageQueueRunning = true;
		Network.SetSendingEnabled(0, true);

		foreach (var go in FindObjectsOfType<GameObject>())
			go.SendMessage("OnNetworkLoadedLevel", SendMessageOptions.DontRequireReceiver);

		Destroy(gameObject);
	}
	
	void OnGUI()
	{
		DrawCentered(() => {
			if (!string.IsNullOrEmpty(status))
			{
				GUILayout.Label(status);
			}
		});
	}

#else

	public string ConnectionIP = "77.80.247.199";

	private bool shouldConnect;
	private bool connecting;

	private string networkErrorString;

	void Update()
	{
		if (shouldConnect)
		{
			shouldConnect = false;
			Connect();
		}
	}

	void Connect()
	{
		if (connecting)
			return;

		connecting = true;
		Network.Connect(ConnectionIP, ConnectionPort);
	}

	void OnConnectedToServer()
	{
		if (!connecting)
			return;

		connecting = false;
		Application.LoadLevel(1);
	}

	void OnFailedToConnect(NetworkConnectionError in_error)
	{
		if (!connecting)
			return;

		networkErrorString = in_error.ToString();
		connecting = false;
	}

	void OnGUI()
	{
		DrawCentered(() => {
			if (!connecting)
			{
				DrawConnect();
			}
			else
			{
				DrawConnecting();
			}
		});
	}

	void DrawConnect()
	{
		if (!string.IsNullOrEmpty(networkErrorString))
		{
			GUILayout.Label(networkErrorString);
			GUILayout.Space(30.0f);
		}

		GUILayout.BeginHorizontal();

		GUILayout.Label("IP");
		ConnectionIP = GUILayout.TextField(ConnectionIP, GUILayout.MinWidth(100));

		GUILayout.EndHorizontal();
		
		shouldConnect = GUILayout.Button("Join");
	}

	void DrawConnecting()
	{
		GUILayout.Label("Connecting to " + ConnectionIP + ":" + ConnectionPort + "...");
	}

#endif

	void DrawCentered(DrawDelegate in_draw)
	{
		GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height));
		GUILayout.FlexibleSpace();
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		
		if (in_draw != null) in_draw();
		
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		GUILayout.FlexibleSpace();
		GUILayout.EndArea();
	}
}
