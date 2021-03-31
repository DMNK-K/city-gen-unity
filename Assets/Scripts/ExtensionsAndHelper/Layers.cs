using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Helper class that allows moving away from using strings for layers and layer masks
/// </summary>
public class Layers
{
    public static LayerMask GetMask(Layer layer)
    {
        return (1 << (int)layer);
    }
}

public enum Layer
{
    //built in
    Default = 0,
    TransparentFX = 1,
    IgnoreRaycast = 2,
    Water = 4,
    UI = 5,

    //mine
    Player = 9,
    RimSphere = 10,
    Street = 11,
    CityBlockTerrain = 12,
    Building = 13,
}
