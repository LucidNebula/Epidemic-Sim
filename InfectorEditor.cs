using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR

[CustomEditor(typeof(Infector))]
public class InfectorEditor : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		Infector t = (Infector)target;
		if (GUILayout.Button("Infect"))
		{
			t.Infect();
		}
	}
}

#endif