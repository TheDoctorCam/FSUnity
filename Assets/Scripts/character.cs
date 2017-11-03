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

	public void TakeDamage(float damage, RaycastHit hit){
		health -= damage;

		if (health < 1) {
			Death ();
		}
	}

	protected void Death() {
		dead = true;
		if (enemyDeath != null) {
			enemyDeath ();
		}
		GameObject.Destroy (gameObject);
	}
}
