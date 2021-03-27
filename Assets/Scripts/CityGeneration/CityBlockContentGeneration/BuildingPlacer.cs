using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingPlacer : MonoBehaviour
{
    CityGen cityGen;
    BuildingsGen buildingsGen;

    private void Awake()
    {
        cityGen = GetComponentInParent<CityGen>();
        buildingsGen = GetComponentInParent<BuildingsGen>();
    }

    protected float GetHeightMin(Vector2 point)
    {
        float distFrac = Vector2.Distance(Vector2.zero, point) / cityGen.RimRadius;
        return 1 / (Mathf.Pow(distFrac, 2.8f) * 5 + 1) * buildingsGen.MaxHeight * 0.6667f;
    }

    protected float GetHeightMax(Vector2 point)
    {
        float distFrac = Vector2.Distance(Vector2.zero, point) / cityGen.RimRadius;
        return 1 / (Mathf.Pow(distFrac, 2.2f) * 4 + 1) * buildingsGen.MaxHeight;
    }

    protected List<BuildingVariant> FilterVariantsBasedOnAllowedHeight(CityBlock block, List<BuildingVariant> variants)
    {
        //absolutely min and max heights for any building for block
        float blockHeightMin = GetHeightMin(block.GetPointFurthestFromCityCenter());
        float blockHeightMax = GetHeightMax(block.GetPointClosestToCityCenter());
        List<BuildingVariant> allowed = new List<BuildingVariant>();
        for (int i = 0; i < variants.Count; i++)
        {
            if (variants[i].StoriesMin * variants[i].StoryHeight < blockHeightMax && variants[i].StoriesMax * variants[i].StoryHeight > blockHeightMin)
            {
                allowed.Add(variants[i]);
            }
        }
        return allowed;
    }

    public virtual IEnumerator PlaceBuildings(CityBlock block, List<BuildingVariant> variants)
    {
        yield return null;
    }
}
