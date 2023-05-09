using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class Agent : MonoBehaviour
{
    public TimeController timeController;
    public AgentType agentType = AgentType.Adult;
    [SerializeField] private CustomNavmeshNavigator nav;
    
    [SerializeField] private TMP_Text guiText;

    private readonly List<Task> taskQueue = new List<Task>();
    public Bed bed;
    public House house;
    
    [Tooltip("The workplace this agent is assigned to. Should only be assigned to adults")]
    public Workplace workplace;

    [Tooltip("The school this agent is assigned to. Should only be assigned to children")]
    public School school;
    public int[] classSchedule = new int[4]; // Each student has four classes, this holds the room numbers of those classes

    public List<Agent> family = new List<Agent>(8);
    public List<Agent> friends = new List<Agent>();
    
    private Task ActiveTask => taskQueue.Count == 0 ? null : taskQueue[0];

    /// <summary>
    /// Default walk speed at 1x simulation, in m/s
    /// </summary>
    public float walkSpeed = 1.42f;

    public bool debug = false;

    [Header("Last Tick Values")]
    [SerializeField] private float waitTime;
    private TimeSpan lastUpdateTime;

    /// <summary>
    /// Fired every tick when there is no task in the queue
    /// </summary>
    public event Action<Agent> AllTasksCompletedEvent;
    private void Start()
    {
    
    }

    private void Update()
    {
        TaskUpdate();
        
        //if (infection_check == false) {
        //    InfectionUpdate();
        //}
        
        // PathUpdate();
        // GuiUpdate();
        lastUpdateTime = timeController.SimulationTime;
    }

    public void AddTask(Task task)
    {
        taskQueue.Add(task);

        foreach (Claimable claim in task.Claims) 
            claim.claimedBy = this;
    }

    private void PathUpdate()
    {
        NavMeshPath path = nav.path;

        if (path is null || path.corners.Length < 2)
            return;

        for (int i = 0; i < path.corners.Length - 1; i++)
            Debug.DrawLine(path.corners[i], path.corners[i + 1], Color.magenta);
    }


    private void TaskUpdate()
    {
        if (ActiveTask == null)
        {
            AllTasksCompletedEvent?.Invoke(this);
            return;
        }
            

        TaskInstruction taskInstruction = ActiveTask.GetCurrentInstruction();

        if (taskInstruction is GotoTaskInstruction ins1)
            HandleGotoInstruction(ins1);
        if (taskInstruction is WaitTaskInstruction ins2)
            HandleWaitInstruction(ins2);
        if (taskInstruction is WaitUntilTaskInstruction ins3)
            HandleWaitUntilInstruction(ins3);
        if (taskInstruction is ReleaseClaimTaskInstruction ins4)
            HandleReleaseClaimInstruction(ins4);

        if (ActiveTask.IsTaskComplete)
        {
            foreach (Claimable claim in ActiveTask.Claims) 
                claim.claimedBy = null;
            
            taskQueue.RemoveAt(0);
        }
    }

    private void HandleReleaseClaimInstruction(ReleaseClaimTaskInstruction ins4)
    {
        ins4.Claimable.claimedBy = null;
        ActiveTask.NextInstruction();
    }

    private void GuiUpdate()
    {
        guiText.text = $"Current task: {ActiveTask?.name ?? "None"}\nTask count: {taskQueue.Count}";
    }

    private void HandleGotoInstruction(GotoTaskInstruction instruction)
    {
        // if (nav.destination != instruction.Goal.position && !instruction.Ticked)
        if (!instruction.Ticked)
        {
            instruction.Ticked = true;
            nav.SetGoal(instruction.Goal.position);
        }
        
        // Regardless of whether we set a path, we want to be able to control the speed because the simulation speed may have changed.
        nav.speed = instruction.Speed * timeController.SimulationScale;
        
        // Straight line distance check is to ensure that we don't end up in a position where the agent is partway through or still checking for a path and it gets flagged as completed
        // if (nav.remainingDistance < 0.2 && !nav.pathPending && Vector3.Distance(nav.transform.position, instruction.Goal.position) < 2 && ActiveTask != null)
        if (nav.RemainingDistance < 0.2 && Vector3.Distance(nav.transform.position, instruction.Goal.position) < 2 && ActiveTask != null)
        {
            ActiveTask.NextInstruction();
        }
            
    }

    private void HandleWaitInstruction(WaitTaskInstruction instruction)
    {
        waitTime += Time.deltaTime * timeController.SimulationScale;

        if (waitTime >= instruction.Duration)
        {
            waitTime = 0;
            ActiveTask.NextInstruction();
        }
    }

    private void HandleWaitUntilInstruction(WaitUntilTaskInstruction instruction)
    {
        if (timeController.SimulationTime > instruction.Until)
        {
            // Debug.Log($"Test: Time is {timeController.TimeOfDay}, simulation time is {timeController.SimulationTime}");
            ActiveTask.NextInstruction();
        }

    }

 /*   private void InfectionUpdate()
    {
        float randomNum = Random.Range(25, 50);
        float chance = infectionTickCount / (randomNum * 10);
        if (chance > 1)
        {
            infection = 1;
            infection_check = true;
        }
        if (infection == 1)
        {
            gameObject.GetComponent<Renderer>().material.color = new Color(1, 0, 0);
        }
        if (nearby_infected == 0)
        {
            infectionTickCount -= 0.1f * (Time.deltaTime * timeController.SimulationScale);

            if (infectionTickCount < 0)
            {
                infectionTickCount = 0;
            }
        }

    }

    // private void OnTriggerStay(Collider other)
    // {
    //     if (other.TryGetComponent(out Agent agent))
    //     {
    //         if (!(Physics.Linecast (this.transform.position, agent.transform.position)) && agent.infection == 1 && infection ==0)
    //         {
    //             // BUG: Distance can divide by 0
    //             float dist = Vector3.Distance(agent.transform.position, this.transform.position);
    //             infectionTickCount += (infectionCoefficient / (dist + 0.001f)) * (Time.deltaTime * timeController.SimulationScale);
    //             Debug.Log("Increment Counter for: " + agent.name);
    //         }
    //         // Debug.Log("Trigger stay by " + agent.name);
    //     }
    // }
    
    // Might add OnTriggerExit to pause InfectionUpdate
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Agent agent))
        {
            nearbyAgents.Add(agent);
            if (agent.infection == 1)
            {
                nearby_infected += 1;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out Agent agent))
        {
            nearbyAgents.Remove(agent);
            if (agent.infection == 1)
            {
                nearby_infected -= 1;
            }
        }
    }*/

    public string GetStatus()
    {
        StringBuilder sb = new StringBuilder();

        foreach (Task task in taskQueue)
        {
            sb.AppendLine(task.ToString());
        }

        return sb.ToString();
    }
}
