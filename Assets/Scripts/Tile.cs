using UnityEngine;

public struct Tile
{
    public GameObject tileObject;
    public BiomeType[] biomes;
    public Quaternion rotation;

    public Tile(GameObject tileObject, BiomeType[] biomes, Quaternion rotation)
    {
        this.tileObject = tileObject;
        this.biomes = biomes;
        this.rotation = rotation;
    }
}
