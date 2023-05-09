using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleTaskGenerator : MonoBehaviour
{
	public Agent agent;
	public Transform target1;
	public float speed1;
	public float delay1;
	public Transform target2;
	public float speed2;
	public float delay2;
	public Transform target3;
	public float speed3;
	public void GenerateTask()
	{
		Task task = new Task();
		task.AddInstruction(new GotoTaskInstruction
		{
			Goal = target1,
			Speed = speed1
		});
		
		task.AddInstruction(new WaitTaskInstruction
		{
			Duration = delay1
		});
		
		task.AddInstruction(new GotoTaskInstruction
		{
			Goal = target2,
			Speed = speed2
		});
		
		task.AddInstruction(new WaitTaskInstruction
		{
			Duration = delay2
		});
		
		task.AddInstruction(new GotoTaskInstruction
		{
			Goal = target3,
			Speed = speed3
		});
		
		agent.AddTask(task);
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.X))
		{
			GenerateTask();
			
			if (Input.GetKey(KeyCode.LeftShift))
				for (int i = 0; i < 100; i++)
				{
					GenerateTask();
				}
		}
	}
}
