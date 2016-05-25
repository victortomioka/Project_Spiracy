using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class _PatternCone : MonoBehaviour, IPattern{

	[Tooltip("the actual bullet count is actually 3 times as high as this, because of how the sphere algorithm works. Beware lagging")]
	[Range(0,100)]
	public int count;

	[Range(0.0001f,1f)]
	public float animationDelay = 0.0001f;

	float counter;
	
	public bool animate = false;

	Quaternion rot;

	Transform coneCenter;

	Transform coneCaster;

	float delta;

	List<Rigidbody> allBullets = new List<Rigidbody>();

	bool destroyOnEnd;

	GameObject bulletPool;

	public void startPattern(Rigidbody[] subBullets,float spread,Transform muzzle,bool destroy){

		coneCenter = new GameObject().transform;
		coneCaster = new GameObject().transform;
		coneCenter.parent = transform;
		coneCaster.parent = coneCenter;
		coneCenter.position = muzzle.position;
		coneCenter.rotation = muzzle.rotation;
		coneCaster.Rotate(spread,0.0f,0.0f);
		delta = 360.0f / count;
		destroyOnEnd = destroy;
	
		if(!animate){
		for(float f = 0; f < count; f++){
				rot = coneCaster.rotation;
				Rigidbody newBullet = Instantiate (subBullets[Random.Range(0,subBullets.Length)],muzzle.position, rot) as Rigidbody;

				bulletPool = GameObject.Find("bulletPool");
				if(bulletPool!=null){
					newBullet.transform.parent = bulletPool.transform;
				}

				Sleep(newBullet);
				allBullets.Add(newBullet);
				coneCenter.Rotate (0.0f, 0.0f, delta);
		}
		Invoke("Awaken",animationDelay);
		}else{
		StartCoroutine(fireCone(subBullets,spread,muzzle));
		}
	}

	IEnumerator fireCone(Rigidbody[] subBullets,float spread,Transform muzzle){
		counter = 0;
		coneCenter.Rotate(coneCenter.rotation.x,coneCenter.rotation.y,Random.Range(-180,180));
		while(counter < count){
			counter++;
			coneCaster.Rotate(spread,0.0f,0.0f);
			rot = coneCaster.rotation;
			Rigidbody newBullet = Instantiate (subBullets[Random.Range(0,subBullets.Length)],muzzle.position, rot) as Rigidbody;

			bulletPool = GameObject.Find("bulletPool");
			if(bulletPool!=null){
				newBullet.transform.parent = bulletPool.transform;
			}

			Sleep(newBullet);
			allBullets.Add(newBullet);
			coneCenter.Rotate (0.0f, 0.0f, delta);
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
