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
        Vector3 botA = new Vector3(bottomCenter.x - dimentions.x * 0.5f, 0, bottomCenter.z + dimentions.z * 0.5f);
        Vector3 botB = new Vector3(bottomCenter.x + dimentions.x * 0.5f, 0, bottomCenter.z + dimentions.z * 0.5f);
        Vector3 botC = new Vector3(bottomCenter.x - dimentions.x * 0.5f, 0, bottomCenter.z - dimentions.z * 0.5f);
        Vector3 botD = new Vector3(bottomCenter.x + dimentions.x * 0.5f, 0, bottomCenter.z - dimentions.z * 0.5f);

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
            4, 5, 6,
            5, 7, 6,
            //back face
            8, 9, 10,
            9, 11, 10,
            //right face
            12, 13, 14,
            13, 15, 14,
            //left face
            16, 17, 18,
            17, 19, 18,
            //top face
            20, 23, 21,
            20, 22, 23
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
}
