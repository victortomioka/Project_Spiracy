﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(AudioSource))]

public class WeaponManager : MonoBehaviour {

	#region variables;

	public Transform plane1;
	Transform mount1;
	Transform mount2;

	[Tooltip("Insert primary Weapon objects here")]
	public Weapon[] PrimaryWeapon;
	[Tooltip("Insert secondary Weapon objects here")]
	public Weapon[] SecondaryWeapon;

	private Transform[] muzzleManager1;
	private Transform[] muzzleManager2;
	
	public int weaponPrimaryNum = 0;
	public int weaponSecondaryNum = 0;
	
	private Transform weaponPrimaryTrans;
	private Transform weaponSecondaryTrans;

	private GameObject WPGameObject;
	private GameObject WSGameObject;

	AudioSource source;
	AudioClip primarySound;
	AudioClip secondarySound;

	[HideInInspector]
	public Weapon primary;
	[HideInInspector]
	public Weapon secondary;

	[HideInInspector]
	public int pIndex;
	[HideInInspector]
	public int sIndex;

	public event System.Action shotPrimary;
	public event System.Action shotSecondary;
	
	#endregion

	#region start

	void Start() {

		PrimaryWeapon[weaponPrimaryNum].cooldown = 0;
		PrimaryWeapon[weaponPrimaryNum].canShoot = true;
		SecondaryWeapon[weaponSecondaryNum].cooldown = 0;
		SecondaryWeapon[weaponSecondaryNum].canShoot = true;
		mount1 = transform.Find ("weaponMount1");
		mount2 = transform.Find ("weaponMount2");
		equipWeapon();
		source = gameObject.GetComponent<AudioSource>();

		PrimaryWeapon[weaponPrimaryNum].spreadFactor = PrimaryWeapon[weaponPrimaryNum].spreadBase;
		SecondaryWeapon[weaponSecondaryNum].spreadFactor = SecondaryWeapon[weaponSecondaryNum].spreadBase;
	}

	#endregion

	#region update
	
	void Update () {

		if(!Globals.playerDead && !Globals.Pause){
		mouseAim ();
		mouseFire ();
		weaponHandler ();
		}

	}

	#endregion

	#region equipWeapon

	void equipWeapon(){

		//primary

		Weapon weaponPrimaryEquipped = Instantiate(PrimaryWeapon[weaponPrimaryNum],mount1.position,transform.rotation) as Weapon;
		primary = weaponPrimaryEquipped;
		WPGameObject = weaponPrimaryEquipped.gameObject;
		weaponPrimaryTrans = WPGameObject.GetComponent<Transform>();
		primarySound = weaponPrimaryEquipped.muzzleSound;
		weaponPrimaryTrans.parent = mount1;
		pIndex = primary.currAmmoIndex;

		muzzleManager1 = new Transform[primary.muzzleCount.Length];

		for(int i = 0; i < primary.muzzleCount.Length; i++){
			muzzleManager1[i] = primary.muzzleCount[i];
		}

		//secondary

		Weapon weaponSecondaryEquipped = Instantiate(SecondaryWeapon[weaponSecondaryNum],mount2.position,transform.rotation) as Weapon;
		secondary = weaponSecondaryEquipped;
		WSGameObject = weaponSecondaryEquipped.gameObject;
		weaponSecondaryTrans = WSGameObject.GetComponent<Transform>();
		secondarySound = weaponSecondaryEquipped.muzzleSound;
		weaponSecondaryTrans.parent = mount2;
		sIndex = secondary.currAmmoIndex;

		muzzleManager2 = new Transform[secondary.muzzleCount.Length];

		for(int j = 0; j < secondary.muzzleCount.Length; j++){
			muzzleManager2[j] = secondary.muzzleCount[j];

		}

	}

	#endregion

	#region mouseAim

	void mouseAim(){
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		
		Plane aimPlane = new Plane (plane1.forward, plane1.position);
		float dist;
		
		if (aimPlane.Raycast (ray, out dist)) {
			Vector3 hit = ray.GetPoint (dist);
			Debug.DrawLine (ray.origin, hit, Color.red);
	
			
			Vector3 hitCorrectedForY1 = new Vector3(hit.x,weaponPrimaryTrans.position.y,hit.z);
			Vector3 hitCorrectedForY2 = new Vector3(hit.x,weaponSecondaryTrans.position.y,hit.z);
			
			//aim weapon 1
			for(int i = 0; i< muzzleManager1.Length; i++){
				muzzleManager1[i].rotation = Quaternion.RotateTowards (muzzleManager1[i].rotation, Quaternion.LookRotation (hit), Mathf.Deg2Rad * 45.0f);
				weaponPrimaryTrans.rotation = Quaternion.RotateTowards(weaponPrimaryTrans.rotation,Quaternion.LookRotation (hitCorrectedForY1),Mathf.Deg2Rad * 45.0f);
			}
			//aim weapon 2
			for(int j = 0; j< muzzleManager2.Length; j++){
				muzzleManager2[j].rotation = Quaternion.RotateTowards (muzzleManager2[j].rotation, Quaternion.LookRotation (hit), Mathf.Deg2Rad * 45.0f);
				weaponSecondaryTrans.rotation = Quaternion.RotateTowards(weaponSecondaryTrans.rotation,Quaternion.LookRotation (hitCorrectedForY2),Mathf.Deg2Rad * 45.0f);
			}
			
		}
	}

	#endregion

	#region mouseFire

	void mouseFire(){

		if (Input.GetKey ("mouse 0")) {
			if(source!=null && primarySound!=null && primary.cooldown<=0 && Weapon.currAmmo[pIndex]>=primary.ammoConsumption){
				playShotSound(source,primary.muzzleSound, primary);
				if(primary.anim!=null){
				primary.anim.enabled=true;
				}
			}
			primary.shoot(primary,muzzleManager1);
			if (shotPrimary != null) { 
				this.shotPrimary();
			}
		}else if (primary.anim!=null){
			primary.anim.enabled=false;
		}
		
		if (Input.GetKey ("mouse 1")) {
			if(source!=null && secondarySound!=null && secondary.cooldown<=0 && Weapon.currAmmo[sIndex]>=secondary.ammoConsumption){
				playShotSound(source,secondary.muzzleSound, secondary);
				if(secondary.anim!=null){
				secondary.anim.enabled=true;
				}
			}
			secondary.shoot(secondary,muzzleManager2);
			if (shotSecondary != null) { 
				this.shotSecondary();
			}
		}else if (secondary.anim!=null){
			secondary.anim.enabled=false;
		}
	}

	#endregion

	#region weaponHandler
	void weaponHandler(){

		//handles spread and cooldown
		
		if (primary.spreadFactor >= primary.spreadMax) {
			primary.spreadFactor = primary.spreadMax;
		}
		
		if (primary.cooldown > 0) {
			primary.cooldown -= Time.deltaTime;
		}
		
		if (primary.spreadFactor > primary.spreadBase) {
			primary.spreadFactor -= primary.spreadIncrease * Time.deltaTime * 5;
		}
		
		//this will handle cooldown and spread for secondary weapons, if present
		
		if (secondary.spreadFactor >= secondary.spreadMax) {
			secondary.spreadFactor = secondary.spreadMax;
		}
		
		if (secondary.cooldown > 0) {
			secondary.cooldown -= Time.deltaTime;
		}
		
		if (secondary.spreadFactor > secondary.spreadBase) {
			secondary.spreadFactor -= secondary.spreadIncrease * Time.deltaTime * 5;
		}

	}
	#endregion

	public void playShotSound(AudioSource source, AudioClip clip, Weapon wep){
		float vol = source.volume*(Random.Range(0.5f,1.2f));
		source.pitch = Random.Range(1,2);
		source.PlayOneShot(clip,vol);
	}
}