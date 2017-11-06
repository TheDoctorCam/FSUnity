using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour, iDamageable {

	public float startHealth;
	protected float health;
	protected bool dead;

	public event System.Action enemyDeath;	//This function is used to note when one enemy spawn wave is complete

	protected virtual void Start() {
		health = startHealth;
	}

	/* TakeDamage passes in damage to be deducted and RaycastHit to instantiate particle effect */
	public void TakeDamage(float damage, RaycastHit hit){
		ApplyDamage (damage);
	}

	/* ApplyDamage deduced the passed in damage from the players current health */
	public void ApplyDamage(float damage){
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
