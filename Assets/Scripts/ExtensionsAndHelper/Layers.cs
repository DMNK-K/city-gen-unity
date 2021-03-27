using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Helper class that allows moving away from using strings for layers and layer masks
/// </summary>
public class Layers
{
    private static readonly Dictionary<Layer, LayerMask> masks = new Dictionary<Layer, LayerMask>()
    {
        {Layer.Default, (1 << (int)Layer.Default)},
        {Layer.TransparentFX, (1 << (int)Layer.TransparentFX)},
        {Layer.IgnoreRaycast, (1 << (int)Layer.IgnoreRaycast)},
        {Layer.Water, (1 << (int)Layer.Water)},
        {Layer.RimSphere, (1 << (int)Layer.RimSphere)},
        {Layer.Street, (1 << (int)Layer.Street)},
        {Layer.CityBlockTerrain, (1 << (int)Layer.CityBlockTerrain)},
        {Layer.Building, (1 << (int)Layer.Building) },
    };

    public static LayerMask GetMask(Layer layer)
    {
        return masks[layer];
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
    RimSphere = 10,
    Street = 11,
    CityBlockTerrain = 12,
    Building = 13,
}
