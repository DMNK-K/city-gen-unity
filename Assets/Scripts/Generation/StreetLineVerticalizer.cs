using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StreetLineVerticalizer : MonoBehaviour
{
    private ElevationGen elevationGen;

    private void Awake()
    {
        elevationGen = GetComponent<ElevationGen>();
    }

    public List<Intersection> ConvertIntersectionPointsToIntersections(List<IntersectionPoint> points)
    {
        DB.Log("CONVERTING INTERSECTION POINTS TO INTERSECTIONS", 1);
        //intersection points are what stipulates elevation of
        //street lines themselves later, but it starts with intersections
        //we make an intersection obj, which is a 3d version of an IntersectionPoint
        List<Intersection> inters = new List<Intersection>(points.Count);
        for (int i = 0; i < points.Count; i++)
        {
            inters.Add(new Intersection(points[i], elevationGen.GetFullElevation(points[i].Position)));
        }
        return inters;
    }

    public List<Street> ConvertStreetLinesToStreets(List<StreetLine> lines)
    {
        DB.Log("CONVERTING STREETLINES TO STREETS", 1);

        //Streets are 3d representations of StreetLines, they basically are StreetLines with elevation
        //Streets get their elevation from Intersections on their ends,
        //or if their StreetLines don't end with an IntersectionPoint they get it from the middle
        //elevation on that end (this is done so the future Street isn't slanting)

        //The elevation within the Street is created from a bunch of sample points spaced out along it
        //and then everything in between those sample points can be interpolated.
        //All of this is done so that the future vertices of the street don't flow in
        //a too steep or chaotic way, and the changes in elevation within the street are more smooth,
        //something that wouldn't happen if every part of a street got its elevation straight
        //from the ElevationGen.
        List<Street> streets = new List<Street>(lines.Count);
        for (int i = 0; i < lines.Count; i++)
        {
            streets.Add(new Street(lines[i], elevationGen));
        }
        return streets;
    }
}
