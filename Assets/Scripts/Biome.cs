using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "BiomeData", menuName = "ScriptableObjects/BiomeScriptableObject", order = 1)]
public class Biome : ScriptableObject
{
    public BiomeType type;
    public string displayName;
    public Material material;

    public float GetWeight(List<BiomeType> neighborTypes)
    {
        var matchingNeighbors = neighborTypes.Count(neighborType => neighborType == type);

        if (type == BiomeType.Water)
        {
            return Mathf.Max(matchingNeighbors, .5f);
        }

        return Mathf.Max(matchingNeighbors * 2, 1.5f);
    }
}

public enum BiomeType
{
    Water = 0,
    Plains = 1,
    Forest = 2
}


