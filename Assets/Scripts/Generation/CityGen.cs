using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityGen : MonoBehaviour
{
    [SerializeField]
    private float rimRadius = 1000;
    [SerializeField]
    private float carLaneWidth = 3f;
    [SerializeField]
    private float sidewalkWidth = 1.2f;

    public float RimRadius { get { return rimRadius; } }
    public float CarLaneWidth { get { return carLaneWidth; } }
    public float SidewalkWidth { get { return sidewalkWidth; } }
    public float LaneWidth { get; private set; }

    private StreetLineGen lineGen;
    private StreetLineExpander lineExpander;
    private CityGenDiagnostics diagnostics;
    private StreetLineVerticalizer lineVerticalizer;
    private StreetMeshGen streetMeshGen;
    private CityBlockGen cityBlockGen;

    private void Awake()
    {
        LaneWidth = SidewalkWidth + CarLaneWidth;
        lineGen = GetComponent<StreetLineGen>();
        lineExpander = GetComponent<StreetLineExpander>();
        diagnostics = GetComponent<CityGenDiagnostics>();
        lineVerticalizer = GetComponent<StreetLineVerticalizer>();
        streetMeshGen = GetComponent<StreetMeshGen>();
        cityBlockGen = GetComponent<CityBlockGen>();
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
        float testangle1 = -Vector2.SignedAngle(Vector2.up, new Vector2(1, 1));
        float testangle2 = -Vector2.SignedAngle(Vector2.up, new Vector2(-1, 1));
        DB.Log($"testangle1: {testangle1} | testangle2: {testangle2}");

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
        List<RimStreetLine> streetLinesOnRim = StreetLine.ExtractRimStreetLines(streetLines);
        //for (int i = 0; i < streetLinesOnRim.Count; i++)
        //{
        //    DB.Log("angle: " + streetLinesOnRim[i].AngularPosOnRim);
        //}
        yield return StartCoroutine(cityBlockGen.ExtractCityBlocks(intersections, streetLinesOnRim));
        yield return StartCoroutine(cityBlockGen.GenerateCityBlockTerrainMeshes());
        FinalizeGeneration();
    }

    void FinalizeGeneration()
    {
        DB.Log("FINALIZING GENERATION", 1);

    }
}
