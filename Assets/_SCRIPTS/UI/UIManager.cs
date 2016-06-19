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
		Invoke ("Setup",0.1f);
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
		primaryCounter.text = primary.ammoType + ": " + primary.GetCurrAmmo(pIndex) + "/" + primary.MaxAmmo;
		secondary = wepCtrl.secondary;
		sIndex = wepCtrl.sIndex;
		secondaryCounter.text = secondary.ammoType + ": " + secondary.GetCurrAmmo(sIndex) + "/" + secondary.MaxAmmo;
	}

	IEnumerator updatePrimary(){
		primary = wepCtrl.primary;
		while(primary.Cooldown>0){
		float fill = primary.Cooldown/primary.weaponCoolDown;
		primaryCDGauge.transform.localScale = new Vector3(1-fill, primaryCDGauge.transform.localScale.y, primaryCDGauge.transform.localScale.z);
		yield return null;
		}
	}

	IEnumerator updateSecondary(){
		secondary = wepCtrl.secondary;
		while(secondary.Cooldown>0){
		float fill = secondary.Cooldown/secondary.weaponCoolDown;
		secondaryCDGauge.transform.localScale = new Vector3(1-fill, secondaryCDGauge.transform.localScale.y, secondaryCDGauge.transform.localScale.z);
		yield return null;
		}
	}

	void Setup(){
		playerObj = FindObjectOfType<ShipController>().gameObject;
		wepCtrl = playerObj.GetComponent<WeaponManager>();

		playerCtrl = playerObj.GetComponent<ShipController>();
		playerCtrl.OnSpecial += UpdateSpecial;

		wepCtrl.shotPrimary += primaryCR;
		wepCtrl.shotSecondary += secondaryCR;
		wepCtrl.primary.ammoChanged += updateAmmo;
		wepCtrl.secondary.ammoChanged += updateAmmo;

		playerHealth = playerObj.GetComponent<HealthManager>();
		playerHealth.OnDamage += UpdateHealthbar;

		bar = GameObject.Find("lifebarFill");
		healthCounter = GameObject.Find("healthCounter").GetComponent<Text>();

		primaryCDGauge = GameObject.Find("primaryCDFill");
		secondaryCDGauge = GameObject.Find("secondaryCDFill");

		primaryCounter = GameObject.Find("primaryAmmoCounter").GetComponent<Text>();
		secondaryCounter = GameObject.Find("secondaryAmmoCounter").GetComponent<Text>();

		specialGauge = GameObject.Find("specialGauge").GetComponent<Image>();

		UpdateHealthbar();
		updateAmmo();
	}

}
