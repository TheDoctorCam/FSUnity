using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

[RequireComponent (typeof (NavMeshAgent))]
public class Enemy : MonoBehaviour {

	NavMeshAgent pathfinder;
	Transform target;

	void Start () {
		pathfinder = GetComponent<NavMeshAgent> ();	
		target = GameObject.FindGameObjectWithTag ("Player").transform;

		StartCoroutine (UpdatePath ());
	}
	
	void Update () {
	}

	/*This function controls the tracking refresh rate of enemies*/
	IEnumerator UpdatePath() {
		float refreshRate = .25f;

		while (target != null){
			Vector3 targetPosition = new Vector3 (target.position.x, 0, target.position.z);
			pathfinder.SetDestination (targetPosition);
			yield return new WaitForSeconds (refreshRate);
			}
	}

}