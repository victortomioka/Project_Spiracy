using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIManager : MonoBehaviour {

    GameObject playerObj;
	ShipController playerCtrl;
    HealthManager playerHealth;
	WeaponManager wepCtrl;
    GameObject bar;
	Text healthCounter;
	Image specialGauge;
	GameObject primaryCDGauge;
	GameObject secondaryCDGauge;
	Text primaryCounter;
	Text secondaryCounter;
	Weapon primary;
	Weapon secondary;
	int pIndex;
	int sIndex;

    void Start()
    {
        playerObj = FindObjectOfType<ShipController>().gameObject;
		wepCtrl = playerObj.GetComponent<WeaponManager>();

		playerCtrl = playerObj.GetComponent<ShipController>();
		playerCtrl.OnSpecial += UpdateSpecial;

		wepCtrl.shotPrimary += primaryCR;
		wepCtrl.shotSecondary += secondaryCR;
		wepCtrl.shotPrimary += updateAmmo;
		wepCtrl.shotSecondary += updateAmmo;
	
		playerHealth = playerObj.GetComponent<HealthManager>();
        playerHealth.OnDamage += UpdateHealthbar;

		bar = GameObject.Find("lifebarFill");
		healthCounter = GameObject.Find("healthCounter").GetComponent<Text>();
   
		primaryCDGauge = GameObject.Find("primaryCDFill");
		secondaryCDGauge = GameObject.Find("secondaryCDFill");

		primaryCounter = GameObject.Find("primaryAmmoCounter").GetComponent<Text>();
		secondaryCounter = GameObject.Find("secondaryAmmoCounter").GetComponent<Text>();

		specialGauge = GameObject.Find("specialGauge").GetComponent<Image>();

		Invoke ("Setup",0.1f);
    }

	void Setup(){
		UpdateHealthbar();
		updateAmmo();
	}

    void UpdateHealthbar()
    {
		float fill = (playerHealth.currentHealth * 1.0f / playerHealth.maxHealth * 1.0f);
		bar.transform.localScale = new Vector3((fill*0.115f), bar.transform.localScale.y, bar.transform.localScale.z);
		healthCounter.text = playerHealth.currentHealth + "/" + playerHealth.maxHealth;
    }

	void StartSpecial()
	{
		specialGauge.fillAmount = 0.4f;
	}

	void UpdateSpecial()
	{
		specialGauge.fillAmount = 0.336f*playerCtrl.currSpecial;
	}

	void primaryCR(){	StartCoroutine("updatePrimary");}
	void secondaryCR(){	StartCoroutine("updateSecondary");}

	void updateAmmo(){
		primary = wepCtrl.primary;
		pIndex = wepCtrl.pIndex;
		primaryCounter.text = primary.ammoType + ": " + Weapon.currAmmo[pIndex] + "/" + primary.maxAmmo;
		secondary = wepCtrl.secondary;
		sIndex = wepCtrl.sIndex;
		secondaryCounter.text = secondary.ammoType + ": " + Weapon.currAmmo[sIndex] + "/" + secondary.maxAmmo;
	}

	IEnumerator updatePrimary(){
		primary = wepCtrl.primary;
		while(primary.cooldown>0){
		float fill = primary.cooldown/primary.weaponCoolDown;
		primaryCDGauge.transform.localScale = new Vector3(1-fill, primaryCDGauge.transform.localScale.y, primaryCDGauge.transform.localScale.z);
		yield return null;
		}
	}

	IEnumerator updateSecondary(){
		secondary = wepCtrl.secondary;
		while(secondary.cooldown>0){
		float fill = secondary.cooldown/secondary.weaponCoolDown;
		secondaryCDGauge.transform.localScale = new Vector3(1-fill, secondaryCDGauge.transform.localScale.y, secondaryCDGauge.transform.localScale.z);
		yield return null;
		}
	}

}
