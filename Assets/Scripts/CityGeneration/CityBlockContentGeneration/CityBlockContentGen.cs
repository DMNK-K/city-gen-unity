using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityBlockContentGen : MonoBehaviour
{
    public virtual IEnumerator Generate(CityBlock block)
    {
        yield return null;
    }
}
