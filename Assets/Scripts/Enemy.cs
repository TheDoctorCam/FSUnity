using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

[RequireComponent (typeof (NavMeshAgent))]
public class Enemy : Character {

	public enum State {Idle, Chasing, Attacking };
	State currentState;

	NavMeshAgent pathfinder;
	Transform target;
	Material enemySkin;

	Color enemyColor;					//stores enemies original color for attacking animation
		
	float attackDistance = 1.5f;		//distance that the enemy can lunge at the player
	float attackTime = 1;				//how long the attack takes
	float attackCooldown;				//time between attacks 
	float playerCollisionRadius;		//Collision detection variable for the distance around the player
	float targetCollisionRadius;		//Collision detection variable for the distance arounf the target

	protected override void Start () {
		base.Start ();
		pathfinder = GetComponent<NavMeshAgent> ();	
		enemySkin = GetComponent<Renderer> ().material;
		enemyColor = enemySkin.color;

		currentState = State.Chasing;
		target = GameObject.FindGameObjectWithTag ("Player").transform;

		playerCollisionRadius = GetComponent<CapsuleCollider> ().radius;
		targetCollisionRadius = target.GetComponent<CapsuleCollider> ().radius;

		StartCoroutine (UpdatePath ());
	}
	
	void Update () {
		if(Time.time > attackCooldown){
		float distanceToTarget = (target.position - transform.position).sqrMagnitude; //reduces need to computer two squared distances

			if (distanceToTarget < Mathf.Pow(attackDistance + playerCollisionRadius + targetCollisionRadius, 2)){
				attackCooldown = Time.time + attackTime;
				StartCoroutine	(Attack ());
			}
		}
	}

	/*This function attacks the player with a lunge to the players position and then returns back to previous position */
	IEnumerator Attack(){
		currentState = State.Attacking;
		pathfinder.enabled = false;

		Vector3 startingPosition = transform.position;
		Vector3 directionToTarget = (target.position - transform.position).normalized;
		Vector3 attackPosition = target.position - directionToTarget * (playerCollisionRadius);

		float attackSpeed = 3;
		float actionPercent = 0;	//percent of attack action animation completed

		enemySkin.color = Color.red;

		while (actionPercent <= 1) {
			actionPercent += Time.deltaTime * attackSpeed;
			float parabolicAttack = (-actionPercent * actionPercent + actionPercent )*4;	//parabolic function returns actionPercent from 0 -> 100 -> 0
			transform.position = Vector3.Lerp(startingPosition, attackPosition, parabolicAttack);

			yield return null;
		}

		enemySkin.color = enemyColor;
		currentState = State.Chasing;
		pathfinder.enabled = true;
	}


	/*This function controls the tracking refresh rate of enemies*/
	IEnumerator UpdatePath() {
		float refreshRate = .25f;

		while (target != null) {
			if (currentState == State.Chasing) {
				Vector3 directionToTarget = (target.position - transform.position).normalized;
				Vector3 targetPosition = target.position - directionToTarget * (playerCollisionRadius + targetCollisionRadius + attackDistance/2);
				if (!dead) {
					pathfinder.SetDestination (targetPosition);
				}

			}	
			yield return new WaitForSeconds (refreshRate);
		}
	}

}