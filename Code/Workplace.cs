using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Workplace : MonoBehaviour
{
    // public Chair[] Chairs => transform.GetComponentsInChildren<Chair>();
    public Chair[] Chairs => chairs ??= transform.GetComponentsInChildren<Chair>();
    private Chair[] chairs;
    public Chair AnyUnclaimedChair => Chairs.First(c => c.claimedBy == null);
    
    public int MaxWorkers => Chairs.Length;
    public int CurrentWorkers => workers.Count;
    public bool IsHiring => CurrentWorkers < MaxWorkers;
    public List<Agent> workers = new List<Agent>();

    /// <summary>
    /// Marks the agent as working for this place, and assigns the agent as a worker here
    /// </summary>
    public void Employ(Agent agent)
    {
        if (!IsHiring)
            throw new Exception("Workplace is full!");
        
        agent.workplace = this;
        workers.Add(agent);
    }
    
}
