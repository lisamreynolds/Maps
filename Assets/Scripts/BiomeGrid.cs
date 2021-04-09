using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BiomeGrid : MonoBehaviour
{
	public int width = 6;
	public int height = 6;

	public BiomeCell biomePrefab;
	public TriangleCell trianglePrefab;

	private List<BiomeCell> biomes;

	void Awake()
	{
		biomes = new List<BiomeCell>();

		for (int r = 0; r < height; r++)
			for (int q = 0; q < width; q++)
				CreateCell(q, r);

		biomes.Where(b => b.coordinates.q < width - 1
					   && b.coordinates.r < height - 1)
			  .ToList()
			  .ForEach(b => CreateUpTriangle(b));

		biomes.Where(b => b.coordinates.q < width - 1
					   && b.coordinates.r > 0)
			  .ToList()
			  .ForEach(b => CreateDownTriangle(b));
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

        biomes.Add(cell);
	}

	void CreateUpTriangle(BiomeCell baseBiome)
    {
		Vector3 basePosition = baseBiome.transform.localPosition;

		var coordsNE = new BiomeCoordinates(baseBiome.coordinates.NorthEast());
		BiomeCell biomeNE = biomes.Single(b => b.coordinates.Equals(coordsNE));
		
		var coordsE = new BiomeCoordinates(baseBiome.coordinates.East());
		BiomeCell biomeE = biomes.Single(b => b.coordinates.Equals(coordsE));

		Vector3[] positions = new BiomeCell[] { baseBiome, biomeNE, biomeE }
			.Select(biome => biome.transform.localPosition)
			.ToArray();

		TriangleCell cell = Instantiate(trianglePrefab, transform);
        cell.SetMesh(positions);
    }

	void CreateDownTriangle(BiomeCell baseBiome)
	{
		Vector3 basePosition = baseBiome.transform.localPosition;
		
		var coordsE = new BiomeCoordinates(baseBiome.coordinates.East());
		BiomeCell biomeE = biomes.Single(b => b.coordinates.Equals(coordsE));

		var coordsSE = new BiomeCoordinates(baseBiome.coordinates.SouthEast());
		BiomeCell biomeSE = biomes.Single(b => b.coordinates.Equals(coordsSE));

		Vector3[] positions = new BiomeCell[] { baseBiome, biomeE, biomeSE }
			.Select(biome => biome.transform.localPosition)
			.ToArray();

		TriangleCell cell = Instantiate(trianglePrefab, transform);
		cell.SetMesh(positions);
	}

	internal void AlterBiomes(BiomeCoordinates coordinates)
    {
		// Just to prove that the coordinate system works

		var plainsMaterial = (Material) Resources.Load("Materials/Plains");

		var affectedCoordinates = new List<BiomeCoordinates>();
		affectedCoordinates.Add(coordinates);
		affectedCoordinates.AddRange(coordinates.AllNeighbors());

		biomes.Where(b => affectedCoordinates.Contains(b.coordinates))
			  .ToList()
			  .ForEach(b => b.ChangeMaterial(plainsMaterial));
    }
}
