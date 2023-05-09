using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavigationTest : MonoBehaviour
{
    public GameObject target;
    public NavMeshPath path;
    public Rigidbody rb;
    public float speed = 1;
    
    public List<Vector3> points = new List<Vector3>(8192);
    
    // Start is called before the first frame update
    void Start()
    { 
	    path = new NavMeshPath();
	    
    }

    // Update is called once per frame
    void Update()
    {
	    for (int i = 0; i < path.corners.Length - 1; i++)
		    Debug.DrawLine(path.corners[i], path.corners[i + 1], Color.magenta);
	    
	    for (int i = 0; i < points.Count - 1; i++)
		    Debug.DrawLine(points[i], points[i + 1], Color.white);
    }

    private void FixedUpdate()
    {
	    if (target != null)
		    NavMesh.CalculatePath(transform.position, target.transform.position, NavMesh.AllAreas, path);
	    
	    
	    if (path is null || path.corners.Length < 2)
		    return;

	    // points.Add(path.corners[0]);
	    points.Add(transform.position);
	    
	    // The direction to move on next update. Speed and stuff will be calculated in the future
	    Vector3 nextPosVector = path.corners[1] - path.corners[0];
	    nextPosVector = new Vector3(nextPosVector.x, 0, nextPosVector.z); // Zero the y vector so it's on a 2d plane

	    Transform transform1 = transform;
	    Debug.DrawRay(transform1.position, nextPosVector, Color.green);
	    
	    // Currently, the nextPosVector is the vector from the navmesh's transform to the next corner, but with no y component.
	    // The length is the distance from the transform to the next corner.;
	    
	    // To prevent overshoot
	    
	    // Local position of the agent's desired next direction. This is essentially a multiplier on nextPosVector.
	    Vector3 nextTickDirection = speed * Time.fixedDeltaTime * nextPosVector.normalized;
	    
	    // If the nextTickDirection vector is longer than nextPosVector, we will overshoot next tick, so we need to shorten it.
	    if (nextTickDirection.magnitude > nextPosVector.magnitude)
		    nextTickDirection = nextPosVector;
	    
	    // By now, nextTickDirection is a non-normalized vector that points in the direction of the next corner, but is not longer than the distance to the next corner.
	    // Again, disregarding the y-component.

	    Debug.DrawRay(transform1.position, nextTickDirection, Color.blue);
	    
	    // Move the agent
	    rb.MovePosition(transform1.position + nextTickDirection);
	    
	    
    }
}
