using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Street
{
    public StreetLine Line { get; private set; }
    public Vector2 A { get { return Line.A; } }
    public Vector2 B { get { return Line.B; } }
    public Intersection InterA { get; private set; } = null;
    public Intersection InterB { get; private set; } = null;

    public float Width { get; private set; }
    public Vector3 CornerAL { get; private set; } = Vector3.positiveInfinity;
    public Vector3 CornerBL { get; private set; } = Vector3.positiveInfinity;
    public Vector3 CornerAR { get; private set; } = Vector3.positiveInfinity;
    public Vector3 CornerBR { get; private set; } = Vector3.positiveInfinity;

    private List<Vector3> edgePointsLeft = new List<Vector3>();
    private List<Vector3> edgePointsRight = new List<Vector3>();

    private float sidewalkHeight;

    public bool AllCornersValid
    {
        get
        { return CornerAL != Vector3.positiveInfinity && CornerBL != Vector3.positiveInfinity && CornerAR != Vector3.positiveInfinity && CornerBR != Vector3.positiveInfinity; }
    }

    public Street(StreetLine line, ElevationGen elevGen, float sidewalkHeight)
    {
        if (line == null) { DB.Error("Trying to create Street from null StreetLine."); }
        Line = line;
        this.sidewalkHeight = sidewalkHeight;
        Line.SetCorrespondingStreet(this);
        Width = line.VirtualWidth;
        Vector2 offset = Vector2.Perpendicular(Line.Dir) * Width * 0.5f;
        float elev;
        if (Line.AOnRim)
        {
            elev = elevGen.GetFullElevation(Line.A);
            CornerAL = (Line.A + offset).ShiftToV3().SwapY(elev);
            CornerAR = (Line.A - offset).ShiftToV3().SwapY(elev);
        }
        else
        {
            InterA = Line.InterPointA.CorrespondingIntersection;
            foreach (IntersectionCorner corner3D in InterA.Corners)
            {
                foreach (Vector2 corner in Line.corners)
                {
                    if (corner.Similar(corner3D.Position.UnShiftToV2()))
                    {
                        if (GeoMath.PointIsOnLeftOfLine(corner, Line.A, Line.B))
                        {
                            CornerAL = corner3D.Position;
                        }
                        else
                        {
                            CornerAR = corner3D.Position;
                        }
                    }
                }
            }
        }


        if (Line.BOnRim)
        {
            elev = elevGen.GetFullElevation(Line.B);
            CornerBL = (Line.B + offset).ShiftToV3().SwapY(elev);
            CornerBR = (Line.B - offset).ShiftToV3().SwapY(elev);
        }
        else
        {
            InterB = Line.InterPointB.CorrespondingIntersection;
            foreach (IntersectionCorner corner3D in InterB.Corners)
            {
                foreach (Vector2 corner in Line.corners)
                {
                    if (corner.Similar(corner3D.Position.UnShiftToV2()))
                    {
                        if (GeoMath.PointIsOnLeftOfLine(corner, Line.A, Line.B))
                        {
                            CornerBL = corner3D.Position;
                        }
                        else
                        {
                            CornerBR = corner3D.Position;
                        }
                    }
                }
            }
        }

        if (CornerAL == Vector3.positiveInfinity)
        {
            DB.Error("CornerAL is invalid");
        }
        if (CornerAR == Vector3.positiveInfinity)
        {
            DB.Error("CornerAR is invalid");
        }
        if (CornerBL == Vector3.positiveInfinity)
        {
            DB.Error("CornerBL is invalid");
        }
        if (CornerBR == Vector3.positiveInfinity)
        {
            DB.Error("CornerBR is invalid");
        }
        CreateElevationOnEdges(elevGen);
        DebugDraw(300f);
    }

    void CreateElevationOnEdges(ElevationGen elevGen)
    {
        edgePointsLeft.Clear();
        edgePointsRight.Clear();
        edgePointsLeft.Add(CornerAL);
        edgePointsRight.Add(CornerAR);
        float lengthLeft = Vector3.Distance(CornerAL.StripY(), CornerBL.StripY());
        float lengthRight = Vector3.Distance(CornerAR.StripY(), CornerBR.StripY());
        float diff = Mathf.Abs(lengthLeft - lengthRight);
        float bonusToLeft = 0;
        float bonusToRight = 0;
        if (diff > 0.01f)
        {
            //lenghts are different, so we need to add 1 correcting point to the longer edge
            //so the rest of the edge has points spaced together on both sides with the same height
            Vector3 point;
            if (lengthLeft > lengthRight)
            {
                bonusToLeft = diff;
                point = CornerAL + Line.Dir.ShiftToV3() * diff;
                point.y = CornerAR.y;
                edgePointsLeft.Add(point);
            }
            else
            {
                bonusToRight = diff;
                point = CornerAR + Line.Dir.ShiftToV3() * diff;
                point.y = CornerAL.y;
                edgePointsRight.Add(point);
            }
        }
        //sampling elevation from the middle of the line in points separated by samplingDist
        //and giving it to both edges
        float samplingDist = 10f;
        int innerEdgePoints = Mathf.FloorToInt(Mathf.Min(lengthLeft, lengthRight) / samplingDist);
        for (int i = 1; i < innerEdgePoints + 1; i++)
        {
            Vector2 elevCoord = Vector2.Lerp(Line.A, Line.B, i / (innerEdgePoints + 1f));
            float elev = elevGen.GetFullElevation(elevCoord);
            Vector3 pointLeft = CornerAL + Line.Dir.ShiftToV3() * (samplingDist * i + bonusToLeft);
            Vector3 pointRight = CornerAR + Line.Dir.ShiftToV3() * (samplingDist * i + bonusToRight);
            pointLeft.y = elev;
            pointRight.y = elev;
            edgePointsLeft.Add(pointLeft);
            edgePointsRight.Add(pointRight);
        }
        edgePointsLeft.Add(CornerBL);
        edgePointsRight.Add(CornerBR);
    }

    /// <summary>
    /// Gets the Intersection on the other end of this street from currentSide. Null when the other end is on a rim or when currenSide is invalid.
    /// </summary>
    public Intersection GetOtherSide(Intersection currentSide)
    {
        if (currentSide == null) { return null; }
        if (currentSide == InterA)
        {
            return InterB;
        }
        if (currentSide == InterB)
        {
            return InterA;
        }
        DB.Log("currentSide is not a correct side of this street");
        return null;
    }

    public Mesh BuildCarLanesMesh()
    {
        if (!AllCornersValid) { return null; }
        List<Vector3> verts = new List<Vector3>();
        List<int> tris = new List<int>();
        verts.Add(edgePointsLeft[0]);
        verts.Add(edgePointsRight[0]);
        int startL = 1, startR = 1;
        if (edgePointsLeft.Count != edgePointsRight.Count)
        {
            if (edgePointsLeft.Count > edgePointsRight.Count)
            {
                verts.Add(edgePointsLeft[1]);
                startL = 2;
            }
            else if (edgePointsLeft.Count < edgePointsRight.Count)
            {
                verts.Add(edgePointsRight[1]);
                startR = 2;
            }
            tris.Add(0);
            tris.Add(1);
            tris.Add(2);
        }
        for (int r = startR, l = startL;  r < edgePointsRight.Count && l < edgePointsLeft.Count; l++, r++)
        {
            verts.Add(edgePointsLeft[l]);
            verts.Add(edgePointsRight[r]);
            tris.Add(verts.Count - 4);
            tris.Add(verts.Count - 2);
            tris.Add(verts.Count - 3);

            tris.Add(verts.Count - 3);
            tris.Add(verts.Count - 2);
            tris.Add(verts.Count - 1);
        }
        return ExtMesh.BuildMesh(verts, tris);
    }

    public Mesh BuildSidewalkMeshLeft() => BuildSidewalkMesh(true);
    public Mesh BuildSidewalkMeshRight() => BuildSidewalkMesh(false);

    private Mesh BuildSidewalkMesh(bool left)
    {
        List<Vector3> edgePoints = (left) ? edgePointsLeft : edgePointsRight;
        Vector3 dirToInner = ((left) ? -Vector2.Perpendicular(Line.Dir) : Vector2.Perpendicular(Line.Dir)).ShiftToV3();
        dirToInner = dirToInner * Line.VirtualSidewalkWidth;
        Vector3 yOffset = Vector3.up * sidewalkHeight;
        List<Vector3> verts = new List<Vector3>(edgePoints.Count * 6);
        List<int> tris = new List<int>();
        for (int i = 0; i < edgePoints.Count; i++)
        {
            //polar and conditionals for these is important for winding order of tris later, so they stay clockwise
            verts.Add(edgePoints[i] - yOffset * GS.GetPolar(left));
            verts.Add(edgePoints[i] + yOffset * GS.GetPolar(left));

            //also the top side of the sidewalk has identical, but separate verts to the sides, so the duplication is intentional
            verts.Add(edgePoints[i] + yOffset + ((left) ? dirToInner : Vector3.zero));
            verts.Add(edgePoints[i] + yOffset + ((left) ? Vector3.zero : dirToInner));

            verts.Add(edgePoints[i] + yOffset * GS.GetPolar(left) + dirToInner);
            verts.Add(edgePoints[i] - yOffset * GS.GetPolar(left) + dirToInner);

            if (i > 0)
            {
                int n = 6 * (i - 1);
                tris.Add(n);
                tris.Add(n + 6);
                tris.Add(n + 1);

                tris.Add(n + 1);
                tris.Add(n + 6);
                tris.Add(n + 7);

                tris.Add(n + 2);
                tris.Add(n + 3);
                tris.Add(n + 8);

                tris.Add(n + 3);
                tris.Add(n + 9);
                tris.Add(n + 8);

                tris.Add(n + 5);
                tris.Add(n + 10);
                tris.Add(n + 11);

                tris.Add(n + 5);
                tris.Add(n + 4);
                tris.Add(n + 10);
            }
        }
        return ExtMesh.BuildMesh(verts, tris);
    }

    public void DebugDraw(float time)
    {
        if (AllCornersValid)
        {
            Debug.DrawLine(CornerAL, CornerBL, Color.white, time);
            Debug.DrawLine(CornerAR, CornerBR, Color.white, time);
        }
    }
}
