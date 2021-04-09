using UnityEngine;

public class BiomeCell : MonoBehaviour
{
    internal BiomeGrid grid;
    internal BiomeCoordinates coordinates;

    private void OnMouseDown()
    {
        grid.AlterBiomes(coordinates);
    }

    public void ChangeMaterial(Material newMaterial)
    {
        GetComponent<MeshRenderer>().material = newMaterial;
    }

    internal void SetCoordinates(int q, int r)
    {
        coordinates = new BiomeCoordinates(q, r);
    }
}
