using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//class name changed by Ryan and Harrison during implementation of particle effect

public interface damages {

    //changed poor naming conventions

	void TakeHit (float damage, Vector3 hitPoint, Vector3 hitDirection);

	void TakeDamage (float damage);

}