using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SplashScreen : MonoBehaviour {
	
	Image splashimg;
	public float duration = 1.0f;
	public float startDelay = 0.5f;
	public bool enterAnimation;
	public float enterDuration = 1.0f;
	public string levelname;

	void Start () {
		splashimg = GameObject.Find("Splash").GetComponent<Image>();
		Time.timeScale=1;
		StartCoroutine(transition());
	}
	
	IEnumerator transition(){
		Color origin = Color.white;
		Color end = new Color(255,255,255,0);
		Color current;
		float counter = 0;

		if(enterAnimation){
		while(counter<enterDuration){
			counter+=Time.deltaTime;
			current = Color.Lerp(end,origin,counter);
			splashimg.color = current;
			yield return new WaitForSeconds(1f/60);
		}

		counter = 0;
		}

		while (counter<startDelay){
			counter+=Time.deltaTime;
			yield return null;
		}
		counter = 0;

		while(counter<duration){
			counter+=Time.deltaTime;
			current = Color.Lerp(origin,end,counter);
			splashimg.color = current;
			yield return new WaitForSeconds(1f/60);
		}

		if(levelname!=null){
		SceneManager.LoadScene(levelname);
		}else{
		Debug.LogError("You need to specify a level!");
		}

		yield return null;
	}
}
