using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;


[RequireComponent(typeof(WeaponManager))]
[RequireComponent(typeof(HealthManager))]
[RequireComponent(typeof(UIManager))]
public class ShipController : MonoBehaviour {

	public string longName;
	public float movementSpeed = 1.0f;
	public int invert = -1;
	public Transform shipModel;
	public float specialCoolDown = 1;
	public int specialCount = 3;
	[HideInInspector]
	public int currSpecial;
	Texture pauseimg;
	float escapeCd;
	Scene currentScene;
	public bool cheating;


	public event System.Action OnSpecial;

	bool canSpecial = true;
	
	void Start () 
	{
		if(PlayerData.ship==null || PlayerData.primary== null || PlayerData.secondary==null){SceneManager.LoadScene("02_hangar");}
		if(cheating){Globals.cheatMode=true;}
		currSpecial = specialCount;
		pauseimg = Resources.Load("pause") as Texture;
		Time.timeScale=1;
		currentScene = SceneManager.GetActiveScene();
	}

	void Update () 
	{
		float horizontal = Input.GetAxis ("Horizontal");
		float vertical = Input.GetAxis ("Vertical");

		Vector3 direction = new Vector3 (horizontal, invert * vertical, 0);
		Vector3 finalDirection = new Vector3 (horizontal, invert * vertical, 5.0f);

		if(!Globals.playerDead && !Globals.Pause){
		transform.position+= direction*movementSpeed*Time.deltaTime*Globals.speedScale;
		Quaternion rot = Quaternion.LookRotation(finalDirection);
		transform.rotation = Quaternion.RotateTowards (transform.rotation, rot ,Mathf.Deg2Rad*60.0f);
		}

		if (Input.GetKey("space")){
			doSpecial();
		}

		if (Input.GetKey("f") && Globals.playerDead){
			Globals.playerDead=false;
			Time.timeScale=1;
			SceneManager.LoadScene(currentScene.buildIndex);
		}

		if (Input.GetKey("escape")){
			if(!Globals.playerDead && escapeCd<=0){

			switch(Globals.Pause){
				case true: Time.timeScale=1; Globals.Pause=false; escapeCd=1; StartCoroutine(pauseCooldown()); break;
				case false: Time.timeScale=0; Globals.Pause=true; escapeCd=1; StartCoroutine(pauseCooldown()); break;
				}
			}
		}
	}

	IEnumerator pauseCooldown(){
		while (escapeCd>=0){
			escapeCd-=1f/20;
			yield return null;
		}
		yield return null;
	}

	void OnGUI(){
		if(Globals.Pause){
			GUI.Box(new Rect(0,0,Screen.width,Screen.height),pauseimg);
			Time.timeScale=0;	
			}
			
		}

	void doSpecial(){
		ISpecial special = this.GetComponent<ISpecial>();
		if(special!=null){
			if(canSpecial & currSpecial>0){
				if(!Globals.cheatMode){
				currSpecial--;
				}
				special.specialAttack();
				StartCoroutine("coolDown");
				if(OnSpecial!=null){
					this.OnSpecial();
				}
			}
		}else{
			Debug.LogError("This ship doesn't have a special attack :(");
		}
	}

	IEnumerator coolDown(){
		float specialTime = 0;
		while(specialTime<specialCoolDown){
			canSpecial = false;
			specialTime+=Time.deltaTime;
			yield return null;
		}
		canSpecial = true;
	}

}