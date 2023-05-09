using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitUntilTaskInstruction : TaskInstruction
{
    public TimeSpan Until { get; set; }
    
    public override string ToString()
    {
        return $"WaitUntilTaskInstruction: Until: {Until}";
    }
}
