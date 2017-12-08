using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//this adds in the effect of shell ejection for shell object
//will be called simulataneously with bullet
public class Shell : MonoBehaviour {

	public Rigidbody myRigidbody;
	public float forceMin;
	public float forceMax; 
	float lifetime = 4;
	float howLong = 2;

	void Start () {
		float strength = Random.Range (forceMin, forceMax);
		myRigidbody.AddForce (transform.right * strength);
		myRigidbody.AddTorque (Random.insideUnitSphere * strength);

		StartCoroutine (Fade ());
	}
	
	IEnumerator Fade() { // controls fading of the shells
		yield return new WaitForSeconds(lifetime);
		float percent = 0;
		float fadeSpeed = 1 / howLong;
		Material newMat = GetComponent<Renderer> ().material;
		Color initialColour = newMat.color;

		while (percent < 1){
			percent += Time.deltaTime * fadeSpeed;
			newMat.color = Color.Lerp(initialColour, Color.clear, percent);
			yield return null;
		}
		Destroy (gameObject);
	}
}
