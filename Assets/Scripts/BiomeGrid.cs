using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BiomeGrid : MonoBehaviour
{
    public int width = 6;
    public int height = 6;

    public BiomeCell biomePrefab;

    public List<Biome> biomes;

    private TileManager tileManager;
    private readonly List<BiomeCell> biomeCells = new List<BiomeCell>();
    private readonly Queue<Action> triangleQueue = new Queue<Action>();

    void Awake()
    {
        tileManager = GetComponent<TileManager>();

        for (int r = 0; r < height; r++)
            for (int q = 0; q < width; q++)
                CreateCell(q, r);
    }

    private void Start()
    {
        GenerateBiomes();

        foreach (var b in biomeCells)
        {
            if (b.coordinates.q >= width - 1)
                continue;
            if (b.coordinates.r < height - 1)
                triangleQueue.Enqueue(() => CreateUpTriangle(b));
            if (b.coordinates.r > 0)
                triangleQueue.Enqueue(() => CreateDownTriangle(b));
        }

        StartCoroutine(nameof(CreateTrianglesFromQueue));
    }

    void CreateCell(int q, int r)
    {
        var position = new Vector3
        {
            x = (q + r * 0.5f - r / 2) * (HexMetrics.innerRadius * 2f),
            y = 0f,
            z = r * (HexMetrics.outerRadius * 1.5f),
        };

        BiomeCell cell = Instantiate(biomePrefab);
        cell.grid = this;
        cell.SetCoordinates(q, r);
        cell.transform.SetParent(transform);
        cell.transform.localPosition = position;

        biomeCells.Add(cell);
    }

    void CreateUpTriangle(BiomeCell baseBiome)
    {
        var coordsNE = new BiomeCoordinates(baseBiome.coordinates.NorthEast());
        BiomeCell biomeNE = this.biomeCells.Single(b => b.coordinates.Equals(coordsNE));

        var coordsE = new BiomeCoordinates(baseBiome.coordinates.East());
        BiomeCell biomeE = this.biomeCells.Single(b => b.coordinates.Equals(coordsE));

        BiomeCell[] biomeCells = new BiomeCell[] { baseBiome, biomeNE, biomeE };
        tileManager.CreateTileCell(biomeCells, true);
    }

    void CreateDownTriangle(BiomeCell baseBiome)
    {
        var coordsE = new BiomeCoordinates(baseBiome.coordinates.East());
        BiomeCell biomeE = this.biomeCells.Single(b => b.coordinates.Equals(coordsE));

        var coordsSE = new BiomeCoordinates(baseBiome.coordinates.SouthEast());
        BiomeCell biomeSE = this.biomeCells.Single(b => b.coordinates.Equals(coordsSE));

        BiomeCell[] biomeCells = new BiomeCell[] { baseBiome, biomeE, biomeSE };
        tileManager.CreateTileCell(biomeCells, false);
    }

    void GenerateBiomes()
    {
        Biome waterBiome = biomes.Single(b => b.type == BiomeType.Water);
        bool IsOnBorder(BiomeCoordinates coords) => coords.q <= 0 || coords.r <= 0 || coords.q >= width - 1 || coords.r >= height - 1;
        biomeCells.Where(b => IsOnBorder(b.coordinates))
                  .ForEach(b => b.SetBiome(waterBiome));

        biomeCells.Where(cell => cell.type == BiomeType.None)
                  .ForEach(cell => ChooseBiome(cell));
    }

    IEnumerator CreateTrianglesFromQueue()
    {
        while (triangleQueue.Any())
        {
            triangleQueue.Dequeue().Invoke();
            yield return new WaitForSeconds(0);
        }
    }

    void ChooseBiome(BiomeCell cell)
    {
        var neighborCoordinates = cell.coordinates.AllNeighbors();
        var neighborTypes = biomeCells.Where(bc => neighborCoordinates.Contains(bc.coordinates))
                                      .Select(neighborCell => neighborCell.type)
                                      .ToList();

        var possibilities = new Dictionary<Biome, float>();
        biomes.Where(b => b.type != BiomeType.None)
              .ForEach(biome => possibilities.Add(biome, biome.GetWeight(neighborTypes)));

        var sum = 0.0;
        var total = possibilities.Values.Sum();
        var rand = UnityEngine.Random.value * total;

        var chosenBiome = possibilities.First(entry => { sum += entry.Value; return rand < sum; }).Key;
        cell.SetBiome(chosenBiome);
    }

    internal void AlterBiomes(BiomeCoordinates coordinates)
    {
        // Just to prove that the coordinate system works

        Biome plainsBiome = biomes.Single(b => b.type == BiomeType.Plains);

        var affectedCoordinates = new List<BiomeCoordinates>();
        affectedCoordinates.Add(coordinates);
        affectedCoordinates.AddRange(coordinates.AllNeighbors());

        biomeCells.Where(b => affectedCoordinates.Contains(b.coordinates))
                  .ForEach(b => b.SetBiome(plainsBiome));
    }
}
