using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemySpawn : MonoBehaviour {

	public Wave[] waves;
	public Enemy enemy;

	Wave currentWave;
	int currentWaveNumber;

	int enemiesToSpawn;		//number of enemies that will be spawned
	int enemiesActive;
	float nextSpawnTime;

	void Start(){
		NextWave ();
	}


	void Update() {
		if (enemiesToSpawn > 0 && Time.time > nextSpawnTime) {
			enemiesToSpawn--;
			nextSpawnTime = Time.time + currentWave.spawnTime;

			Enemy spawnedEnemy = Instantiate (enemy, Vector3.zero, Quaternion.identity) as Enemy;
			spawnedEnemy.enemyDeath += enemyDeath;
		}
	}


	void enemyDeath(){
		enemiesActive--;

		if (enemiesActive == 0) {
			NextWave ();
		}
	}

	void NextWave(){
		currentWaveNumber++;
		if (currentWaveNumber - 1 < waves.Length) {
			currentWave = waves [currentWaveNumber - 1];

			enemiesToSpawn = currentWave.enemyCount;
			enemiesActive = enemiesToSpawn;
		}
	}


	[System.Serializable]
	public class Wave {
		public int enemyCount;
		public float spawnTime;
	}
}
