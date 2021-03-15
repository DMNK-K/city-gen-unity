using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtVector
{
    public static Vector2 Clamp(this Vector2 v, float min, float max)
    {
        if (min > max)
        {
            float n = max;
            max = min;
            min = n;
        }

        v.x = Mathf.Clamp(v.x, min, max);
        v.y = Mathf.Clamp(v.y, min, max);
        return v;
    }

    public static Vector3 Clamp(this Vector3 v, float min, float max)
    {
        if (min > max)
        {
            float n = max;
            max = min;
            min = n;
        }

        v.x = Mathf.Clamp(v.x, min, max);
        v.y = Mathf.Clamp(v.y, min, max);
        v.z = Mathf.Clamp(v.z, min, max);
        return v;
    }

    public static Vector2Int Abs(this Vector2Int v)
    {
        return new Vector2Int(Mathf.Abs(v.x), Mathf.Abs(v.y));
    }

    public static Vector2 Abs(this Vector2 v)
    {
        return new Vector2(Mathf.Abs(v.x), Mathf.Abs(v.y));
    }

    public static Vector3 Abs(this Vector3 v)
    {
        return new Vector3(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z));
    }

    public static Vector2Int RoundToInt(this Vector2 v2)
    {
        return new Vector2Int(Mathf.RoundToInt(v2.x), Mathf.RoundToInt(v2.y));
    }

    public static Vector3Int RoundToInt(this Vector3 v3)
    {
        return new Vector3Int(Mathf.RoundToInt(v3.x), Mathf.RoundToInt(v3.y), Mathf.RoundToInt(v3.z));
    }

    public static Vector2Int FloorToInt(this Vector2 v2)
    {
        return new Vector2Int(Mathf.FloorToInt(v2.x), Mathf.FloorToInt(v2.y));
    }

    public static Vector2Int CeilToInt(this Vector2 v2)
    {
        return new Vector2Int(Mathf.CeilToInt(v2.x), Mathf.CeilToInt(v2.y));
    }

    public static Vector2Int StripX(this Vector2Int v2)
    {
        return new Vector2Int(0, v2.y);
    }

    public static Vector2Int StripY(this Vector2Int v2)
    {
        return new Vector2Int(v2.x, 0);
    }

    public static Vector3 StripX(this Vector3 v3)
    {
        return new Vector3(0, v3.y, v3.z);
    }

    public static Vector3 StripY(this Vector3 v3)
    {
        return new Vector3(v3.z, 0, v3.z);
    }

    public static Vector3 StripZ(this Vector3 v3)
    {
        return new Vector3(v3.x, v3.y, 0);
    }

    public static Vector2 V3ToV2CutZ(this Vector3 v3)
    {
        return new Vector2(v3.x, v3.y);
    }

    public static Vector2 ToV2(this Vector2Int v2)
    {
        return new Vector2(v2.x, v2.y);
    }

    public static Vector3 ToV3(this Vector2Int v2)
    {
        return new Vector3(v2.x, v2.y, 0);
    }

    public static Vector3 ToV3(this Vector2 v2)
    {
        return new Vector3(v2.x, v2.y, 0);
    }

    public static Vector3 SwapX(this Vector3 v3, float value)
    {
        return new Vector3(value, v3.y, v3.z);
    }

    public static Vector3 SwapY(this Vector3 v3, float value)
    {
        return new Vector3(v3.x, value, v3.z);
    }

    public static Vector3 SwapZ(this Vector3 v3, float value)
    {
        return new Vector3(v3.x, v3.y, value);
    }

    public static bool ComponentsWithin01(this Vector2 v)
    {
        return (v.x >= 0 && v.x <= 1 && v.y >= 0 && v.y <= 1);
    }

    public static bool ComponentsWithinRange(this Vector2 v, float min, float max)
    {
        return (v.x >= min && v.y >= min && v.x <= max && v.y <= max);
    }

    public static List<Vector2Int> BuildDiamondMatrix(this Vector2Int center, int radius, bool exclude_center = false)
    {
        List<Vector2Int> matrix = new List<Vector2Int>();
        if (radius < 0) { radius = 0; }
        for (int y = 0; y <= radius; y++)
        {
            int row_radius = radius - y;
            for (int x = 0; x <= row_radius; x++)
            {
                if (y == 0 && x == 0)
                {
                    if (!exclude_center) { matrix.Add(center); }
                }
                else if (y == 0)
                {
                    matrix.Add(center - Vector2Int.right * x);
                    matrix.Add(center + Vector2Int.right * x);
                }
                else if (x == 0)
                {
                    matrix.Add(center - Vector2Int.up * y);
                    matrix.Add(center + Vector2Int.up * y);
                }
                else
                {
                    matrix.Add(center - Vector2Int.right * x - Vector2Int.up * y);
                    matrix.Add(center + Vector2Int.right * x - Vector2Int.up * y);
                    matrix.Add(center - Vector2Int.right * x + Vector2Int.up * y);
                    matrix.Add(center + Vector2Int.right * x + Vector2Int.up * y);
                }
            }
        }
        return matrix;
    }

    public static int DistMHTo(this Vector2Int from, Vector2Int to)
    {
        Vector2Int v2 = from - to;
        v2 = v2.Abs();
        return v2.x + v2.y;
    }

    public static Vector3 RotateAroundPivot(this Vector3 point, Vector3 pivot, Quaternion rot)
    {
        return rot * (point - pivot) + pivot;
    }

    public static Vector3 RandomInRange(Vector3 range)
    {
        Vector3 v3 = Vector3.zero;
        v3.x = Random.Range(-range.x, range.x);
        v3.y = Random.Range(-range.y, range.y);
        v3.z = Random.Range(-range.z, range.z);
        return v3;
    }

    public static Vector3 ShiftToV3(this Vector2 v2)
    {
        return new Vector3(v2.x, 0, v2.y);
    }

    public static Vector2 UnShiftToV2(this Vector3 v3)
    {
        return new Vector2(v3.x, v3.z);
    }

    public static bool SameOrOppositeDir(this Vector2 v2, Vector2 other, float tolerance = 0.001f)
    {
        return (Mathf.Abs(Vector2.Dot(v2.normalized, other.normalized)) >= 1f - tolerance);
    }

    public static bool Similar(this Vector2 v2, Vector2 other, float tolerance = 0.001f)
    {
        return (Mathf.Abs(v2.x - other.x) < tolerance && Mathf.Abs(v2.y - other.y) < tolerance);
    }

    public static bool Similar(this Vector3 v3, Vector3 other, float tolerance = 0.001f)
    {
        Vector3 diff = (v3 - other).Abs();
        return (diff.x < tolerance && diff.y < tolerance && diff.z < tolerance);
    }

    public static Vector2 RotateAroundPivot(this Vector2 v, Vector2 pivot, float angle)
    {
        float radians = angle * Mathf.PI / 180f;
        v -= pivot;
        float cos = Mathf.Cos(radians);
        float sin = Mathf.Sin(radians);
        return pivot + new Vector2(v.x * cos + v.y * sin, -v.x * sin + v.y * cos);
    }
}
