using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityBlock
{
    public List<Street> Bounds { get; private set; }
    public bool OnRim { get; private set; }
    //private Vector3 rawCenter3D;
    public Vector3 Center3D { get; private set; }
    public Vector2 Center { get { return Center3D.UnShiftToV2(); } }
    private List<Vector2> corners;
    private List<Vector3> corners3D;
    public float Area { get; private set; }
    public float DistFromOrigin { get; private set; }
    public CityBlockPurpose Purpose { get; private set; }
    private List<Triangle2> concentricTris;
    private List<float> concentricTrisAreas; //used as a bias for picking triangles for random points on city block

    public CityBlock(List<StreetTraversal> traversedBounds, bool hasRimSkip)
    {
        Bounds = new List<Street>();
        corners3D = new List<Vector3>();
        corners = new List<Vector2>();
        OnRim = hasRimSkip;
        Vector3 sum = Vector3.zero;
        //since the alg for finding CityBlocks runs clockwise
        //the corners for calculating the center will always be
        //on the right when the street was traversed from A to B,
        //and on the left when traversed from B to A

        //we only add the first corner from any street in order of traversal
        //since they repeat between one street and the next, except for
        //the situation when there is a rim skip
        for (int i = 0; i < traversedBounds.Count; i++)
        {
            Bounds.Add(traversedBounds[i].Street);
            Vector3 corner = (traversedBounds[i].FromAToB) ? traversedBounds[i].Street.CornerAR : traversedBounds[i].Street.CornerBL;
            sum += corner;
            corners3D.Add(corner);
            corners.Add(corner.UnShiftToV2());
            bool rimSkipAhead = (traversedBounds[i].FromAToB) ? traversedBounds[i].Street.Line.BOnRim : traversedBounds[i].Street.Line.AOnRim;
            if (rimSkipAhead)
            {
                corner = (traversedBounds[i].FromAToB) ? traversedBounds[i].Street.CornerBR : traversedBounds[i].Street.CornerAL;
                sum += corner;
                corners3D.Add(corner);
            }
        }
        Center3D = sum / corners3D.Count;
        DistFromOrigin = Vector2.Distance(Vector2.zero, Center);
        //Area = GeoMath.AreaOfPolygon(corners);
        for (int i = 1; i <= corners.Count; i++)
        {
            concentricTris.Add(new Triangle2(corners[i - 1], corners[GS.TrueMod(i, corners3D.Count)], Center));
            concentricTrisAreas.Add(concentricTris.Last().Area);
        }
        Area = concentricTrisAreas.Sum();
    }

    public Vector3 GetRandomPoint()
    {
        Triangle2 tri = concentricTris.RandomElementWithBias(concentricTrisAreas);
        return tri.RandomPointInside().ShiftToV3().SwapY(Center3D.y);
    }

    public Vector2 GetPointClosestToCityCenter()
    {
        return GeoMath.PointInPolygonClosestToPoint(corners, Vector2.zero);
    }

    public Vector2 GetPointFurthestFromCityCenter()
    {
        Vector2 farPoint = Center * 100;
        return GeoMath.PointInPolygonClosestToPoint(corners, farPoint);
    }

    public void SetPurpose(CityBlockPurpose purpose)
    {
        Purpose = purpose;
    }

    public Mesh BuildMesh()
    {
        Mesh m = new Mesh();
        List<Vector3> verts = new List<Vector3>(corners3D.Count + 1);
        for (int i = 0; i < corners3D.Count; i++)
        {
            verts.Add(corners3D[i] - Center3D);
        }
        verts.Add(Vector3.zero);
        List<int> tris = new List<int>();
        for (int i = 0; i < verts.Count - 1; i++)
        {
            tris.Add(i);
            tris.Add(GS.TrueMod(i + 1, verts.Count - 1));
            tris.Add(verts.Count - 1);
        }
        m.vertices = verts.ToArray();
        m.triangles = tris.ToArray();
        m.RecalculateBounds();
        m.RecalculateNormals();
        m.RecalculateTangents();
        return m;
    }

    public override string ToString()
    {
        return $"CityBlock with Center3D at {Center3D} has {Bounds.Count} streets as bounds. OnRim={OnRim}";
    }

    public void DebugDraw(float time)
    {
        for (int i = 0; i < corners3D.Count; i++)
        {
            Debug.DrawLine(Center3D, corners3D[i], Color.green, time);
        }
    }
}

public enum CityBlockPurpose
{
    Buildings,
    Plaza,
    Park,
}

