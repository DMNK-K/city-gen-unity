using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityGen : MonoBehaviour
{
    [SerializeField]
    private float rimRadius = 1000;
    [SerializeField]
    private float laneWidth = 3f;

    public float RimRadius { get { return rimRadius; } }
    public float LaneWidth { get { return laneWidth; } }
    private StreetLineGen lineGen;
    private StreetLineExpander lineExpander;
    private CityGenDiagnostics diagnostics;
    private StreetLineVerticalizer lineVerticalizer;
    private StreetMeshGen streetMeshGen;

    private void Awake()
    {
        lineGen = GetComponent<StreetLineGen>();
        lineExpander = GetComponent<StreetLineExpander>();
        diagnostics = GetComponent<CityGenDiagnostics>();
        lineVerticalizer = GetComponent<StreetLineVerticalizer>();
        streetMeshGen = GetComponent<StreetMeshGen>();
    }

    void Start()
    {
        StartGeneration();
    }

    void SpawnRimSphere()
    {
        GameObject obj = new GameObject("RimSphere");
        obj.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
        obj.layer = LayerMask.NameToLayer("RimSphere");
        SphereCollider col = obj.AddComponent<SphereCollider>();
        col.radius = rimRadius;
        col.isTrigger = true;
    }

    public void StartGeneration()
    {
        DB.Log("STARTING GENERATION", 1);
        SpawnRimSphere();
        StartCoroutine(Generate());
    }

    IEnumerator Generate()
    {
        yield return StartCoroutine(lineGen.GenStreetLines());
        yield return StartCoroutine(lineGen.FillIntersectionPointConnections());
        List<StreetLine> streetLines = lineGen.GetPlaced();
        List<IntersectionPoint> interPoints = lineGen.GetPlacedInterPoints();
        diagnostics.CheckIfIntersectionPointsOverlap(interPoints);
        yield return StartCoroutine(lineExpander.ExpandStreetLines(streetLines, interPoints));
        diagnostics.DebugDrawStreetLineCorners(streetLines, 300f);
        List<Intersection> intersections = lineVerticalizer.ConvertIntersectionPointsToIntersections(interPoints);
        List<Street> streets = lineVerticalizer.ConvertStreetLinesToStreets(streetLines);
        for (int i = 0; i < intersections.Count; i++)
        {
            intersections[i].ExtractConnected();
        }
        yield return StartCoroutine(streetMeshGen.GenerateIntersectionMeshes(intersections));
        yield return StartCoroutine(streetMeshGen.GenerateStreetMeshes(streets));
        FinalizeGeneration();
    }

    void FinalizeGeneration()
    {
        DB.Log("FINALIZING GENERATION", 1);

    }
}
