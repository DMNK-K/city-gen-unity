using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    public BuildingStats Stats { get; private set; }


    public void Init(BuildingStats stats)
    {
        Stats = stats;
    }
}
