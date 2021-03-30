using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingVariant : ScriptableObject
{
    [SerializeField]
    private MeshFilter prefabFoundation;
    [SerializeField]
    private MeshFilter prefabBody;

    [SerializeField]
    [Tooltip("Signifies that this building's wall must be a length that is a multiple of this number")]
    private float wallLengthMultiple = 1;

    [SerializeField]
    private float wallLengthMin = 10;
    [SerializeField]
    private float wallLengthMax = 200;

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
}
