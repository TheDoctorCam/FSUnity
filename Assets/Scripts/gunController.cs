using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gunController : MonoBehaviour {

	public Transform weaponHold;
	public gun defaultGun;
	gun equippedGun;

	void Start(){
		if (defaultGun != null) {
			Equip (defaultGun);
		}
	}

	public void Equip(gun gunToEquip){
		if (equippedGun != null) {
			Destroy (equippedGun.gameObject);
		}
		equippedGun = Instantiate (gunToEquip, weaponHold.position, weaponHold.rotation) as gun;
		equippedGun.transform.parent = weaponHold;
	}

	public void Shoot(){
		if (equippedGun != null) {
			equippedGun.Shoot ();		
		}
	}
}
