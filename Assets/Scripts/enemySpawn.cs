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

	levelGenerator level;		//reference to levelGenerator class to get open grid coordinates

	void Start(){
		level = FindObjectOfType<levelGenerator> ();
		NextWave ();
	}


	void Update() {
		if (enemiesToSpawn > 0 && Time.time > nextSpawnTime) {
			enemiesToSpawn--;
			nextSpawnTime = Time.time + currentWave.spawnTime;

			StartCoroutine (spawnEnemy ());
		}
	}

	/* Spawns enemy to open grid object and flash it's material before spawning */
	IEnumerator spawnEnemy() {
		float spawnDelay = 1;		//time between grid object flashing and enemy spawned
		float gridFlashSpeed = 4;	//amount of flashes per second 


		Transform randomGrid = level.getRandomOpenCoordinate ();
		Material gridColor = randomGrid.GetComponent<Renderer> ().material;
		Color initialColor = gridColor.color;	//Store original grid color
		Color flashingColor = Color.red;		//Set to desired color of flash
		float spawnTimer = 0;					//time spent since coroutine started 

		while (spawnTimer < spawnDelay) {
			gridColor.color = Color.Lerp (initialColor, flashingColor, Mathf.PingPong (spawnTimer * gridFlashSpeed, 1));	//Mathf.PingPong() bounces between two passed in values

			spawnTimer += Time.deltaTime;
			yield return null;
		}
			
		Enemy spawnedEnemy = Instantiate (enemy, randomGrid.position + Vector3.up, Quaternion.identity) as Enemy;
		spawnedEnemy.enemyDeath += enemyDeath;
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
