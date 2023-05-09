using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR

[CustomEditor(typeof(Agent))]
public class AgentEditor : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		Agent t = (Agent)target;
		if (GUILayout.Button("Get status"))
		{
			Debug.Log(t.GetStatus());
		}
	}
}

#endif