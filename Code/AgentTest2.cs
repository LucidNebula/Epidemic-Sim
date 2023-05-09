using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentTest2 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Agent agent))
        {
            Debug.Log("Heya " + agent.name);
        }
    }
}
