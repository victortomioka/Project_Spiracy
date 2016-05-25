using UnityEngine;
using System.Collections;

public class ObstacleSpawner : MonoBehaviour {

	public GameObject[] obstacles;
	public float xRange = 1f;
	public float yRange = 1f;
	public float minSpawnTime = 1f;
	public float maxSpawnTime = 10f;
	GameObject obsMaster;


	
	void Start () 
	{
		Invoke ("SpawnObstacle",Random.Range (minSpawnTime,maxSpawnTime));
		obsMaster = new GameObject();
		obsMaster.name = "Obstacles";
	}

	void SpawnObstacle () 
	{
		float xOffset = Random.Range(-xRange, xRange);
		float yOffset = Random.Range(-yRange,yRange);
		GameObject newObstacle = Instantiate(obstacles[Random.Range(0,obstacles.Length)],transform.position + new Vector3(xOffset,yOffset,0f),transform.rotation) as GameObject;
		newObstacle.transform.parent = obsMaster.transform;
		Invoke ("SpawnObstacle",Random.Range (minSpawnTime,maxSpawnTime)); 
	}
}
