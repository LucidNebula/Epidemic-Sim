using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Obsolete("Use TimeSpan instead")]
public struct SimulationTime
{
    public int Days { get; set; }
    public int Hours { get; set; }
    public int Minutes { get; set; }
    public float Seconds { get; set; }
    public float TotalTime { get; set; }
    
    public float TotalDays => TotalTime / 86400;

    [Obsolete("Use TimeSpan instead")]
    public TimeOfDay TimeOfDay => new TimeOfDay
    {
        Hours = Hours,
        Minutes = Minutes,
        Seconds = Seconds
    };
    
    public SimulationTime(float seconds)
    {
        TotalTime = seconds;
        Days = (int)(seconds / 86400);
        seconds %= 86400;
        Hours = (int)(seconds / 3600);
        seconds %= 3600;
        Minutes = (int)(seconds / 60);
        seconds %= 60;
        Seconds = seconds;
    }

    public override string ToString()
    {
        return $"{Days}:{Hours:00}:{Minutes:00}:{Seconds:00.00}";
    }
}
