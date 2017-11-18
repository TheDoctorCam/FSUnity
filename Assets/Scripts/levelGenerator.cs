using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class levelGenerator : MonoBehaviour {

	public Transform gridPrefab;		//prefab for each grid object
	public Transform obstaclePrefab;	//prefab for each obstacle object
	public Transform navMeshSize;		//size of the NavMesh floor which the level sits on
	public Transform navMeshBorder;		//prefab for the size of the NavMesh floor
	public Vector2 levelSize;			//size of level
	public Vector2 maxLevelSize;		//maxmimum size of the level , == navMeshSize
	public int seed = 10;				//seed value for Fisher-Yates shuffle

	[Range(0,1)]
	public float gridBorderPercent;		//percent of outline width per grid object generated
	[Range(0,1)]
	public float obstacleCountPercent;  //percent of level populated by obstacles

	public float gridSize;				//controls the dimensions of each grid object

	List<coordinate> levelGridCoordinates;			//coordinates of each grid object
	Queue<coordinate> shuffledGridCoordinates;		//coordinates of each grid object after Fishcer-Yates shuffle

	coordinate levelCenter;				//center coordinates of level (player spawn)

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
		levelCenter = new coordinate( (int)levelSize.x/2, (int)levelSize.y/2);

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
				newGrid.localScale = Vector3.one * (1 - gridBorderPercent) * gridSize;
				newGrid.parent = levelHolder;
			}
		}

		/* this bool is used to make sure that the obstacles never block off sections of the map */
		bool[,] obstacleMap = new bool[(int)levelSize.x, (int)levelSize.y];		
		int obstacleCount = (int) (levelSize.x * levelSize.y * obstacleCountPercent);	//max number of obstacles possible
		int currentObstacleCount = 0;	//current number of obstacles generated


		for (int i = 0; i < obstacleCount; i++) {
			coordinate randomCoordinate = getRandomCoordinate ();
			obstacleMap [randomCoordinate.x, randomCoordinate.y] = true;
			currentObstacleCount++;

			if (randomCoordinate != levelCenter && isLevelAccessible (obstacleMap, currentObstacleCount)) {
				Vector3 obstaclePosition = coordinateToPosition (randomCoordinate.x, randomCoordinate.y);

				Transform newObstacle = Instantiate (obstaclePrefab, obstaclePosition + Vector3.up * .5f, Quaternion.identity) as Transform;
				newObstacle.parent = levelHolder;
				newObstacle.localScale = Vector3.one * (1 - gridBorderPercent) * gridSize;


			} 

			/*If the obstacle cannot be created */
			else {
				obstacleMap [randomCoordinate.x, randomCoordinate.y] = false;
				currentObstacleCount--;
			}
		}

		/* Create Nav Mesh border so that the enemies can't walk around level*/
		Transform leftBorder = Instantiate (navMeshBorder, Vector3.left * (levelSize.x + maxLevelSize.x) / 4 * gridSize, Quaternion.identity) as Transform;
		leftBorder.parent = levelHolder;
		leftBorder.localScale = new Vector3 ((maxLevelSize.x - levelSize.x) / 2, 1, levelSize.y) * gridSize;

		Transform rightBorder = Instantiate (navMeshBorder, Vector3.right * (levelSize.x + maxLevelSize.x) / 4 * gridSize, Quaternion.identity) as Transform;
		rightBorder.parent = levelHolder;
		rightBorder.localScale = new Vector3 ((maxLevelSize.x - levelSize.x) / 2, 1, levelSize.y) * gridSize;


		Transform topBorder = Instantiate (navMeshBorder, Vector3.forward * (levelSize.y + maxLevelSize.y) / 4 * gridSize, Quaternion.identity) as Transform;
		topBorder.parent = levelHolder;
		topBorder.localScale = new Vector3 (maxLevelSize.x, 1, (maxLevelSize.y - levelSize.y) / 2) * gridSize;

		Transform bottomBorder = Instantiate (navMeshBorder, Vector3.back * (levelSize.y + maxLevelSize.y) / 4 * gridSize, Quaternion.identity) as Transform;
		bottomBorder.parent = levelHolder;
		bottomBorder.localScale = new Vector3 (maxLevelSize.x, 1, (maxLevelSize.y - levelSize.y) / 2) * gridSize;

		navMeshSize.localScale = new Vector3 (maxLevelSize.x, maxLevelSize.y) * gridSize;
	}

	/* Test condition to see if the player is able to access all avalailable grids.
	 * Uses: Flood-fill alogrithm
	 * True if: there are no obstacles that will completely border an empty grid
	 */

	bool isLevelAccessible(bool[,] obstacleMap, int numObstacles){
		bool[,] levelFlags = new bool[obstacleMap.GetLength (0), obstacleMap.GetLength (1)];
		Queue<coordinate> queue = new Queue<coordinate> ();
		queue.Enqueue (levelCenter);
		levelFlags [levelCenter.x, levelCenter.y] = true;

		int accessibleGridCount = 1;

		while (queue.Count > 0) {
			coordinate grid = queue.Dequeue ();

			for (int x = -1; x <= 1; x++) {
				for (int y = -1; y <= 1; y++) {
					int neighborX = grid.x + x;
					int neighborY = grid.y + y;
					if (x == 0 || y == 0) {
						if (neighborX >= 0 && neighborX < obstacleMap.GetLength (0) && neighborY >= 0 && neighborY < obstacleMap.GetLength (1)) {
							if (!levelFlags [neighborX, neighborY] && !obstacleMap [neighborX, neighborY]) {
								levelFlags [neighborX, neighborY] = true;
								queue.Enqueue (new coordinate (neighborX, neighborY));
								accessibleGridCount++;
			
							}
						}
					}
				}
			}
		}

		int targetGridCount = (int)(levelSize.x * levelSize.y - numObstacles);
		return targetGridCount == accessibleGridCount;

	}


	// Helper function to return the coordinates of a grid object
	Vector3 coordinateToPosition (int x, int y){
		return new Vector3 (-levelSize.x / 2 + 0.5f + x, 0, -levelSize.y / 2 + 0.5f + y) * gridSize;
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

		public static bool operator == (coordinate c1, coordinate c2) {
			return c1.x == c2.x && c1.y == c2.y;
		}

		public static bool operator != (coordinate c1, coordinate c2) {
			return !(c1 == c2);
		}
	}

}
