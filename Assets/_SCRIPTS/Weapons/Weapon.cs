using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class Weapon : MonoBehaviour {

	public enum ammo{
		bullet,shell,energy,explo
	}
	public ammo ammoType;

	public int ammoConsumption = 1;

	public string longName;
	public Text toolTip;

	public Rigidbody bullet;
	public float spreadBase = 15;
	public float weaponCoolDown = 0.1f;

	public Transform[] muzzleCount;

	[Tooltip("Staggered means it'll cycle between the multiple muzzles, if present. If non-staggered, it will shoot all muzzles at once")]
	public bool isStaggered;

	protected int maxAmmo;
	protected static int[] currAmmo = new int[4];
	protected int currAmmoIndex;

	protected float spreadFactor;
	protected float spreadIncrease;
	protected float spreadMax;
	protected float cooldown;

	protected int staggerCount = 0;
	public AudioClip muzzleSound;
	public Animator anim;

	public event System.Action ammoChanged;

	void Awake(){
		setup();
	}

	public virtual void shoot(Weapon wep, Transform[] muz){
		if (Globals.canShoot && cooldown <= 0 && currAmmo[currAmmoIndex]>=ammoConsumption) {
			process(wep);
			ammoChange(ammoConsumption, false);

			if(!isStaggered){
				for(int j = 0; j<muz.Length; j++){
						Rigidbody newBullet = Instantiate (wep.bullet, muz[j].position, muz[j].rotation) as Rigidbody;
						newBullet.transform.Rotate (Utility.calcSpread(spreadFactor));
				}
			}
			if(isStaggered){
				staggerCount++;
				if (staggerCount>=muzzleCount.Length){
					staggerCount = 0;
				}
					Rigidbody newBullet = Instantiate (wep.bullet, muz [staggerCount].position, muz [staggerCount].rotation) as Rigidbody;
					newBullet.transform.Rotate (Utility.calcSpread (spreadFactor));
			}
		}
	}
		
	public virtual void ammoChange(int quantity, bool reverse){
		if(!Globals.cheatMode){
			if(!reverse){currAmmo[currAmmoIndex]-=quantity;}
			if(reverse){currAmmo[currAmmoIndex]+=quantity;}
			if(ammoChanged!=null){
				this.ammoChanged();
			}
		}
	}

	protected virtual void process(Weapon wep){
		spreadIncrease = wep.spreadBase / 10;
		spreadMax = wep.spreadBase * 3;
		cooldown = wep.weaponCoolDown;
		spreadFactor += spreadIncrease;
	}

	protected virtual void setup(){
		switch(ammoType){
		case ammo.bullet:maxAmmo = 999;currAmmoIndex = 0;break;
		case ammo.shell:maxAmmo = 250;currAmmoIndex = 1;break;
		case ammo.explo:maxAmmo = 250;currAmmoIndex = 2;break;
		case ammo.energy:maxAmmo = 250;currAmmoIndex = 3;break;
		}
		this.cooldown = 0;
		this.spreadFactor = this.spreadBase;
		anim = GetComponent<Animator>();
		currAmmo[currAmmoIndex] = maxAmmo;
	}

	#region properties

	public float SpreadFactor {
		get {
			return this.spreadFactor;
		}
		set {
			spreadFactor = value;
		}
	}

	public float Cooldown {
		get {
			return this.cooldown;
		}
		set {
			cooldown = value;
		}
	}

	public int CurrAmmoIndex {
		get {
			return this.currAmmoIndex;
		}
		set {
			currAmmoIndex = value;
		}
	}

	public int MaxAmmo {
		get {
			return this.maxAmmo;
		}
		set {
			maxAmmo = value;
		}
	}

	public int GetCurrAmmo(int index){
		return Weapon.currAmmo[index];
		}
	#endregion

}