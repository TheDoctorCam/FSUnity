using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//Adds in the flash seen when gun is fired


public class MuzzleFlash : MonoBehaviour {

	public GameObject flashHolder;
	public Sprite[] flashSprites;
	public SpriteRenderer[] spriteRenderers;

	public float flashTime;
	
	void Start() {
		Deactivate ();
	}

		//on mouse click, when a player shoots, the muzzle flash is also activated
	public void Activate() {
		flashHolder.SetActive (true);

		int flashSpriteIndex = Random.Range (0, flashSprites.Length);
		for (int i =0; i < spriteRenderers.Length; i ++) {
			spriteRenderers[i].sprite = flashSprites[flashSpriteIndex];
		}

		Invoke ("Deactivate", flashTime);
	}

	void Deactivate() {
		flashHolder.SetActive (false);
	}
}
