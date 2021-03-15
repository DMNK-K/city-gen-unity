using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StreetMeshGen : MonoBehaviour
{
    [SerializeField]
    private MeshFilter prefabIntersection;
    [SerializeField]
    private MeshFilter prefabStreet;

    public IEnumerator GenerateIntersectionMeshes(List<Intersection> inters)
    {
        if (prefabIntersection == null) { yield break; }
        for (int i = 0; i < inters.Count; i++)
        {
            Mesh mesh = inters[i].BuildMesh();
            if (mesh == null) { continue; }
            MeshFilter mf = Instantiate(prefabIntersection, inters[i].Position, Quaternion.identity);
            mf.name = "Intersection" + inters[i].Position.x + "_" + inters[i].Position.z;
            mf.mesh = mesh;
            yield return null;
        }
    }

    public IEnumerator GenerateStreetMeshes(List<Street> streets)
    {
        if (prefabStreet == null) { yield break; }
        for (int i = 0; i < streets.Count; i++)
        {
            Mesh mesh = streets[i].BuildMesh();
            if (mesh == null) { continue; }
            MeshFilter mf = Instantiate(prefabStreet, Vector3.zero, Quaternion.identity);
            mf.name = "Street";
            mf.mesh = mesh;
            yield return null;
        }
    }
}
