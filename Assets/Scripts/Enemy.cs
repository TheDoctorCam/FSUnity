using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

[RequireComponent (typeof (UnityEngine.AI.NavMeshAgent))]
public class Enemy : character {

	public ParticleSystem explosion_effect;
	public static event System.Action enemyDeathStatic;

	[RequireComponent (typeof (NavMeshAgent))
	public enum State {Idle, Chasing, Attacking};  //enum type State to set players animation
	State currentState;


	UnityEngine.AI.NavMeshAgent pathfinder;			//provides target via player's mesh
	Transform target;								//designates target's position as they move
	character targetCharacter;						//detects when target has died after attack
	Material enemySkin;

	Color enemyColor;								//stores enemies original color for attacking animation

	float attackDistance = .5f;			//distance that the enemy can lunge at the player
	float attackTime = 1;				//how long the attack takes
	float attackCooldown;				//time between attacks 
	float damage = 1;					//amount damage to player per enemy attack 
	float playerCollisionRadius;		//Collision detection variable for the distance around the player
	float targetCollisionRadius;		//Collision detection variable for the distance arounf the target

	bool hasTarget;						//True if the enemy is currently tracking a target

	void Awake() {
		pathfinder = GetComponent<UnityEngine.AI.NavMeshAgent> ();			//Awaken the enemy so that he can run
		
		if (GameObject.FindGameObjectWithTag ("Player") != null) {
			hasTarget = true;
			
			target = GameObject.FindGameObjectWithTag ("Player").transform;
			targetCharacter = target.GetComponent<character> ();
			
			playerCollisionRadius = GetComponent<CapsuleCollider> ().radius;
			targetCollisionRadius = target.GetComponent<CapsuleCollider> ().radius;			//will establish target
		}
	}
	
	protected override void Start () {							//starts enemy
		base.Start ();

		if (hasTarget) {
			currentState = State.Chasing;
			targetCharacter.enemyDeath += OnTargetDeath;

			StartCoroutine (UpdatePath ());					//create path
		}
	}

	public void SetCharacteristics(float moveSpeed, int hitsToKillPlayer, float enemyHealth, Color skinColour) {		//instantiate the enemy
		pathfinder.speed = moveSpeed;

		if (hasTarget) {
			damage = Mathf.Ceil(targetCharacter.startHealth / hitsToKillPlayer);
		}
		startHealth = enemyHealth;

		//adds to the explosion effects
		
		
		explosion_effect.startColor = new Color (skinColour.r, skinColour.g, skinColour.b, 1);
		enemySkin = GetComponent<Renderer> ().material;
		enemySkin.color = skinColour;
		enemyColor = enemySkin.color;
	}

	public override void TakeHit (float damage, Vector3 hitPoint, Vector3 hitDirection)					//define how enemy health works
	{
		if (damage >= health && !dead) {
			if (enemyDeathStatic != null) {
				enemyDeathStatic ();
			}
			Destroy(Instantiate(explosion_effect.gameObject, hitPoint, Quaternion.FromToRotation(Vector3.forward, hitDirection)) as GameObject, explosion_effect.startLifetime);
		}
		base.TakeHit (damage, hitPoint, hitDirection);
	}

	void OnTargetDeath() {
		hasTarget = false;
		currentState = State.Idle;
	}

	void Update () {				//after the  attack happens

		if (hasTarget) {
			if (Time.time > attackCooldown) {
				float distanceToTarget = (target.position - transform.position).sqrMagnitude;
				if (distanceToTarget < Mathf.Pow (attackDistance + playerCollisionRadius + targetCollisionRadius, 2)) {
					attackCooldown = Time.time + attackTime;
					StartCoroutine (Attack ());
				}

			}
		}

	}
	/*
	Enumeration of the attack
	
	
	
	*/
	IEnumerator Attack() {

		currentState = State.Attacking;
		pathfinder.enabled = false;

		Vector3 originalPosition = transform.position;
		Vector3 dirToTarget = (target.position - transform.position).normalized;
		Vector3 attackPosition = target.position - dirToTarget * (playerCollisionRadius);

		float attackSpeed = 3;
		float actionPercent = 0;

		enemySkin.color = Color.red;
		bool hasDamaged = false;

		while (actionPercent <= 1) {				//definition of what an attack does

			if (actionPercent >= .5f && !hasDamaged) {
				hasDamaged = true;
				targetCharacter.TakeDamage(damage);
			}

			actionPercent += Time.deltaTime * attackSpeed;
			float parabolicAttack = (-Mathf.Pow(actionPercent,2) + actionPercent) * 4;
			transform.position = Vector3.Lerp(originalPosition, attackPosition, parabolicAttack);

			yield return null;
		}

		enemySkin.color = enemyColor;
		currentState = State.Chasing;
		pathfinder.enabled = true;
	}

	IEnumerator UpdatePath() {				//this is a different type of path update
		float refreshRate = .25f;

		while (hasTarget) {
			if (currentState == State.Chasing) {
				Vector3 dirToTarget = (target.position - transform.position).normalized;
				Vector3 targetPosition = target.position - dirToTarget * (playerCollisionRadius + targetCollisionRadius + attackDistance/2);
				if (!dead) {
					pathfinder.SetDestination (targetPosition);
				}
			}
			yield return new WaitForSeconds(refreshRate);
		}
	}
}