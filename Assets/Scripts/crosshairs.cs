using UnityEngine;
using System.Collections;

public class crosshairs : MonoBehaviour {
	
	public LayerMask targetMask;
	public SpriteRenderer dot;
	public Color dotHighlightColour;
	Color originalDotColour;


	void Start() {
		Cursor.visible = false;
		originalDotColour = dot.color;
	}
	
	void Update () {
		transform.Rotate (Vector3.forward * -45 * Time.deltaTime);
		
	}

	public void DetectTargets(Ray tracker) {
		if (Physics.Raycast (tracker, 100, targetMask)) {
			dot.color = dotHighlightColour;
		} else {
			dot.color = originalDotColour;
		}
	}
}