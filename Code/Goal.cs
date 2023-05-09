using System;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class Goal : MonoBehaviour
{
    public float t;

    private void Update()
    {
        t += Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Destroy(other.gameObject);
    }
}
