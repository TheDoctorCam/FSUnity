using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemySpawn : MonoBehaviour {

	public Wave[] waves;		//The number of waves 
	public Enemy enemy;			//Generated enemy type per wave

	Wave currentWave;			//Current wave properties (enemiesToSpawn, enemiesActive)
	int currentWaveNumber;		//Current wave number

	int enemiesToSpawn;			//number of enemies that will be spawned
	int enemiesActive;			//number of enemies with state !dead
	float nextSpawnTime;		//time netween spawned enemies

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
