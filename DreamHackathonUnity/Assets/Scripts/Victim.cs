using UnityEngine;
using System.Collections;

public class Victim : MonoBehaviour
{
	public Vector3 DieSphereOffset;
	public float LookSphereRadius = 0.5f;
	public float DieDistance = 0.5f;

	public Transform RaycastOrigin; 
	public Transform Model;
	public Transform CameraRoot;

	public AudioClip DeathClip;

#if UNITY_EDITOR
	public float DebugGizmoDistance = 5.0f;
#endif

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

	Ray GetRay()
	{
		return new Ray(RaycastOrigin.position, RaycastOrigin.TransformDirection(Vector3.forward));
	}

	void Update()
	{
		Model.rotation = Quaternion.Euler(0, CameraRoot.rotation.eulerAngles.y, 0.0f);
		var ray = GetRay(); 

		Debug.DrawLine(ray.origin, ray.origin + ray.direction * 10000.0f);
//		Debug.DrawRay(ray.origin, ray.direction);

		if (!Network.isServer) return;

		// TODO: Add props layer here
		int layers = (1 << 9) | (1 << 10) | (1 << 11) | (1 << 12) | (1 << 13);

		RaycastHit hitInfo;

		if (Physics.SphereCast(ray, LookSphereRadius, out hitInfo, 100000.0f, layers))
		{
			Debug.Log("Hit! " + hitInfo.collider);
			if (hitInfo.collider.gameObject.layer != 13)
			{
				if (Vector3.Distance(transform.position + DieSphereOffset, hitInfo.collider.transform.position) <= DieDistance)
				{
					Die();
				}
				else
				{
					hitInfo.collider.BroadcastMessage("WasHit", SendMessageOptions.DontRequireReceiver);
				}
			}

		}
	}

	[RPC]
	void Die()
	{
		var game = GameObject.FindGameObjectWithTag("GameController").GetComponent<Game>();
		if (game.RestartingGame) return;
		if (networkView.isMine)
		{
			networkView.RPC("Die", RPCMode.OthersBuffered);
		}
		var audiopos = transform.position + DieSphereOffset;
		PlayDeathClip(audiopos.x, audiopos.y, audiopos.z);
		game.RestartGame();
	}

	[RPC]
	void PlayDeathClip(float in_x, float in_y, float in_z)
	{
		if (networkView.isMine)
		{
			networkView.RPC("PlayDeathClip", RPCMode.OthersBuffered, in_x, in_y, in_z);
		}
		AudioSource.PlayClipAtPoint(DeathClip, new Vector3(in_x, in_y, in_z));
	}

#if UNITY_EDITOR
	void OnDrawGizmosSelected()
	{
		Gizmos.DrawWireSphere(transform.position + DieSphereOffset, DieDistance);

		Ray ray = GetRay();
		var start = ray.origin;
		var end = ray.origin + ray.direction * DebugGizmoDistance;
		Gizmos.DrawLine(start, end);
		Gizmos.DrawWireSphere(end, LookSphereRadius);
	}
#endif
}
