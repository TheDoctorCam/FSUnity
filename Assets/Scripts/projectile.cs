using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class projectile : MonoBehaviour {

	public LayerMask collisionMask;
	float speed = 10;						//projectile speed
	float damage = 1;						//damage per projectile

	float lifetime = 3;						//time a projectile is in the air
	float skinWidth = .1f;					//size of visible projectile

	void Start() {
		Destroy (gameObject, lifetime);

		//intersectCollision is an array of all the colliders that the projectile intersects with
		Collider[] initialCollisions = Physics.OverlapSphere (transform.position, .1f, collisionMask);
		if (initialCollisions.Length > 0) {
			HitObject(initialCollisions[0], transform.position);	//on collision call HitObject
		}

	}

	public void setSpeed(float newSpeed) {
		speed = newSpeed;
	}
	
	
	//update the projectile position and check for collisions
	void Update () {
		float moveDistance = speed * Time.deltaTime;
		CheckCollisions (moveDistance);
		transform.Translate (Vector3.forward * moveDistance);
	}


	void CheckCollisions(float moveDistance) {
		Ray ray = new Ray (transform.position, transform.forward);
		RaycastHit hit;

		if (Physics.Raycast(ray, out hit, moveDistance + skinWidth, collisionMask, QueryTriggerInteraction.Collide)) {
			HitObject(hit.collider, hit.point);
		}
	}

	void HitObject(Collider c, Vector3 hitPoint) {
		damages damageableObject = c.GetComponent<damages> ();
		if (damageableObject != null) {
			damageableObject.TakeHit(damage, hitPoint, transform.forward);
		}
		GameObject.Destroy (gameObject);
	}
}
