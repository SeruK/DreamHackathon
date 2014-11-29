using UnityEngine;
using System.Collections;

public class SpawnPoint : MonoBehaviour
{
	void OnDrawGizmos()
	{
		Gizmos.DrawSphere(transform.position, 1.0f);
	}
}
