using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class projectile : MonoBehaviour {

	float speed = 10;

	public void setSpeed(float newSpeed){
		speed = newSpeed;
	}

	void Update () {
		transform.Translate (Vector3.back * Time.deltaTime * speed);
	}
}
