using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour {

	public GameObject mainMenuHolder;


	public Toggle[] resolutionToggles;
	public Toggle fullscreenToggle;
	public int[] screenWidths;
	int activeScreenResIndex;

	void Start() {
	}


	public void Play() {
		SceneManager.LoadScene ("Game");
	}

	public void Quit() {
		Application.Quit ();
	}

	public void MainMenu() {
		mainMenuHolder.SetActive (true);
	}




}
