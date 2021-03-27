using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class responsible for handling generation of contents of a city block.
/// <para>
/// It decides what kind of contents will be generated and gives the task to a class that specializes in that specific type of content.
/// </para>
/// </summary>
public class CityBlockPurposeDeterminer : MonoBehaviour
{
    [Header("Relative probabilities of assigning one of these purpuses for a CityBlock")]
    [SerializeField]
    private float buildingBias = 1;
    [SerializeField]
    private float parkBias = 0.15f;
    [SerializeField]
    private float plazaBias = 0.1f;

    [Header("Between what distances from center can parks be generated, represented as a fraction of radius.")]
    [SerializeField]
    private float parkRadiusFractionMin = 0.15f;
    [SerializeField]
    private float parkRadiusFractionMax = 0.7f;

    [SerializeField]
    private float plazaAreaSizeLimit = 300f;

    private CityGen cityGen;
    
    private float parkDistMin;
    private float parkDistMax;

    private List<CityBlockPurpose> purposes = new List<CityBlockPurpose>()
    {
        CityBlockPurpose.Buildings, CityBlockPurpose.Plaza, CityBlockPurpose.Park
    };

    private void Awake()
    {
        cityGen = GetComponent<CityGen>();
        parkDistMin = cityGen.RimRadius * parkRadiusFractionMin;
        parkDistMax = cityGen.RimRadius * parkRadiusFractionMax;    
    }

    public void DeterminePurposeOfCityBlocks(List<CityBlock> blocks)
    {
        DB.Log("DETERMINING PURPOSE OF CITY BLOCKS", 1);
        List<float> biases = new List<float>() { buildingBias, 1, 1};
        for (int i = 0; i < blocks.Count; i++)
        {
            biases[1] = (blocks[i].Area <= plazaAreaSizeLimit) ? plazaBias : 0;
            biases[2] = (blocks[i].DistFromOrigin >= parkDistMin && blocks[i].DistFromOrigin <= parkDistMax) ? parkBias : 0;

            blocks[i].SetPurpose(purposes.RandomElementWithBias(biases));
        }
    }


}
