using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityGenDiagnostics : MonoBehaviour
{


    private void Awake()
    {
        
    }

    public void BasicStreetLineStats(List<StreetLine> lines)
    {
        if (lines.Count == 0) { return; }
        float avgLength = 0;
        float maxLength = 0;
        float minLength = Mathf.Infinity;
        int diagonal = 0;
        int northSouth = 0;
        int eastWest = 0;
        for (int i = 0; i < lines.Count; i++)
        {
            avgLength += lines[i].Length;
            if (lines[i].Length < minLength)
            {
                minLength = lines[i].Length;
            }
            if (lines[i].Length > maxLength)
            {
                maxLength = lines[i].Length;
            }
            if (Vector2.up.SameOrOppositeDir(lines[i].Dir))
            {
                northSouth++;
            }
            else if (Vector2.right.SameOrOppositeDir(lines[i].Dir))
            {
                eastWest++;
            }
            else
            {
                diagonal++;
            }
        }
        avgLength /= lines.Count;
        DB.Log($"There are {lines.Count} lines, {northSouth} N-S, {eastWest} E-W, {diagonal} diagonal. Avg length: {avgLength}, min: {minLength}, max: {maxLength}");
    }

    public void DebugDrawIntersectionPoints(List<IntersectionPoint> inters, float time)
    {
        for (int i = 0; i < inters.Count; i++)
        {
            inters[i].DebugDraw(time);
        }
    }

    public void CheckIfIntersectionPointsOverlap(List<IntersectionPoint> points)
    {
        int overlapping = 0;
        for (int i = 0; i < points.Count; i++)
        {
            for (int o = 0; o < points.Count; o++)
            {
                if (o == i) { continue; }
                if (points[i].Position.Similar(points[o].Position, 0.01f))
                {
                    overlapping++;
                }
            }
        }
        if (overlapping > 0)
        {
            DB.Error($"There are {overlapping} overlapping IntersectionPoints.");
        }
    }


    public void DebugDrawStreetLineCorners(List<StreetLine> lines, float time)
    {
        for (int i = 0; i < lines.Count; i++)
        {
            lines[i].DebugDrawCorners(time);
        }
    }
}
