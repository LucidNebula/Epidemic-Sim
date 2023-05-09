using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Timers;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

public class SimulationManager : MonoBehaviour
{
    [SerializeField] private TimeController timeController;
    [SerializeField] private TaskMaker taskMaker;
    [SerializeField] private EpidemicTracker epidemicTracker;
    [SerializeField] private GameObject agentPrefab;
    [SerializeField] private House[] houses;
    [SerializeField] private List<Agent> agents = new List<Agent>();
    [SerializeField] private School[] schools;
    [SerializeField] private Workplace[] workplaces;
    
    [Tooltip("Static scenario data for the infector, describes infection curves, points to infection, intervals, etc.")]
    [SerializeField] private Scenario scenario;
    
    private int lastHour;
    [SerializeField] private GameObject goal;
    [SerializeField] private float LastMaskComplianceTime = 0;

    // Start is called before the first frame update
    private void Start()
    {
        houses = FindObjectsOfType<House>();
        schools = FindObjectsOfType<School>();
        workplaces = FindObjectsOfType<Workplace>();
        // Set default compliance level
        scenario.maskComplianceLevels = 30;

        foreach (House house in houses)
        {
            List<Agent> family = new List<Agent>();
            foreach (Bed bed in house.Beds)
            {
                Transform t = bed.sleepPosition;
                GameObject o = Instantiate(agentPrefab, t.position, t.rotation);
                Agent agent = o.GetComponent<Agent>();
                agent.timeController = timeController;
                agent.bed = bed;
                agent.house = house;
                agent.agentType = bed.agentType;
                agent.AllTasksCompletedEvent += GenerateEventFor;
                
                Infector infector = agent.GetComponent<Infector>();
                infector.timeController = timeController;
                infector.epidemicTracker = epidemicTracker;
                infector.scenario = scenario;
                
                // TODO: Remove me - debug only
                if (agent.agentType == AgentType.Adult)
                    agent.GetComponent<Renderer>().material.color = new Color(0, 1, 0);
                
                // Assign a workplace randomly, if it's hiring (adults only, no child labour here, nope!)
                if (agent.agentType == AgentType.Adult)
                {
                    Workplace w = GetHiringWorkplace();
                    w.Employ(agent);
                }
                else if (agent.agentType == AgentType.Child)
                {
                    School s = GetOpenSchool();
                    s.Enroll(agent);
                }

                agents.Add(agent);
                family.Add(agent);
            }
            
            // Assign family
            foreach (Agent agent in family)
            {
                // Copy family to a new list
                agent.family = new List<Agent>(family);
                agent.family.Remove(agent); // Remove self from family
            }
        }

        // Infect some agents
        for (int i = 0; i < scenario.initialInfectedAgents; i++)
        {
            Agent agent = agents[Random.Range(0, agents.Count)];
            Infector infector = agent.GetComponent<Infector>();
            infector.Infect();
        }
        
        // Assign schedules to students
        
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            // Teleport to a random infected agent
            Agent[] infecteds = agents.Where(a => a.GetComponent<Infector>().infectionStage == InfectionStage.Infectious).ToArray();
            if (infecteds.Length == 0)
                return;
            
            Agent agent = infecteds[Random.Range(0, infecteds.Length)];
            
            Camera.main.transform.position = agent.transform.position + new Vector3(0, 10, 0);
        }
        IncrementMaskCompliance();
    }

    /// <summary>
    /// Generates an event for this agent based on factors like time.
    /// </summary>
    public void GenerateEventFor(Agent agent)
    {
        TimeSpan timeOfDay = timeController.TimeOfDay;
        TimeSpan simulationTime = timeController.SimulationTime;
        int currentDay = timeController.SimulationTime.Days;

        //Generate Random Time Offset
        int ranSecond = Random.Range(0, 60);
        int ranMinute = Random.Range(0, 14);

        // School/work time
        if (timeOfDay >= new TimeSpan(8, 30, 0) && timeOfDay <= new TimeSpan(15, 0, 0))
        {
            if (agent.agentType == AgentType.Adult)
            {
                Chair schoolChair = agent.workplace.AnyUnclaimedChair;
                TimeSpan until = new TimeSpan(currentDay, 15, ranMinute, ranSecond);
                agent.AddTask(taskMaker.MakeGotoWorkUntilTask(agent, schoolChair, until));
            }
            
            if (agent.agentType == AgentType.Child)
            {
                ranMinute = Random.Range(0, 2);
                
                // TODO: Consider fixing issue where high speed may break this because of too large timesteps
                
                if (timeOfDay >= new TimeSpan(8, 30, 0) && timeOfDay <= new TimeSpan(10, 30, 0))
                {
                    Chair schoolChair = agent.school.AnyUnclaimedChairInClassroom(agent.classSchedule[0]);
                    TimeSpan until = new TimeSpan(currentDay, 10, 30 + ranMinute, ranSecond);
                    TimeSpan release = until - new TimeSpan(0, 0, 30, 0);
                    agent.AddTask(taskMaker.MakeGotoClassUntilTask(agent, schoolChair, release, until));
                }
                else if (timeOfDay >= new TimeSpan(10, 30, 0) && timeOfDay <= new TimeSpan(12, 0, 0))
                {
                    Chair schoolChair = agent.school.AnyUnclaimedChairInClassroom(agent.classSchedule[1]);
                    TimeSpan until = new TimeSpan(currentDay, 12, ranMinute, ranSecond);
                    TimeSpan release = until - new TimeSpan(0, 0, 30, 0);
                    agent.AddTask(taskMaker.MakeGotoClassUntilTask(agent, schoolChair, release, until));
                }
                else if (timeOfDay >= new TimeSpan(12, 0, 0) && timeOfDay <= new TimeSpan(13, 30, 0))
                {
                    Chair schoolChair = agent.school.AnyUnclaimedChairInClassroom(agent.classSchedule[2]);
                    TimeSpan until = new TimeSpan(currentDay, 13, 30 + ranMinute, ranSecond);
                    TimeSpan release = until - new TimeSpan(0, 0, 30, 0);
                    agent.AddTask(taskMaker.MakeGotoClassUntilTask(agent, schoolChair, release, until));
                }
                else if (timeOfDay >= new TimeSpan(13, 30, 0) && timeOfDay <= new TimeSpan(15, 0, 0))
                {
                    Chair schoolChair = agent.school.AnyUnclaimedChairInClassroom(agent.classSchedule[3]);
                    TimeSpan until = new TimeSpan(currentDay, 15, ranMinute, ranSecond);
                    TimeSpan release = until - new TimeSpan(0, 0, 30, 0);
                    agent.AddTask(taskMaker.MakeGotoClassUntilTask(agent, schoolChair, release, until));
                }
            }
        }
        // After school/work
        else if (timeOfDay >= new TimeSpan(15, 0, 0) && timeOfDay <= new TimeSpan(22, 0, 0))
        {
            TimeSpan until = new TimeSpan(currentDay, 22, ranMinute, ranSecond);
            agent.AddTask(taskMaker.MakeDoActivityAtHomeTableUntilTask(agent, agent.house.AnyUnclaimedChair, until));
        }
        // Bedtime
        else if (timeOfDay >= new TimeSpan(21, 0, 0) || timeOfDay <= new TimeSpan(6, 0, 0))
        {
            // If we are past midnight, then we don't need to increment the day since it would have been done above.
            int nextDay = timeOfDay <= new TimeSpan(6, 0, 0) ? currentDay : currentDay + 1;
            
            TimeSpan until = new TimeSpan(currentDay + 1, 6, ranMinute, ranSecond);
            agent.AddTask(taskMaker.MakeSleepUntilTask(agent, until));
        }
        // Before school/work
        else if (timeOfDay >= new TimeSpan(6, 0, 0) && timeOfDay <= new TimeSpan(8, 30, 0))
        {
            TimeSpan until = new TimeSpan(currentDay, 8, 30 + ranMinute, ranSecond);
            agent.AddTask(taskMaker.MakeDoActivityAtHomeTableUntilTask(agent, agent.house.AnyUnclaimedChair, until));
        }
        // IsBetween doesn't handle times exactly on the dot. While extremely unlikely, if it happens, we should wait a few seconds then try again.
        else
        {
            agent.AddTask(taskMaker.MakeIdleForTask(agent, 3));
        }
    }

    /// <summary>
    /// Returns a workplace that has at least one unfilled slot. The exact workplace is determined by uniform random chance. An error is thrown if there are no open workplaces
    /// </summary>
    public Workplace GetHiringWorkplace()
    {
        
        Workplace[] workplacesWithOpenSlots = workplaces.Where(w => w.IsHiring).ToArray();
        
        if (workplacesWithOpenSlots.Length == 0)
            throw new Exception("No workplaces with open slots");

        int random = Random.Range(0, workplacesWithOpenSlots.Length);
        Workplace w = workplacesWithOpenSlots[random];
        
        return w;
    }

    /// <summary>
    /// Finds a school that is still accepting enrolments. The exact school is determined by uniform random chance. An error is thrown if there are no open schools
    /// </summary>
    /// <returns></returns>
    public School GetOpenSchool()
    {
        School[] schoolsWithOpenSlots = schools.Where(s => s.IsEnrolling).ToArray();
        
        if (schoolsWithOpenSlots.Length == 0)
            throw new Exception("No schools with open slots");
        
        int random = Random.Range(0, schoolsWithOpenSlots.Length);
        School s = schoolsWithOpenSlots[random];
        return s;
    }

    private void IncrementMaskCompliance()
    {
        if (timeController.SimulationTime.TotalHours - LastMaskComplianceTime > 4 && scenario.maskComplianceLevels < 95)
        {
            LastMaskComplianceTime += 3;
            scenario.maskComplianceLevels++;
        }
    }
}

