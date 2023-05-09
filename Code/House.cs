using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class House : MonoBehaviour
{
    [SerializeField] private Bed[] beds;
    
    public Bed[] Beds => transform.GetComponentsInChildren<Bed>();
    public Chair[] Chairs => transform.GetComponentsInChildren<Chair>();
    
    /// <summary>
    /// Gets an unclaimed chair. The operation throws an exception if there is no unclaimed chair available.
    /// </summary>
    public Chair AnyUnclaimedChair => Chairs.First(c => c.claimedBy == null);

    // Start is called before the first frame update
    private void Start()
    {
        beds = Beds;

        if (Beds.Length > Chairs.Length)
            throw new Exception($"House {transform.name} has more beds than chairs!");
    }

}
