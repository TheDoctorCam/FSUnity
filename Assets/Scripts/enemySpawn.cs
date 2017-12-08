using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class enemySpawn : MonoBehaviour {

	public bool devMode;

	public Wave[] waves;		//The number of waves 
	public Enemy enemy;			//Generated enemy type per wave

	character player;			//Player character object used to set alive/dead state
	Transform playerTransform;

	Wave currentWave;			//Current wave properties (enemiesToSpawn, enemiesActive)
	int currentWaveNumber;		//Current wave number

	int enemiesRemainingToSpawn;			//number of enemies that will be spawned
	int enemiesActive;			//number of enemies with state !dead
	float nextSpawnTime;		//time netween spawned enemies

	levelGenerator level;		//reference to levelGenerator class to get open grid coordinates

	float playerMovementCheck  = 2;	//time between check to see if player has moved from previous coordinates
	float campThresholdDistance = 1.5f; //Minimum units required to not trigger anti-camping methods
	float nextMovementCheck;		//next time check
	Vector3 oldPlayerPosition;		//coordinates of the player after the last check
	bool isCamping;				//True if the player's coordinate <= movementThreshold

	bool isDead;				//True if the playerTransform had died

	public event System.Action<int> OnNewWave;

	void Start() {
		player = FindObjectOfType<Player> ();
		playerTransform = player.transform;

		nextMovementCheck = playerMovementCheck  + Time.time;
		oldPlayerPosition = playerTransform.position;
		player.enemyDeath += playerDeath;

		level = FindObjectOfType<levelGenerator> ();
		NextWave ();
	}

	void Update() {
		if (!isDead) {
			if (Time.time > nextMovementCheck) {
				nextMovementCheck = Time.time + playerMovementCheck;

				isCamping = (Vector3.Distance (playerTransform.position, oldPlayerPosition) < campThresholdDistance);
				oldPlayerPosition = playerTransform.position;
			}

			if ((enemiesRemainingToSpawn > 0 || currentWave.infinite) && Time.time > nextSpawnTime) {
				enemiesRemainingToSpawn--;
				nextSpawnTime = Time.time + currentWave.spawnTime;

				StartCoroutine ("spawnEnemy");
			}
		}

		if (devMode) {
			if (Input.GetKeyDown(KeyCode.Return)) {
				StopCoroutine("spawnEnemy");
				foreach (Enemy enemy in FindObjectsOfType<Enemy>()) {
					GameObject.Destroy(enemy.gameObject);
				}
				NextWave();
			}
		}
	}
	
	/* Spawns enemy to open grid object and flash it's material before spawning */
	IEnumerator spawnEnemy() {
		float spawnDelay = 1;		//time between grid object flashing and enemy spawned
		float gridFlashSpeed = 4;	//amount of flashes per second 

		Transform spawnTile = level.getRandomOpenCoordinate ();
		if (isCamping) {
			spawnTile = level.gridFromCurrentPosition(playerTransform.position);
		}
		
		Material tileMat = spawnTile.GetComponent<Renderer> ().material;
		Color initialColour = Color.white;	//Store original grid color
		Color flashColour = Color.red;		//Set to desired color of flash
		float spawnTimer = 0; 					//time spent since coroutine started 

		while (spawnTimer < spawnDelay) {

			tileMat.color = Color.Lerp(initialColour,flashColour, Mathf.PingPong(spawnTimer * gridFlashSpeed, 1));

			spawnTimer += Time.deltaTime;
			yield return null;
		}

		Enemy spawnedEnemy = Instantiate(enemy, spawnTile.position + Vector3.up, Quaternion.identity) as Enemy;
		spawnedEnemy.enemyDeath += enemyDeath;
		spawnedEnemy.SetCharacteristics (currentWave.moveSpeed, currentWave.hitsToKillPlayer, currentWave.enemyHealth, currentWave.skinColour);
	}
	
	/* Player death state set to avoid the enemies from looking for a null transform */
	void playerDeath() {
		isDead = true;
	}

	void enemyDeath() {
		enemiesActive --;

		if (enemiesActive == 0) {
			NextWave();
		}
	}

	void ResetPlayerPosition() {
		playerTransform.position = level.gridFromCurrentPosition (Vector3.zero).position + Vector3.up * 3;
	}

	void NextWave() {
		currentWaveNumber++; // increment wave counter

		if (currentWaveNumber - 1 < waves.Length) {
			currentWave = waves [currentWaveNumber - 1];
			enemiesRemainingToSpawn = currentWave.enemyCount;
			enemiesActive = enemiesRemainingToSpawn;

			if (OnNewWave != null) {
				OnNewWave(currentWaveNumber);
			}
			ResetPlayerPosition();
		}
	}

	[System.Serializable]
	public class Wave {
		public bool infinite;
		public int enemyCount;
		public float spawnTime;
		public float moveSpeed;
		public int hitsToKillPlayer;
		public float enemyHealth;
		public Color skinColour;
	}

}
