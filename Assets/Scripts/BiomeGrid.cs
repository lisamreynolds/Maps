using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Assets.Scripts;

public class BiomeGrid : MonoBehaviour
{
	public int width = 6;
	public int height = 6;

	public BiomeCell biomePrefab;

	public List<Biome> biomes;

	private TileManager tileManager;
	private List<BiomeCell> biomeCells;

	void Awake()
	{
		tileManager = GetComponent<TileManager>();
		biomeCells = new List<BiomeCell>();

		for (int r = 0; r < height; r++)
			for (int q = 0; q < width; q++)
				CreateCell(q, r);

		biomeCells.Where(b => b.coordinates.q < width - 1
					   && b.coordinates.r < height - 1)
			  .ForEach(b => CreateUpTriangle(b));

		biomeCells.Where(b => b.coordinates.q < width - 1
					   && b.coordinates.r > 0)
			  .ForEach(b => CreateDownTriangle(b));

		GenerateBiomes();
	}

	void CreateCell(int q, int r)
	{
		Vector3 position;
		position.x = (q + r * 0.5f - r / 2) * (HexMetrics.innerRadius * 2f);
		position.y = 0f;
		position.z = r * (HexMetrics.outerRadius * 1.5f);

		BiomeCell cell = Instantiate(biomePrefab);
		cell.grid = this;
		cell.SetCoordinates(q, r);
		cell.transform.SetParent(transform);
        cell.transform.localPosition = position;

        biomeCells.Add(cell);
	}

	void CreateUpTriangle(BiomeCell baseBiome)
    {
		Vector3 basePosition = baseBiome.transform.localPosition;

		var coordsNE = new BiomeCoordinates(baseBiome.coordinates.NorthEast());
		BiomeCell biomeNE = biomeCells.Single(b => b.coordinates.Equals(coordsNE));
		
		var coordsE = new BiomeCoordinates(baseBiome.coordinates.East());
		BiomeCell biomeE = biomeCells.Single(b => b.coordinates.Equals(coordsE));

		Vector3[] positions = new BiomeCell[] { baseBiome, biomeNE, biomeE }
			.Select(biome => biome.transform.localPosition)
			.ToArray();

		tileManager.CreateTileCell(positions[0], true);
    }

	void CreateDownTriangle(BiomeCell baseBiome)
	{
		Vector3 basePosition = baseBiome.transform.localPosition;
		
		var coordsE = new BiomeCoordinates(baseBiome.coordinates.East());
		BiomeCell biomeE = biomeCells.Single(b => b.coordinates.Equals(coordsE));

		var coordsSE = new BiomeCoordinates(baseBiome.coordinates.SouthEast());
		BiomeCell biomeSE = biomeCells.Single(b => b.coordinates.Equals(coordsSE));

		Vector3[] positions = new BiomeCell[] { baseBiome, biomeE, biomeSE }
			.Select(biome => biome.transform.localPosition)
			.ToArray();

		tileManager.CreateTileCell(positions[0], false);
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

	void GenerateBiomes()
    {
		Biome waterBiome = biomes.Single(b => b.type == BiomeType.Water);
		bool IsOnBorder(BiomeCoordinates coords) => coords.q <= 0 || coords.r <= 0 || coords.q >= width - 1 || coords.r >= height - 1;
		biomeCells.Where(b => IsOnBorder(b.coordinates))
				  .ForEach(b => b.SetBiome(waterBiome));

		biomeCells.Where(cell => cell.type == BiomeType.None)
				  .ForEach(cell => ChooseBiome(cell));
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
}
