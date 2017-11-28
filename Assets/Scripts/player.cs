using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent (typeof (playerController))]
[RequireComponent (typeof (gunController))]
public class player : Character {

	public crosshairs crosshair;	// create crosshair
	
	public float moveSpeed = 5;			//Move speed of player 

	Camera viewCamera;					//FOV of game
	playerController controller;		//WASD 
	gunController gunControl;			//Left-click and right-click 
	

	protected override void Start () {
		base.Start ();
		controller = GetComponent<playerController> ();
		gunControl = GetComponent<gunController> ();
		viewCamera = Camera.main;
	}

	void Update () {
		/* Movement */
		Vector3 moveInput = new Vector3 (Input.GetAxisRaw ("Horizontal"), 0, Input.GetAxisRaw ("Vertical"));
		Vector3 moveVelocity = moveInput.normalized * moveSpeed;
		controller.Move (moveVelocity);

		/* Look */
		Ray tracker = viewCamera.ScreenPointToRay (Input.mousePosition);
		Plane ground = new Plane(Vector3.up, Vector3.up * gunControl.GunHeight);
		float trackerDistance;

		// assings trackerDistance iff ray intersects with plane
		if (ground.Raycast (tracker, out trackerDistance)) {
			Vector3 point = tracker.GetPoint(trackerDistance);
			controller.LookAt (point);
			crosshair.transform.position = point;
			crosshair.DetectTargets(tracker);
		}

		/* Weapon Input */

		//Left click fire function
		if (Input.GetMouseButton (0)) {
			gunControl.Shoot ();
		
		}


	}
}
