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
	UIManager UImgr;

	// Use this for initialization
	void Start () {
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
		}

		rb.AddForce(transform.forward*speed,ForceMode.Acceleration);
	
	}
		


	public void OnTriggerEnter(Collider tgt){
		if (mask == (mask | (1 << tgt.gameObject.layer))) {
			target = tgt.gameObject.transform;
			rb.velocity=Vector3.zero;
			rb.angularVelocity=Vector3.zero;
			speedMFinal/=3;
			hasFoundTarget = true;
		}
	}
		

void Chase (Transform t) {
	transform.LookAt(target);
	float dist = Vector3.Distance(transform.position, t.position);
	rb.AddForce(transform.forward*speed,ForceMode.Acceleration);
	if(speed>speedMFinal){
		speed*=0.3f;
	}


		if (dist < 10f) {
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
		GameObject.Destroy(this.gameObject);
	}

	void specialUp(){
		ShipController playerCtrl = FindObjectOfType<ShipController>();
		if(playerCtrl.currSpecial<3){
			playerCtrl.currSpecial++;
		}
		GameObject.Destroy(this.gameObject);
	}

	void healthUp(){
		ShipController playerCtrl = FindObjectOfType<ShipController>();
		playerCtrl.GetComponent<HealthManager>().currentHealth += 2;
		GameObject.Destroy(this.gameObject);
	}
}


