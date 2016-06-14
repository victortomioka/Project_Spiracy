using UnityEngine;
using System.Collections;

public class MouseRotate : MonoBehaviour {

	public float rotSpeed = 30;
	public float accel = 1;

	float currSpeed = 0;

	float rotX;
	float rotY;

	float lastMouseX;
	float lastMouseY;

	Animator rotAnim;

	void Awake(){
		rotAnim = GameObject.Find("shipMaster").GetComponent<Animator>();
	}

	void OnMouseDrag(){
		rotAnim.enabled=false;
		StopAllCoroutines();
		if(currSpeed<rotSpeed){
			currSpeed+=accel;
		}
		rotX = Input.GetAxis("Mouse X")*currSpeed*Mathf.Deg2Rad;
		rotY = Input.GetAxis("Mouse Y")*currSpeed*Mathf.Deg2Rad;
		lastMouseX = Input.GetAxis("Mouse X");
		lastMouseY = Input.GetAxis("Mouse Y");

		transform.RotateAround(Vector3.up,-rotX);
		transform.RotateAround(Vector3.right,-rotY);
	}

	void OnMouseUp(){

		StartCoroutine(Inertia());
	}

	IEnumerator Inertia(){
		float momentum = currSpeed;
		float counter = 0;
		while(currSpeed>0){
			counter+=Time.deltaTime;
			currSpeed=Mathf.SmoothStep(momentum,0,counter);

			rotX = lastMouseX*currSpeed*Mathf.Deg2Rad;
			rotY = lastMouseY*currSpeed*Mathf.Deg2Rad;

			transform.RotateAround(Vector3.up,-rotX);
			transform.RotateAround(Vector3.right,-rotY);

			yield return null;
		}
		yield return new WaitForSeconds(0.1f);
		rotAnim.enabled=true;
		StartCoroutine(tweenPlayback());
		yield return null;
	}

	IEnumerator tweenPlayback(){
		rotAnim.speed=0;
		while(rotAnim.speed<1){
			rotAnim.speed+=Time.deltaTime;
			yield return null;
		}
		yield return null;
	}

}
