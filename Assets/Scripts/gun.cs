using UnityEngine;
using System.Collections;
using System.Collections.Generic;


//This will define how the gun is fired and will add functionality to the bullet

public class gun : MonoBehaviour {

	public enum FireMode {Auto, Burst, Single};
	public FireMode fireMode;

	//There are multiple guns so multiple objects need to be defined
	
	public Transform[] muzzle;			//Position of gun muzzle
	public projectile bullet;			//the bullet being fired
	public float msBetweenShots = 100;			//time between bursts
	public float muzzleVelocity = 35;		//speed of bullet
	public int burstCount;					//for burst machine gun
	public int projectilesPerMag;			//implemented for possible reload feature
	public float reloadTime = .3f;			//time to reload

	
	
	
	[Header("Recoil")]
	public Vector2 kickMinMax = new Vector2(.05f,.2f);		//angle of recoil
	public Vector2 recoilAngleMinMax = new Vector2(3,5);	//maximum recoil angle
	public float recoilMoveSettleTime = .1f;				//time for gun to settle after shot
	public float recoilRotationSettleTime = .1f;

	
	
	[Header("Effects")]
	public Transform shell;				//shell ejected from each shot
	public Transform shellEjection;	
	
	
	
	MuzzleFlash muzzleflash;			//object for flash from each shot
	float bulletBuffer;

	
	
	bool triggerReleasedSinceLastShot;	//check if trigger was released(for burst firing)
	int shotsRemainingInBurst;			//number of shots in the burst
	int projectilesRemainingInMag;
	bool isReloading;					//is the player reloading
	
	

	Vector3 recoilSmoothDampVelocity;
	float recoilRotSmoothDampVelocity;
	float recoilAngle;

	void Start() {		//when guncontroller calls gun
		muzzleflash = GetComponent<MuzzleFlash> ();
		shotsRemainingInBurst = burstCount;
		projectilesRemainingInMag = projectilesPerMag;
	}

	void LateUpdate() {
		//begin animation of the gun
		transform.localPosition = Vector3.SmoothDamp (transform.localPosition, Vector3.zero, ref recoilSmoothDampVelocity, recoilMoveSettleTime);
		recoilAngle = Mathf.SmoothDamp (recoilAngle, 0, ref recoilRotSmoothDampVelocity, recoilRotationSettleTime);
		transform.localEulerAngles = transform.localEulerAngles + Vector3.left * recoilAngle;

		if (!isReloading && projectilesRemainingInMag == 0) {
			Reload();
		}
	}

	void Shoot() {
		//if the player is not reloading(not working) then the shot is fired on click
		if (!isReloading && Time.time > bulletBuffer && projectilesRemainingInMag > 0) {
			if (fireMode == FireMode.Burst) {
				if (shotsRemainingInBurst == 0) {
					return;
				}
				shotsRemainingInBurst --;
			}
			else if (fireMode == FireMode.Single) {
				if (!triggerReleasedSinceLastShot) {
					return;
				}
			}

			for (int i =0; i < muzzle.Length; i ++) {
				if (projectilesRemainingInMag == 0) {
					break;
				}
				projectilesRemainingInMag --;
				bulletBuffer = Time.time + msBetweenShots / 1000;
				projectile newProjectile = Instantiate (bullet, muzzle[i].position, muzzle[i].rotation) as projectile;
				newProjectile.setSpeed (muzzleVelocity);
			}

			Instantiate(shell,shellEjection.position, shellEjection.rotation);
			muzzleflash.Activate();
			transform.localPosition -= Vector3.forward * Random.Range(kickMinMax.x, kickMinMax.y);
			recoilAngle += Random.Range(recoilAngleMinMax.x, recoilAngleMinMax.y);
			recoilAngle = Mathf.Clamp(recoilAngle, 0, 30);

		}
	}

	public void Reload() {
		if (!isReloading && projectilesRemainingInMag != projectilesPerMag) {
			StartCoroutine (AnimateReload ());

		}
	}

	IEnumerator AnimateReload() {
		//on reload the angle of the gun is changed to simulate the action of reloading
		isReloading = true;
		yield return new WaitForSeconds (.2f);

		float reloadSpeed = 1f / reloadTime;
		float percent = 0;
		Vector3 initialRot = transform.localEulerAngles;
		float maxReloadAngle = 30;

		while (percent < 1) {
			percent += Time.deltaTime * reloadSpeed;
			float interpolation = (-Mathf.Pow(percent,2) + percent) * 4;
			float reloadAngle = Mathf.Lerp(0, maxReloadAngle, interpolation);
			transform.localEulerAngles = initialRot + Vector3.left * reloadAngle;

			yield return null;
		}


		isReloading = false;
		projectilesRemainingInMag = projectilesPerMag;
	}

	public void Shoot(Vector3 shoot) {
		//for aim positioning
		if (!isReloading) {
			transform.LookAt (shoot);
		}
	}

	public void OnTriggerHold() {
		//for burst firing
		Shoot ();
		triggerReleasedSinceLastShot = false;
	}

	public void OnTriggerRelease() {
		//reset the burst
		triggerReleasedSinceLastShot = true;
		shotsRemainingInBurst = burstCount;
	}
}
