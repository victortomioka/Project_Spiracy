using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EquipMenu : MonoBehaviour {

	public ShipController[] shipDb;
	int shipIndex = 0;
	public Weapon[] weaponDb;
	int weaponIndex = 0;
	Weapon primaryweapon;
	Weapon secondaryweapon;
	Scrollbar shipScroll,primaryScroll,secondaryScroll;
	Text shipName, primName, secName;
	List<Scrollbar> allScroll = new List<Scrollbar>();
	List<Text> allName = new List<Text>();
	GameObject shipDummy;
	Transform shipPos;
	Transform mount1;
	Transform mount2;
	GameObject weapon1Dummy;
	GameObject weapon2Dummy;

	void Awake () {
		setup();
	}


	public void updateWeapons(){
		float primVal = primaryScroll.value;
		float secVal = secondaryScroll.value;

		if(weapon1Dummy!=null){
			GameObject.Destroy(weapon1Dummy);
		}
		if(weapon2Dummy!=null){
			GameObject.Destroy(weapon2Dummy);
		}

		weapon1Dummy = Instantiate(weaponDb[valueToInt(primVal,weaponDb.Length)].gameObject,mount1.position,mount1.rotation) as GameObject;
		weapon2Dummy = Instantiate(weaponDb[valueToInt(secVal,weaponDb.Length)].gameObject,mount2.position,mount2.rotation) as GameObject;

		weapon1Dummy.GetComponent<Weapon>().enabled=false;
		weapon2Dummy.GetComponent<Weapon>().enabled=false;

		weapon1Dummy.transform.localScale*=0.5f;
		weapon2Dummy.transform.localScale*=0.5f;

		weapon1Dummy.transform.parent = mount1.transform;
		weapon2Dummy.transform.parent = mount2.transform;
	}

	public void updateNames(){
		float shipVal = shipScroll.value;
		float primVal = primaryScroll.value;
		float secVal = secondaryScroll.value;

		shipName.text = shipDb[valueToInt(shipVal,shipDb.Length)].longName;
		primName.text = weaponDb[valueToInt(primVal,weaponDb.Length)].longName;
		secName.text = weaponDb[valueToInt(secVal,weaponDb.Length)].longName;
	}

	public void launchGame(){
		float shipVal = shipScroll.value;
		float primVal = primaryScroll.value;
		float secVal = secondaryScroll.value;

		PlayerData.ship = shipDb[valueToInt(shipVal,shipDb.Length)];
		PlayerData.primary = weaponDb[valueToInt(primVal,weaponDb.Length)];
		PlayerData.secondary = weaponDb[valueToInt(secVal,weaponDb.Length)];

		SceneManager.LoadScene(3);
	}
		
	void setup (){
		shipScroll = GameObject.Find("ship_scrollbar").GetComponent<Scrollbar>();
		allScroll.Add(shipScroll);
		primaryScroll = GameObject.Find("primary_scrollbar").GetComponent<Scrollbar>();
		allScroll.Add(primaryScroll);
		secondaryScroll = GameObject.Find("secondary_scrollbar").GetComponent<Scrollbar>();
		allScroll.Add(secondaryScroll);

		shipName = GameObject.Find("shipname").GetComponent<Text>();
		allName.Add(shipName);
		primName = GameObject.Find("primname").GetComponent<Text>();
		allName.Add(primName);
		secName = GameObject.Find("secname").GetComponent<Text>();
		allName.Add(secName);

		shipScroll.numberOfSteps = shipDb.Length;
		shipScroll.size = 1/shipDb.Length;
		primaryScroll.numberOfSteps = weaponDb.Length;
		primaryScroll.size = 1/weaponDb.Length;
		secondaryScroll.numberOfSteps = weaponDb.Length;
		secondaryScroll.size = 1/weaponDb.Length;

		shipPos = GameObject.Find("shipRot").transform;

		shipName.text = shipDb[0].longName;
		primName.text = weaponDb[0].longName;
		secName.text = weaponDb[0].longName;

		shipSetup();
	}

	void shipSetup () {
		float shipVal = shipScroll.value;
		GameObject dummy = Instantiate(shipDummy = shipDb[valueToInt(shipVal,shipDb.Length)].gameObject,shipPos.position,shipPos.rotation) as GameObject;
		dummy.transform.parent = shipPos;
		dummy.GetComponent<WeaponManager>().enabled = false;
		dummy.GetComponent<ShipController>().enabled = false;
		dummy.GetComponent<HealthManager>().enabled = false;
		dummy.GetComponent<UIManager>().enabled = false;
		dummy.transform.localScale*=0.5f;
		mount1 = GameObject.Find("weaponMount1").transform;
		mount2 = GameObject.Find("weaponMount2").transform;

		updateWeapons();
	}

	int valueToInt(float value, int length){
		float val = value * length;
		int i = (int)val;
		if(i<length){
			return(i);
		}else{
			return(length-1);
		}
	}

}
