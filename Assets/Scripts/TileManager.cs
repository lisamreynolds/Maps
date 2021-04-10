using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    public TileCell trianglePrefab;
    public List<Tile> tiles;
    
    internal List<TileCell> tileCells;

    private void Awake()
    {
        tileCells = new List<TileCell>();
    }

    internal void CreateTileCell(Vector3 basePosition, bool upTriangle)
    {
        var rotation = Quaternion.Euler(90, upTriangle ? 0 : 180, 0);
        var position = basePosition + HexMetrics.corners[upTriangle ? 1 : 2];
        position.y = -HexMetrics.outerRadius; // Weird artifact of Blender model?

        var tile = Instantiate(tiles.Random().tileObject, position, rotation, transform);
        tile.transform.localScale = new Vector3(HexMetrics.outerRadius, HexMetrics.outerRadius, HexMetrics.outerRadius);
    }
}
