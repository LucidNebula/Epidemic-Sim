using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighSpeedTriggerTest : MonoBehaviour
{
    public CustomNavmeshNavigator nav;
    public Transform goal;
    public float speed = 100;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log("Go!");
            nav.SetGoal(goal.position);
            nav.speed = speed;
        }
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Goal goal))
        {
            Debug.Log("Enter!");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out Goal goal))
        {
            Debug.Log("Exit!");
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent(out Goal goal))
        {
            Debug.Log("Stay!");
        }
    }
}
