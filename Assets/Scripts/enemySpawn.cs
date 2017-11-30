using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemySpawn : MonoBehaviour {

	public Wave[] waves;		//The number of waves 
	public Enemy enemy;			//Generated enemy type per wave

	character player;			//Player character object used to set alive/dead state
	Transform playerTransform;
	bool isDead;				//True if the player had died


	Wave currentWave;			//Current wave properties (enemiesToSpawn, enemiesActive)
	int currentWaveNumber;		//Current wave number

	int enemiesToSpawn;			//number of enemies that will be spawned
	int enemiesActive;			//number of enemies with state !dead
	float nextSpawnTime;		//time netween spawned enemies

	float playerMovementCheck = 2;	//time between check to see if player has moved from previous coordinates
	float movementThreshold = 1.5f; //Minimum units required to not trigger anti-camping methods
	float nextMovementCheck;		//next time check
	Vector3 oldPlayerPosition;		//coordinates of the player after the last check
	bool isCamping;					//True if the player's coordinate <= movementThreshold


	levelGenerator level;		//reference to levelGenerator class to get open grid coordinates


	void Start(){
		player = FindObjectOfType<player> ();
		playerTransform = player.transform;

		nextMovementCheck = playerMovementCheck + Time.time;
		oldPlayerPosition = playerTransform.position;
		player.enemyDeath += playerDeath;

		level = FindObjectOfType<levelGenerator> ();
		NextWave ();
	}


	void Update() {

		if (!isDead) {
			if (Time.time > nextMovementCheck) {
				nextMovementCheck = Time.time + playerMovementCheck;

				isCamping = (Vector3.Distance (playerTransform.position, oldPlayerPosition) < movementThreshold);
				oldPlayerPosition = playerTransform.position;
			}
				
			if (enemiesToSpawn > 0 && Time.time > nextSpawnTime) {
				enemiesToSpawn--;
				nextSpawnTime = Time.time + currentWave.spawnTime;

				StartCoroutine (spawnEnemy ());
			}
		}
	}

	/* Spawns enemy to open grid object and flash it's material before spawning */
	IEnumerator spawnEnemy() {
		float spawnDelay = 1;		//time between grid object flashing and enemy spawned
		float gridFlashSpeed = 4;	//amount of flashes per second 


		Transform spawnTile = level.getRandomOpenCoordinate ();
		if (isCamping) {
			spawnTile = level.gridFromCurrentPosition (playerTransform.position);
		}
		Material gridColor = spawnTile.GetComponent<Renderer> ().material;
		Color initialColor = gridColor.color;	//Store original grid color
		Color flashingColor = Color.red;		//Set to desired color of flash
		float spawnTimer = 0;					//time spent since coroutine started 

		while (spawnTimer < spawnDelay) {
			gridColor.color = Color.Lerp (initialColor, flashingColor, Mathf.PingPong (spawnTimer * gridFlashSpeed, 1));	//Mathf.PingPong() bounces between two passed in values

			spawnTimer += Time.deltaTime;
			yield return null;
		}
			
		Enemy spawnedEnemy = Instantiate (enemy, spawnTile.position + Vector3.up, Quaternion.identity) as Enemy;
		spawnedEnemy.enemyDeath += enemyDeath;
	}

	/* Player death state set to avoid the enemies from looking for a null transform */
	void playerDeath(){
		isDead = true;
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
