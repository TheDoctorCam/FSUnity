using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour {

	public Image fadeScreen;
	public GameObject gameOverUI;

	void Start () {
		FindObjectOfType<player> ().enemyDeath += endGame;
	}

	void endGame() { //  will fade screen to black and show the game over message
		StartCoroutine(Fade (Color.clear, Color.black,1));
		Cursor.visible = true; // make mouse cursor visible again
		gameOverUI.SetActive(true);
	}
	
	IEnumerator Fade(Color from, Color to, float time) {
		float speed = 1 / time;
		float percent = 0;

		while (percent < 1) {
			percent += Time.deltaTime * speed;
			fadeScreen.color = Color.Lerp(from,to,percent);
			yield return null;
		}
	}

	public void StartNewGame() {
		SceneManager.LoadScene ("Enemy Scene");
	}
}