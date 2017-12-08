using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour {

	//Add the game objects and other user components
	//this will also define functionality calling other classes
	//we removed our previous fading system for simplicity

	public GameObject allUI;
	public GameObject gameOverUI;

	public RectTransform newWaveBanner;
	public Text newWaveTitle;
	public Text newWaveEnemyCount;
	public Text scoreUI;
	public Text gameOverScoreUI;
	public RectTransform healthBar;

	
	
	enemySpawn spawner;		//where the enemy spawns
	Player player;

	void Start () {
		allUI.SetActive (true);
		player = FindObjectOfType<Player> ();
		player.enemyDeath += OnGameOver;
	}

	
	//Adds new wave of enemies
	void Awake() {
		spawner = FindObjectOfType<enemySpawn> ();
		spawner.OnNewWave += OnNewWave;
	}

	
	//updating health bar in conjunction with health
	void Update() {
		scoreUI.text = ScoreKeeper.score.ToString("D6");
		float healthPercent = 0;
		if (player != null) {
			healthPercent = player.health / player.startHealth;
		}
		healthBar.localScale = new Vector3 (healthPercent, 1, 1);
	}

	
	//when a new wave of enemies come
	void OnNewWave(int waveNumber) {
		string[] numbers = { "One", "Two", "Three", "Four", "Five" };
		newWaveTitle.text = "- Wave " + numbers [waveNumber - 1] + " -";
		string enemyCountString = ((spawner.waves [waveNumber - 1].infinite) ? "Infinite" : spawner.waves [waveNumber - 1].enemyCount + "");
		newWaveEnemyCount.text = "Enemies: " + enemyCountString;

		StopCoroutine ("AnimateNewWaveBanner");
		StartCoroutine ("AnimateNewWaveBanner");
	}
	//when death	
	void OnGameOver() {
		Cursor.visible = true;
		gameOverScoreUI.text = scoreUI.text;
		scoreUI.gameObject.SetActive (false);
		healthBar.transform.parent.gameObject.SetActive (false);
		gameOverUI.SetActive (true);
	}

	
	//game physics which is later defined in metadata
	IEnumerator AnimateNewWaveBanner() {

		float delayTime = 1.5f;
		float speed = 3f;
		float animatePercent = 0;
		int dir = 1;

		float endDelayTime = Time.time + 1 / speed + delayTime;

		while (animatePercent >= 0) {
			animatePercent += Time.deltaTime * speed * dir;

			if (animatePercent >= 1) {
				animatePercent = 1;
				if (Time.time > endDelayTime) {
					dir = -1;
				}
			}

			newWaveBanner.anchoredPosition = Vector2.up * Mathf.Lerp (-170, 45, animatePercent);
			yield return null;
		}

	}


	// UI Input
	public void StartNewGame() {
		SceneManager.LoadScene ("Game");
	}

	public void ReturnToMainMenu() {
		SceneManager.LoadScene ("Menu");
	}
}
