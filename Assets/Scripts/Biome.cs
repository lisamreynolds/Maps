using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "BiomeData", menuName = "ScriptableObjects/BiomeScriptableObject", order = 1)]
public class Biome : ScriptableObject
{
    public BiomeType type;
    public Material material;

    public float GetWeight(List<BiomeType> neighborTypes)
    {
        int MatchingNeighbors() => neighborTypes.Count(neighborType => neighborType == type);

        if (type == BiomeType.Water)
        {
            return Mathf.Max(MatchingNeighbors(), .5f);
        }

        return Mathf.Max(MatchingNeighbors() * 2, 1.5f);
    }
}

public enum BiomeType
{
    None,
    Water,
    Plains,
    Forest
}


