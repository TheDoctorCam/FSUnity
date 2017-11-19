using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof (levelGenerator))]

public class LevelEditor : Editor {

	/* Allows dynamic level change in Unity without having to restart game */
	public override void OnInspectorGUI (){

		levelGenerator level = target as levelGenerator;

		if (DrawDefaultInspector ()) {
			level.GenerateLevel ();
		}

		if (GUILayout.Button("Generate Level")){
			level.GenerateLevel ();
		}
	}

}
