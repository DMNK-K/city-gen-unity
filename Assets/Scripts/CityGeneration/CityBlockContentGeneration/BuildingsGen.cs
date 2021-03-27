using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class that is responsible for generating buildings in a CityBlock, by picking and using one of specific classes that each represent different methods of building placing.
/// </summary>
public class BuildingsGen : CityBlockContentGen
{
    [SerializeField]
    private float maxHeight = 200f;
    [SerializeField]
    private List<BuildingPlacementMethod> allowedPlacementMethods;
    [SerializeField]
    private float boxFitBias = 1;
    [SerializeField]
    private float shapeFitBias = 1;
    [SerializeField]
    private float ringFillBias = 1;
    [SerializeField]
    private float singleBuildingBias = 0.5f;
    [SerializeField]
    private List<BuildingVariant> buildingVariants;

    private List<float> biases = new List<float>();
    private Dictionary<BuildingPlacementMethod, BuildingPlacer> placers;

    public float MaxHeight { get { return maxHeight; } }

    private void Awake()
    {
        for (int i = 0; i < allowedPlacementMethods.Count; i++)
        {
            //float biasToAdd = allowedPlacementMethods[i] switch
            //{
            //    BuildingPlacementMethod.BoxFit => boxFitBias,
            //    BuildingPlacementMethod.ShapeFit => shapeFitBias,
            //};

            //all of this could be simplified once Unity makes Dictionaries<> editable in inspector
            switch (allowedPlacementMethods[i])
            {
                case BuildingPlacementMethod.BoxFit:
                    biases.Add(boxFitBias);
                    break;
                case BuildingPlacementMethod.ShapeFit:
                    biases.Add(shapeFitBias);
                    break;
                case BuildingPlacementMethod.RingFill:
                    biases.Add(ringFillBias);
                    break;
                case BuildingPlacementMethod.SingleBuilding:
                    biases.Add(singleBuildingBias);
                    break;
                default:
                    break;
            }
        }

        placers.Add(BuildingPlacementMethod.BoxFit, GetComponentInChildren<BuildingPlacerBoxFit>());
    }

    public override IEnumerator Generate(CityBlock block)
    {
        if (allowedPlacementMethods.Count == 0)
        {
            allowedPlacementMethods.Add(BuildingPlacementMethod.BoxFit);
            DB.Log("None allowed placement methods, reverting to BoxFit as default.");
        }
        BuildingPlacementMethod method = allowedPlacementMethods.RandomElementWithBias(biases);
        yield return StartCoroutine(placers[method].PlaceBuildings(block, buildingVariants));
    }
}

public enum BuildingPlacementMethod
{
    BoxFit,
    ShapeFit,
    RingFill,
    SingleBuilding
}