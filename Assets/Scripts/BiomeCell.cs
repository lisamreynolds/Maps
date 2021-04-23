using System;
using UnityEngine;

public class BiomeCell : MonoBehaviour
{
    public Material defaultMaterial;

    internal BiomeType? type;
    internal BiomeCoordinates coordinates;

    internal BiomeGrid grid;

    public void SetBiome(Biome biome)
    {
        type = biome.type;
        GetComponent<MeshRenderer>().material = biome.material;
    }

    internal void SetCoordinates(int q, int r)
    {
        coordinates = new BiomeCoordinates(q, r);
    }

    internal void Reset()
    {
        type = null;
        GetComponent<MeshRenderer>().material = defaultMaterial;
    }
}
