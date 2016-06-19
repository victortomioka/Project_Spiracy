using UnityEngine;
using System.Collections;

public class Weapon_Shotgun : Weapon {

	public int shotCount = 1;

	public override void shoot (Weapon wep, Transform[] muz)
	{
		if (Globals.canShoot && cooldown <= 0 && currAmmo[currAmmoIndex]>=ammoConsumption) {
			process(wep);
			ammoChange(ammoConsumption, false);

			if(!isStaggered){
				for(int j = 0; j<muz.Length; j++){
					for(int i = 0; i<this.shotCount; i++){
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
				for(int i = 0; i<this.shotCount; i++){
					Rigidbody newBullet = Instantiate (wep.bullet, muz[staggerCount].position, muz[staggerCount].rotation) as Rigidbody;
						newBullet.transform.Rotate (Utility.calcSpread(spreadFactor));
				}
			}
		}
	}


}
