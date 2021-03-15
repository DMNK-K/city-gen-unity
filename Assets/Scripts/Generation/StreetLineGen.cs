using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StreetLineGen : MonoBehaviour
{

    [SerializeField]
    private float centerBias = 0.6f;
    [SerializeField]
    private int thresholdForGeneratingFromExistingLines = 8;
    [SerializeField]
    private float generateFromExistingChance = 66f;

    [Space]
    [SerializeField]
    private float minSeparation = 20f;
    [SerializeField]
    private int minStreets = 15;
    [SerializeField]
    private int maxStreets = 20;
    [SerializeField]
    private float propagateChance = 75f;
    [SerializeField]
    private float diagonalChance = 15f;

    //[Space]
    //[SerializeField]
    //private float minIntersectionSeparation = 20f;

    private float rimRadius;
    private float laneWidth = 3f;
    private float maxRoadWidth;
    private int absoluteMaxStreetCount;
    private int desiredStreetCount;
    private int maxTriesPerLine = 100;
    private CityGen cityGen;
    List<Vector2> diags = new List<Vector2>()
    {
        new Vector2(1, 2), new Vector2(3, 4), new Vector2(4, 3), new Vector2(2, 1),
        new Vector2(1, -2), new Vector2(3, -4), new Vector2(4, -3), new Vector2(2, -1),
        new Vector2(-1, -2), new Vector2(-3, -4), new Vector2(-4, -3), new Vector2(-2, -1),
        new Vector2(-1, 2), new Vector2(-3, 4), new Vector2(-4, 3), new Vector2(-2, 1),
    };

    List<StreetLine> placed = new List<StreetLine>();
    List<IntersectionPoint> placedInterPoints = new List<IntersectionPoint>();

    public class StreetLineIntersectionResult
    {
        public StreetLine line;
        public Vector2 intersection;

        public StreetLineIntersectionResult(StreetLine line, Vector2 intersection)
        {
            this.line = line;
            this.intersection = intersection;
        }
    }


    private void Awake()
    {
        cityGen = GetComponent<CityGen>();
        rimRadius = cityGen.RimRadius;
        laneWidth = cityGen.LaneWidth;
        maxRoadWidth = 4 * laneWidth;
        if (minSeparation < maxRoadWidth * 2) { minSeparation = maxRoadWidth * 2; }
        if (minStreets > maxStreets) { minStreets = maxStreets; }
        desiredStreetCount = Random.Range(minStreets, maxStreets + 1);
        for (int i = 0; i < diags.Count; i++)
        {
            diags[i] = diags[i].normalized;
        }
    }

    public IEnumerator GenStreetLines()
    {
        DB.Log("GENERATING STREET LINES", 1);
        //to keep track of how many streets we create we need a seperate variable,
        //since placed.Count doesn't reflect the same thing - 1 whole street can be
        //comprised of many StreetLine objects one after the other with intersections in between
        //That's because a SreetLine obj represents a street from intersection to intersection,
        //but in terms of count we care more about those long continous streets
        int wholeStreetsCount = 0; 
        int tries = 0;
        float rayRange = rimRadius * 2.1f;
        while (wholeStreetsCount < desiredStreetCount)
        {
            tries += 1;
            bool genFromExisting = (placed.Count >= thresholdForGeneratingFromExistingLines && GS.RChance(generateFromExistingChance));
            Vector2 start, dir;
            StreetLine existingStartLine = null;
            if (genFromExisting)
            {
                existingStartLine = placed.RandomElement();
                start = Vector2.Lerp(existingStartLine.A, existingStartLine.B, Random.Range(0.15f, 0.85f));
                dir = GetDirection(start, existingStartLine);
            }
            else
            {
                start = GS.ROnUnitCircle(rimRadius);
                dir = GetDirection(start);
            }

            RaycastHit hit;
            Ray ray = new Ray(start.ShiftToV3() + dir.ShiftToV3() * rayRange, -dir.ShiftToV3());
            //this raycast has to be reversed because they dont work from within colliders
            if (!Physics.Raycast(ray, out hit, rayRange, GS.MaskRimSphere, QueryTriggerInteraction.Collide))
            {
                continue; //this should never happen, the raycast is set up to always hit
            }
            
            Vector2 end = hit.point.UnShiftToV2(); //end of the whole street
            List<StreetLineIntersectionResult> potentiallyCrossed = GetIntersectWithStreetLineResults(start, end, placed, existingStartLine);

            //propagating the whole street forward
            bool placedAtLeastOne = false;
            for (int i = 0; i <= potentiallyCrossed.Count; i++) //mind the <=
            {
                Vector2 streetLineStart = (i == 0) ? start : potentiallyCrossed[i - 1].intersection;
                Vector2 streetLineEnd = (i == potentiallyCrossed.Count) ? end : potentiallyCrossed[i].intersection;
                //DB.Log($"streetLineStart: {streetLineStart} | streetLineEnd: {streetLineEnd} | dir: {dir} | i: {i}/{potentiallyCrossed.Count}");
                if (Vector2.Distance(streetLineStart, streetLineEnd) < minSeparation)
                {
                    //if the distance is smaller than min separation, it means the potential streetline encounters
                    //another street line and crosses it very near from start point, which would make it difficult later
                    //for placing intersection meshes
                    //we also need a separate check for this, because the regular separation check only takes
                    //into account separation with lines that are nearly paralell
                    DB.Log("Length too small");
                    break;
                }
                if (!PotentialLineHasCorrectSeparation(streetLineStart, streetLineEnd, placed))
                {
                    //we need to check if this potential line isn't to close to other lines that are running nearly paralel to it
                    //to prevent creating narrow city blocks
                    DB.Log("Separation not correct.");
                    break;
                }
                StreetLine lineOnStart = null;
                if (genFromExisting && i == 0)
                {
                    lineOnStart = existingStartLine;
                }
                else if (i > 0)
                {
                    lineOnStart = FindStreetLineWithIntersectionAtPoint(streetLineStart, placed);
                    if (lineOnStart == null) { DB.Error("line on start is null"); }
                }
                StreetLine lineOnEnd = (i == potentiallyCrossed.Count) ? null : potentiallyCrossed[i].line;
                PlaceStreetLine(streetLineStart, streetLineEnd, lineOnStart, lineOnEnd);
                placedAtLeastOne = true;
                placed.Last().DebugDraw(300f);
                //yield return new WaitForSeconds(2.5f);
                if (!GS.RChance(propagateChance)) { break; }
            }
            if (placedAtLeastOne)
            {
                tries = 0;
                wholeStreetsCount += 1;
            }
            yield return null;
            
            DB.Log("");
            if (tries == maxTriesPerLine) //safety feature
            {
                tries = 0;
                desiredStreetCount -= 1;
                DB.Log("Max tries reached.");
            }
        }
    }

    public IEnumerator FillIntersectionPointConnections()
    {
        DB.Log("Filling IntersectionPoint connections.");
        placedInterPoints = new List<IntersectionPoint>(placed.Count / 2); //placed.Count/2 as capacity is almost guaranteed to be filled
        for (int i = 0; i < placed.Count; i++)
        {
            if (placed[i].InterPointA != null)
            {
                placed[i].InterPointA.Connected.AddIfAbsent(placed[i]);
                placedInterPoints.AddIfAbsent(placed[i].InterPointA);
            }
            if (placed[i].InterPointB != null)
            {
                placed[i].InterPointB.Connected.AddIfAbsent(placed[i]);
                placedInterPoints.AddIfAbsent(placed[i].InterPointB);
            }
            yield return null;
        }
    }

    public List<IntersectionPoint> GetPlacedInterPoints()
    {
        return placedInterPoints;
    }

    Vector2 GetDirection(Vector2 start)
    {
        if (GS.RChance(diagonalChance))
        {
            Vector2 result = diags.RandomElement();
            while (Vector2.Angle(result, Vector2.zero - start) > 55)
            {
                result = diags.RandomElement();
            }
            return result;
        }

        List<Vector2> options = new List<Vector2>();
        for (int i = 0; i < GS.CardinalDirs.Count; i++)
        {
            float angleToZero = Vector2.Angle(GS.CardinalDirs[i], Vector2.zero - start);
            if (angleToZero < 50)
            {
                options.Add(GS.CardinalDirs[i]);
            }
        }
        return options.RandomElement();
    }

    Vector2 GetDirection(Vector2 start, StreetLine toAvoid)
    { 
        //this version doesn't check for heading to Vector2.zero,
        //because it has to check for toAvoid
        if (GS.RChance(diagonalChance))
        {
            Vector2 result = diags.RandomElement();
            while (Vector2.Dot(result, (toAvoid.A - toAvoid.B).normalized) > 0.5)
            {
                result = diags.RandomElement();
            }
            return result;
        }

        List<Vector2> options = new List<Vector2>();
        for (int i = 0; i < GS.CardinalDirs.Count; i++)
        {
            float dot = (toAvoid != null) ? Vector2.Dot(GS.CardinalDirs[i], (toAvoid.B - toAvoid.A).normalized) : 0;
            if (Mathf.Abs(dot) < 0.1f)
            {
                options.Add(GS.CardinalDirs[i]);
            }
        }
        return options.RandomElement();
    }

    List<StreetLineIntersectionResult> GetIntersectWithStreetLineResults(Vector2 start, Vector2 end, List<StreetLine> options, StreetLine ignored)
    {
        List<StreetLineIntersectionResult> intersectionResults = new List<StreetLineIntersectionResult>();
        Vector2 intersection;
        for (int i = 0; i < options.Count; i++)
        {
            if (ignored != null && ignored == options[i]) { continue; }
            if (GeoMath.LineSegmentsIntersect(start, end, options[i].A, options[i].B, out intersection))
            {
                intersectionResults.Add(new StreetLineIntersectionResult(options[i], intersection));
            }
        }

        intersectionResults.Sort((a, b) => (Vector2.SqrMagnitude(start - a.intersection) < Vector2.SqrMagnitude(start - b.intersection)) ? -1 : 1);
        return intersectionResults;
    }

    bool PotentialLineHasCorrectSeparation(Vector2 start, Vector2 end, List<StreetLine> comparisonBase)
    {
        for (int i = 0; i < comparisonBase.Count; i++)
        {
            //we only want to check separation with ines to which we are close to paralell or straight up paralell
            float dot = Vector2.Dot((end - start).normalized, (comparisonBase[i].A - comparisonBase[i].B).normalized);
            if (Mathf.Abs(dot) < 0.7f) { continue; }
            //we also don't want to check separation with lines from which we are continuuing from
            if (start == comparisonBase[i].A || end == comparisonBase[i].A || start == comparisonBase[i].B || end == comparisonBase[i].B)
            {
                continue;
            }
            if (GeoMath.ClosestDistBetweenLineSegments(start, end, comparisonBase[i].A, comparisonBase[i].B) < minSeparation)
            {
                return false;
            }
        }
        return true;
    }

    void PlaceStreetLine(Vector2 start, Vector2 end, StreetLine intersectedAtStart, StreetLine intersectedAtEnd)
    {
        IntersectionPoint interStart;
        IntersectionPoint interEnd;
        if (intersectedAtStart != null)
        {
            if (intersectedAtStart.A == start && !intersectedAtStart.AOnRim)
            {
                interStart = intersectedAtStart.InterPointA;
            }
            else if (intersectedAtStart.B == start && !intersectedAtStart.BOnRim)
            {
                interStart = intersectedAtStart.InterPointB;
            }
            else
            {
                //the StreetLine intersected at start of the line-to-be has not been split yet,
                //so we need to split it
                interStart = SplitStreetLine(intersectedAtStart, start);
            }

            if (intersectedAtEnd != null)
            {
                interEnd = SplitStreetLine(intersectedAtEnd, end);
                placed.Add(new StreetLine(interStart, interEnd));
            }
            else
            {
                placed.Add(new StreetLine(interStart, end));
            }
        }
        else if (intersectedAtEnd != null)
        {
            interEnd = SplitStreetLine(intersectedAtEnd, end);
            placed.Add(new StreetLine(start, interEnd));
        }
        else
        {
            placed.Add(new StreetLine(start, end));
        }
    }

    IntersectionPoint SplitStreetLine(StreetLine line, Vector2 point)
    {
        //replaces line with 2 lines with one shared intersection
        placed.Remove(line);
        IntersectionPoint inter = new IntersectionPoint(point);
        placed.Add((line.AOnRim) ? new StreetLine(line.A, inter) : new StreetLine(line.InterPointA, inter));
        placed.Add((line.BOnRim) ? new StreetLine(inter, line.B) : new StreetLine(inter, line.InterPointB));
        return inter;
    }

    public StreetLine FindStreetLineWithIntersectionAtPoint(Vector2 point, List<StreetLine> options)
    {
        for (int i = 0; i < options.Count; i++)
        {
            if (options[i].HasIntersectionAtPoint(point)) { return options[i]; }
        }
        return null;
    }

    public List<StreetLine> GetPlaced()
    {
        return placed;
    }
}
