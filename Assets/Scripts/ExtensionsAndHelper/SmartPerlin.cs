using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SmartPerlin
{
    [SerializeField]
    private float range;
    [SerializeField]
    private float spread;

    [SerializeField]
    private Vector2 originOffset;

    public float Range { get { return range; } }

    private Vector2 origin = Vector2.zero;
    
    public void RandomizeOrigin()
    {
        origin = (Random.insideUnitCircle + originOffset) * spread;
    }

    public float GetValue(Vector2 coord)
    {
        coord = (spread != 0) ? coord / spread + origin : origin;
        float value = (Mathf.PerlinNoise(coord.x, coord.y) - 0.5f) * 2;
        return Mathf.Clamp(value * range, -range, range);
    }
}
