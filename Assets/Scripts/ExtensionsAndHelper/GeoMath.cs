using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GeoMath
{
    public static bool LineSegmentsIntersect(Vector2 a1, Vector2 b1, Vector2 a2, Vector2 b2, out Vector2 intersection)
    {
        Vector2 s1, s2;
        s1 = b1 - a1;
        s2 = b2 - a2;

        //s = (-s1_y * (p0_x - p2_x) + s1_x * (p0_y - p2_y)) / (-s2_x * s1_y + s1_x * s2_y);
        //t = (s2_x * (p0_y - p2_y) - s2_y * (p0_x - p2_x)) / (-s2_x * s1_y + s1_x * s2_y);
        float s = (-s1.y * (a1.x - a2.x) + s1.x * (a1.y - a2.y)) / (-s2.x * s1.y + s1.x * s2.y);
        float t = (s2.x * (a1.y - a2.y) - s2.y * (a1.x - a2.x)) / (-s2.x * s1.y + s1.x * s2.y);

        if (s >= 0 && s <= 1 && t >= 0 && t <= 1)
        {
            intersection = a1 + (s1 * t);
            return true;
        }
        intersection = Vector2.positiveInfinity;
        return false;
    }

    public static bool LineSegmentsIntersect(Vector2 a1, Vector2 b1, Vector2 a2, Vector2 b2)
    {
        Vector2 intersection;
        return LineSegmentsIntersect(a1, b1, a2, b2, out intersection);
    }

    public static float DistFromPointToLineSegment(Vector2 point, Vector2 lineEnd1, Vector2 lineEnd2)
    {
        if (lineEnd1 == lineEnd2) { return Vector2.Distance(point, lineEnd1); }
        float lengthSq = Vector2.SqrMagnitude(lineEnd2 - lineEnd1); //will never be 0 because of above precaution
        float t = Mathf.Clamp01(Vector2.Dot(point - lineEnd1, lineEnd2 - lineEnd1) / lengthSq);
        Vector2 projected = lineEnd1 + t * (lineEnd2 - lineEnd1);
        return Vector2.Distance(point, projected);
    }

    public static float ClosestDistBetweenLineSegments(Vector2 a1, Vector2 b1, Vector2 a2, Vector2 b2)
    {
        //this works because there is no way for a line segment to be closest to another line segment somewhere within it's middle
        //because that would require the line segment to be bent
        return Mathf.Min(DistFromPointToLineSegment(a1, a2, b2), DistFromPointToLineSegment(b1, a2, b2));
    }

    public static bool PointIsOnLeftOfLine(Vector2 point, Vector2 a, Vector2 b)
    {
        return ((b.x - a.x) * (point.y - a.y) - (b.y - a.y) * (point.x - a.x)) > 0;
    }

    //public float QuadArea(Vector2 a, Vector2 b, Vector2 c, Vector2 d)
    //{
    //    return 0.5f * d1 * d2 * sinalpha
    //}
}
