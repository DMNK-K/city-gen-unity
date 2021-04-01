using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingPlacerBoxFit : BuildingPlacer
{
    [SerializeField]
    private float squareChance = 20f;
    [SerializeField]
    private float buildingSeparation = 2f;
    private float minAreaFraction = 0.5f;
    private int maxFindPositionFails = 400;
    private int maxResets = 10;

    private List<Quaternion> acceptableRotations = new List<Quaternion>();

    public override IEnumerator PlaceBuildings(CityBlock block, List<BuildingVariant> variants)
    {
        this.block = block;
        this.variants = variants;
        Placed = new List<Building>();
        PickAcceptableRotations();
        FilterVariantsBasedOnAllowedHeight();
        float occupiedArea = 0;
        float occupiedAreaMin = block.Area * minAreaFraction;
        //DB.Log($"block.Area: {block.Area}");
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
            float wallLengthZ = (GS.RChance(squareChance)) ? wallLengthX : pickedVariant.GetRandomWallLength(wallLengthX);
            Vector3 pos;
            bool foundPosition = TryFindSuitablePosition(pickedVariant, rot, wallLengthX, wallLengthZ, out pos);
            if (foundPosition)
            {
                PlaceVariant(pickedVariant, pos, rot, wallLengthX, wallLengthZ);
                occupiedArea += wallLengthX * wallLengthZ;
            }
            else
            {
                //Debug.DrawRay(pos, Vector3.up * 10f, Color.red, 10f);
                findPositionFailCounter++;
                if (findPositionFailCounter >= maxFindPositionFails)
                {
                    //reset
                    resetCounter++;
                    findPositionFailCounter = 0;
                    occupiedArea = 0;
                    DestroyPlaced();
                    if (resetCounter >= maxResets)
                    {
                        resetCounter = 0;
                        occupiedAreaMin = (occupiedAreaMin - 1f) * 0.9f;
                    }
                    yield return null;
                }
            }
            
        }
    }

    private bool TryFindSuitablePosition(BuildingVariant variant, Quaternion rot, float xWall, float zWall, out Vector3 pos)
    {
        pos = block.GetRandomPoint();
        Vector3 halfEx = new Vector3(xWall * 0.5f + buildingSeparation, buildingsGen.MaxHeight, zWall * 0.5f + buildingSeparation);
        //for a position to be suitable, there needs to be no buildings there,
        //it has to not exceed the city block
        //and it has to allow a height that is within the range possible for the building variant
        LayerMask mask = Layers.GetMask(Layer.Building) | Layers.GetMask(Layer.Street);
        bool spaceIsFree = Physics.OverlapBox(pos, halfEx, rot, mask).Length == 0;
        float hAtPosMin = GetHeightMin(pos.UnShiftToV2());
        float hAtPosMax = GetHeightMax(pos.UnShiftToV2());
        bool heightWithinRange = (hAtPosMin < variant.HeightMax) && (hAtPosMax > variant.HeightMin);
        return spaceIsFree && heightWithinRange;
    }  

    private void PickAcceptableRotations()
    {
        acceptableRotations.Clear();
        int numberToPick = Random.Range(1, Mathf.Min(3, block.Bounds.Count));
        //duplicates are allowed
        for (int i = 0; i < numberToPick; i++)
        {
            Vector3 dirToFace = Vector2.Perpendicular(block.Bounds.RandomElement().Line.Dir).ShiftToV3();
            acceptableRotations.Add(Quaternion.LookRotation(dirToFace));
        }
    }

    private void PlaceVariant(BuildingVariant variant, Vector3 pos, Quaternion rot, float xWall, float zWall)
    {
        //to place a building variant first thing that needs to happen is
        //calc of the elevation to which the building foundation will extend
        //which is the highest place within the building area + foundation height
        Vector3 castOrigin = pos.SwapY(buildingsGen.MaxHeight);
        Vector3 halfEx = new Vector3(xWall * 0.5f, 1f, zWall * 0.5f);
        LayerMask mask = Layers.GetMask(Layer.CityBlockTerrain);
        RaycastHit hit;
        float foundationElev = pos.y;
        if (Physics.BoxCast(castOrigin, halfEx, Vector3.down, out hit, rot, buildingsGen.MaxHeight * 2, mask))
        {
            foundationElev = hit.point.y;
        }
        foundationElev += variant.FoundationHeight;
        float foundationBottom = foundationElev - 20;
        float foundationYSpan = foundationElev - foundationBottom;
        Vector3 foundationDimentions = new Vector3(xWall, foundationYSpan, zWall);
        Mesh foundationMesh = MeshGenerator.GenerateBox(foundationDimentions * 0.5f);

        Vector3 foundationPos = pos.SwapY(foundationElev - 0.5f * foundationYSpan);
        MeshFilter instFoundation = Instantiate(variant.PrefabFoundation, foundationPos, rot);
        instFoundation.mesh = foundationMesh;
        BoxCollider col = instFoundation.gameObject.AddComponent<BoxCollider>();
        col.size = foundationDimentions;

        Vector2 pos2D = pos.UnShiftToV2();
        int storiesMin = Mathf.Max(variant.StoriesMin, Mathf.FloorToInt(GetHeightMin(pos2D) / variant.StoryHeight));
        int storiesMax = Mathf.Min(variant.StoriesMax, Mathf.CeilToInt(GetHeightMax(pos2D) / variant.StoryHeight));
        float bodyHeight = Random.Range(storiesMin, storiesMax + 1) * variant.StoryHeight;
        Vector3 bodyDimentions = new Vector3(xWall, bodyHeight, zWall);
        Mesh bodyMesh = MeshGenerator.GenerateBox(bodyDimentions * 0.5f);

        Vector3 bodyPosition = pos.SwapY(foundationElev + bodyHeight * 0.5f);
        MeshFilter instBody = Instantiate(variant.PrefabBody, bodyPosition, rot);
        instBody.mesh = bodyMesh;
        Building newlyPlaced = instBody.gameObject.AddComponent<Building>();
        col = instBody.gameObject.AddComponent<BoxCollider>();
        col.size = bodyDimentions;

        instFoundation.transform.parent = newlyPlaced.transform;
        Placed.Add(newlyPlaced);
    }

    
}
