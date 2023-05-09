using UnityEngine;

public class GotoTaskInstruction : TaskInstruction
{
	public Transform Goal { get; set; }
	public float Speed { get; set; }
	
	/// <summary>
	/// Marks if the instruction has been run. Computing ta path and setting it every frame causes lag and sometimes makes agents pause permanently.
	/// </summary>
	public bool Ticked { get; set; }

	public override string ToString()
	{
		return $"GotoTaskInstruction: Goal: {Goal.position}, Speed: {Speed}, Ticked: {Ticked}";
	}
}