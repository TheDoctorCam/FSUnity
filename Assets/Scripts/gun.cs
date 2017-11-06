using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gun : MonoBehaviour {

	public Transform muzzle;			//Position of gun muzzle
	public projectile bullet;			//Type of projectile
	public float rateOfFire = 100;		//Rate of fire in milliseconds
	public float bulletVelocity = 35;	//Velocity of bullet 

	float bulletBuffer;					//Time between projectile's firing 

	public void Shoot(){

		if (Time.time > bulletBuffer) {
			bulletBuffer = Time.time + rateOfFire / 1000;
			projectile newProjectile = Instantiate (bullet, muzzle.position, muzzle.rotation) as projectile;
			newProjectile.setSpeed (bulletVelocity);
		}
	}
}
