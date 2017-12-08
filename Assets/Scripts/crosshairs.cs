using UnityEngine;
using System.Collections;

public class crosshairs : MonoBehaviour {

	public LayerMask target;
	public int crosshair_movement_speed = 10;	//this controls the speed of the crosshair
	public float crosshair_refresh = 10f;		//reset speed of crosshair refresh
	public SpriteRenderer dot;				//for aim
	public float vector_seeding = 1;		//to seed the counter
	public Color the_highlighter;				//red
	Color d_color;

	void Start() {
		Cursor.visible = false;
		d_color = dot.color;
	}
	/*
	update the crosshairs as enemy becomes present
	*/
	void Update () {
		transform.Rotate (Vector3.forward * -40 * Time.deltaTime);
	}

	public void DetectTargets(Ray ray) {
		if (Physics.Raycast (ray, 100, target)) {
			dot.color = the_highlighter;
		}
	//The god effect

		else {
			dot.color = d_color;
		}
	}
}
