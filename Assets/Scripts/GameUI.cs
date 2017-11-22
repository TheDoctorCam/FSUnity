using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameUI : MonoBehaviour {

	public Image fadeScreen;
	public GameObject gameOverUI;

	void Start () {
		FindObjectOfType<player> ().enemyDeath += endGame;
	}

	void endGame() { //  will fade screen to black and show the game over message
		StartCoroutine(Fade (Color.clear, Color.black,1));
		gameOverUI.SetActive(true);
	}
	IEnumerator Fade(Color from, Color to, float time) { // handles the fading of the screen
		float speed = 1 / time;
		float percent = 0;

		while (percent < 1) {
			percent += Time.deltaTime * speed;
			fadeScreen.color = Color.Lerp(from,to,percent);
			yield return null;
		}
	}

	public void StartNewGame() {
		Application.LoadLevel("Enemy Scene");
	}
}