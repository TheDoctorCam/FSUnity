using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface iDamageable{


	void TakeDamage (float damage, Vector3 hitPoint, Vector3 hitDirection);

	void ApplyDamage (float damage);
}