using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityBlockGen : MonoBehaviour
{
    private CityGen cityGen;
    public List<CityBlock> Blocks { get; private set; }

    [SerializeField]
    private MeshFilter prefabCityBlock;

    private void Awake()
    {
        cityGen = GetComponent<CityGen>();
    }

    public IEnumerator ExtractCityBlocks(List<Intersection> inters, List<RimStreetLine> linesWithEndOnRim)
    {
        DB.Log("EXTRACTING CITY BLOCKS", 1);
        Blocks = new List<CityBlock>();
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
        //extra precaution.

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
                        current = traversedAfterSkip.FromAToB ? traversedAfterSkip.Street.InterB : traversedAfterSkip.Street.InterA;
                    }
                    else
                    {
                        traversal.Add(PickNextTraversed(current, traversal.Last()));
                        current = traversal.Last().Street.GetOtherSide(current);
                    }
                    if (traversal.Count > 50) { lost = true; DB.Log("traversal got lost."); break; }
                }

                if (!lost)
                {
                    CityBlock block = new CityBlock(traversal, hasRimSkip);
                    if (!BlockIsADuplicate(block))
                    {
                        Blocks.Add(block);
                        block.DebugDraw(300f);
                        //DB.Log(block.ToString());
                        //yield return new WaitForSeconds(1f);
                    }
                }
            }
            yield return null;
        }
        DB.Log($"{Blocks.Count} CITY BLOCKS EXTRACTED", 1);
    }

    bool BlockIsADuplicate(CityBlock block)
    {
        //detecting duplicates is done by comparing centers of city blocks
        //since no matter the order of streets that comprise bounds of a city block,
        //the center is going to be in the same place every time
        for (int i = 0; i < Blocks.Count; i++)
        {
            if (block.Center3D.Similar(Blocks[i].Center3D))
            {
                return true;
            }
        }
        return false;
    }

    StreetTraversal PickNextTraversed(Intersection currentIntersection, StreetTraversal prevTraversed)
    {
        //the float as the key doesnt pose issues because the only way we access anyway is by MaxKey
        Dictionary<float, StreetTraversal> streetsByAngle = new Dictionary<float, StreetTraversal>();
        //picking connected Streets that go counterclockwise, or straight, compared to prevTraversed
        foreach (Street connected in currentIntersection.Connected)
        {
            if (connected == prevTraversed.Street) { continue; } //we dont want to backtrack, so skipping the one from which we came from
            Vector2 prevDir = GS.GetPolar(prevTraversed.FromAToB) * prevTraversed.Street.Line.Dir;
            bool connectedFromAToB = (currentIntersection == connected.InterA);
            Vector2 connectedDir = GS.GetPolar(connectedFromAToB) * connected.Line.Dir;
            float angle = -Vector2.SignedAngle(prevDir, connectedDir);
            if (angle >= -0.01f)
            {
                streetsByAngle.Add(angle, new StreetTraversal(connected, connectedFromAToB));
            }
        }
        if (streetsByAngle.Count > 0)
        {
            return streetsByAngle[streetsByAngle.MaxKey()];
        }
        return null;
    }

    StreetTraversal GetTraversalFromSkip(Vector2 posOnRim, List<RimStreetLine> linesWithEndOnRim)
    {
        if (linesWithEndOnRim == null || linesWithEndOnRim.Count < 1)
        {
            DB.Error("linesWithEndOnRim is invalid, this should never happen, unless generation of StreetLines failed");
            return null;
        }
        float angle = -Vector2.SignedAngle(-Vector2.up, posOnRim);
        //Since angle computations have 1 place where the angle switches abruptly
        //(in the case of SignedAngle it's a switch from almost 180 to almost -180)
        //there is a possibility that the current posOnrim has an angle of 179-ish and 
        //the next one in the circle clockwise is past that plus-minus sign switch and is -179-ish,
        //in that case a simple comparison of what is larger will not suffice to detect the next
        //line starting on rim, without some special case scenario - that is why we return the first
        //line in list when the loop is over - we found no line for which the angle was bigger, which means the only
        //possibility is the next line is actually past that abrupt sign switch and is therefore smaller.
        for (int i = 0; i < linesWithEndOnRim.Count; i++)
        {
            if (linesWithEndOnRim[i].AngularPosOnRim > angle)
            {
                return new StreetTraversal(linesWithEndOnRim[i].Line.CorrespondingStreet, linesWithEndOnRim[i].AngularPosCalculatedFromA);
            }
        }
        return new StreetTraversal(linesWithEndOnRim[0].Line.CorrespondingStreet, linesWithEndOnRim[0].AngularPosCalculatedFromA);
    }

    public IEnumerator GenerateCityBlockTerrainMeshes()
    {
        if (prefabCityBlock == null) { yield break; }
        DB.Log("GENERATING TERRAIN MESHES FOR CITY BLOCKS", 1);
        for (int i = 0; i < Blocks.Count; i++)
        {
            Mesh mesh = Blocks[i].BuildMesh();
            MeshFilter inst = Instantiate(prefabCityBlock, Blocks[i].Center3D, Quaternion.identity);
            inst.mesh = mesh;
            inst.GetComponent<MeshCollider>().sharedMesh = mesh;
            yield return null;
        }
    }
}
