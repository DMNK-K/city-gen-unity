using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StreetLine
{
    public IntersectionPoint InterPointA { get; private set; }
    public IntersectionPoint InterPointB { get; private set; }

    public bool AOnRim { get; private set; }
    public bool BOnRim { get; private set; }

    private Vector2 rimA;
    private Vector2 rimB;

    public Vector2 A { get { return (AOnRim) ? rimA : InterPointA.Position; } }
    public Vector2 B { get { return (BOnRim) ? rimB : InterPointB.Position; } }

    public float VirtualWidth { get; private set; } = float.NaN;
    public float Length { get; private set; }
    public Vector2 Dir { get; private set; }

    //corners are similar to edges, but they are corrected by the size of
    //the IntersectionPoints on the ends of this street line (if it has those)
    //those corners are shared between this StreetLine and the IntersectionPoints
    public List<Vector2> corners = new List<Vector2>();
    public Street CorrespondingStreet { get; private set; } = null;

    public StreetLine(Vector2 start, Vector2 end)
    {
        rimA = start;
        rimB = end;
        AOnRim = true;
        BOnRim = true;
        Length = Vector2.Distance(start, end);
        Dir = (B - A).normalized;
    }

    public StreetLine(IntersectionPoint a, Vector2 b)
    {
        InterPointA = a;
        rimB = b;
        AOnRim = false;
        BOnRim = true;
        Length = Vector2.Distance(a.Position, b);
        Dir = (B - A).normalized;
    }

    public StreetLine(Vector2 a, IntersectionPoint b)
    {
        rimA = a;
        InterPointB = b;
        AOnRim = true;
        BOnRim = false;
        Length = Vector2.Distance(a, b.Position);
        Dir = (B - A).normalized;
    }

    public StreetLine(IntersectionPoint a, IntersectionPoint b)
    {
        InterPointA = a;
        InterPointB = b;
        AOnRim = false;
        BOnRim = false;
        Length = Vector2.Distance(a.Position, b.Position);
        Dir = (B - A).normalized;
    }

    public bool HasIntersectionAtPoint(Vector2 point)
    {
        return ((InterPointA != null && InterPointA.Position == point) || (InterPointB != null && InterPointB.Position == point));
    }

    public void SetVirtualWidth(float width)
    {
        DB.Log($"setting VirtualWidth to {width} for line A: {A} to B: {B} with length: {Length}");
        VirtualWidth = width;
    }

    public void AddCornerIfAbsent(Vector2 corner)
    {
        //due to possible floating point undeterminism same corner might be
        //calculated 2 times differently, it's safest to check similarity rather than simply
        //call Contains() or something
        for (int i = 0; i < corners.Count; i++)
        {
            if (corners[i].Similar(corner)) { return; }
        }
        corners.Add(corner);
    }

    public void CleanUpAndSortCorners()
    {
        //if this street line has an end that is on a rim,
        //not all the corners will be added through an IntersectionPoint,
        //since there isn't an intersection point on that end to add them
        //they have to be added here so future use is easier
        //then they have to be ordered so that [0] and [1] correspond to one side of the street
        //and [2] and [3] to the other
        if (float.IsNaN(VirtualWidth))
        {
            DB.Log("Width hasn't been assigned yet to this StreetLine");
            return;
        }
        if (AOnRim)
        {
            AddCornerIfAbsent(A + Vector2.Perpendicular(Dir) * VirtualWidth * 0.5f);
            AddCornerIfAbsent(A - Vector2.Perpendicular(Dir) * VirtualWidth * 0.5f);
        }
        if (BOnRim)
        {
            AddCornerIfAbsent(B + Vector2.Perpendicular(Dir) * VirtualWidth * 0.5f);
            AddCornerIfAbsent(B - Vector2.Perpendicular(Dir) * VirtualWidth * 0.5f);
        }

        if (corners.Count < 4)
        {
            DB.Log("corners.Count is != 4, calling CreateCornersForConnectingLines() on intersection points");
            InterPointA?.CreateCornersForConnected();
            InterPointB?.CreateCornersForConnected();
        }

        if (corners.Count == 4)
        {
            if (!Dir.SameOrOppositeDir(corners[0] - corners[1]))
            {
                Vector2 temp = corners[1];
                corners[1] = corners[2];
                corners[2] = temp;
            }
        }
    }

    public void DebugDraw(float time)
    {
        Debug.DrawLine(new Vector3(A.x, 0, A.y), new Vector3(B.x, 0, B.y), Color.blue, time);
        Debug.DrawRay(new Vector3(A.x, 0, A.y), Vector3.up * 4, (AOnRim) ? Color.yellow : Color.green, time);
        Debug.DrawRay(new Vector3(B.x, 0, B.y), Vector3.up * 4, (BOnRim) ? Color.yellow : Color.green, time);
    }

    public void DebugDrawCorners(float time)
    {
        if (corners.Count == 4)
        {
            for (int i = 0; i < corners.Count; i++)
            {
                Debug.DrawRay(corners[i].ShiftToV3(), Vector3.up * 6, Color.cyan, time);
            }
        }
        else
        {
            for (int i = 0; i < corners.Count; i++)
            {
                Debug.DrawRay(corners[i].ShiftToV3(), Vector3.up * 6, Color.red, time);
            }
        }
    }

    public void SetCorrespondingStreet(Street street)
    {
        if (CorrespondingStreet == null)
        {
            CorrespondingStreet = street;
        }
        else
        {
            DB.Log("This StreetLine already had its CorrespondingStreet made and assigned.");
        }
    }

    /// <summary>
    /// Returns a list of StreetLines that have at least one end on the rim of the city circle, sorted by where that end is on the circle in terms of the angle it forms with the center, clockwise.
    /// <para>Lines that have both ends on the rim (very rare, only possible in low counts of street lines) are duplicated and in 2 different spots in the sort to reflect positions of both of their ends.</para>
    /// </summary>
    public static List<StreetLine> ExtractLinesOnRimSortedByAngleOfEnd(List<StreetLine> lines)
    {
        List<StreetLine> result = new List<StreetLine>();
        List<StreetLine> duplicatesToBeInserted = new List<StreetLine>();
        for (int i = 0; i < lines.Count; i++)
        {
            if (lines[i].AOnRim || lines[i].BOnRim) { result.Add(lines[i]); }
            if (lines[i].AOnRim && lines[i].BOnRim) { duplicatesToBeInserted.Add(lines[i]); }
        }
        result.Sort((x, y) => (x.AngleOfRimEnd() < y.AngleOfRimEnd() ? -1 : 1));
        //this is kinda ineficcient, caching the angles somewhere would be better, but
        //the duplicates are so rare creating a separate class for the sort results is meh
        for (int i = 0; i < duplicatesToBeInserted.Count; i++)
        {
            float angle = Vector2.SignedAngle(-Vector2.up, duplicatesToBeInserted[i].Dir);
            for (int o = 0; o < result.Count; o++)
            {
                float anglePrev = result[GS.TrueMod(o - 1, result.Count)].AngleOfRimEnd();
                float angleCurr = result[o].AngleOfRimEnd();
                if (angle >= anglePrev && angle <= angleCurr)
                {
                    result.Insert(0, duplicatesToBeInserted[i]);
                    break;
                }
            }
        }
        return result;
    }

    public float AngleOfRimEnd()
    {
        if (!AOnRim && !BOnRim) { return 0; }
        return Vector2.SignedAngle(-Vector2.up, (AOnRim) ? -Dir : Dir);
    }
}
