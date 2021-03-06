using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is the main class responsible for overseeing the generation. It doesn't do anything concrete on it's own.
/// </summary>
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

    //classes responsible for actual generation of concrete things and their manipulation
    private StreetLineGen lineGen;
    private StreetLineExpander lineExpander;
    private CityGenDiagnostics diagnostics;
    private StreetLineVerticalizer lineVerticalizer;
    private StreetMeshGen streetMeshGen;
    private CityBlockGen cityBlockGen;
    private CityBlockPurposeDeterminer cityBlockPurposeDeterminer;
    private Dictionary<CityBlockPurpose, CityBlockContentGen> cityBlockContentGenerators = new Dictionary<CityBlockPurpose, CityBlockContentGen>();

    private void Awake()
    {
        LaneWidth = SidewalkWidth + CarLaneWidth;
        lineGen = GetComponent<StreetLineGen>();
        lineExpander = GetComponent<StreetLineExpander>();
        diagnostics = GetComponent<CityGenDiagnostics>();
        lineVerticalizer = GetComponent<StreetLineVerticalizer>();
        streetMeshGen = GetComponent<StreetMeshGen>();
        cityBlockGen = GetComponent<CityBlockGen>();
        cityBlockPurposeDeterminer = GetComponent<CityBlockPurposeDeterminer>();
        cityBlockContentGenerators.Add(CityBlockPurpose.Park, GetComponentInChildren<ParkGen>());
        cityBlockContentGenerators.Add(CityBlockPurpose.Plaza, GetComponentInChildren<PlazaGen>());
        cityBlockContentGenerators.Add(CityBlockPurpose.Buildings, GetComponentInChildren<BuildingsGen>());
    }

    void Start()
    {
        StartGeneration();
    }

    public void StartGeneration()
    {
        DB.Log("STARTING GENERATION", 1);
        SpawnRimSphere();
        StartCoroutine(Generate());
    }

    void SpawnRimSphere()
    {
        GameObject obj = new GameObject("RimSphere");
        obj.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
        obj.layer = (int)Layer.RimSphere;
        SphereCollider col = obj.AddComponent<SphereCollider>();
        col.radius = rimRadius;
        col.isTrigger = true;
    }

    IEnumerator Generate()
    {
        yield return StartCoroutine(lineGen.GenStreetLines());
        yield return StartCoroutine(lineGen.FillIntersectionPointConnections());
        List<StreetLine> streetLines = lineGen.GetPlaced();
        List<IntersectionPoint> interPoints = lineGen.GetPlacedInterPoints();
        diagnostics.CheckIfIntersectionPointsOverlap(interPoints);
        yield return StartCoroutine(lineExpander.ExpandStreetLines(streetLines, interPoints));
        //diagnostics.DebugDrawStreetLineCorners(streetLines, 300f);
        List<Intersection> intersections = lineVerticalizer.ConvertIntersectionPointsToIntersections(interPoints);
        List<Street> streets = lineVerticalizer.ConvertStreetLinesToStreets(streetLines);
        for (int i = 0; i < intersections.Count; i++)
        {
            intersections[i].ExtractConnected();
        }
        yield return StartCoroutine(streetMeshGen.GenerateIntersectionMeshes(intersections));
        yield return StartCoroutine(streetMeshGen.GenerateStreetMeshes(streets));
        List<RimStreetLine> streetLinesOnRim = StreetLine.ExtractRimStreetLines(streetLines);
        yield return StartCoroutine(cityBlockGen.ExtractCityBlocks(intersections, streetLinesOnRim));
        yield return StartCoroutine(cityBlockGen.GenerateCityBlockTerrainMeshes());
        //List<CityBlock> blocks = cityBlockGen.Blocks;
        //cityBlockPurposeDeterminer.DeterminePurposeOfCityBlocks(blocks);
        //for (int i = 0; i < blocks.Count; i++)
        //{
        //    if (cityBlockContentGenerators.ContainsKey(blocks[i].Purpose))
        //    {
        //        yield return StartCoroutine(cityBlockContentGenerators[blocks[i].Purpose].Generate(blocks[i]));
        //    }
        //}

        FinalizeGeneration();
    }

    void FinalizeGeneration()
    {
        DB.Log("FINALIZING GENERATION", 1);

    }
}
