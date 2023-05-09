using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class School : MonoBehaviour
{
    public Chair[] Chairs => chairs ??= transform.GetComponentsInChildren<Chair>();
    private Chair[] chairs;
    public Chair AnyUnclaimedChair => Chairs.FirstOrDefault(c => c.claimedBy == null);
    
    // Chairs as above, but by classroom
    public List<Chair>[] ChairsByClassroom => chairsByClassroom ??= chairsByClassroom = Chairs.GroupBy(t => t.roomId).Select(t => t.ToList()).ToArray();
    private List<Chair>[] chairsByClassroom;
    public Chair AnyUnclaimedChairInClassroom(int classroom) => ChairsByClassroom[classroom].FirstOrDefault(c => c.claimedBy == null);
    
    public bool IsEnrolling => students.Count < Chairs.Length;
    
    public List<Agent> students = new List<Agent>();

    // Number of classrooms in the school
    public int ClassroomCount => classroomCount == -1 ? classroomCount = Chairs.Select(t => t.roomId).Distinct().Count() : classroomCount;
    [SerializeField] private int classroomCount = -1;

    // The capacity of each classroom by its room id
    public int[] ClassroomCapacity => classroomCapacity ??= classroomCapacity = Chairs.GroupBy(t => t.roomId).Select(t => t.Count()).ToArray();
    private int[] classroomCapacity;
    
    /// <summary>
    /// The list itself holds agents, and the double array is indexed as [block, room], where block is 0-3
    /// </summary>
    [SerializeField] private List<Agent>[,] classSchedule;

    private void Awake()
    {
        classSchedule = new List<Agent>[4, ClassroomCount];
        
        for (int i = 0; i < 4; i++)
        for (int j = 0; j < ClassroomCount; j++)
            classSchedule[i, j] = new List<Agent>();
    }

    public void Enroll(Agent agent)
    {
        if (!IsEnrolling)
            throw new Exception("School is full!");

        // Assign a schedule for the student
        for (int i = 0; i < 4; i++)
        {
            int room = GetAvailableClassroomForBlock(i);
            classSchedule[i, room].Add(agent);
            agent.classSchedule[i] = room;
        }
        
        agent.school = this;
        students.Add(agent);
    }

    /// <summary>
    /// Given a block, randomly chooses a classroom that is not full and returns the room number
    /// </summary>
    private int GetAvailableClassroomForBlock(int block)
    {
        List<int> availableRooms = new List<int>();
        for (int i = 0; i < ClassroomCount; i++)
        {
            if (classSchedule[block, i].Count < ClassroomCapacity[i])
                availableRooms.Add(i);
        }

        int random = UnityEngine.Random.Range(0, availableRooms.Count);
        
        return availableRooms[random];
    }
}
