using UnityEngine;
using System.Collections;
using UnityStandardAssets.ImageEffects;

public class HealthManager : MonoBehaviour, IDamageable {

	public int maxHealth = 8;
	public GameObject deathEffect;
	public int currentHealth;
	private bool dead;
	public float flamesDelay = 2f;
	private float flamesTimeLeft = 0;
	private bool burning = false;
	private int fireDamage;
	public float iFrames = 0;
	bool invul = false;
	Texture gameoverImage;

    public event System.Action OnDamage;
    public event System.Action OnDeath;

	protected virtual void Start () 
	{
		currentHealth = maxHealth;
		gameoverImage = Resources.Load("udedlol") as Texture;
	}

	public void FlamesStart (int fdmg ,float flamesTime)
	{
		if(!burning){
		flamesTimeLeft = flamesTime;
		fireDamage = fdmg;
		
		StartCoroutine("Burning");
		}else if (burning){
		flamesTimeLeft = flamesTime;
		fireDamage = fdmg;
		}
	}
	

	IEnumerator Burning(){
		burning = true;
		GameObject flameEff = Instantiate (Resources.Load ("Effects/onFireEffect"),transform.position,transform.rotation) as GameObject;
		flameEff.transform.parent=this.transform;

		while(flamesTimeLeft>0){
			flamesTimeLeft-=Time.deltaTime;
				DealDamage(fireDamage);
				yield return new WaitForSeconds(flamesDelay);
		}

		GameObject.Destroy(flameEff);
		burning = false;

	}

	public void DealDamage (int dmg)
	{
		if(dmg<0)
		{
			Debug.LogError("You cannot pass a negative value for DealDamage, use HealDamage instead!");
			return;
		}
		if(!invul){
		currentHealth -= dmg;

		
        if (OnDamage != null) { 
            this.OnDamage();
        }
		if(gameObject.tag == "Player"){
		Camera.main.gameObject.GetComponent<DamageEffects>().ScreenShakeWAberration(dmg);
		if(iFrames>0){
			StartCoroutine("IFrames");
		}

		}else{
			StartCoroutine("IFramesEnemy");
		}

		if(currentHealth<=0 && !dead)
		{
			Death();
		}
		}
	}

	public void HealDamage (int heal)
	{
		if(heal<0)
		{
			Debug.LogError("You cannot pass a negative value for HealDamage, use DealDamage instead!");
			return;
		}

		currentHealth += heal;

		if(currentHealth>=maxHealth){
			currentHealth=maxHealth;
		}
	}

	int getHealth(){
		HealthManager thisHealth = gameObject.GetComponent<HealthManager>();
		return(thisHealth.currentHealth);
	}

	void Death (){
        if (OnDeath != null)
        {
            this.OnDeath();
        }
        
        if(gameObject.tag=="Player"){
			gameOver();
			print("you died!");
		}

		if(gameObject.tag=="Enemy"){
			if(deathEffect!=null){
			Utility.spawnEffect(deathEffect,transform.position,transform.rotation);
			}
			dead = true;
			GameObject.Destroy(gameObject);
		}
	}

	IEnumerator IFrames(){
		Material mat = this.GetComponent<Renderer>().material;
		Color startColor = mat.color;
		Color fadeColor = new Color(mat.color.r,mat.color.g,mat.color.b,mat.color.a*0.2f);
		float iFrameTime = 0;
		
		while(iFrameTime<iFrames){
			mat.color = Color.Lerp(startColor,fadeColor,Mathf.PingPong(iFrameTime*(10/iFrames), 1));
			iFrameTime+=Time.deltaTime;
			invul = true;
			yield return null;
		}
		mat.color = startColor;
		if(!Globals.cheatMode && this.gameObject.tag=="Player"){
		invul = false;
		}
	}
	
	IEnumerator IFramesEnemy(){
		Material mat = this.GetComponent<Renderer>().material;
		Color startColor = mat.color;
		Color fadeColor = Color.red;
		float iFrameTime = 0;
		
		while(iFrameTime<iFrames){
			mat.color = Color.Lerp(startColor,fadeColor,Mathf.PingPong(iFrameTime*(2/iFrames), 1));
			iFrameTime+=Time.deltaTime;
			invul = true;
			yield return null;
		}
		mat.color = startColor;
		invul = false;
	}

	void gameOver(){
		Time.timeScale=0;
		Globals.playerDead = true;
		iTween.StopByName("shakepos");
		iTween.StopByName("shakerot");
	}

	void OnGUI(){
		if(Globals.playerDead){
			GUI.Box(new Rect(0,0,Screen.width,Screen.height),gameoverImage);
		}
	}
}
