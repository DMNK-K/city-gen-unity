using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityBlockGen : MonoBehaviour
{
    private CityGen cityGen;
    private List<CityBlock> blocks;

    private void Awake()
    {
        cityGen = GetComponent<CityGen>();
    }

    public IEnumerator ExtractCityBlocks(List<Intersection> inters, List<StreetLine> linesWithEndOnRim)
    {
        DB.Log("EXTRACTING CITY BLOCKS", 1);
        blocks = new List<CityBlock>();
        //extracting city blocks looks like this:
        //From every Intersection the algorithm tries to traverse the Streets connected to it
        //and upon encountering an Intersection on the other end it makes the first clockwise
        //turn available, unless there is no such turn, then it continues in the same direction.
        //When it doesn't encounter an Intersection on the end of a Street, but instead a dead end
        //on the rim of the city circle, it jumps to the next point clockwise along the rim
        //and continues traversal. This way the alg will eventually reach the same Intersection it started
        //from and this way it will save all the Streets encountered as the bounds of the city block.

        //In the case when the traversal gets lost and after a lot of points it still hasn't come back,
        //it is skipped, but I doubt if getting lost is even mathematically possible, it's just an 
        // extra precaution.

        for (int i = 0; i < inters.Count; i++)
        {
            foreach (Street startStreet in inters[i].Connected)
            {
                bool lost = false;
                bool hasRimSkip = false;
                Intersection current = startStreet.GetOtherSide(inters[i]);
                List<StreetTraversal> traversal = new List<StreetTraversal>();
                bool startingTraversedFromAToB = (inters[i] == startStreet.InterA);
                traversal.Add(new StreetTraversal(startStreet, startingTraversedFromAToB));

                while (current != inters[i])
                {
                    if (current == null)
                    {
                        //current Intersection is null meaning we've reached city rim on its end
                        //need to jump along the rim clockwise
                        hasRimSkip = true;
                        Vector2 pointOnRim = (traversal.Last().FromAToB) ? traversal.Last().Street.B : traversal.Last().Street.A;
                        StreetTraversal traversedAfterSkip = GetTraversalFromSkip(pointOnRim, linesWithEndOnRim);
                        traversal.Add(traversedAfterSkip);
                    }
                    else
                    {
                        traversal.Add(PickNextTraversed(current, traversal.Last().Street));
                    }
                    current = traversal.Last().Street.GetOtherSide(current);
                    if (traversal.Count > 7) { lost = true; break; }
                }

                if (!lost)
                {
                    CityBlock block = new CityBlock(traversal, hasRimSkip);
                    if (!BlockIsADuplicate(block))
                    {
                        blocks.Add(block);
                    }
                }
            }
            yield return null;
        }
        DB.Log($"{blocks.Count} CITY BLOCKS EXTRACTED", 1);
    }

    bool BlockIsADuplicate(CityBlock block)
    {
        //detecting duplicates is done by comparing centers of city blocks
        //since no matter the order of streets that comprise bounds of a city block,
        //the center is going to be in the same place every time
        for (int i = 0; i < blocks.Count; i++)
        {
            if (block.Center3D.Similar(blocks[i].Center3D))
            {
                return true;
            }
        }
        return false;
    }

    StreetTraversal PickNextTraversed(Intersection currentIntersection, Street prevTraversed)
    {

    }

    StreetTraversal GetTraversalFromSkip(Vector2 posOnRim, List<StreetLine> linesWithEndOnRim)
    {
        float angle = Vector2.SignedAngle(-Vector2.up, posOnRim);
        for (int i = 0; i < linesWithEndOnRim.Count; i++)
        {
            if ()
        }
    }
}
