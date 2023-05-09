using System;
using JetBrains.Annotations;
using UnityEngine;

[Obsolete]
public class TaskQueue : MonoBehaviour
{
	/// <summary>
	/// Gets the next task in the queue. This is the first element, and should be considered the task being executed.
	/// Returns null if the task queue is empty
	/// </summary>
	[CanBeNull]
	public Task GetTask()
	{
		return transform.childCount == 0 ? null : transform.GetChild(0).GetComponent<Task>();
	}

	
	/// <summary>
	/// Marks the task at the head of the queue as completed, and deletes it
	/// </summary>
	public void NextTask()
	{
		if (transform.childCount == 0)
			return;
		
		Destroy(transform.GetChild(0).gameObject);
	}

	/// <summary>
	/// Adds a new task to the back of the queue
	/// </summary>
	public void AddTask(Task task)
	{

	}
}