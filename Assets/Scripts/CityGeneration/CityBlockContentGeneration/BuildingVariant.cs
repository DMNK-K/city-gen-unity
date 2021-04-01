using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Bld", menuName = "ScriptableObjects/BuildingVariant", order = 1)]
public class BuildingVariant : ScriptableObject
{
    [SerializeField]
    private MeshFilter prefabFoundation;
    [SerializeField]
    private MeshFilter prefabBody;

    [SerializeField]
    [Tooltip("Signifies that this building's wall must be a length that is a multiple of this number, 0 for no restriction.")]
    private float wallLengthMultiple = 1;

    [SerializeField]
    private float wallLengthMin = 10;
    [SerializeField]
    private float wallLengthMax = 120;
    [SerializeField]
    [Range(1, 10f)]
    private float wallRatioMax = 4;

    [Space]
    [SerializeField]
    private int storiesMin = 1;
    [SerializeField]
    private int storiesMax = 100;
    [SerializeField]
    private float storyHeight = 3f;
    [SerializeField]
    private float foundationHeight = 0.2f;

    public MeshFilter PrefabFoundation { get { return prefabFoundation; } }
    public MeshFilter PrefabBody { get { return prefabBody; } }

    public float WallLengthMultiple { get { return wallLengthMultiple; } }
    public float WallLengthMin { get { return (wallLengthMultiple <= 0) ? wallLengthMin : GS.CeilTo(wallLengthMin, wallLengthMultiple); } }
    public float WallLengthMax { get { return (wallLengthMultiple <= 0) ? wallLengthMax : GS.FloorTo(wallLengthMax, wallLengthMultiple); } }
    public float WallRatioMax { get { return Mathf.Max(1, wallRatioMax); } }

    public int StoriesMin { get { return storiesMin; } }
    public int StoriesMax { get { return storiesMax; } }
    public float StoryHeight { get { return storyHeight; } }
    public float FoundationHeight { get { return foundationHeight; } }
    public float HeightMax { get { return storyHeight * storiesMax + foundationHeight; } }
    public float HeightMin { get { return storyHeight * storiesMin + foundationHeight; } }

    public float GetRandomWallLength()
    {
        return GS.RoundTo(Random.Range(WallLengthMin, WallLengthMax), wallLengthMultiple);
    }

    public float GetRandomWallLength(float otherWallLength)
    {
        if (WallRatioMax == 1) { return otherWallLength; }
        float ratioEnforcedMin = otherWallLength / WallRatioMax;
        float ratioEnforcedMax = otherWallLength * WallRatioMax;
        if (wallLengthMultiple > 0)
        {
            ratioEnforcedMin = GS.CeilTo(ratioEnforcedMin, wallLengthMultiple);
            ratioEnforcedMax = GS.FloorTo(ratioEnforcedMax, wallLengthMultiple);
        }
        float actualMin = Mathf.Max(ratioEnforcedMin, WallLengthMin);
        float actualMax = Mathf.Min(ratioEnforcedMax, WallLengthMax);
        return GS.RoundTo(Random.Range(actualMin, actualMax), wallLengthMultiple);
    }

    public Vector2 GetRandomWallLengths(float squaringChance)
    {
        Vector2 lengths = new Vector2(GetRandomWallLength(), 0);
        lengths.y = (GS.RChance(squaringChance)) ? lengths.x : GetRandomWallLength(lengths.x);
        return lengths;
    }
}
