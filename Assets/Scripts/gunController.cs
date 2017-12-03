using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gunController : MonoBehaviour {

	public Transform weaponHold;		//Position on character where they hold the weapon
	public gun defaultGun;				//Prefab of gun
	gun equippedGun;					//Active prefab fun

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
	
	public float GunHeight { // returns height of gun (used for crosshairs height to provide precise aiming)
		get{
			return weaponHold.position.y;
		}
	}
}