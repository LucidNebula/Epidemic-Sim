using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TMPro;
using UnityEditor;
using UnityEngine;

public class EpidemicTracker : MonoBehaviour
{
    [SerializeField] private TimeController timeController;
    private List<AgentRecord> records = new List<AgentRecord>();
    private TimeSpan lastRecordedTime;
    
    [Tooltip("The delay between record sampling, in simulation hours")]
    [SerializeField] private float recordInterval = 1;
    
    [Tooltip("GUI text to display the agent infection stats")]
    [SerializeField] private TMP_Text guiText;

    public int susceptibleAgents;
    public int infectedAgents;
    public int recoveredAgents;
    
    // Start is called before the first frame update
    void Start()
    {
        lastRecordedTime = timeController.SimulationTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (timeController.SimulationTime - lastRecordedTime >= TimeSpan.FromSeconds(recordInterval * 3600))
        {
            lastRecordedTime = timeController.SimulationTime;
            records.Add(new AgentRecord
            {
                days = timeController.SimulationTime.TotalDays,
                susceptibleAgents = susceptibleAgents,
                infectedAgents = infectedAgents,
                recoveredAgents = recoveredAgents
            });
        }

        guiText.text = $"S/I/R: {susceptibleAgents}/{infectedAgents}/{recoveredAgents}";

        if (Input.GetKeyDown(KeyCode.H))
        {
            Dump();
        }
    }

    public void Dump()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("Days,Susceptible,Infected,Recovered");
        
        foreach (var record in records)
        {
            sb.AppendLine($"{record.days},{record.susceptibleAgents},{record.infectedAgents},{record.recoveredAgents}");
        }
        
        File.WriteAllText("epidemic.csv", sb.ToString());
    }
}

#if UNITY_EDITOR

[CustomEditor(typeof(EpidemicTracker))]
public class EpidemicTrackerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EpidemicTracker t = (EpidemicTracker)target;
        if (GUILayout.Button("Dump"))
        {
            t.Dump();
        }
    }
}

#endif

struct AgentRecord
{
    public double days;
    public int susceptibleAgents;
    public int infectedAgents;
    public int recoveredAgents;
}