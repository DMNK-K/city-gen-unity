using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Representation of a virtual triangle in 2D.
/// </summary>
public struct Triangle2
{
    public Vector2 A { get; private set; }
    public Vector2 B { get; private set; }
    public Vector2 C { get; private set; }

    public float Area { get; private set; }
    public float Perimiter { get; private set; }

    public float AB { get; private set; }
    public float BC { get; private set; }
    public float CA { get; private set; }

    public Triangle2(Vector2 a, Vector2 b, Vector2 c)
    {
        A = a;
        B = b;
        C = c;

        AB = Vector2.Distance(a, b);
        BC = Vector2.Distance(b, c);
        CA = Vector2.Distance(c, a);

        Perimiter = AB + BC + CA;
        float s = Perimiter * 0.5f;
        Area = Mathf.Sqrt(s * (s - AB) * (s - BC) * (s - CA));
    }

    public Vector2 RandomPointInside()
    {
        float r1 = Mathf.Sqrt(Random.Range(0f, 1f));
        float r2 = Random.Range(0f, 1f);
        return (A * (1 - r1)) + (B * (r1 * (1 - r2))) + (C * (r2 * r1));
    }
}
