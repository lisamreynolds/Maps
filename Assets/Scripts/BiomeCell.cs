using UnityEngine;

public class BiomeCell : MonoBehaviour
{
    public BiomeType type;

    internal BiomeGrid grid;
    internal BiomeCoordinates coordinates;

    private void OnMouseDown()
    {
        grid.AlterBiomes(coordinates);
    }

    public void SetBiome(Biome biome)
    {
        type = biome.type;
        GetComponent<MeshRenderer>().material = biome.material;
    }

    internal void SetCoordinates(int q, int r)
    {
        coordinates = new BiomeCoordinates(q, r);
    }
}
