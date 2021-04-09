using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BiomeGrid : MonoBehaviour
{
	public int width = 6;
	public int height = 6;

	public BiomeCell cellPrefab;

	private List<BiomeCell> biomes;

	void Awake()
	{
		biomes = new List<BiomeCell>();

		for (int r = 0; r < height; r++)
			for (int q = 0; q < width; q++)
				CreateCell(q, r);
	}

	void CreateCell(int q, int r)
	{
		Vector3 position;
		position.x = (q + r * 0.5f - r / 2) * (HexMetrics.innerRadius * 2f);
		position.y = 0f;
		position.z = r * (HexMetrics.outerRadius * 1.5f);

		BiomeCell cell = Instantiate(cellPrefab);
		cell.grid = this;
		cell.SetCoordinates(q, r);
		cell.transform.SetParent(transform, false);
		cell.transform.localPosition = position;

		biomes.Add(cell);
	}

	internal void AlterBiomes(BiomeCoordinates coordinates)
    {
		// Just to prove that the coordinate system works

		Material plainsMaterial = (Material)Resources.Load("Materials/Plains");

		List<BiomeCoordinates> affectedCoordinates = new List<BiomeCoordinates>();
		affectedCoordinates.Add(coordinates);
		affectedCoordinates.AddRange(coordinates.AllNeighbors());

        biomes.Where(b => affectedCoordinates.Any(ac => ac.q == b.coordinates.q && ac.r == b.coordinates.r))
			  .ToList()
			  .ForEach(b => b.ChangeMaterial(plainsMaterial));
    }
}
