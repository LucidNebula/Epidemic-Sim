using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskMaker : MonoBehaviour
{
    public Task MakeSleepUntilTask(Agent agent, TimeSpan until)
    {
        Task task = new Task("Sleep until");
        task.AddInstruction(new GotoTaskInstruction {Goal = agent.bed.sleepPosition, Speed = agent.walkSpeed});
        task.AddInstruction(new WaitUntilTaskInstruction {Until = until});
        return task;
    }

    [Obsolete("Use MakeSleepUntilTask instead")]
    public Task MakeGotoBedTask(Agent agent)
    {
        Task task = new Task("Go to bed");
        task.AddInstruction(new GotoTaskInstruction {Goal = agent.bed.sleepPosition, Speed = agent.walkSpeed});
        return task;
    }

    public Task MakeDebugGotoAndWaitUntilTask(Agent agent, Transform target, TimeSpan until)
    {
        Task task = new Task("Debug goto and wait until");
        task.AddInstruction(new GotoTaskInstruction {Goal = target, Speed = agent.walkSpeed});
        task.AddInstruction(new WaitUntilTaskInstruction {Until = until});
        return task;
    }
    
    public Task MakeDoActivityAtHomeTableUntilTask(Agent agent, Chair chair, TimeSpan until)
    {
        Task task = new Task("Do activity at table until");
        task.AddInstruction(new GotoTaskInstruction {Goal = chair.transform, Speed = agent.walkSpeed});
        task.AddInstruction(new WaitUntilTaskInstruction {Until = until});
        task.AddClaim(chair);
        return task;
    }
    
    public Task MakeGotoSchoolUntilTask(Agent agent, Chair chair, TimeSpan until)
    {
        Task task = new Task("Go to school until");
        task.AddInstruction(new GotoTaskInstruction {Goal = chair.transform, Speed = agent.walkSpeed});
        task.AddInstruction(new WaitUntilTaskInstruction {Until = until});
        task.AddClaim(chair);
        return task;
    }
    
    public Task MakeGotoClassUntilTask(Agent agent, Chair chair, TimeSpan release, TimeSpan until)
    {
        // Release is the time to release the claim so the next class can use it. Recommend setting
        // it about 30-45 minutes before the class ends.
        
        Task task = new Task("Go to class until");
        task.AddInstruction(new GotoTaskInstruction {Goal = chair.transform, Speed = agent.walkSpeed});
        task.AddInstruction(new WaitUntilTaskInstruction {Until = release});
        task.AddInstruction(new ReleaseClaimTaskInstruction {Claimable = chair});
        task.AddInstruction(new WaitUntilTaskInstruction {Until = until});
        task.AddClaim(chair);
        return task;
    }
    
    public Task MakeGotoWorkUntilTask(Agent agent, Chair chair, TimeSpan until)
    {
        Task task = new Task("Go to work until");
        task.AddInstruction(new GotoTaskInstruction {Goal = chair.transform, Speed = agent.walkSpeed});
        task.AddInstruction(new WaitUntilTaskInstruction {Until = until});
        task.AddClaim(chair);
        return task;
    }

    public Task MakeIdleForTask(Agent agent, float duration)
    {
        // Note: Duration is in seconds, simulation time.
        Task task = new Task($"Idle for {duration} seconds");
        task.AddInstruction(new WaitTaskInstruction {Duration = duration});
        return task;

    }
}
