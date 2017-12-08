using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//speed and other rigidbodies are introduced

[RequireComponent (typeof (Rigidbody))]
public class playerController : MonoBehaviour {
	Vector3 current_speed;
	int speed_variable;
	Rigidbody body;

	void Start () { 
		body = GetComponent<Rigidbody> ();
		speed_variable = 5; // sets player controller speed
	}

	public void Move(Vector3 speed) {
		current_speed = speed; // changes speed based on passed in value
	}

	public void LookAt(Vector3 see) {
		Vector3 newHeight = new Vector3 (see.x, transform.position.y, see.z);
		transform.LookAt (newHeight);
	}

	void FixedUpdate() {
		body.MovePosition (body.position + current_speed * Time.fixedDeltaTime);

	}
	
	
}
