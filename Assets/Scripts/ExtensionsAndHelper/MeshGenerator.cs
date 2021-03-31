using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshGenerator
{

    public static Mesh GenerateBoxUpwards(Vector3 bottomCenter, Vector3 dimentions)
    {
        //          ----> X+ direction
        //C----D   |
        //|    |   V Z+ direction
        //A----B
        Vector3 botA = new Vector3(bottomCenter.x - dimentions.x * 0.5f, bottomCenter.y, bottomCenter.z + dimentions.z * 0.5f);
        Vector3 botB = new Vector3(bottomCenter.x + dimentions.x * 0.5f, bottomCenter.y, bottomCenter.z + dimentions.z * 0.5f);
        Vector3 botC = new Vector3(bottomCenter.x - dimentions.x * 0.5f, bottomCenter.y, bottomCenter.z - dimentions.z * 0.5f);
        Vector3 botD = new Vector3(bottomCenter.x + dimentions.x * 0.5f, bottomCenter.y, bottomCenter.z - dimentions.z * 0.5f);

        Vector3 topA = botA + Vector3.up * dimentions.y;
        Vector3 topB = botB + Vector3.up * dimentions.y;
        Vector3 topC = botC + Vector3.up * dimentions.y;
        Vector3 topD = botD + Vector3.up * dimentions.y;

        Vector3[] verts = new Vector3[24]
        {
            //bottom face
            botA, botB, botC, botD,
            //front face
            topA, topB, botA, botB,
            //back face
            topD, topC, botD, botC,
            //right face
            topB, topD, botB, botD,
            //left face
            topC, topA, botC, botA,
            //top face
            topA, topB, topC, topD
        };

        int[] tris = new int[36]
        {
            //bottom face
            0, 3, 1,
            0, 2, 3,
            //front face
            6, 5, 4,
            6, 7, 5,
            //back face
            10, 9, 8,
            10, 11, 9,
            //right face
            14, 13, 12,
            14, 15, 13,
            //left face
            18, 17, 16,
            18, 19, 17,
            //top face
            21, 23, 20,
            23, 22, 20
        };

        Mesh m = new Mesh();
        m.vertices = verts;
        m.triangles = tris;
        m.RecalculateNormals();
        m.RecalculateBounds();
        m.RecalculateTangents();
        return m;
    }

    public static Mesh GenerateBoxUpwards(Vector3 a, Vector3 b, Vector3 c, Vector3 d, float height)
    {
        if (a.x == b.x || c.x == d.x || a.z == c.z || b.z == d.z)
        {
            DB.Error("Wrong order of points");
        }
        Vector3 botCenter = (a + b + c + d) / 4;
        Vector3 dimentions = new Vector3(Mathf.Abs(b.x - a.x), height, Mathf.Abs(c.x - a.x));
        return GenerateBoxUpwards(botCenter, dimentions);
    }

    public static Mesh GenerateBox(Vector3 center, Vector3 halfExtents)
    {
        return GenerateBoxUpwards(center - Vector3.up * halfExtents.y, halfExtents * 2);
    }

    public static Mesh GenerateBox(Vector3 halfExtents)
    {
        return GenerateBox(Vector3.zero, halfExtents);
    }
}
