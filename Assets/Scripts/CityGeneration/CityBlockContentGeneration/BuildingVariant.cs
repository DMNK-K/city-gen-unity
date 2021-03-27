using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingVariant : ScriptableObject
{
    [SerializeField]
    [Tooltip("Signifies that this building's wall must be a length that is a multiple of this number")]
    private float wallLengthMultiple;

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

    public float WallLengthMultiple { get { return wallLengthMultiple; } }
    public float WallLengthMin { get { return wallLengthMin; } }
    public float WallLengthMax { get { return wallLengthMax; } }

    public int StoriesMin { get { return storiesMin; } }
    public int StoriesMax { get { return storiesMax; } }
    public float StoryHeight { get { return storyHeight; } }

    public float GetRandomWallLength()
    {
        return Random.Range(wallLengthMin, wallLengthMax);
    }
}
