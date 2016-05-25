using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class _PatternSnake : MonoBehaviour, IPattern{

	[Tooltip("the actual bullet count is actually 3 times as high as this, because of how the sphere algorithm works. Beware lagging")]
	[Range(0,100)]
	public int count;

	[Range(0.0001f,1f)]
	public float animationDelay = 0.0001f;

	float counter;

	public bool animate = false;

	Quaternion rot;

	List<Rigidbody> allBullets = new List<Rigidbody>();
	
	bool destroyOnEnd;

	GameObject bulletPool;

	public void startPattern(Rigidbody[] subBullets,float spread,Transform muzzle,bool destroy){
		destroyOnEnd = destroy;
		if(!animate){
		for(float f = 0; f < count; f++){
				rot.eulerAngles = new Vector3(0,spread*Mathf.Sin(f*Mathf.PI*0.1f),0);
				Rigidbody newBullet = Instantiate (subBullets[Random.Range(0,subBullets.Length)],muzzle.position, rot) as Rigidbody;
				newBullet.AddForce (newBullet.transform.forward*f*count*animationDelay, ForceMode.VelocityChange);

				bulletPool = GameObject.Find("bulletPool");
				if(bulletPool!=null){
					newBullet.transform.parent = bulletPool.transform;
				}

				Sleep(newBullet);
				allBullets.Add(newBullet);
			
		}
			Invoke("Awaken",animationDelay);
		}else{
		StartCoroutine(fireSnake(subBullets,spread,muzzle));
		}
	}

	IEnumerator fireSnake(Rigidbody[] subBullets,float spread,Transform muzzle){
		counter = 0;
		while(counter < count){
			counter++;
			rot.eulerAngles = new Vector3(0,spread*Mathf.Sin(counter*Mathf.PI*0.1f),0);
			Rigidbody newBullet = Instantiate (subBullets[Random.Range(0,subBullets.Length)],muzzle.position, rot) as Rigidbody;
			newBullet.AddForce (newBullet.transform.forward*counter*count*animationDelay, ForceMode.VelocityChange);

			bulletPool = GameObject.Find("bulletPool");
			if(bulletPool!=null){
				newBullet.transform.parent = bulletPool.transform;
			}

			Sleep(newBullet);
			allBullets.Add(newBullet);
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
