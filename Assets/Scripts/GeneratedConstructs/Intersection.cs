using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Intersection
{
    public float Elevation { get; private set; } = 0;
    public IntersectionPoint Point { get; private set; } = null;
    public Vector3 Position { get; private set; }
    public List<IntersectionCorner> Corners { get; private set; }
    public List<Street> Connected { get; private set; } = new List<Street>();

    private float sidewalkHeight;

    public Intersection(IntersectionPoint interPoint, float elev, float sidewalkHeight)
    {
        Point = interPoint;
        Point.SetCorrespondingIntersection(this);
        Elevation = elev;
        this.sidewalkHeight = sidewalkHeight;
        Corners = new List<IntersectionCorner>();
        Position = new Vector3(Point.Position.x, elev, Point.Position.y);
        for (int i = 0; i < Point.Corners.Count; i++)
        {
            Corners.Add(new IntersectionCorner(new Vector3(Point.Corners[i].x, elev, Point.Corners[i].y)));
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
        for (int i = 0; i < Corners.Count; i++)
        {
            Corners[i].FillConnectionData(Connected);
        }
    }

    public Mesh BuildCarLaneMesh()
    {
        if (Corners.Count != 4) { return null; }
        Mesh m = new Mesh();
        Vector3[] verts = new Vector3[4] { Corners[0].Position, Corners[1].Position, Corners[2].Position, Corners[3].Position };
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

    public List<Mesh> BuildSidewalkMeshes()
    {
        if (Corners.Count != 4) { return null; }
        List<Mesh> meshes = new List<Mesh>();
        for (int i = 0; i < Corners.Count; i++)
        {
            List<Vector3> verts = new List<Vector3>();
            List<int> tris = new List<int>();
            //a corner can only ever be connected to 1 or 2 Streets
            if (Corners[i].ConnectedTo.Count == 1)
            {

            }
            else if (Corners[i].ConnectedTo.Count == 2)
            {
                //Vector3 offset0 = Corners[i].ConnectedTo[0].;
                //Vector3 offset1;
                //verts.Add(Corners[i].Position);
                //verts.Add(Corner Corners[i].ConnectedTo[0])
            }
            else
            {
                DB.Error("Corner of Intersection can't be built because of invalid n of connections");
            }
            meshes.Add(ExtMesh.BuildMesh(verts, tris));
        }
        return meshes;
    }
}
