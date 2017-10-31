using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent (typeof (playerController))]
public class player : MonoBehaviour {


	public float moveSpeed = 5;
	playerController controller;

	void Start () {
		controller = GetComponent<playerController> ();
	}

	void Update () {
		Vector3 moveInput = new Vector3 (Input.GetAxisRaw ("Horizontal"), 0, Input.GetAxisRaw ("Vertical"));
		Vector3 moveVelocity = moveInput.normalized * moveSpeed;
		controller.Move (moveVelocity);
	}
}
