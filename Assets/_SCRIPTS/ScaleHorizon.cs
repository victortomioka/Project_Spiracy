using UnityEngine;
using System.Collections;

public class ScaleHorizon : MonoBehaviour {

	public Vector3 endScale = Vector3.one*2;
	private Vector3 startScale = Vector3.one;

	public float time = 60.0f;

	void Start () {
		StartCoroutine(ScaleCoroutine());
	}

	IEnumerator ScaleCoroutine(){
		float t = 0.0f;

		while (t<time){
			transform.localScale = Vector3.Lerp(startScale,endScale,t/time);
			t+=Time.deltaTime;
			yield return null;
		}
		transform.localScale = endScale;
	}
}
