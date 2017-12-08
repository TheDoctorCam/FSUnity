using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor (typeof (levelGenerator))]
public class MapEditor : Editor {

	public override void OnInspectorGUI ()
	{

		levelGenerator level = target as levelGenerator;

		if (DrawDefaultInspector ()) {
			level.GenerateLevel ();
		}

		if (GUILayout.Button("Generate Level")) {
			level.GenerateLevel ();
		}


	}
	
}
