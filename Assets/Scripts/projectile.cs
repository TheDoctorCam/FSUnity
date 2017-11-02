using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class projectile : MonoBehaviour {

	public LayerMask collisionMask;
	float speed = 10;

	public void setSpeed(float newSpeed){
		speed = newSpeed;
	}

	void Update () {
		float moveDistance = speed * Time.deltaTime;
		CheckCollisions (moveDistance);
		transform.Translate (Vector3.back * moveDistance);
	}

	/* Used for bullet collisions recognition */
	void CheckCollisions(float moveDistance){
		Ray ray = new Ray (transform.position, transform.forward);
		RaycastHit hit;

		if(Physics.Raycast(ray, out hit, moveDistance, collisionMask, QueryTriggerInteraction.Collide)){
			Hit (hit);
		}
	}

	void Hit(RaycastHit hit){

	
		GameObject.Destroy (gameObject);
	}
}
