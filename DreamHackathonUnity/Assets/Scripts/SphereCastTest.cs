using UnityEngine;
using System.Collections;

public class SphereCastTest : MonoBehaviour {

	public Transform Orig;

	// Use this for initialization
	void Start () {
	
	}

	void Update()
	{
		Ray ray = new Ray(Orig.position, Orig.InverseTransformDirection(Vector3.forward));
		// TODO: Add props layer here
		int layers = (1 << 9) | (1 << 10) | (1 << 11) | (1 << 12);
		
		RaycastHit hitInfo;
		
		if (Physics.SphereCast(ray, 0.5f, out hitInfo, 100000.0f, layers))
		{
			Debug.Log("Hit! " + hitInfo.collider);
//			hitInfo.collider.BroadcastMessage("WasHit", SendMessageOptions.DontRequireReceiver);
		}

	}

	void FixedUpdate()
	{

	}
}
