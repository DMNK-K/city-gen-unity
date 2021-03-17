using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Intersection
{
    public float Elevation { get; private set; } = 0;
    public IntersectionPoint Point { get; private set; } = null;
    public Vector3 Position { get; private set; }
    public List<Vector3> Corners { get; private set; }
    public List<Street> Connected { get; private set; } = new List<Street>();

    public Intersection(IntersectionPoint interPoint, float elev)
    {
        Point = interPoint;
        Point.SetCorrespondingIntersection(this);
        Elevation = elev;
        Corners = new List<Vector3>();
        Position = new Vector3(Point.Position.x, elev, Point.Position.y);
        for (int i = 0; i < Point.Corners.Count; i++)
        {
            Corners.Add(new Vector3(Point.Corners[i].x, elev, Point.Corners[i].y));
        }
        if (Corners.Count != 4)
        {
            DB.Error("Invalid number of Corners in Intersection.");
        }
    }

    public void ExtractConnected()
    {
        for (int i = 0; i < Point.Connected.Count; i++)
        {
            if (Point.Connected[i].CorrespondingStreet != null)
            {
                Connected.AddIfAbsent(Point.Connected[i].CorrespondingStreet);
            }
        }
    }

    public Mesh BuildMesh()
    {
        if (Corners.Count != 4) { return null; }
        Mesh m = new Mesh();
        Vector3[] verts = Corners.ToArray();
        for (int i = 0; i < verts.Length; i++)
        {
            verts[i] -= Position;
        }
        m.vertices = verts;
        int[] tris = new int[6] { 0, 2, 1, 2, 3, 1 }; 
        m.triangles = tris;
        //m.Subdivide4();
        m.RecalculateNormals();
        m.RecalculateBounds();
        m.RecalculateTangents();
        return m;
    }
}
