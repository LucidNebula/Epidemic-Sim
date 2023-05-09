using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class TimeController : MonoBehaviour
{
    
    [Header("TextMeshPro")] 
    [SerializeField] private TMP_Text tmpSpeed;
    [SerializeField] private TMP_Text tmpTimeElapsed;
    
    /// <summary>
    /// The base scale between the real time and simulation time. A value of 1 means a 1:1 scale,
    /// 60 means one real second is one simulation minute, 120 means one real second is 2 simulation minutes, etc.
    /// Do not modify this while the game is running (or at all!)
    /// </summary>
    [Header("Configuration")]
    [SerializeField] private float scale = 60;

    /// <summary>
    /// What time to start at. Defaults to midnight.
    /// </summary>
    [SerializeField] private float startTime;

    /// <summary>
    /// How much to multiply the scale by. Use this for things like speeding up/slowing the simulation mid-game 
    /// </summary>
    public float multiplier = 1;

    [Tooltip("The maximum time between frames, in seconds. If the simulation is lagging, this sets an upper bound as to how much time has passed since the previous update")]
    public float maxTimeBetweenUpdates = 1200;

    /// <summary>
    /// Elapsed time in seconds from the simulation's perspective
    /// </summary>
    [Header("Display")]
    [SerializeField] private float elapsedTime;

    /// <summary>
    /// The ratio between real and simulated time
    /// </summary>
    public float SimulationScale => scale * multiplier;

    public TimeSpan SimulationTime => TimeSpan.FromSeconds(elapsedTime);
    public TimeSpan TimeOfDay => TimeSpan.FromMilliseconds(SimulationTime.TotalMilliseconds % 86400000);
    

    // Start is called before the first frame update
    private void Start()
    {
        elapsedTime = startTime;
        
        
    }

    // Update is called once per frame
    private void Update()
    {
        float delta = Time.deltaTime * SimulationScale;
        elapsedTime += Math.Min(delta, maxTimeBetweenUpdates);

        if (delta > maxTimeBetweenUpdates)
            Debug.LogWarning($"Simulation is lagging! Delta time: {delta} seconds, max time between updates: {maxTimeBetweenUpdates} seconds");
        
        if (Input.GetKeyDown(KeyCode.Alpha1))
            multiplier = 1;
        if (Input.GetKeyDown(KeyCode.Alpha2))
            multiplier = 2;
        if (Input.GetKeyDown(KeyCode.Alpha3))
            multiplier = 16;
        if (Input.GetKeyDown(KeyCode.Alpha4))
            multiplier = 64;
        if (Input.GetKeyDown(KeyCode.Alpha5))
            multiplier = 256;
        if (Input.GetKeyDown(KeyCode.Space))
            multiplier = 0;
        if (Input.GetKeyDown(KeyCode.BackQuote))
            multiplier = 0.5f;
        if (Input.GetKeyDown(KeyCode.Alpha0))
            multiplier = 1 / scale;

        tmpSpeed.text = $"Speed: {SimulationScale:0.00} ({multiplier:0.00}x)";
        tmpTimeElapsed.text = $"Time Elapsed: {SimulationTime:dd\\:hh\\:mm\\:ss\\.fff} ({elapsedTime:0.00})";
    }
}
