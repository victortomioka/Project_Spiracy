using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider))]
public class Missile : MonoBehaviour {


	public bool isHoming = false;
	public bool isRandomized = false;
	public float maxSpeed;
	public float accel;
	public float turnRate;
	public int damage = 1;
	public float radius = 10;
	public float detRange = 1;
	public GameObject explosion;
	public float timeUntilDestruction = 3;
	private float timeElapsed;
	public LayerMask collisionMask;

	public int flameDamage;
	public float flameTime;

	private bool doNotSeek = false;
	private float DontSeekTimer = 0;

	private Transform target;
	private float speed;
	private float speedM1;
	private float speedM2;
	private float speedMFinal;
	private float tRate1;
	private float tRate2;
	private float tRateFinal;
	private bool hasFoundTarget = false;
	private Rigidbody thisMissile;

	// Use this for initialization
	void Start () {
		speed = 0;
		if (isRandomized) {
			speedM1 = maxSpeed / 3;
			speedM2 = maxSpeed;
			speedMFinal = Random.Range (speedM1, speedM2); 
			tRate1 = turnRate / 3;
			tRate2 = turnRate;
			tRateFinal = Random.Range (tRate1, tRate2);
		} else {
			speedMFinal = maxSpeed;
			tRateFinal = turnRate;
		}
		thisMissile = gameObject.GetComponent<Rigidbody>();
	}

	void Update () {

		timeElapsed+=Time.deltaTime;
		if(timeElapsed>=timeUntilDestruction){
			Detonate(transform,radius);
		}

		if(isHoming){
			if(hasFoundTarget&&target!=null){
				Chase(target);
			}
		}

		if(speed<speedMFinal){
			speed+=accel;
		}else if(speed>=speedMFinal){
			speed=speedMFinal;
		}

		thisMissile.AddForce(transform.forward*speed,ForceMode.Acceleration);

		if (doNotSeek) {
			DontSeekTimer -= Time.deltaTime;
			if (DontSeekTimer <= 0) {
				doNotSeek = false;
			}
		}

	}

	public void OnTriggerEnter(Collider tgt){
		if (collisionMask == (collisionMask | (1 << tgt.gameObject.layer))) {
			target = tgt.gameObject.transform;
			hasFoundTarget = true;
		}
	}

	public void OnTriggerExit(Collider tgt){
			target = null;
			hasFoundTarget = false;
			doNotSeek = true;
			DontSeekTimer = 2;
	}

	// enemy detected
	void Chase (Transform t) {
			Quaternion targetRot;
			targetRot = Quaternion.LookRotation (t.position - transform.position);
			transform.rotation = Quaternion.RotateTowards (transform.rotation, targetRot, tRateFinal * Time.deltaTime);
			transform.Translate (transform.forward * speed * Time.deltaTime);

			float dist = Vector3.Distance(transform.position, t.position);

				if (dist < detRange) {
					Detonate (t,radius);
				}
	}



	void Detonate (Transform t, float radius){
			Collider[] hitColliders = Physics.OverlapSphere(t.position,radius);
			for(int i = 0; i < hitColliders.Length; i++){
				IDamageable damageableObject = hitColliders[i].GetComponent<IDamageable>();
				if(damageableObject!=null){

				if(flameTime>0){
					damageableObject.FlamesStart(flameDamage, flameTime);
				}	

				damageableObject.DealDamage(damage);
				}
			}
			if(explosion!=null){
			Utility.spawnEffect(explosion, transform.position, transform.rotation);
		}
			GameObject.Destroy (gameObject);
	}

	void OnDrawGizmosSelected(){
		Gizmos.DrawWireSphere(transform.position,radius);
	}
}
