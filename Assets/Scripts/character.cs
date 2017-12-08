using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class character : MonoBehaviour, damages {

	public float startHealth; //starting health of character extended children
	public float health { get; protected set; } //current health of character extended child
	protected bool dead; //true if the characters health is <= 0

	public event System.Action enemyDeath;

	protected virtual void Start() {		//to initiate the health
		health = startHealth;
	}

	public virtual void TakeHit(float damage, Vector3 hitPoint, Vector3 hitDirection) {
		// Do some stuff here with hit var
		TakeDamage (damage);
	}

	public virtual void TakeDamage(float damage) {
		health -= damage;
		
		if (health < 1 && !dead) { // if no health, then dead
			Death();
		}
	}

	/* Death if player's health <= 0 */
	public virtual void Death() {
		dead = true; // if dead, then dead!
		if (enemyDeath != null) { 
			enemyDeath();
		}
		GameObject.Destroy (gameObject);
	}
}
