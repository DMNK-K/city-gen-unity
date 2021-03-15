using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntersectionPoint
{
    public Vector2 Position { get; private set; }
    public List<StreetLine> Connected { get; private set; } = new List<StreetLine>();
    public List<Vector2> Corners { get; private set; } = new List<Vector2>();
    public Intersection CorrespondingIntersection { get; private set; } = null;

    public IntersectionPoint(Vector2 pos)
    {
        Position = pos;
    }

    public void AddCornerIfAbsent(Vector2 corner)
    {
        //due to possible floating point undeterminism same corner might be
        //calculated 2 times differently, it's safest to check similarity rather than simply
        //call Contains() or something
        for (int i = 0; i < Corners.Count; i++)
        {
            if (Corners[i].Similar(corner)) { return; }
        }
        Corners.Add(corner);
    }

    public void DebugDraw(float time)
    {
        if (Connected.Count == 1)
        {
            Debug.DrawRay(Position.ShiftToV3(), Vector3.up * 12, Color.red, time);
        }
        else if (Connected.Count == 2)
        {
            Debug.DrawRay(Position.ShiftToV3(), Vector3.up * 9, new Color(0.4f, 0, 0), time);
        }
        else
        {
            Debug.DrawRay(Position.ShiftToV3(), Vector3.up * 5, new Color(0, 0.5f, 0.1f), time);
        }
    }

    public void CreateCornersForConnected()
    {
        //DB.Log($"Creating corners for Connected (ConnectingStreetLines.Count: {Connected.Count}).");
        if (Connected.Count < 3 || Connected.Count > 4) { return; }

        for (int i = 0; i < Connected.Count; i++)
        {
            for (int o = 0; o < Connected.Count; o++)
            {
                if (o == i) { continue; }
                //we do this to ensure that those dirs are dirs pointing away from this intersection point and not directly towards it
                Vector2 iDir = (this == Connected[i].InterPointA) ? Connected[i].B - Connected[i].A : Connected[i].A - Connected[i].B;
                Vector2 oDir = (this == Connected[o].InterPointA) ? Connected[o].B - Connected[o].A : Connected[o].A - Connected[o].B;
                iDir = iDir.normalized;
                oDir = oDir.normalized;
                if (iDir.SameOrOppositeDir(oDir, 0.01f) && Connected.Count == 3)
                {
                    //connected are basically continuations of each other, so the corners are in a direction
                    //that is exactly half way between each of them and a line perpendicular to them,
                    //       x\ | /x
                    //  [i]____\|/____[o]
                    //
                    //but there are of course two perpendicular lines we could pick from, so we check which one doesn't interfere with other
                    //Connected StreetLines, we will always find one that doesn't, because this scenario assumes Connected.Count == 3
                    Vector2 perp1 = Vector2.Perpendicular(iDir);
                    Vector2 perp2 = -Vector2.Perpendicular(iDir);
                    StreetLine onlyOtherConnected = null;
                    for (int p = 0; p < Connected.Count; p++)
                    {
                        if (p != o && p != i) { onlyOtherConnected = Connected[p]; break; }
                    }
                    //we do this to ensure this dir is the one that points away from this intersection point, not straight towards it
                    Vector2 otherDir = (this == onlyOtherConnected.InterPointA) ? onlyOtherConnected.B - onlyOtherConnected.A : onlyOtherConnected.A - onlyOtherConnected.B;
                    otherDir = otherDir.normalized;
                    float angle1 = Vector2.Angle(perp1, otherDir);
                    float angle2 = Vector2.Angle(perp2, otherDir);
                    Vector2 pickedPerp = (angle1 > angle2) ? perp1 : perp2;
                    Vector2 oCorner = Position + (pickedPerp + oDir) * Connected[o].VirtualWidth * 0.5f;
                    Vector2 iCorner = Position + (pickedPerp + iDir) * Connected[i].VirtualWidth * 0.5f;
                    AddCornerIfAbsent(oCorner);
                    AddCornerIfAbsent(iCorner);
                    Connected[i].AddCornerIfAbsent(iCorner);
                    Connected[o].AddCornerIfAbsent(oCorner);
                }
                else if (!iDir.SameOrOppositeDir(oDir, 0.01f))
                {
                    Vector2 corner = Position + (iDir * Connected[o].VirtualWidth + oDir * Connected[i].VirtualWidth) * 0.5f;
                    AddCornerIfAbsent(corner);
                    Connected[i].AddCornerIfAbsent(corner);
                    Connected[o].AddCornerIfAbsent(corner);
                }
            }
        }

        if (Corners.Count != 4)
        {
            DB.Error("Invalid number of Corners in IntersectionPoint.");
            Debug.DrawRay(Position.ShiftToV3(), Vector3.up * 30f, Color.magenta, 300f);
        }
    }

    public void SetCorrespondingIntersection(Intersection inter)
    {
        if (CorrespondingIntersection == null)
        {
            CorrespondingIntersection = inter;
        }
        else
        {
            DB.Log("This IntersectionPoint already had its corresponding Intersection made and assigned.");
        }
    }
}
