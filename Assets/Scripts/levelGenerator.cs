using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class levelGenerator : MonoBehaviour {

	public Transform gridPrefab;		//prefab for each grid object
	public Transform obstaclePrefab;	//prefab for each obstacle object
	public Vector2 levelSize;			//size of level
	public int seed = 10;				//seed value for Fisher-Yates shuffle

	[Range(0,1)]
	public float gridBorderPercent;		//percent of outline width per grid object generated

	List<coordinate> levelGridCoordinates;			//coordinates of each grid object
	Queue<coordinate> shuffledGridCoordinates;		//coordinates of each grid object after Fishcer-Yates shuffle

	void Start(){

		GenerateLevel ();
	}

	public void GenerateLevel(){

		levelGridCoordinates = new List<coordinate> ();
		for (int x = 0; x < levelSize.x; x++) {
			for (int y = 0; y < levelSize.y; y++) {
				levelGridCoordinates.Add (new coordinate (x, y));
			}
		}
		shuffledGridCoordinates = new Queue<coordinate> (utility.fisherYatesShuffle (levelGridCoordinates.ToArray(), seed));		//returns shuffled coordinates

		string levelName = "Generated Level";	//used to store generated level object in Unity 
		//Destroy generated level while running dynamically IF new level variables are entered (size, gridBorder, etc)
		if (transform.Find (levelName)) {
			DestroyImmediate (transform.Find (levelName).gameObject);
		}

		Transform levelHolder = new GameObject (levelName).transform;
		levelHolder.parent = transform;

		/* Generates tiles for the level */
		for (int x = 0; x < levelSize.x; x++) {
			for (int y = 0; y < levelSize.y; y++) {
				//Creates initial grid starting position
				Vector3 startPosition = coordinateToPosition(x,y);
				//Adds grid prefab to startPosition, rotating the grid object 90 degreesm to face the camera 
				Transform newGrid = Instantiate(gridPrefab, startPosition, Quaternion.Euler(Vector3.right * 90)) as Transform;
				newGrid.localScale = Vector3.one * (1 - gridBorderPercent);
				newGrid.parent = levelHolder;
			}
		}

		int obstacleCount = 10;
		for (int i = 0; i < obstacleCount; i++) {
			coordinate randomCoordinate = getRandomCoordinate ();
			Vector3 obstaclePosition = coordinateToPosition (randomCoordinate.x, randomCoordinate.y);
			Transform newObstacle = Instantiate (obstaclePrefab, obstaclePosition + Vector3.up * .5f, Quaternion.identity) as Transform;
			newObstacle.parent = levelHolder;
		}
	}

	/* Helper function to return the coordinates of a grid object */
	Vector3 coordinateToPosition (int x, int y){
		return new Vector3 (-levelSize.x / 2 + 0.5f + x, 0, -levelSize.y / 2 + 0.5f + y);
	}

	/* Dequeue and return first shuffled grid coordinate from shuffled queue */
	public coordinate getRandomCoordinate(){
		coordinate randomCoordinate = shuffledGridCoordinates.Dequeue ();
		shuffledGridCoordinates.Enqueue (randomCoordinate);
		return randomCoordinate;
	}

	/* Generate (x,y) coordinate of grid  objects in order to place 
	 * the randomly generated objects
	 */
	public struct coordinate{
		public int x;	//x-axis value
		public int y;	//y-axis value

		public coordinate(int _x, int _y){
			x = _x;
			y = _y;
		}
	}
}
