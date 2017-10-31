using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent (typeof (playerController))]
public class player : MonoBehaviour {


	public float moveSpeed = 5;

	Camera viewCamera;
	playerController controller;

	void Start () {
		controller = GetComponent<playerController> ();
		viewCamera = Camera.main;
	}

	void Update () {
		Vector3 moveInput = new Vector3 (Input.GetAxisRaw ("Horizontal"), 0, Input.GetAxisRaw ("Vertical"));
		Vector3 moveVelocity = moveInput.normalized * moveSpeed;
		controller.Move (moveVelocity);

		Ray tracker = viewCamera.ScreenPointToRay (Input.mousePosition);
		Plane ground = new Plane (Vector3.up, Vector3.zero);
		float trackerDistance;

		// assings trackerDistance iff ray intersects with plane
		if (ground.Raycast (tracker, out trackerDistance)) {
			Vector3 point = tracker.GetPoint(trackerDistance);
			controller.LookAt (point);
		}
	}
}
