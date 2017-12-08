using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent (typeof (playerController))]
[RequireComponent (typeof (gunController))]
public class Player : character {

	public float moveSpeed = 5;

	public crosshairs crosshairs;

	Camera viewCamera;
	playerController controller;
	gunController gunControl;
	
	protected override void Start () {
		base.Start ();
	}

	//introduced in third iteration
	
	void Awake() {
		controller = GetComponent<playerController> ();
		gunControl = GetComponent<gunController> ();
		viewCamera = Camera.main;
		FindObjectOfType<enemySpawn> ().OnNewWave += OnNewWave;
	}

	
	//the players health
	void OnNewWave(int waveNumber) {
		health = startHealth;
		gunControl.Equip (waveNumber - 1);
	}

	void Update () {
		// Movement input
		Vector3 moveInput = new Vector3 (Input.GetAxisRaw ("Horizontal"), 0, Input.GetAxisRaw ("Vertical"));
		Vector3 moveVelocity = moveInput.normalized * moveSpeed;
		controller.Move (moveVelocity);

		// Look input
		Ray ray = viewCamera.ScreenPointToRay (Input.mousePosition);
		Plane groundPlane = new Plane (Vector3.up, Vector3.up * gunControl.GunHeight);
		float rayDistance;

		if (groundPlane.Raycast(ray,out rayDistance)) {
			Vector3 point = ray.GetPoint(rayDistance);
			controller.LookAt(point);
			crosshairs.transform.position = point;
			crosshairs.DetectTargets(ray);
			if ((new Vector2(point.x, point.z) - new Vector2(transform.position.x, transform.position.z)).sqrMagnitude > 1) {
				gunControl.Shoot(point);
			}
		}

		// Weapon input
		if (Input.GetMouseButton(0)) {
			gunControl.OnTriggerHold();
		}
		if (Input.GetMouseButtonUp(0)) {
			gunControl.OnTriggerRelease();
		}
		if (Input.GetKeyDown (KeyCode.R)) {
			gunControl.Reload();
		}

		if (transform.position.y < -10) {
			TakeDamage (health);
		}
	}

	public override void Death ()
	{
		base.Death ();
	}
		
}
