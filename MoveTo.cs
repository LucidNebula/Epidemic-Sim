using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MoveTo : MonoBehaviour
{
    public Transform goal;
    public NavMeshAgent agent;
    // Start is called before the first frame update
    private void Start()
    {
        
    }

    private void Update()
    {
        if (agent.remainingDistance < 5)
        {
            gameObject.GetComponent<Renderer>().material.color = Color.cyan;
        }
    }
}
