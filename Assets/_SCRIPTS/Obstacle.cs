using UnityEngine;
using System.Collections;

public class Obstacle : MonoBehaviour {

	public float speed = 1f;
	private Rigidbody rb;
	public int damage = 1;

	void Awake (){
		rb = gameObject.GetComponent<Rigidbody>();
	}

	void Update () {
		rb.AddForce(new Vector3(0,0,-speed*Globals.speedScale));
	}

	void OnTriggerEnter (Collider col) {
		if(col.gameObject.tag == "Player"){
		IDamageable damageableObject = col.GetComponent<IDamageable>();
		if(damageableObject!=null){
		damageableObject.DealDamage(damage);
		}
		if(col.gameObject.tag == "Enemy"){
			GameObject.Destroy(col.gameObject);
		}
	}
	}

	void OnBecameInvisible(){
		GameObject.Destroy(gameObject);
	}
}
