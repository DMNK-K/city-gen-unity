using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevationGen : MonoBehaviour
{

    [SerializeField]
    private SmartPerlin baseLayer;
    [SerializeField]
    private SmartPerlin midLayer1;
    [SerializeField]
    private SmartPerlin midLayer2;
    [SerializeField]
    private SmartPerlin detailLayer;

    private void Awake()
    {
        baseLayer.RandomizeOrigin();
        midLayer1.RandomizeOrigin();
        midLayer2.RandomizeOrigin();
        detailLayer.RandomizeOrigin();
    }

    public float GetBase(Vector2 coord)
    {
        return baseLayer.GetValue(coord);
    }

    public float GetMid1(Vector2 coord)
    {
        return midLayer1.GetValue(coord);
    }

    public float GetMid2(Vector2 coord)
    {
        return midLayer2.GetValue(coord);
    }

    public float GetDetail(Vector2 coord)
    {
        return detailLayer.GetValue(coord);
    }

    public float GetFullElevation(Vector2 coord)
    {
        return GetDetail(coord) + GetMid2(coord) + GetMid1(coord) + GetBase(coord);
    }

    //public Texture2D CreateVisualization()
    //{
    //    int size = 1024;
    //    Texture2D tex = new Texture2D(size, size);
    //    tex.filterMode = FilterMode.Bilinear;
    //    for (int y = 0; y < size; y++)
    //    {
    //        for (int x = 0; x < size; x++)
    //        {
    //            float val = GetFullElevation(new Vector2(x, y));
    //            tex.SetPixel(x, y, new Color())
    //        }
    //    }
    //}
}
