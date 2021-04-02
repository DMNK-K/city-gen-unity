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

    public Texture2D MakeDebugTexture(float distPerPixel, int size)
    {
        Texture2D tex = new Texture2D(size, size);
        tex.filterMode = FilterMode.Bilinear;
        float start = -distPerPixel * size / 2;
        float fullRange = baseLayer.Range + midLayer1.Range + midLayer2.Range + detailLayer.Range;
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float val = GetFullElevation(new Vector2(start + distPerPixel * x, start + distPerPixel * y));
                val = GS.DualLerp(0f, 1f, -fullRange, fullRange, val);
                tex.SetPixel(x, y, new Color(val, val, val));
            }
        }
        tex.Apply();
        return tex;
    }
}
