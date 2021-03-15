using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StreetTraversal
{
    public Street Street { get; private set; }
    public bool FromAToB { get; private set; }

    public StreetTraversal(Street street, bool fromAToB)
    {
        Street = street;
        FromAToB = fromAToB;
    }
}
