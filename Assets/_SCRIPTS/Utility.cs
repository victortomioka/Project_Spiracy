using UnityEngine;

public class Utility : MonoBehaviour {

	//this class is used for utility functions, to be called from anywhere in any other class in the game.
	//static functions:
	//calcSpread
	//arcingLines

	void Awake(){
		DontDestroyOnLoad(this);
	}

	void Start(){

	
	}

	public static Vector3 calcSpread(float spreadFactor){
		float rotX = Random.Range(-spreadFactor, spreadFactor);
		float rotY = Random.Range(-spreadFactor, spreadFactor);
		float rotZ = Random.Range(-spreadFactor, spreadFactor);
		Vector3 quat = new Vector3(rotX, rotY, rotZ);
		return quat;
	}

	public static GameObject spawnEffect(GameObject effect,Vector3 pos,Quaternion rot){
		GameObject newEff = Instantiate (effect,pos,rot) as GameObject;
		return newEff;
	}

}
