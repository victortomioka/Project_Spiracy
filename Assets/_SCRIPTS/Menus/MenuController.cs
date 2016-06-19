using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour {

	#region vars

	public enum menus{
		pressany,main,play,options,credits,quit
	}
	private menus currentMenu = menus.pressany;

	public float fadeDuration = 0.2f;
	public Color fadeEnd = Color.white;

	Text title,pressany,play,options,credits,quitbutton,sure,yes,no,creditsText,creditsTextImply;
	List<Text> allUI = new List<Text>();
	List<Text> mainMenu = new List<Text>();
	List<Text> optionsMenu = new List<Text>();
	List<Text> creditScreen = new List<Text>();
	List<Text> quitScreen = new List<Text>();
	private bool canPressAny = false;
	Color fadeStart = Color.clear;

	#endregion

	#region setup

	void Awake(){
		setup();
	}

	void Start(){
		StartCoroutine(fade(pressany,false));
		StartCoroutine(waitAndFlip(fadeDuration, val => canPressAny = val, canPressAny));
	}

	#endregion

	#region interaction

	void Update(){

	
		if(Input.anyKeyDown){
			if(canPressAny){
				goToMain();
			}
			if(Input.GetKeyDown(KeyCode.Escape)){
				returnButton();
			}
		}
	}

	void returnButton(){
		switch(currentMenu){
		case menus.play:break;
		case menus.options:goToMain();break;
		case menus.credits:goToMain();break;
		case menus.quit:goToMain();break;
		case menus.pressany:quitApp();break;
		case menus.main:goToPressAny();break;
		}
	}

	#endregion

	#region go to

	void goToPressAny(){
		canPressAny = true;
		currentMenu = menus.pressany;
		fadeUIGroup(mainMenu,true);
		fadeUIGroup(pressany,false);
	}

	public void goToMain(){
		switch(currentMenu){
		case menus.options: fadeUIGroup(optionsMenu,true);fadeUIGroup(mainMenu,false);break;
		case menus.credits: fadeUIGroup(creditScreen,true);fadeUIGroup(mainMenu,false);break;
		case menus.quit: fadeUIGroup(quitScreen,true);fadeUIGroup(mainMenu,false);break;
		case menus.pressany: fadeUIGroup(pressany,true);fadeUIGroup(mainMenu,false);canPressAny = false;break;
		}
		currentMenu = menus.main;
	}

	public void goToPlay(){
		fadeUIGroup(mainMenu,true);
		currentMenu = menus.play;
		StartCoroutine(waitAndPlay(fadeDuration));
	}

	public void goToOptions(){
		currentMenu = menus.options;
		fadeUIGroup(mainMenu,true);
	}

	public void goToCredits(){
		currentMenu = menus.credits;
		fadeUIGroup(mainMenu,true);
		fadeUIGroup(creditScreen,false);

	}

	public void goToConfirmQuit(){
		print("aaa");
		currentMenu = menus.quit;
		fadeUIGroup(mainMenu,true);
		fadeUIGroup(quitScreen,false);
	}

	public void quitApp(){
		fadeUIGroup(quitScreen,true);
		fadeUIGroup(pressany,true);
		Application.Quit();
	}

	#endregion
		
	#region animation


	void fadeUIGroup(List<Text> l,bool reverse){
		foreach(Text t in l){
			checkForAnimButton(t.gameObject,reverse);
			StartCoroutine(fade(t,reverse));
		}
	}

	void fadeUIGroup(Text t,bool reverse){
		checkForAnimButton(t.gameObject,reverse);	
		StartCoroutine(fade(t,reverse));
	}

	//fade for text, overloaded for image
	IEnumerator fade(Text t, bool reverse){
		float timer = 0;
		Color fa, fb;

		if(reverse){
			fa = fadeEnd;
			fb = fadeStart;
		}else{
			fa = fadeStart;
			fb = fadeEnd;
		}
		while(timer<fadeDuration){
			t.color = Color.Lerp(fa, fb, timer/fadeDuration);
			timer+=Time.deltaTime;
			yield return null;
		}


		yield return null;
	}

	//fade for image, overloaded from text
	IEnumerator fade(Image i, bool reverse){
		float timer = 0;
		Color fa, fb;
	

		if(reverse){
			fa = fadeEnd;
			fb = fadeStart;
		}else{
			fa = fadeStart;
			fb = fadeEnd;
		}
		while(timer<fadeDuration){
			i.color = Color.Lerp(fa, fb, timer/fadeDuration);
			timer+=Time.deltaTime;
			yield return null;
		}


		yield return null;
	}
		
	#endregion

	#region utility

	IEnumerator waitAndFlip(float t, System.Action<bool> val, bool b){
		yield return new WaitForSeconds(t);
		b = !b;
		val(b);
	}

	IEnumerator waitAndPlay(float t){
		yield return new WaitForSeconds(t);
		SceneManager.LoadScene(2);
	}

	void checkForAnimButton(GameObject tgt, bool reverse){
		Animator ta = tgt.GetComponent<Animator>();
		Button tb = tgt.GetComponent<Button>();

		if(ta!=null){
			if(reverse){
				ta.Rebind();
				ta.enabled=false;
			}
			if(!reverse){
				ta.enabled=true;
		
			}
		
		}
		if(tb!=null){
			if(reverse){
				tb.enabled=false;
			}
			if(!reverse){
				tb.enabled=true;
			}
		}

	}

	#endregion

	#region shit tier ugly code
	//optimize later somehow
	void setup(){

		title = GameObject.Find("title").GetComponent<Text>();
		allUI.Add(title);
		mainMenu.Add(title);
		pressany = GameObject.Find("pressany").GetComponent<Text>();
		allUI.Add(pressany);
		play = GameObject.Find("play").GetComponent<Text>();
		allUI.Add(play);
		mainMenu.Add(play);
		options = GameObject.Find("options").GetComponent<Text>();
		allUI.Add(options);
		mainMenu.Add(options);
		credits = GameObject.Find("credits").GetComponent<Text>();
		allUI.Add(credits);
		mainMenu.Add(credits);
		quitbutton = GameObject.Find("quitbutton").GetComponent<Text>();
		allUI.Add(quitbutton);
		mainMenu.Add(quitbutton);
		sure = GameObject.Find("sure").GetComponent<Text>();
		allUI.Add(sure);
		quitScreen.Add(sure);
		yes = GameObject.Find("yes").GetComponent<Text>();
		allUI.Add(yes);
		quitScreen.Add(yes);
		no = GameObject.Find("no").GetComponent<Text>();
		allUI.Add(no);
		quitScreen.Add(no);
		creditsText = GameObject.Find("creditsText").GetComponent<Text>();
		allUI.Add(creditsText);
		creditScreen.Add(creditsText);
		creditsTextImply = GameObject.Find("implying").GetComponent<Text>();
		allUI.Add(creditsTextImply);
		creditScreen.Add(creditsTextImply);
		currentMenu = menus.pressany;

		foreach(Text t in allUI){
			t.color = fadeStart;
			if(t.GetComponent<Animator>()!=null){
			t.GetComponent<Animator>().enabled=false;
			}
			if(t.GetComponent<Button>()!=null){
			t.GetComponent<Button>().enabled=false;
			}
		}
	}
	#endregion
}
