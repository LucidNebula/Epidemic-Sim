using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Component responsible for handling infections
/// </summary>
public class Infector : MonoBehaviour
{
    public TimeController timeController;
    public EpidemicTracker epidemicTracker;

    [Tooltip("Static scenario data for the infector, describes infection curves, points to infection, intervals, etc.")]
    public Scenario scenario;

    [Tooltip("The current infection stage of this agent")]
    public InfectionStage infectionStage = InfectionStage.Susceptible;

    [Tooltip("Current infection points. If it reaches infectionLevelPoints, the agent becomes infected")]
    [SerializeField] private float infectionPoints;
    
    [Tooltip("The number of infection points needed to become infected")]
    [SerializeField] private float infectionLevelPoints;

    [Tooltip("Set by an infected agent if it's nearby. It is this agent's job to unset it after")]
    public bool infectedAgentNearby;
    
    // The simulation time when this agent became infected
    private TimeSpan infectionTime;

    // How long it takes for this agent to show symptoms/recover, in days
    private float symptomTime;
    private float recoveryTime;
    
    // We don't want to update the infection points every frame, so we keep track of the last time we updated it
    // and update it when a certain amount of time has passed, or when an agent enters/leaves the trigger
    private TimeSpan lastInfectionUpdateTime;

    [SerializeField] private float nextOffset;
    
    [SerializeField] private List<Infector> nearbyAgents = new List<Infector>();

    [SerializeField] private bool masked; // If the agent is wearing a mask
    private bool refusal; // If the agent is refusing to wear a mask. Overrides rngRefusal
    private float rngRefusal; // Odds of a person refusing to comply with mask mandates
    

    public float debug = 0;
    // Start is called before the first frame update
    private void Start()
    {
        infectionLevelPoints = Random.Range(scenario.minInfectionPoints, scenario.maxInfectionPoints);
        symptomTime = Random.Range(scenario.minSymptomTime, scenario.maxSymptomTime);
        recoveryTime = Random.Range(scenario.minRecoveryTime, scenario.maxRecoveryTime);
        
        epidemicTracker.susceptibleAgents++;
        refusal = true;
        rngRefusal = Random.Range(0, 50);
        if (rngRefusal >= 5)
        {
            refusal = false;
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (timeController.SimulationTime - lastInfectionUpdateTime > TimeSpan.FromSeconds(scenario.infectionUpdateInterval + nextOffset))
        {
            InfectionUpdate();
            nextOffset = Random.Range(-scenario.infectionUpdateIntervalOffset, scenario.infectionUpdateIntervalOffset);
        }
    }

    public void Infect()
    {
        infectionStage = InfectionStage.Infectious;
        infectionTime = timeController.SimulationTime;

        epidemicTracker.susceptibleAgents--;
        epidemicTracker.infectedAgents++;
            
        gameObject.GetComponent<Renderer>().material.color = new Color(1, 0, 0);
    }

    private void InfectionUpdate()
    {
        // How this infector can infect others
        
        // Set agent to infected if conditions are met
        // There is a little glitch wherein when the simulation starts, trigger enters may run this
        // method before start/awake, and so infectionLevelPoints will be 0. Thus, we must use
        // > and not >= to prevent the agent being instantly set to infectious.
        if (infectionStage == InfectionStage.Susceptible && infectionPoints > infectionLevelPoints)
        {
            Infect();
        }
        
        // Checks if agent will put on a mask
        if (masked == false)
        {
            MaskCheck();
        }

        // Set agent to recovered if conditions are met
        if (infectionStage == InfectionStage.Infectious && timeController.SimulationTime - infectionTime > TimeSpan.FromSeconds(recoveryTime * 24 * 60 * 60))
        {
            infectionStage = InfectionStage.Recovered;
            gameObject.GetComponent<Renderer>().material.color = new Color(1, 1, 1);

            epidemicTracker.infectedAgents--;
            epidemicTracker.recoveredAgents++;
        }

        if (infectionStage == InfectionStage.Infectious)
        {
            foreach (Infector other in nearbyAgents)
            {
                other.infectedAgentNearby = true; // If we are infected, alert all nearby agents that we are
                
                // Skip agents that we do not have line of sight to
                // Linecast returns true if there's anything in the way.
                
                // Additionally, skip any agents that are already infected.
                // Originally we kept only susceptible agents in the nearbyAgents list, but it turns out
                // we also want to keep track of other agent types to handle stuff like dropoff
                if (other.infectionStage != InfectionStage.Susceptible || Physics.Linecast(transform.position, other.transform.position))
                    continue;
                

                // TODO: Debug multiplier and implement dropoff when no infected agents nearby
                float distance = Vector3.Distance(transform.position, other.transform.position);
                float multiplier = scenario.transmissionCurve.Evaluate((float)(timeController.SimulationTime - infectionTime).TotalSeconds / (24 * 60 * 60));
                
                //                              In points per hour,                         Ratio of how much of an hour has passed since the last tick
                float pointsToAdd = scenario.infectionRangeCurve.Evaluate(distance) * (float)(timeController.SimulationTime - lastInfectionUpdateTime).TotalSeconds / (60 * 60);
                if (masked == true || other.masked == true)
                {
                    pointsToAdd = scenario.infectionRangeCurveMasked.Evaluate(distance) * (float)(timeController.SimulationTime - lastInfectionUpdateTime).TotalSeconds / (60 * 60);
                    if (masked == true && other.masked == true)
                    {
                        pointsToAdd = pointsToAdd * 2/3;
                    }
                }
                pointsToAdd *= multiplier;
            
                other.infectionPoints += pointsToAdd;

            }
        }

        // if (in_range == false && infected == false)
        if (infectionStage != InfectionStage.Infectious && !infectedAgentNearby)
        {
            //               In points per hour,                 Ratio of how much of an hour has passed since the last tick
            infectionPoints -= scenario.falloff * (float)(timeController.SimulationTime - lastInfectionUpdateTime).TotalSeconds / (60 * 60);
            debug = scenario.falloff * (float)(timeController.SimulationTime - lastInfectionUpdateTime).TotalSeconds / (60 * 60);
            // How much the infection points go down for this Agent per tick IF there are no infected in linecast range
            if (infectionPoints < 0)
            {
                infectionPoints = 0;
            }
        }

        lastInfectionUpdateTime = timeController.SimulationTime;
        infectedAgentNearby = false;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Infector agent))
        {
            nearbyAgents.Add(agent);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out Infector agent))
        {
            InfectionUpdate();
            nearbyAgents.Remove(agent);
        }
    }

    private void MaskCheck()
    {
        if (scenario.SickMask)
        {
            if (infectionStage == InfectionStage.Infectious && masked == false && refusal == false)
            {
                if (timeController.SimulationTime.TotalHours - infectionTime.TotalHours > (symptomTime * 24))
                {
                    masked = true;
                    gameObject.GetComponent<Renderer>().material.color = new Color(1, 0, 1);
                }
            }   
        }

        else if (scenario.AllMask && masked == false && refusal == false)
        {
            if (rngRefusal + scenario.maskComplianceLevels > 100)
            {
                masked = true;
                gameObject.GetComponent<Renderer>().material.color = new Color(1, 0, 1);
            }
            else if (infectionStage == InfectionStage.Infectious && (timeController.SimulationTime.TotalHours - infectionTime.TotalHours > (symptomTime * 24)))
            {
                masked = true;
                gameObject.GetComponent<Renderer>().material.color = new Color(1, 0, 1);
            }
        }
    }
}
