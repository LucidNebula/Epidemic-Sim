using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[Obsolete("Use TimeSpan instead")]
public struct TimeOfDay : IEquatable<TimeOfDay>
{
	public int Hours { get; set; }
	public int Minutes { get; set; }
	public float Seconds { get; set; }
	
	/// <summary>
	/// Converts hours, minutes, and seconds to seconds. Equivalent to the number of seconds past midnight.
	/// </summary>
	public float SecondsPastMidnight => Hours * 3600 + Minutes * 60 + Seconds;
	
	
	public override string ToString()
	{
		return $"{Hours:00}:{Minutes:00}:{Seconds:00.00}";
	}

	/// <summary>
	/// Returns true if value is between a and b. This method handles new days correctly
	/// </summary>
	public static bool IsBetween(TimeOfDay a, TimeOfDay b, TimeOfDay value)
	{
		if (a == b)
			return false;

		// Normal case, no time crosses midnight
		if (a < b)
		{
			return value > a && value < b;
		}
		else // Crosses midnight. It will show up as b < a. Run the above test, swapping a and b, but invert the results, because if it's in this half it can't be in the other.
		{
			return !(value > b && value < a);
		}
	}

	public static bool operator ==(TimeOfDay a, TimeOfDay b)
	{
		return a.Hours == b.Hours && a.Minutes == b.Minutes && Math.Abs(a.Seconds - b.Seconds) < 0.0001;
	}

	public static bool operator !=(TimeOfDay a, TimeOfDay b)
	{
		return !(a == b);
	}

	public static bool operator >(TimeOfDay a, TimeOfDay b)
	{
		if (a.Hours > b.Hours)
			return true;
		if (a.Hours < b.Hours)
			return false;

		if (a.Minutes > b.Minutes)
			return true;
		if (a.Minutes < b.Minutes)
			return false;

		if (a.Seconds > b.Seconds)
			return true;

		return false;
	}

	public static bool operator <(TimeOfDay a, TimeOfDay b)
	{
		if (a.Hours < b.Hours)
			return true;
		if (a.Hours > b.Hours)
			return false;

		if (a.Minutes < b.Minutes)
			return true;
		if (a.Minutes > b.Minutes)
			return false;

		if (a.Seconds < b.Seconds)
			return true;

		return false;
	}
	
	public bool Equals(TimeOfDay other)
	{
		return this == other;
	}

	public override bool Equals(object obj)
	{
		return obj is TimeOfDay other && Equals(other);
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(Hours, Minutes, Seconds);
	}

	/// <summary>
	/// Returns the number of seconds until time other. This method handles new days correctly.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public float TimeUntil(TimeOfDay other)
	{
		if (this == other)
			return 0;
		
		// Does not cross midnight boundary
		if (other > this)
			return other.SecondsPastMidnight - SecondsPastMidnight;
		
		// Crosses midnight boundary
		return 86400 - (SecondsPastMidnight - other.SecondsPastMidnight);
	}
	
	
	

	public int AgentWaitHandle(TimeOfDay a, TimeOfDay b)
    {
		int secondsA = (int)Math.Floor(a.Seconds);
		int secondsB = (int)Math.Ceiling(b.Seconds);
		int TimeA = a.Hours * 3600 + a.Minutes * 60 + secondsA;
		int TimeB = b.Hours * 3600 + b.Minutes * 60 + secondsB;
		return TimeA - TimeB;
    }

}
