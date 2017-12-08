using UnityEngine;
using System.Collections.Generic;
using System.Collections;


//how score is kept in the game and displayed through the UI
public class ScoreKeeper : MonoBehaviour {

	public static int score { get; private set; }
	float lastEnemyKillTime;
	int streakCount;
	float streakExpiryTime = 1;

	void Start() {
		score = 0; // start score at 0 with each game
		Enemy.enemyDeathStatic += OnEnemyKilled;
		FindObjectOfType<Player> ().enemyDeath += OnPlayerDeath;
	}

	
	//once the enemy is killed update the score
	void OnEnemyKilled() {
		if (Time.time < lastEnemyKillTime + streakExpiryTime) {
			streakCount++;
		} else {
			streakCount = 0;
		}

		lastEnemyKillTime = Time.time;

		score += 3 + 2 * streakCount;
	}

	void OnPlayerDeath() {
		
		Enemy.enemyDeathStatic -= OnEnemyKilled;
	}
	
}
