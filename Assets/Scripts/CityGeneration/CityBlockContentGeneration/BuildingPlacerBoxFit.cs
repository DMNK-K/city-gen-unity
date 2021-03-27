using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingPlacerBoxFit : BuildingPlacer
{
    private float minAreaFraction = 0.5f;
    private float squareChance = 20f;
    private int maxFindPositionFails = 100;
    private int maxResets = 10;

    public override IEnumerator PlaceBuildings(CityBlock block, List<BuildingVariant> variants)
    {
        List<Quaternion> acceptableRotations = new List<Quaternion>();
        //find rotations here
        List<GameObject> placed = new List<GameObject>();
        variants = FilterVariantsBasedOnAllowedHeight(block, variants);
        float occupiedArea = 0;
        float occupiedAreaMin = block.Area * minAreaFraction;
        int findPositionFailCounter = 0;
        int resetCounter = 0;

        //The algorithm is going to fill the area with buildings so that at least occupiedAreaMin is filled.
        //However, it might run into situations where it's unlikely that a combination of building size, rotation will be
        //able to be acommodated, and the alg will fail to find a suitable position many times. If that ever happens, the buildings
        //that were already placed are erased and the alg resets. If it resets too many times, the occupiedAreaMin will
        //get smaller to prevent getting stuck. This is only a precaution and isn't likely to happen, unless settings
        //within building variants are so strict and prohibitive (eg minimal sizes are so large they can't be fit) that
        //the alg can't find any place to put them. This could be additionaly prevented with some additional checks and balances
        //in the future.
        while (occupiedArea < occupiedAreaMin)
        {
            Quaternion rot = acceptableRotations.RandomElement();
            BuildingVariant pickedVariant = variants.RandomElement();
            float wallLengthX = pickedVariant.GetRandomWallLength();
            float wallLengthZ = (GS.RChance(squareChance)) ? wallLengthX : pickedVariant.GetRandomWallLength();
            Vector3 pos = Vector3.zero;
            bool foundPosition = TryFindSuitablePosition(pickedVariant, rot, wallLengthX, wallLengthZ, out pos);
            if (!foundPosition)
            {
                findPositionFailCounter++;
                if (findPositionFailCounter >= maxFindPositionFails)
                {
                    //reset
                    resetCounter++;
                    findPositionFailCounter = 0;
                    occupiedArea = 0;
                    ResetPlaced(placed);
                    if (resetCounter >= maxResets)
                    {
                        resetCounter = 0;
                        occupiedAreaMin *= 0.9f;
                    }
                }
                continue;
            }


            yield return null;
        }
    }

    private bool TryFindSuitablePosition(BuildingVariant variant, Quaternion rot, float xWall, float zWall, out Vector3 pos)
    {
        pos = Vector3.zero;
        return false;
    }  

    private void ResetPlaced(List<GameObject> placed)
    {
        for (int i = 0; i < placed.Count; i++)
        {
            Destroy(placed[i]);
        }
    }
}
