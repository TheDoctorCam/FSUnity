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

	public void FixedUpdate(){
		_rigidbody.MovePosition (_rigidbody.position + velocity * Time.fixedDeltaTime);
	}
}
