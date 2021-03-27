using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StreetLineExpander : MonoBehaviour
{
    [SerializeField]
    private float doubleLaneGuaranteedDistFraction = 0.33f;
    [SerializeField]
    private float lengthRequiredForDoubleLane = 35f;
    [SerializeField]
    private float doubleLaneChance = 15;

    private CityGen cityGen;

    private void Awake()
    {
        cityGen = GetComponent<CityGen>();
    }

    public IEnumerator ExpandStreetLines(List<StreetLine> placedLines, List<IntersectionPoint> interPoints)
    {
        DB.Log("EXPANDING STREET LINES", 1);
        //first give virtual thickness to all of placed street lines
        float guaranteedDoubleLaneDistSqr = Mathf.Pow(doubleLaneGuaranteedDistFraction * cityGen.RimRadius, 2);
        DB.Log("Setting virtual width");
        for (int i = 0; i < placedLines.Count; i++)
        {
            Vector2 midPoint = Vector2.Lerp(placedLines[i].A, placedLines[i].B, 0.5f);
            bool doubleLanes = (placedLines[i].Length >= lengthRequiredForDoubleLane && (Vector2.SqrMagnitude(midPoint) < guaranteedDoubleLaneDistSqr || GS.RChance(doubleLaneChance)));
            placedLines[i].SetVirtualWidth(cityGen.CarLaneWidth, cityGen.SidewalkWidth, (doubleLanes) ? 4 : 2);
        }
        yield return null;
        for (int i = 0; i < interPoints.Count; i++)
        {
            interPoints[i].CreateCornersForConnected();
            yield return null;
        }
        for (int i = 0; i < placedLines.Count; i++)
        {
            placedLines[i].CleanUpAndSortCorners();
            yield return null;
        }
    }
}
