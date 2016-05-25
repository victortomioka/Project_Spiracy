using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class _PatternSphere : MonoBehaviour, IPattern{

	[Tooltip("the actual bullet count is actually 3 times as high as this, because of how the sphere algorithm works. Beware lagging")]
	[Range(0,100)]
	public int count;

	[Range(0.0001f,1f)]
	public float animationDelay = 0.0001f;

	float counter;
	float rotator;
	
	public bool animate = false;

	Quaternion rot;
	List<Rigidbody> allBullets = new List<Rigidbody>();
	
	bool destroyOnEnd;

	GameObject bulletPool;

	public void startPattern(Rigidbody[] subBullets,float spread,Transform muzzle,bool destroy){
		destroyOnEnd = destroy;
		if(!animate){
		for(float f = 0; f < count; f++){
			for(float g = 0; g <count; g++){
				rot.eulerAngles = new Vector3(((g/count)*360),((f/count)*360),-100);
				Rigidbody newBullet = Instantiate (subBullets[Random.Range(0,subBullets.Length)],muzzle.position, rot) as Rigidbody;

					bulletPool = GameObject.Find("bulletPool");
					if(bulletPool!=null){
						newBullet.transform.parent = bulletPool.transform;
					}

				Sleep(newBullet);
				allBullets.Add(newBullet);
			}
		}
			Invoke("Awaken",animationDelay);
		}else{
		StartCoroutine(fireSphere(subBullets,spread,muzzle));
		}
	}

	IEnumerator fireSphere(Rigidbody[] subBullets,float spread,Transform muzzle){
		counter = 0;
		rotator = 0;
		while(counter < count){
			counter++;
			while(rotator < counter*count){
				rotator++;
				rot.eulerAngles = new Vector3(((rotator/count)*360),((counter/count)*360),-100);
				Rigidbody newBullet = Instantiate (subBullets[Random.Range(0,subBullets.Length)],muzzle.position, rot) as Rigidbody;

				bulletPool = GameObject.Find("bulletPool");
				if(bulletPool!=null){
					newBullet.transform.parent = bulletPool.transform;
				}

				newBullet.AddForce(newBullet.transform.forward*15, ForceMode.VelocityChange);
				Sleep(newBullet);
				allBullets.Add(newBullet);
				yield return new WaitForSeconds(animationDelay);
			}
			yield return new WaitForSeconds(animationDelay);
		}
		Invoke("Awaken",animationDelay);
		yield return null;
	}

	void Sleep(Rigidbody target){
		Bullet bulComp = target.GetComponent<Bullet>();
		Missile misComp = target.GetComponent<Missile>();
		if(bulComp!=null){
			bulComp.enabled = false;
		}
		if(misComp!=null){
			misComp.enabled = false;
		}
	}

	void Awaken(){
		foreach(Rigidbody bul in allBullets){
			GameObject bulletPool = GameObject.Find("bulletPool");
			if(bulletPool!=null){
			bul.transform.parent = bulletPool.transform;
			}
			Bullet bulComp = bul.GetComponent<Bullet>();
			Missile misComp = bul.GetComponent<Missile>();
			if(bulComp!=null){
				bulComp.enabled = true;
			}
			if(misComp!=null){
				misComp.enabled = true;
			}
		}
		if(destroyOnEnd){
			GameObject.Destroy(gameObject);
		}
	}

	public int getCount(){
		return count;
	}

}
