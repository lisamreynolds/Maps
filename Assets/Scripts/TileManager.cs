using Assets.Scripts;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    public List<TileInput> tileInputs;

    internal List<Tile> tileOptions = new List<Tile>();
    internal Dictionary<Vector3, GameObject> placedTiles = new Dictionary<Vector3, GameObject>();

    private void Awake()
    {
        foreach (var tileInput in tileInputs)
        {
            // If you're reading this, send help
            tileOptions.Add(new Tile()
            {
                tileObject = tileInput.tileObject,
                biomes = tileInput.biomes,
                rotation = Quaternion.identity
            });

            if (tileInput.flip)
                tileOptions.Add(new Tile()
                {
                    tileObject = tileInput.tileObject,
                    biomes = tileInput.biomes.Reverse().ToArray(),
                    rotation = Quaternion.Euler(0, 180, 0)
                });

            if (tileInput.rotate)
            {
                tileOptions.Add(new Tile()
                {
                    tileObject = tileInput.tileObject,
                    biomes = new BiomeType[] { tileInput.biomes[1], tileInput.biomes[2], tileInput.biomes[0] },
                    rotation = Quaternion.Euler(0, 0, -120)
                });
                tileOptions.Add(new Tile()
                {
                    tileObject = tileInput.tileObject,
                    biomes = new BiomeType[] { tileInput.biomes[2], tileInput.biomes[0], tileInput.biomes[1] },
                    rotation = Quaternion.Euler(0, 0, -240)
                });

                if (tileInput.flip)
                {
                    tileOptions.Add(new Tile()
                    {
                        tileObject = tileInput.tileObject,
                        biomes = new BiomeType[] { tileInput.biomes[1], tileInput.biomes[0], tileInput.biomes[2] },
                        rotation = Quaternion.Euler(0, 180, 120)
                    });
                    tileOptions.Add(new Tile()
                    {
                        tileObject = tileInput.tileObject,
                        biomes = new BiomeType[] { tileInput.biomes[0], tileInput.biomes[2], tileInput.biomes[1] },
                        rotation = Quaternion.Euler(0, 180, 240)
                    });
                }
            }
        }
    }

    internal void CreateTileCell(BiomeCell[] biomeCells, bool upTriangle)
    {
        var tileChoice = ChooseTile(biomeCells);

        // Interesting base rotation due to the Blender models having different ideas of what y and z mean
        var rotation = Quaternion.Euler(-90, upTriangle ? -180 : -120, 0) * tileChoice.rotation;

        var basePosition = biomeCells[0].transform.localPosition;
        var position = basePosition + HexMetrics.corners[upTriangle ? 1 : 2];

        if (placedTiles.ContainsKey(position))
        {
            Destroy(placedTiles[position]);
            placedTiles.Remove(position);
        }

        var newTile = Instantiate(tileChoice.tileObject, position, rotation, transform);
        newTile.transform.localScale = new Vector3(HexMetrics.outerRadius, HexMetrics.outerRadius, HexMetrics.outerRadius);

        placedTiles.Add(position, newTile);
    }

    private Tile ChooseTile(BiomeCell[] biomeCells)
    {
        var biomeTypes = biomeCells.Select(bc => bc.type)
                                   .ToArray();

        var matchingTiles = tileOptions.Where(tile => tile.biomes.SequenceEqual(biomeTypes));

        return matchingTiles.Any() ? matchingTiles.Random() : tileOptions.First();
    }
}
