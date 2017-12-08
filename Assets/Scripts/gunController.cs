using System.Collections;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class gunController : MonoBehaviour {

	public Transform weaponHold;
	public gun[] allGuns;		//added additional weapons
	gun equippedGun;			//Prefab of guns that are active

	void Start() {
		/*if (defaultGun != null) {
			Equip (defaultGun);
		}*/
		
		
	}


	public void Equip(gun gunToEquip) {
		if (equippedGun != null) {
			Destroy(equippedGun.gameObject);
		}
		equippedGun = Instantiate (gunToEquip, weaponHold.position,weaponHold.rotation) as gun;
		equippedGun.transform.parent = weaponHold;
	}

	//Apply to all objects
	public void Equip(int weaponIndex) {
		Equip(allGuns [weaponIndex]);
	}

	//when it is held down 
	public void OnTriggerHold() {
		if (equippedGun != null) {
			equippedGun.OnTriggerHold();
		}
	}

	public void OnTriggerRelease() {
		if (equippedGun != null) {
			equippedGun.OnTriggerRelease();
		}
	}

	public float GunHeight {
		get {
			return weaponHold.position.y;
		}
	}

	public void Shoot(Vector3 shoot) {
		if (equippedGun != null) {
			equippedGun.Shoot(shoot);
		}
	}

	//this will add time to the reloading process
	public void Reload() {
		if (equippedGun != null) {
			equippedGun.Reload();
		}
	}

}