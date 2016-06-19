using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour {

	public Wave[] waves;
	public Wave bossWave;
	Wave currentWave;
	int currentWaveNumber;
	int maxWaves;

	public float xRange = 1f;
	public float yRange = 1f;
	public float zRange = 1f;
	public float minWaveDelay = 1f;
	public float maxWaveDelay = 10f;
	public float distThreshold = 10;
	public int enemyPoints = 100;
		
	GameObject enMaster;
	int squadsRemainingToSpawn;
	int enemiesRemainingAlive;
	float nextSpawnTime = 0;
	Squadron[] currentSquadWave;
	int currentSquadNumber;
	int enemiesRemainingInSquad;
	int enemiesAliveInSquad;

	Text waveCounter;
	Animator waveAnim;
	bool transitioning;
	bool beaten = false;

	private GameObject player;

	public event System.Action<int> OnNewWave;

	
	void Start () 
	{
		waveCounter = GameObject.Find("waveCounter").GetComponent<Text>();
		waveAnim = waveCounter.GetComponent<Animator>();
		player = FindObjectOfType<ShipController>().gameObject;
		player.GetComponent<HealthManager>().OnDeath += nullTarget;
		enMaster = new GameObject();
		enMaster.name = "enemiesPool";
		maxWaves = waves.Length;
		StartCoroutine(waveTransition());
	}

	void Update()
	{
		if(this.enabled==true){
			if(squadsRemainingToSpawn> 0 && Time.time > nextSpawnTime){
				if(!transitioning){
				cycleSquad();
				}
			}
		}
	}

	void cycleSquad(){
		if(currentSquadNumber<waves[currentWaveNumber-1].squads.Length){
		squadsRemainingToSpawn--;
		currentSquadNumber++;
		print (currentWaveNumber + " " + currentSquadNumber);
		nextSpawnTime = Time.time + Random.Range(currentWave.timeBetweenSpawns/2,currentWave.timeBetweenSpawns);
		enemiesRemainingInSquad = currentSquadWave[currentSquadNumber-1].enemies.Length;
		enemiesAliveInSquad = enemiesRemainingInSquad;
		StartCoroutine(spawnSquad(currentSquadWave[currentSquadNumber-1],0));
		}
	}

	IEnumerator spawnSquad(Squadron squad, int count){
		count = 0;
		while(enemiesRemainingInSquad>0){
			count++;
			enemiesRemainingInSquad--;
			SpawnEnemy(squad.enemies[count-1]);
		}
		yield return null;
	}

	void SpawnEnemy (GameObject enemy) 
	{
		float xOffset = Random.Range (-xRange, xRange);
		float yOffset = Random.Range (-yRange, yRange);
		float zOffset = Random.Range (-zRange, zRange);
		GameObject newEnemy = Instantiate (enemy, transform.position + new Vector3 (xOffset, yOffset, zOffset), transform.rotation) as GameObject;
		newEnemy.transform.parent = enMaster.transform;
		newEnemy.GetComponent<HealthManager>().OnDeath += enemyDeath;
	}


	void nextWave(){
			currentWaveNumber++;
			
		if(currentWaveNumber<maxWaves){
			currentWave = waves[currentWaveNumber -1];
		}


			squadsRemainingToSpawn = currentWave.squads.Length;

			for(int i = 0; i < currentWave.squads.Length; i++){
				enemiesRemainingAlive += currentWave.squads[i].enemies.Length;
			}

			currentSquadWave = currentWave.squads;
			currentSquadNumber = 0;
			transitioning = false;
			cycleSquad();

			if(OnNewWave !=null){
				OnNewWave(currentWaveNumber);
			}
		
	}

	void enemyDeath(){
		enemiesRemainingAlive--;
		enemiesAliveInSquad--;


		if(enemiesAliveInSquad == 0 && enemiesRemainingAlive>0){
			if(!transitioning){
			cycleSquad();
			}
		}

		if(enemiesRemainingAlive == 0){
			if(currentWaveNumber+1>maxWaves){
				beaten = true;
			}else{
			StartCoroutine(waveTransition());
			}
		}
	}
	
	void nullTarget(){
		this.enabled = false;
	}

	IEnumerator waveTransition(){
		print ("Wave " + (currentWaveNumber));
		transitioning = true;
		waveCounter.enabled = true;
		waveAnim.enabled = true;
		waveCounter.text = "Wave " + (currentWaveNumber);
		waveAnim.Rebind();
		waveAnim.Play("waveTransition",-1,0f);
		while(waveAnim.GetCurrentAnimatorStateInfo(0).IsName("waveTransition")){
			yield return null;
		}
		waveCounter.enabled = false;
		waveAnim.enabled = false;
		nextWave();
	}
		
	void OnGUI(){
		if(beaten){
		Time.timeScale=10;
		Texture creditImg = Resources.Load("credits") as Texture;
		GUI.Box(new Rect(0,0,Screen.width,Screen.height),creditImg);
		}
	}

	[System.Serializable]
	public class Squadron{
		public GameObject[] enemies;
	}

	[System.Serializable]
	public class Wave{
		public Squadron[] squads;
		public int timeBetweenSpawns;
	}

}
