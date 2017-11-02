using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour, iDamageable {

	public float startHealth;
	protected float health;
	protected bool dead;

	protected virtual void Start() {
		health = startHealth;
	}

	public void TakeDamage(float damage, RaycastHit hit){
		health -= damage;

		if (health < 1) {
			Death ();
		}
	}

	protected void Death() {
		dead = true;
		GameObject.Destroy (gameObject);
	}
}
