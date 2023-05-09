using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Custom navmesh agent implementation that fixes the issue of agents travelling too far at high speed
/// and getting stuck. However, it does mean that an agent will always spend at least one frame at each corner.
/// </summary>
public class CustomNavmeshNavigator : MonoBehaviour
{
    public Vector3 target;
    public NavMeshPath path;
    public int targetPathIndex = 1;
    public int pathCorners; // For debug only
    public Rigidbody rb;
    
    // Speed in meters/second
    public float speed = 1;

    public bool drawDebugLines;
    public bool debugFlag;

    public float RemainingDistance
    {
	    get
	    {
		    // No path, no distance
		    if (targetPathIndex >= path.corners.Length)
			    return 0;
		    
		    float distance = 0;
			for (int i = targetPathIndex; i < path.corners.Length - 1; i++)
			    distance += Vector3.Distance(path.corners[i], path.corners[i + 1]);
			
			// Above doesn't handle from targetPathIndex to current position, so we add it here
			// Add a down vector because our agent's transform is about 1 meter above the ground
			// We do this only if the distance is less than 10, to save computation.
			// This does have the small drawback of the value being possibly slightly inaccurate by about up to 2
			// meters when the distance is greater than 10 meters.
			if (distance < 10)
			{
				NavMesh.SamplePosition(transform.position + Vector3.down, out NavMeshHit hit, 3, NavMesh.AllAreas);
				distance += Vector3.Distance(hit.position, path.corners[targetPathIndex]);
			}
			else
			{
				distance += Vector3.Distance(transform.position + Vector3.down, path.corners[targetPathIndex]);
			}
			
			return distance;
	    }
    }

    // Start is called before the first frame update
    void Start()
    { 
	    path = new NavMeshPath();
    }

    // Update is called once per frame
    void Update()
    {
	    if (drawDebugLines)
		    for (int i = 0; i < path.corners.Length - 1; i++)
			    Debug.DrawLine(path.corners[i], path.corners[i + 1], Color.magenta);
    }
    
    public void SetGoal(Vector3 pos)
	{
	    target = pos;
	    targetPathIndex = 1;

	    if (!NavMesh.CalculatePath(transform.position, target, NavMesh.AllAreas, path))
	    {
		    throw new Exception($"Could not calculate a path for agent {transform.name}");
	    }

	    if (path.status == NavMeshPathStatus.PathInvalid)
		    throw new Exception($"Invalid path for agent {transform.name}");
	    
	    pathCorners = path.corners.Length;
	}

    private void FixedUpdate()
    {

	    if (path is null || path.corners.Length < 2)
		    return;
	    
	    // Handle partial paths if necessary by recomputing when at the end of the path
	    if (targetPathIndex >= path.corners.Length)
	    {
		    if (path.status == NavMeshPathStatus.PathPartial)
		    {
			    Vector3 goal = target;
			    SetGoal(goal);
		    }
		    else // Otherwise, the path is not partial (and we assume is valid), so it's done. Return.
		    {
			    return;
		    }
	    }
		    
	    Transform transform1 = transform;
	    
	    // The direction to move on next update. Speed and stuff will be calculated in the future
	    Vector3 nextPosVector = path.corners[targetPathIndex] - transform1.position;
	    nextPosVector = new Vector3(nextPosVector.x, 0, nextPosVector.z); // Zero the y vector so it's on a 2d plane

	    
	    
	    
	    // Currently, the nextPosVector is the vector from the navmesh's transform to the next corner, but with no y component.
	    // The length is the distance from the transform to the next corner.;
	    
	    // To prevent overshoot
	    
	    // Local position of the agent's desired next direction. This is essentially a multiplier on nextPosVector.
	    Vector3 nextTickDirection = speed * Time.fixedDeltaTime * nextPosVector.normalized;
	    
	    // If the nextTickDirection vector is longer than nextPosVector, we will overshoot next tick, so we need to shorten it
	    // and also advance the path index.
	    if (nextTickDirection.magnitude > nextPosVector.magnitude)
	    {
		    nextTickDirection = nextPosVector;
		    targetPathIndex++;
	    }
		    
	    
	    // By now, nextTickDirection is a non-normalized vector that points in the direction of the next corner, but is not longer than the distance to the next corner.
	    // Again, disregarding the y-component.

	    Vector3 position = transform1.position; // Suppresses a warning

	    if (drawDebugLines)
	    {
		    Debug.DrawRay(position, nextTickDirection, Color.blue);
		    Debug.DrawRay(position, nextPosVector, Color.green);
	    }
	    
	    
	    // Move the agent
	    rb.MovePosition(position + nextTickDirection);
    }
}
