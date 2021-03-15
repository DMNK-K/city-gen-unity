using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DB
{
    private static bool enabled = true;

    public static void Log(string msg, int importance = 0)
    {
        if (enabled)
        {
            if (importance > 0)
            {
                msg = (importance > 1) ? "||==== ||==== " + msg + " " : "==== " + msg + " ";
                msg = msg.PadRight(30 + 10 * importance, '=');
            }
            Debug.Log(msg);
        }
    }

    public static void Error(string msg)
    {
        if (enabled) { Debug.LogError(msg); }
    }

    public static void VisualizeOverlapBox(Vector3 center, Vector3 halfEx, Quaternion orientation, Color color, float time, bool ignoreY)
    {
        List<Vector3> points = new List<Vector3>(4);
        points.Add(center + new Vector3(-halfEx.x, 0, halfEx.z));
        points.Add(center + new Vector3(halfEx.x, 0, halfEx.z));
        points.Add(center + new Vector3(halfEx.x, 0, -halfEx.z));
        points.Add(center + new Vector3(-halfEx.x, 0, -halfEx.z));
        if (ignoreY)
        {
            for (int i = 0; i < points.Count; i++)
            {
                points[i] = points[i].RotateAroundPivot(center, orientation);
            }
            for (int i = 0; i < 4; i++)
            {
                Debug.DrawLine(points[i], points[(i + 1) % 4], color, time);
            }
        }
        else
        {
            for (int i = 0; i < 4; i++)
            {
                points.Add(points[i] + Vector3.up * halfEx.y);
                points[i] -= Vector3.up * halfEx.y;
            }
            for (int i = 0; i < points.Count; i++)
            {
                points[i] = points[i].RotateAroundPivot(center, orientation);
            }
            for (int i = 0; i < 4; i++)
            {
                Debug.DrawLine(points[i], points[(i + 1) % 4], color, time);
                Debug.DrawLine(points[i * 2], points[(i * 2 + 1) % 4], color, time);
                Debug.DrawLine(points[i], points[i + 4], color, time);
            }
        }
    }
}
