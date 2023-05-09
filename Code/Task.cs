using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;

public class Task
{
	// Task instructions managed by the Hierarchy.
	private readonly List<TaskInstruction> instructions = new List<TaskInstruction>();
	private readonly List<Claimable> claims = new List<Claimable>();
	private int currentInstructionIndex;
	public string name;

	public Task(string name = "Unnamed Task")
	{
		this.name = name;
	}

	/// <summary>
	/// Appends an instruction to this task
	/// </summary>
	public void AddInstruction(TaskInstruction instruction)
	{
		instructions.Add(instruction);
	}
	
	/// <summary>
	/// Marks a claimable object needed by this task. When this task is given to the agent's queue,
	/// it should reserve all claims, and release the claims when the task is done.
	/// </summary>
	public void AddClaim(Claimable claim)
	{
		claims.Add(claim);
	}
	
	public IReadOnlyCollection<Claimable> Claims => claims;

	/// <summary>
	/// Returns the next instruction to be executed, or null if there isn't one.
	/// The next instruction is determined by the currentInstructionIndex, which is
	/// incremented when this method is called.
	/// </summary>
	/// <returns></returns>
	[CanBeNull]
	public TaskInstruction GetCurrentInstruction()
	{
		return IsTaskComplete ? null : instructions[currentInstructionIndex];
	}

	/// <summary>
	/// Advances the instruction index by one
	/// </summary>
	public void NextInstruction()
	{
		currentInstructionIndex++;
	}

	/// <summary>
	/// If all instructions have been executed, this is true
	/// </summary>
	/// <returns></returns>
	public bool IsTaskComplete => currentInstructionIndex >= instructions.Count;

	public override string ToString()
	{
		StringBuilder sb = new StringBuilder();
		
		sb.AppendLine($"Task: {name}");
		sb.AppendLine($"Instruction index: {currentInstructionIndex}");

		foreach (TaskInstruction instruction in instructions)
		{
			sb.AppendLine(instruction.ToString());
		}

		return sb.ToString();
	}
}