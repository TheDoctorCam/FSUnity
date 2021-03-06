﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class projectile : MonoBehaviour {

	public LayerMask collisionMask;
	float speed = 10;			//projectile speed
	float damage = 1;			//damage per projectile 

	float falloffDistance = 3;	//projectiles falloff after 3 seconds

	void Start(){
		Destroy (gameObject, falloffDistance);	//projectile deleted after falloff distance 

		//intersectCollision is an array of all the colliders that the projectile intersects with
		Collider[] intersectCollision = Physics.OverlapSphere (transform.position, .1f, collisionMask);
		if (intersectCollision.Length > 0) {
			HitObject (intersectCollision [0], transform.position);
		}
	}

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
			HitObject (hit.collider, hit.point);
		}
	}


	void HitObject(Collider collision, Vector3 hitPoint){
		iDamageable hitObject = collision.GetComponent<iDamageable> ();
		if (hitObject != null) {
			hitObject.TakeDamage (damage, hitPoint, transform.forward);
		}
		GameObject.Destroy (gameObject);
	}

}
