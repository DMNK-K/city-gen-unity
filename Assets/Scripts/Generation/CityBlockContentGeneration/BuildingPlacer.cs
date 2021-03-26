using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingPlacer : MonoBehaviour
{
    public virtual IEnumerator PlaceBuildings(CityBlock block)
    {
        yield return null;
    }
}
