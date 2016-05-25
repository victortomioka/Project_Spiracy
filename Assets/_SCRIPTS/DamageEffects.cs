using UnityEngine;
using System.Collections;
using UnityStandardAssets.ImageEffects;

public class DamageEffects : MonoBehaviour {

	private static Vector3 cameraOriginalPos;
	private static Quaternion cameraOriginalRot;
	private static Quaternion camRot;

	void Awake(){
		cameraOriginalPos = Camera.main.transform.position;
		cameraOriginalRot = Camera.main.transform.rotation;
	}

	public void ScreenShakeWAberration(int dmg){
		VignetteAndChromaticAberration vig = Camera.main.GetComponent<VignetteAndChromaticAberration>();
		GameObject cameraTransf = Camera.main.gameObject;
		Vector3 magn = new Vector3(dmg,dmg,dmg);
				
		Hashtable reorientParams = new Hashtable();
		reorientParams.Add("Vector3",cameraOriginalPos);
		reorientParams.Add("Quaternion",cameraOriginalRot);
		
		vig.chromaticAberration = 10f*dmg;
		
		StartCoroutine(fadeChromAb(vig, dmg));
		
		iTween.ShakePosition(cameraTransf,iTween.Hash(
			"name","shakepos",
			"amount", magn,
			"time", 0.5f,
			"delay", 0,
			"oncomplete", "Reorient",
			"oncompleteparams", reorientParams,
			"ignoretimescale", true));
		iTween.ShakeRotation(cameraTransf,iTween.Hash(
			"name","shakerot",
			"amount", magn,
			"time", 0.5f,
			"delay", 0));
		}
		
	void Reorient(object vals){
		Hashtable ht = (Hashtable)vals;
		
		Camera.main.transform.position = (Vector3)ht["Vector3"];
		Camera.main.transform.rotation = (Quaternion)ht["Quaternion"];
	}

	IEnumerator fadeChromAb(VignetteAndChromaticAberration vig, int dmg){
		for(float f = 1f; f >= 0 ; f-= 0.1f){
			vig.chromaticAberration = f*10*dmg;
			yield return null;
		}
	}

}
