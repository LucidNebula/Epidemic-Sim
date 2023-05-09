using UnityEngine;

public class WaitTaskInstruction : TaskInstruction
{
	public float Duration { get; set; }

	public override string ToString()
	{
		return $"WaitTaskInstruction: Duration: {Duration}";
	}
}