using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gun : MonoBehaviour {

	public Transform muzzle;
	public projectile bullet;
	public float rateOfFire = 100;	//in ms
	public float bulletVelocity = 35;

	float bulletBuffer;

	public void Shoot(){

		if (Time.time > bulletBuffer) {
			bulletBuffer = Time.time + rateOfFire / 1000;
			projectile newProjectile = Instantiate (bullet, muzzle.position, muzzle.rotation) as projectile;
			newProjectile.setSpeed (bulletVelocity);
		}
	}
}
