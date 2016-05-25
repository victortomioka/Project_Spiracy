using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(Rigidbody))]
public class Pickup : MonoBehaviour {

	public enum Type { Health, Special, Ammo };
	public Type thisType;
	public LayerMask mask;
	public float turnRate = 45f;
	public float maxSpeed = 100f;
	public float accel = 1f;
	public float radius;

	float speedM1;
	float speedM2;
	float speedMFinal;
	float tRate1;
	float tRate2;
	float tRateFinal;
	float speed;
	Rigidbody rb;

	Transform target;
	bool hasFoundTarget;

	// Use this for initialization
	void Start () {
		switch(thisType){
		case Type.Health: break;
		case Type.Special: break;
		case Type.Ammo: break;
		} 

		GetComponent<SphereCollider>().radius= radius;
		speedM1 = maxSpeed / 3;
		speedM2 = maxSpeed;
		speedMFinal = Random.Range (speedM1, speedM2); 
		tRate1 = turnRate / 3;
		tRate2 = turnRate;
		tRateFinal = Random.Range (tRate1, tRate2);
		rb = this.GetComponent<Rigidbody>();
	}

	void Update () {
		if(speed<speedMFinal){
			speed+=accel;
		}else if(speed>=speedMFinal){
			speed=speedMFinal;
		}

		if(hasFoundTarget){
			Chase(target);
		}else{
		rb.AddForce(transform.forward*speed,ForceMode.Acceleration);
		}

	}
		


	public void OnTriggerEnter(Collider tgt){
		if (mask == (mask | (1 << tgt.gameObject.layer))) {
			target = tgt.gameObject.transform;
			hasFoundTarget = true;
		}
	}
		

void Chase (Transform t) {
	Quaternion targetRot;
	targetRot = Quaternion.LookRotation (t.position - transform.position);
	transform.rotation = Quaternion.RotateTowards (transform.rotation, targetRot, tRateFinal * Time.deltaTime);
	transform.Translate (transform.forward * speed * Time.deltaTime);

	float dist = Vector3.Distance(transform.position, t.position);

	if (dist < 1f) {
			switch(thisType){
			case Type.Health: healthUp(); break;

			case Type.Special: specialUp(); break;
				
			case Type.Ammo: ammoUp(); break;
				
			} 
		}
	}

	void ammoUp(){
		int randomInt = Random.Range(0,2);
		if (randomInt == 0){
			
			Weapon wep = GameObject.FindObjectOfType<WeaponManager>().primary;
			if(Weapon.currAmmo[wep.currAmmoIndex]<wep.maxAmmo){
			Weapon.currAmmo[wep.currAmmoIndex]+=wep.maxAmmo/10;
		}
		}else if (randomInt == 1){
			Weapon wep = GameObject.FindObjectOfType<WeaponManager>().secondary;
			if(Weapon.currAmmo[wep.currAmmoIndex]<wep.maxAmmo){
			Weapon.currAmmo[wep.currAmmoIndex]+=wep.maxAmmo/10;
			}
		}
	}

	void specialUp(){
		ShipController playerCtrl = FindObjectOfType<ShipController>();
		if(playerCtrl.currSpecial<3){
			playerCtrl.currSpecial++;
		}
	}

	void healthUp(){
		ShipController playerCtrl = FindObjectOfType<ShipController>();
		playerCtrl.GetComponent<HealthManager>().currentHealth += 2;
	}

	void OnDrawGizmos(){
		Gizmos.DrawWireSphere(transform.position, radius);
	}
}


