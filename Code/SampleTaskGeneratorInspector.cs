using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR

[CustomEditor(typeof(SampleTaskGenerator))]
public class SampleTaskGeneratorInspector : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		SampleTaskGenerator t = (SampleTaskGenerator)target;
		if (GUILayout.Button("Enqueue task"))
		{
			t.GenerateTask();
		}
		
		if (GUILayout.Button("Enqueue task x100"))
		{
			for (int i = 0; i < 100; i++)
				t.GenerateTask();
		}
	}
}

#endif