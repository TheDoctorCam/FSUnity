using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class levelGenerator : MonoBehaviour {

	public Level[] levels;				//Level array of generated levels
	public int levelIndex;				//level currently accessed

	public Transform gridPrefab;		//prefab for each grid object
	public Transform obstaclePrefab;	//prefab for each obstacle object
	public Transform navMeshSize;		//size of the NavMesh floor which the level sits on
	public Transform navMeshBorder;		//prefab for the size of the NavMesh floor
	public Vector2 maxLevelSize;		//maxmimum size of the level , == navMeshSize

	[Range(0,1)]
	public float gridBorderPercent;		//percent of outline width per grid object generated

	public float gridSize;				//controls the dimensions of each grid object

	List<coordinate> levelGridCoordinates;			//coordinates of each grid object
	Queue<coordinate> shuffledGridCoordinates;		//coordinates of each grid object after Fishcer-Yates shuffle
	Queue<coordinate> openGridCoordinates;			//coordinates of grid objects that don't have an obstacle
	Transform[,] gridMap;							//Used to store accessible grids (i.e., without an obstacle on them)

	Level currentLevel;					//Level object to store the currently generated level

	void Start(){
		GenerateLevel ();
	}

	public void GenerateLevel(){
		currentLevel = levels [levelIndex];
		gridMap = new Transform[currentLevel.levelSize.x, currentLevel.levelSize.y];
		System.Random rng = new System.Random (currentLevel.seed);		//random height for obstacles
		GetComponent<BoxCollider>().size = new Vector3(currentLevel.levelSize.x * gridSize, .05f, currentLevel.levelSize.y * gridSize);	//box collider for player to walk on

		/* Generate coordinates */
		levelGridCoordinates = new List<coordinate> ();
		for (int x = 0; x < currentLevel.levelSize.x; x++) {
			for (int y = 0; y < currentLevel.levelSize.y; y++) {
				levelGridCoordinates.Add (new coordinate (x, y));
			}
		}
		shuffledGridCoordinates = new Queue<coordinate> (utility.fisherYatesShuffle (levelGridCoordinates.ToArray(), currentLevel.seed));		//returns shuffled coordinates

		/* Create level object */
		string levelName = "Generated Level";	//used to store generated level object in Unity 
		//Destroy generated level while running dynamically IF new level variables are entered (size, gridBorder, etc)
		if (transform.Find (levelName)) {
			DestroyImmediate (transform.Find (levelName).gameObject);
		}

		Transform levelHolder = new GameObject (levelName).transform;
		levelHolder.parent = transform;

		/* Generates tiles for the level */
		for (int x = 0; x < currentLevel.levelSize.x; x++) {
			for (int y = 0; y < currentLevel.levelSize.y; y++) {
				//Creates initial grid starting position
				Vector3 startPosition = coordinateToPosition(x,y);
				//Adds grid prefab to startPosition, rotating the grid object 90 degreesm to face the camera 
				Transform newGrid = Instantiate(gridPrefab, startPosition, Quaternion.Euler(Vector3.right * 90)) as Transform;
				newGrid.localScale = Vector3.one * (1 - gridBorderPercent) * gridSize;
				newGrid.parent = levelHolder;
				gridMap [x, y] = newGrid;
			}
		}

		/* Spawn randomly generated obstacles */
		bool[,] obstacleMap = new bool[(int)currentLevel.levelSize.x, (int)currentLevel.levelSize.y];	//generates array of of levelSize		
		int obstacleCount = (int) (currentLevel.levelSize.x * currentLevel.levelSize.y * currentLevel.obstacleCountPercent);	//max number of obstacles possible
		int currentObstacleCount = 0;//current number of obstacles generated
		List<coordinate> availableCoordinates = new List<coordinate> (levelGridCoordinates);		//position of coordinates without an obstacle

		for (int i = 0; i < obstacleCount; i++) {
			coordinate randomCoordinate = getRandomCoordinate ();
			obstacleMap [randomCoordinate.x, randomCoordinate.y] = true;
			currentObstacleCount++;

			if (randomCoordinate != currentLevel.levelCenter && isLevelAccessible (obstacleMap, currentObstacleCount)) {
				float obstacleHeight = Mathf.Lerp (currentLevel.minObstacleHeight, currentLevel.maxObstacleHeight, (float)rng.NextDouble ());
				Vector3 obstaclePosition = coordinateToPosition (randomCoordinate.x, randomCoordinate.y);

				Transform newObstacle = Instantiate (obstaclePrefab, obstaclePosition + Vector3.up * obstacleHeight/2 , Quaternion.identity) as Transform;
				newObstacle.parent = levelHolder;
				newObstacle.localScale = new Vector3( (1 - gridBorderPercent) * gridSize, obstacleHeight, (1 - gridBorderPercent) * gridSize);
			
				availableCoordinates.Remove (randomCoordinate);
			} 
			/*If the obstacle cannot be created */
			else {
				obstacleMap [randomCoordinate.x, randomCoordinate.y] = false;
				currentObstacleCount--;
			}
		}

		openGridCoordinates = new Queue<coordinate> (utility.fisherYatesShuffle (availableCoordinates.ToArray(), currentLevel.seed));		//returns shuffled coordinates


		/* Create Nav Mesh border so that the enemies can't walk around level*/
		Transform leftBorder = Instantiate (navMeshBorder, Vector3.left * (currentLevel.levelSize.x + maxLevelSize.x) / 4f * gridSize, Quaternion.identity) as Transform;
		leftBorder.parent = levelHolder;
		leftBorder.localScale = new Vector3 ((maxLevelSize.x - currentLevel.levelSize.x) / 2f, 1, currentLevel.levelSize.y) * gridSize;

		Transform rightBorder = Instantiate (navMeshBorder, Vector3.right * (currentLevel.levelSize.x + maxLevelSize.x) / 4f * gridSize, Quaternion.identity) as Transform;
		rightBorder.parent = levelHolder;
		rightBorder.localScale = new Vector3 ((maxLevelSize.x - currentLevel.levelSize.x) / 2f, 1, currentLevel.levelSize.y) * gridSize;


		Transform topBorder = Instantiate (navMeshBorder, Vector3.forward * (currentLevel.levelSize.y + maxLevelSize.y) / 4f * gridSize, Quaternion.identity) as Transform;
		topBorder.parent = levelHolder;
		topBorder.localScale = new Vector3 (maxLevelSize.x, 1, (maxLevelSize.y - currentLevel.levelSize.y) / 2f) * gridSize;

		Transform bottomBorder = Instantiate (navMeshBorder, Vector3.back * (currentLevel.levelSize.y + maxLevelSize.y) / 4f * gridSize, Quaternion.identity) as Transform;
		bottomBorder.parent = levelHolder;
		bottomBorder.localScale = new Vector3 (maxLevelSize.x, 1, (maxLevelSize.y - currentLevel.levelSize.y) / 2f) * gridSize;

		navMeshSize.localScale = new Vector3 (maxLevelSize.x, maxLevelSize.y) * gridSize;
	}



	/* Test condition to see if the player is able to access all avalailable grids.
	 * Uses: Flood-fill alogrithm
	 * True if: there are no obstacles that will completely border an empty grid
	 */
	bool isLevelAccessible(bool[,] obstacleMap, int numObstacles){
		bool[,] levelFlags = new bool[obstacleMap.GetLength (0), obstacleMap.GetLength (1)];
		Queue<coordinate> queue = new Queue<coordinate> ();
		queue.Enqueue (currentLevel.levelCenter);
		levelFlags [currentLevel.levelCenter.x, currentLevel.levelCenter.y] = true;

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

		int targetGridCount = (int)(currentLevel.levelSize.x * currentLevel.levelSize.y - numObstacles);
		return targetGridCount == accessibleGridCount;

	}


	// Helper function to return the coordinates of a grid object
	Vector3 coordinateToPosition (int x, int y){
		return new Vector3 (-currentLevel.levelSize.x / 2f + 0.5f + x, 0, -currentLevel.levelSize.y / 2f + 0.5f + y) * gridSize;
	}

	/* Dequeue and return first shuffled grid coordinate from shuffled queue */
	public coordinate getRandomCoordinate(){
		coordinate randomCoordinate = shuffledGridCoordinates.Dequeue ();
		shuffledGridCoordinates.Enqueue (randomCoordinate);
		return randomCoordinate;
	}

	/* Return the coordinates of a grid object without an obstacle */
	public Transform getRandomOpenCoordinate(){
		coordinate randomCoordinate = openGridCoordinates.Dequeue ();
		openGridCoordinates.Enqueue (randomCoordinate);
		return gridMap[randomCoordinate.x, randomCoordinate.y];
	}

	[System.Serializable]
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
		
	/* Stores level class to save and store created levels */
	[System.Serializable]
	public class Level{

			public coordinate levelSize;		//current level size
			[Range(0,1)]
			public float obstacleCountPercent;	//percent of obstacles in relation to levelSize
			public int seed;					//Fischer-Yates shuffle seed value
			public float minObstacleHeight;		//Smallest size of obstacle object
			public float maxObstacleHeight;		//Largest size of obstacle object			
			public Color foregroundColor;
			public Color backgroundColor;

			//Returns the center of the generated level
			public coordinate levelCenter {
				get{
				return new coordinate (levelSize.x / 2, levelSize.y / 2);
				}
			}

	}
			
}
