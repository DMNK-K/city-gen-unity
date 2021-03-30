using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Global Script class with helper methods for various purposes that don't really belong to anywhere in specific.
/// </summary>
public class GS : MonoBehaviour
{
    public static int GetPolar(bool boolean)
    {
        return (boolean) ? 1 : -1;
    }

    public static float RoundTo(float n, float multiple)
    {
        return Mathf.Round(n / multiple) * multiple;
    }

    public static float FloorTo(float n, float multiple)
    {
        return Mathf.Floor(n / multiple) * multiple;
    }

    public static float CeilTo(float n, float multiple)
    {
        return Mathf.Ceil(n / multiple) * multiple;
    }

    public static List<Vector2> CardinalDirs = new List<Vector2>() { Vector2.up, Vector2.right, Vector2.down, Vector2.left };

    public static float RRangeWithBias(float min, float max, float bias)
    {
        if (bias < 0) { bias = 0; }
        float biaser = Random.Range(0f, 1f);
        biaser = Mathf.Pow(biaser, bias);
        return min + (max - min) * biaser;
    }

    public static bool HaveOppositeSign(float a, float b)
    {
        return (a > 0 && b < 0) || (a < 0 && b > 0);
    }

    public static bool RChance(float chance)
    {
        return (Random.Range(0f, 100f - float.Epsilon) < chance);
    }

    public static int RMultiRoll(float chance, int maxCount)
    {
        int count;
        for (count = 0; count < maxCount; count++)
        {
            if (!RChance(chance)) { break; }
        }
        return count;
    }

    public static Vector2 ROnUnitCircle(float radius)
    {
        float angle = Random.Range(0f, Mathf.PI * 2);
        float x = Mathf.Sin(angle) * radius;
        float y = Mathf.Cos(angle) * radius;
        return new Vector2(x, y);
    }

    public static int RPolar()
    {
        return (RChance(50)) ? 1 : -1;
    }

    public static float TrueMod(float a, float b)
    {
        return a - b * Mathf.Floor(a / b);
    }

    public static int TrueMod(int a, int b)
    {
        return a - b * Mathf.FloorToInt((a * 1f) / (b * 1f));
    }

    public static float DualLerp(float outputMin, float outputMax, float inputMin, float inputMax, float input)
    {
        float t = Mathf.InverseLerp(inputMin, inputMax, input);
        return Mathf.Lerp(outputMin, outputMax, t);
    }

    /// <summary>
    /// Calculates rotation that the parent would need to have to make the child face in aim direction.
    /// </summary>
    public static Quaternion RotationToAimChildAt(Quaternion parentRot, Quaternion childRot, Vector3 aimDir)
    {
        Quaternion lookRot = Quaternion.LookRotation(aimDir);
        lookRot = lookRot * Quaternion.Inverse(childRot);
        return lookRot * parentRot;
    }
}
