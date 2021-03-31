using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StreetMeshGen : MonoBehaviour
{
    [SerializeField]
    private MeshFilter prefabIntersection;
    [SerializeField]
    private MeshFilter prefabStreet;
    [SerializeField]
    private MeshFilter prefabSidewalk;

    public IEnumerator GenerateIntersectionMeshes(List<Intersection> inters)
    {
        if (prefabIntersection == null) { yield break; }
        for (int i = 0; i < inters.Count; i++)
        {
            Mesh mesh = inters[i].BuildCarLaneMesh();
            string name = "Intersection" + inters[i].Position.x + "_" + inters[i].Position.z;
            InstantiateMesh(mesh, prefabIntersection, inters[i].Position, Quaternion.identity, name);

            List<Mesh> sidewalkMeshes = inters[i].BuildSidewalkMeshes();
            for (int o = 0; o < sidewalkMeshes.Count; o++)
            {
                name = $"Corner{o}";
                InstantiateMesh(sidewalkMeshes[o], prefabSidewalk, inters[i].Position, Quaternion.identity, name);
            }
            yield return null;
        }
    }

    public IEnumerator GenerateStreetMeshes(List<Street> streets)
    {
        if (prefabStreet == null || prefabSidewalk == null) { yield break; }
        for (int i = 0; i < streets.Count; i++)
        {
            Mesh mesh = streets[i].BuildCarLanesMesh();
            InstantiateMesh(mesh, prefabStreet, Vector3.zero, Quaternion.identity, "Street");

            mesh = streets[i].BuildSidewalkMeshLeft();
            InstantiateMesh(mesh, prefabSidewalk, Vector3.zero, Quaternion.identity, "SidewalkL");

            mesh = streets[i].BuildSidewalkMeshRight();
            InstantiateMesh(mesh, prefabSidewalk, Vector3.zero, Quaternion.identity, "SidewalkR");
            yield return null;
        }
    }

    private MeshFilter InstantiateMesh(Mesh mesh, MeshFilter prefab, Vector3 pos, Quaternion rot, string name)
    {
        MeshFilter mf = Instantiate(prefab, pos, rot);
        mf.gameObject.name = name;
        mf.mesh = mesh;
        MeshCollider col = mf.GetComponent<MeshCollider>();
        if (col != null) { col.sharedMesh = mf.mesh; }
        return mf;
    }
}
