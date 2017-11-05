using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class levelGenerator : MonoBehaviour {

	public Transform gridPrefab;		//controls the tiled grid of the map
	public Vector2 levelSize;			//size of level

	[Range(0,1)]
	public float gridBorderPercent;		//percent of outline width per grid object generated


	void Start(){

		GenerateLevel ();
	}

	public void GenerateLevel(){

		string levelName = "Generated Level";	//used to store generated level object in Unity 
		//Destroy generated level while running dynamically IF new level variables are entered (size, gridBorder, etc)
		if (transform.FindChild (levelName)) {
			DestroyImmediate (transform.FindChild (levelName).gameObject);
		}

		Transform levelHolder = new GameObject (levelName).transform;
		levelHolder.parent = transform;

		for (int x = 0; x < levelSize.x; x++) {
			for (int y = 0; y < levelSize.y; y++) {
				//Creates initial grid starting position
				Vector3 startPosition = new Vector3 (-levelSize.x / 2 + 0.5f + x, 0, -levelSize.y / 2 + 0.5f + y);
				//Adds grid prefab to startPosition, rotating the grid object 90 degreesm to face the camera 
				Transform newGrid = Instantiate(gridPrefab, startPosition, Quaternion.Euler(Vector3.right * 90)) as Transform;
				newGrid.localScale = Vector3.one * (1 - gridBorderPercent);
				newGrid.parent = levelHolder;
			}
		}
	}
}
