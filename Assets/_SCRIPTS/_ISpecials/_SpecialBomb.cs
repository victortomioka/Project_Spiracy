using UnityEngine;
using System.Collections;

public class _SpecialBomb : MonoBehaviour, ISpecial {
	
	public LayerMask mask;
	public GameObject blastEffect;
	public GameObject projEffect;

public void specialAttack(){
		if(blastEffect!=null){
			Instantiate(blastEffect,transform.position,Quaternion.identity);
		}
		Bullet[] bullets = Object.FindObjectsOfType<Bullet>();
		Missile[] missiles = Object.FindObjectsOfType<Missile>();


		foreach(Bullet bul in bullets){
			if (mask == (mask | (1 << bul.gameObject.layer))){
				bul.GetComponent<Rigidbody>().velocity= Vector3.zero;
				if(projEffect!=null){
					Instantiate(projEffect,bul.transform.position,Quaternion.identity);
				}
				GameObject.Destroy(bul.gameObject,0.1f);
			}
		}

		foreach(Missile mis in missiles){
			if (mask == (mask | (1 << mis.gameObject.layer))){
				mis.GetComponent<Rigidbody>().velocity= Vector3.zero;
				if(projEffect!=null){
					Instantiate(projEffect,mis.transform.position,Quaternion.identity);
				}
				GameObject.Destroy(mis.gameObject,0.1f);
			}
		}
	}
}
