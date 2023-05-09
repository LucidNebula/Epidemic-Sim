using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR

[CustomEditor(typeof(CustomNavmeshNavigator))]
public class CustomNavmeshNavigatorEditor : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		CustomNavmeshNavigator t = (CustomNavmeshNavigator)target;
		if (GUILayout.Button("Recompute path"))
		{
			Vector3 target = t.target;
			t.SetGoal(target);
		}
	}
}

#endif