using UnityEngine;

[CreateAssetMenu(fileName = "TileData", menuName = "ScriptableObjects/TileScriptableObject", order = 1)]
public class TileInput : ScriptableObject
{
    public GameObject tileObject;
    public bool flip;
    public bool rotate;
    public BiomeType[] biomes = new BiomeType[3];
}
