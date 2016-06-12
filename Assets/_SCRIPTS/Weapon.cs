using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class Weapon : MonoBehaviour {

	public enum ammo{
		bullet,shell,energy,explo
	}

	public ammo ammoType;

	[HideInInspector]
	public int maxAmmo;

	[HideInInspector]
	public static int[] currAmmo = new int[4];
	[HideInInspector]
	public int currAmmoIndex;

	public int ammoConsumption = 1;

	public string longName;
	public Text toolTip;

	public Rigidbody bullet;
	public float spreadBase = 15;
	public float weaponCoolDown = 0.1f;
	public int shotCount = 1;

	public Transform[] muzzleCount;

	public bool isStaggered;
	public bool isScattered;

	[HideInInspector]
	public bool canShoot;
	[HideInInspector]
	public float spreadFactor;
	[HideInInspector]
	public float spreadIncrease;
	[HideInInspector]	
	public float spreadMax;
	[HideInInspector]
	public float cooldown;

	private int staggerCount = 0;
	public AudioClip muzzleSound;
	public Animator anim;

	void Awake(){
		switch(ammoType){
		case ammo.bullet:maxAmmo = 999;currAmmoIndex = 0;break;
		case ammo.shell:maxAmmo = 250;currAmmoIndex = 1;break;
		case ammo.explo:maxAmmo = 250;currAmmoIndex = 2;break;
		case ammo.energy:maxAmmo = 250;currAmmoIndex = 3;break;
		}

		currAmmo[currAmmoIndex] = maxAmmo;
	}

	void Start(){
		anim = GetComponent<Animator>();

	}

	public void shoot(Weapon wep, Transform[] muz){
		if (canShoot && cooldown <= 0 && currAmmo[currAmmoIndex]>=ammoConsumption) {
			spreadIncrease = wep.spreadBase / 10;
			spreadMax = wep.spreadBase * 3;
			cooldown = wep.weaponCoolDown;
			spreadFactor += spreadIncrease;

			if(!Globals.cheatMode){
				currAmmo[currAmmoIndex]-=ammoConsumption;
			}

			if(!isStaggered){
				for(int j = 0; j<muz.Length; j++){
					if(isScattered)
					{
						for(int i = 0; i<wep.shotCount; i++){
							Rigidbody newBullet = Instantiate (wep.bullet, muz[j].position, muz[j].rotation) as Rigidbody;
							newBullet.transform.Rotate (Utility.calcSpread(spreadFactor));
						}
					}

					if(!isScattered){
						Rigidbody newBullet = Instantiate (wep.bullet, muz[j].position, muz[j].rotation) as Rigidbody;
						newBullet.transform.Rotate (Utility.calcSpread(spreadFactor));
					}
				}
			}
			if(isStaggered){
				staggerCount++;
				if (staggerCount>=muzzleCount.Length){
					staggerCount = 0;
				}

				if(isScattered)
				{
					for(int i = 0; i<wep.shotCount; i++){
						Rigidbody newBullet = Instantiate (wep.bullet, muz[staggerCount].position, muz[staggerCount].rotation) as Rigidbody;
						newBullet.transform.Rotate (Utility.calcSpread(spreadFactor));
					}
				}


				if (!isScattered) {
					Rigidbody newBullet = Instantiate (wep.bullet, muz [staggerCount].position, muz [staggerCount].rotation) as Rigidbody;
					newBullet.transform.Rotate (Utility.calcSpread (spreadFactor));
				}
			}
		}
	}

}