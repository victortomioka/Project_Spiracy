using UnityEngine;
using System.Collections;

public class EffectSelfDestruct : MonoBehaviour {

	public float t = 1;

	static GameObject effectPool;

	void Start () {

		effectPool = GameObject.Find("effectPool");
		if(effectPool!=null){
			transform.parent = effectPool.transform;
		}else{
			effectPool = new GameObject();
			effectPool.name = "effectPool";
			transform.parent = effectPool.transform;
		}	
			GameObject.Destroy(gameObject,t);
	}
}
