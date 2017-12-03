using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class character : MonoBehaviour, iDamageable {

	public float startHealth;	//starting health of character extended children
	protected float health;		//current health of character extended child
	protected bool dead;		//true if the characters health is <= 0

	public event System.Action enemyDeath;	//This function is used to note when one enemy spawn wave is complete

	protected virtual void Start() {
		health = startHealth;
	}

	/* TakeDamage passes in damage to be deducted and RaycastHit to instantiate particle effect */
	public virtual void TakeDamage(float damage, Vector3 hitPoint, Vector3 hitDirection){
		ApplyDamage (damage);
	}

	/* ApplyDamage deduced the passed in damage from the players current health */
	public virtual void ApplyDamage(float damage){
		health -= damage;

		if (health < 1) {
			Death ();
		}
	}

	/* Death if player's health <= 0 */
	protected void Death() {
		dead = true;
		if (enemyDeath != null) {
			enemyDeath ();
		}
		GameObject.Destroy (gameObject);
	}
}
