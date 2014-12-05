using UnityEngine;
using System.Collections;

public class Hunter : MonoBehaviour
{
	public Camera Cam;
	public Transform Model;
	public uint PlayerIndex;
	public float StopTime = 0.5f;
	
	public AudioSource WalkSource;
	public AudioSource DeathSource;
	public AudioClip DeathClip;

	private Game game;
	private Vector3 modelOffset;
	private float stopTimer;
	private bool invincible {
		get { return stopTimer <= 0.0f; }
	}
	private CharacterController charController;

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

//			var listener = GetComponentInChildren<AudioListener>();
//			listener.enabled = false;

			var motor = GetComponent<CharacterMotor>();
			motor.enabled = false;
		}

		charController = GetComponent<CharacterController>();

		modelOffset = Model.transform.position - transform.position;
		Model.transform.parent = null;

		var go = GameObject.FindGameObjectWithTag("GameController");
		if (go == null)
		{
			Debug.LogError("Couldn't find GameController!");
			return;
		}
		game = go.GetComponent<Game>();
		if (game == null)
		{
			Debug.LogError("GameController did not have Game script!");
			return;
		}
		game.OnHunterSpawned(this);
	}

	public void LateUpdate()
	{
		if (!Network.isClient) return;
		if (charController.velocity.magnitude > 0.0f)
		{
			SetWalkMute(false);
			stopTimer = StopTime;
		}
		else
		{
			SetWalkMute(true);
		}
	}

	public void SetPlayerIndex(uint in_index)
	{
		PlayerIndex = in_index;
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
		SetPhysicsLayerRecursive(gameObject, layer);
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
		if (stopTimer > 0.0f) stopTimer -= Time.deltaTime;

		if (Model)
		{
			var trans = Model.transform;
			trans.position = Vector3.Lerp(trans.position, transform.position + modelOffset, 0.1f);
			trans.rotation = Quaternion.Slerp(trans.rotation, transform.rotation, 0.1f);
		}
	}

	public void WasHit()
	{
		networkView.RPC("Die", RPCMode.OthersBuffered);
	}

	[RPC]
	void Die()
	{
		if (!invincible)
		{
			var p = transform.position;
			PlayDeathClip(p.x, p.y, p.z);
			game.RespawnHunter(this);
		}
	}

	[RPC]
	void SetWalkMute(bool in_mute)
	{
		if (networkView.isMine)
		{
			networkView.RPC("SetWalkMute", RPCMode.OthersBuffered, in_mute);
		}
		WalkSource.mute = in_mute;
	}

	[RPC]
	void PlayDeathClip(float in_x, float in_y, float in_z)
	{
		if (networkView.isMine)
		{
			networkView.RPC("PlayDeathClip", RPCMode.OthersBuffered, in_x, in_y, in_z);
		}
		DeathSource.PlayOneShot(DeathClip);
	}
}
