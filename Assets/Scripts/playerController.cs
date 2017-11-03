using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent (typeof (Rigidbody))]
public class playerController : MonoBehaviour {

	Vector3 velocity;
	Rigidbody _rigidbody;

	void Start () {
		_rigidbody = GetComponent<Rigidbody> ();	
	}
	
	public void Move(Vector3 v){
		velocity = v;
	}

	/* Turns the player using mouse */
	public void LookAt(Vector3 point){
		Vector3 yCorrection = new Vector3 (point.x, transform.position.y, point.z);
		transform.LookAt (yCorrection);
	}
	void FixedUpdate(){
		_rigidbody.MovePosition (_rigidbody.position + velocity * Time.fixedDeltaTime);
	}
}
