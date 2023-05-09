using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

[CreateAssetMenu(fileName = "Scenario", menuName = "Scenario", order = 0)]
public class Scenario : ScriptableObject
{
	[Tooltip("How many infection units this agent, if infected, transmits to other agents per hour, in meters, based on its distance to the other agent, assuming 1x transmissivity")]
	public AnimationCurve infectionRangeCurve;
    
	[Tooltip("The multiplier of how many infection units to confer onto nearby agents as a function of how long this agent has been infected, in days")]
	public AnimationCurve transmissionCurve;
	public AnimationCurve infectionRangeCurveMasked;

	// Uniformly pick a value between these two. This is the number of infection points needed to become infected
	public float minInfectionPoints = 90;
	public float maxInfectionPoints = 110;
	
	[Tooltip("The interval at which we update the infection points, in seconds of simulation time")]
	public float infectionUpdateInterval = 60.0f;

	[Tooltip("Infection time +/- this value for the next tick. Helps to spread things out and avoid lag spikes")]
	public float infectionUpdateIntervalOffset = 20.0f;
	
	[Tooltip("Minimum number of days it takes from infection to symptoms")]
	public float minSymptomTime = 1.5f;
	
	[Tooltip("Maximum number of days it takes from infection to symptoms")]
	public float maxSymptomTime = 2.5f;

	[Tooltip("Minimum number of days it takes from infection to recovery")]
	public float minRecoveryTime = 6;
	
	[Tooltip("Maximum number of days it takes from infection to recovery")]
	public float maxRecoveryTime = 9;

	[Tooltip("If an agent is not near any infected agents, then decrease this many infection points per hour")] 
	public float falloff = 5;

	[Tooltip("The number of agents that start off infected")]
	public int initialInfectedAgents = 1;

	[Tooltip("Compliance of people to wear masks")]
	public float maskComplianceLevels = 30;
	// Set to 30 when using masks
	public bool Maskless = false;
	public bool SickMask = false;
	public bool AllMask = true;
}
