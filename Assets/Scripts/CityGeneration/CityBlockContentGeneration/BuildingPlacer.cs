using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingPlacer : MonoBehaviour
{
    CityGen cityGen;
    protected BuildingsGen buildingsGen;
    protected CityBlock block;
    protected List<BuildingVariant> variants;
    public List<Building> Placed { get; protected set; }

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

    protected void FilterVariantsBasedOnAllowedHeight()
    {
        //absolutely min and max heights for any building for block
        float blockHeightMin = GetHeightMin(block.GetPointFurthestFromCityCenter());
        float blockHeightMax = GetHeightMax(block.GetPointClosestToCityCenter());
        List<BuildingVariant> allowed = new List<BuildingVariant>();
        for (int i = 0; i < variants.Count; i++)
        {
            if (variants[i].HeightMin < blockHeightMax && variants[i].HeightMax > blockHeightMin)
            {
                allowed.Add(variants[i]);
            }
        }
        variants = allowed;
    }

    public virtual IEnumerator PlaceBuildings(CityBlock block, List<BuildingVariant> variants)
    {
        yield return null;
    }

    protected void DestroyPlaced()
    {
        if (Placed != null)
        {
            for (int i = 0; i < Placed.Count; i++)
            {
                Destroy(Placed[i].gameObject);
            }
            Placed.Clear();
        }
    }

}
