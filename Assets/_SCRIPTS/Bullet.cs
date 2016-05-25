using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]

public class Bullet : MonoBehaviour {

	public GameObject effect;
	public Rigidbody[] subBullets;
	public float SpreadBullets;
	public LayerMask colMask;
	public float timeUntilDestruction = 2;
	public float velocity;
	public float skinWidth = 0.1f;
	public int damage;
	public int flameDamage;
	public float flameTime;
	public int penetration = 1;


	public bool isDebug = false;
	public bool isHitscan = false;
	public bool isSplit = false;
	public bool effectTimeUp = false;

	private int penRemaining;	
	private float timeElapsed;
	private Rigidbody thisBullet;
	private Transform splitPos;
	private ParticleSystem trail;
	static GameObject bulletPool;
	static GameObject effectPool;

	public AudioClip sound;
	AudioSource source;
	
	void Start () {
		timeElapsed = 0;
		thisBullet = gameObject.GetComponent<Rigidbody> ();
		penRemaining = penetration;
		splitPos = transform;
		source = this.GetComponent<AudioSource>();

		bulletPool = GameObject.Find("bulletPool");
		if(bulletPool!=null){
			transform.parent = bulletPool.transform;
		}else{
			bulletPool = new GameObject();
			bulletPool.name = "bulletPool";
			transform.parent = bulletPool.transform;
		}

		if(transform.childCount>0){trail = transform.GetChild(0).gameObject.GetComponent<ParticleSystem>();}


		thisBullet.AddForce (transform.forward * velocity, ForceMode.VelocityChange);
	}
		
	void Update () {
		if (!isHitscan) {
			rigidBodyBullet ();
		}

		if (isHitscan) {
			hitscanBullet ();
		}
		selfDestruct();
	}




	void rigidBodyBullet(){
		if (thisBullet != null) {
			float moveDistance = thisBullet.velocity.z * Time.deltaTime * 1;
			CheckCollision (moveDistance);
			timeElapsed += Time.deltaTime;

		} else {
			Debug.LogError(gameObject.name + " needs a RigidBody component!");
		}
	}

	void hitscanBullet(){

		Ray ray = new Ray (transform.position, transform.forward);
		RaycastHit[] hits;

		hits = Physics.RaycastAll (ray, 1000f, colMask, QueryTriggerInteraction.Collide);

		foreach (RaycastHit hit in hits) {
			if(penRemaining>0){
			OnScanHit(hit);
			penRemaining--;
			}else if(penRemaining<=0){
				bulletDestroy ();
			}
		}
	}

	void CheckCollision (float dist) {
		Ray ray = new Ray (transform.position, transform.forward);
		RaycastHit hit;

		if (isDebug) {
			Debug.DrawLine (transform.position, transform.forward*(1*skinWidth+dist), Color.red);
			print ("hit something!");
		}

		if (Physics.Raycast (ray, out hit, dist+skinWidth, colMask, QueryTriggerInteraction.Collide)) {

			OnHit(hit);

		}

	}

	void OnHit(RaycastHit hit){
		
		IDamageable damageableObject = hit.collider.GetComponent<IDamageable>();
		if(damageableObject!=null){
			penRemaining--;

			if(flameTime>0){
				damageableObject.FlamesStart(flameDamage, flameTime);
			}

			if(damage>0){
				damageableObject.DealDamage(damage);
			
			if(penRemaining<=0){
				
				if (isSplit){
					splitBullet();
					hitEff ();
				} else {

				if (effectTimeUp) {
					hitEff ();
				}
						bulletDestroy();
				}
			}
			hitEff ();
			}
		}
	}

	void OnScanHit(RaycastHit hit){
		
		IDamageable damageableObject = hit.collider.GetComponent<IDamageable>();
		if(damageableObject!=null){
			damageableObject.DealDamage(damage);
			if(flameTime>0){
				damageableObject.FlamesStart(flameDamage, flameTime);
			}
		}
		hitEff ();
	}

	void splitBullet(){
		IPattern pattern = gameObject.GetComponent<IPattern>();
		if(pattern!=null){
			gameObject.GetComponent<Rigidbody>().velocity = new Vector3(0,0,0);
			bool destruct = true;
			pattern.startPattern(subBullets,SpreadBullets,splitPos,destruct);
		}else{
		Debug.LogError("Splitting bullets need an IPattern module!");
		}
	}
	
	void selfDestruct(){
		if (timeElapsed > timeUntilDestruction) {
			if (isSplit){
				if( source!=null && sound!=null){
					playSound();
				}	
				splitBullet();
				gameObject.GetComponent<Bullet>().enabled=false;
			}else{

			if (effectTimeUp) {
				hitEff ();
			}

				bulletDestroy();
		}
	}
}

	void hitEff(){
		if (trail!=null){
			trail.transform.parent = null;
			trail.emissionRate = 0;
			trail.GetComponent<trailCleanup>().StartCleanup();
		}

		if (effect != null) {
			Utility.spawnEffect(effect, transform.position, transform.rotation);
			
		}
	}

	public void playSound(){
		IPattern pattern = gameObject.GetComponent<IPattern>();
		float vol;
		if(pattern!=null){
			vol = source.volume*(0.01f*pattern.getCount());
		}else{
			vol = source.volume*(Random.Range(0.5f,1.2f));
		}
		source.pitch = Random.Range(1,2);
		source.PlayOneShot(sound,vol);
	}

	void bulletDestroy(){
	
		GameObject.Destroy(gameObject);
		}
	
}
