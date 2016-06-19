using UnityEngine;
using System.Collections;

public class trailCleanup : MonoBehaviour {

	ParticleSystem thisPart;
	static GameObject pool;
	
	public void StartCleanup () {
		thisPart = this.GetComponent<ParticleSystem>();
		var em = thisPart.emission;
		em.enabled=false;
		StartCoroutine(Cleanup());
		pool = GameObject.Find("trailsPool");
		if(pool!=null){
			transform.parent = pool.transform;
		}else{
		pool = new GameObject();
		pool.name = "trailsPool";
		transform.parent = pool.transform;
		}
	}


	IEnumerator Cleanup () {
		while(thisPart.particleCount>0){
			yield return null;
		}
		GameObject.Destroy(gameObject);

	}
}
