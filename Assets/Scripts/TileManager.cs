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
        BiomeType[] RearrangeBiomes(BiomeType[] biomes, int first, int second, int third) => new BiomeType[] { biomes[first], biomes[second], biomes[third] };

        Quaternion flip = Quaternion.Euler(0, 180, 0);
        Quaternion rotateForward = Quaternion.Euler(0, 0, 120);
        Quaternion rotateBackward = Quaternion.Euler(0, 0, -120);

        foreach (var input in tileInputs)
        {
            tileOptions.Add(new Tile(input.tileObject, input.biomes, Quaternion.identity));

            if (input.flip)
                tileOptions.Add(new Tile(input.tileObject, input.biomes.Reverse().ToArray(), flip));

            if (input.rotate)
            {
                tileOptions.Add(new Tile(input.tileObject, RearrangeBiomes(input.biomes, 1, 2, 0), rotateBackward));
                tileOptions.Add(new Tile(input.tileObject, RearrangeBiomes(input.biomes, 2, 0, 1), rotateBackward * rotateBackward));

                if (input.flip)
                {
                    tileOptions.Add(new Tile(input.tileObject, RearrangeBiomes(input.biomes, 1, 0, 2), flip * rotateForward));
                    tileOptions.Add(new Tile(input.tileObject, RearrangeBiomes(input.biomes, 0, 2, 1), flip * rotateForward * rotateForward));
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
