using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BiomeGrid : MonoBehaviour
{
    public int width = 6;
    public int height = 6;

    public Camera cam;
    public Dropdown biomeDropdown;
    public BiomeCell biomePrefab;

    public List<Biome> inputBiomes;

    private TileManager tileManager;
    private readonly Dictionary<BiomeType, Biome> biomes = new Dictionary<BiomeType, Biome>();
    private readonly List<BiomeCell> biomeCells = new List<BiomeCell>();
    private readonly Queue<BiomeCell> triangleQueue = new Queue<BiomeCell>();
    private Biome selectedBiome;

    void Awake()
    {
        tileManager = GetComponent<TileManager>();

        for (int r = 0; r < height; r++)
            for (int q = 0; q < width; q++)
                CreateCell(q, r);
    }

    private void Start()
    {
        InitializeBiomes();
        ChangeSelectedBiome();
        Generate();
    }

    public void FixedUpdate()
    {
        if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            LayerMask biomeLayerMask = LayerMask.GetMask("Biomes");
            Vector3 mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f);
            Ray ray = cam.ScreenPointToRay(mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 1000f, biomeLayerMask))
                AlterBiome(hit.collider.gameObject.GetComponent<BiomeCell>());
        }
    }

    public void Reset()
    {
        tileManager.Reset();
        foreach (var biomeCell in biomeCells)
            biomeCell.Reset();
    }

    public void Generate()
    {
        GenerateBiomeCells();

        foreach (var b in biomeCells)
        {
            if (b.coordinates.q >= width - 1)
                continue;
            if (b.coordinates.r < height - 1)
                triangleQueue.Enqueue(b);
            if (b.coordinates.r > 0)
                triangleQueue.Enqueue(b);
        }

        StartCoroutine(nameof(CreateTrianglesFromQueue));
    }

    public void ChangeSelectedBiome()
    {
        selectedBiome = biomes[(BiomeType)biomeDropdown.value];
    }

    void InitializeBiomes()
    {
        var options = new List<Dropdown.OptionData>();
        foreach (var biome in inputBiomes.OrderBy(biome => biome.type))
        {
            biomes.Add(biome.type, biome);
            options.Add(new Dropdown.OptionData(biome.displayName));
        }

        biomeDropdown.AddOptions(options);
    }

    void CreateCell(int q, int r)
    {
        var position = new Vector3
        {
            x = (q + r * 0.5f - r / 2) * (HexMetrics.innerRadius * 2f),
            y = 0f,
            z = r * (HexMetrics.outerRadius * 1.5f),
        };

        BiomeCell cell = Instantiate(biomePrefab, transform);
        cell.grid = this;
        cell.SetCoordinates(q, r);
        cell.transform.localPosition = position;

        biomeCells.Add(cell);
    }

    void CreateUpTriangle(BiomeCell baseBiome)
    {
        var coordsNE = new BiomeCoordinates(baseBiome.coordinates.NorthEast());
        BiomeCell biomeNE = biomeCells.Single(b => b.coordinates.Equals(coordsNE));

        var coordsE = new BiomeCoordinates(baseBiome.coordinates.East());
        BiomeCell biomeE = biomeCells.Single(b => b.coordinates.Equals(coordsE));

        BiomeCell[] relevantCells = new BiomeCell[] { baseBiome, biomeNE, biomeE };
        tileManager.CreateTileCell(relevantCells, true);
    }

    void CreateDownTriangle(BiomeCell baseBiome)
    {
        var coordsE = new BiomeCoordinates(baseBiome.coordinates.East());
        BiomeCell biomeE = biomeCells.Single(b => b.coordinates.Equals(coordsE));

        var coordsSE = new BiomeCoordinates(baseBiome.coordinates.SouthEast());
        BiomeCell biomeSE = biomeCells.Single(b => b.coordinates.Equals(coordsSE));

        BiomeCell[] relevantCells = new BiomeCell[] { baseBiome, biomeE, biomeSE };
        tileManager.CreateTileCell(relevantCells, false);
    }

    void GenerateBiomeCells()
    {
        bool IsOnBorder(BiomeCoordinates coords) => coords.q <= 0 || coords.r <= 0 || coords.q >= width - 1 || coords.r >= height - 1;
        biomeCells.Where(cell => !cell.type.HasValue
                              && IsOnBorder(cell.coordinates))
                  .ForEach(b => b.SetBiome(biomes[BiomeType.Water]));

        biomeCells.Where(cell => !cell.type.HasValue)
                  .ForEach(cell => ChooseBiome(cell));
    }

    IEnumerator CreateTrianglesFromQueue()
    {
        while (triangleQueue.Any())
        {
            CreateTrianglesForBiome(triangleQueue.Dequeue());
            yield return new WaitForSeconds(0);
        }
    }

    void ChooseBiome(BiomeCell cell)
    {
        var neighborCoordinates = cell.coordinates.AllNeighbors();
        var neighborTypes = biomeCells.Where(biomeCell => neighborCoordinates.Contains(biomeCell.coordinates) && biomeCell.type.HasValue)
                                      .Select(neighborCell => neighborCell.type.Value)
                                      .ToList();

        var possibilities = new Dictionary<Biome, float>();
        biomes.Values.ForEach(biome => possibilities.Add(biome, biome.GetWeight(neighborTypes)));

        var sum = 0.0;
        var total = possibilities.Values.Sum();
        var rand = UnityEngine.Random.value * total;

        var chosenBiome = possibilities.First(entry => { sum += entry.Value; return rand < sum; }).Key;
        cell.SetBiome(chosenBiome);
    }

    internal void AlterBiome(BiomeCell biomeCell)
    {
        if (biomeCell.type == selectedBiome.type)
            return;

        biomeCell.SetBiome(selectedBiome);
        CreateTrianglesForBiome(biomeCell);
    }

    void CreateTrianglesForBiome(BiomeCell biomeCell)
    {
        var coordinates = biomeCell.coordinates;

        var neighborNE = biomeCells.SingleOrDefault(bc => bc.coordinates.Equals(new BiomeCoordinates(coordinates.NorthEast())));
        var neighborE = biomeCells.SingleOrDefault(bc => bc.coordinates.Equals(new BiomeCoordinates(coordinates.East())));
        var neighborSE = biomeCells.SingleOrDefault(bc => bc.coordinates.Equals(new BiomeCoordinates(coordinates.SouthEast())));
        var neighborSW = biomeCells.SingleOrDefault(bc => bc.coordinates.Equals(new BiomeCoordinates(coordinates.SouthWest())));
        var neighborW = biomeCells.SingleOrDefault(bc => bc.coordinates.Equals(new BiomeCoordinates(coordinates.West())));
        var neighborNW = biomeCells.SingleOrDefault(bc => bc.coordinates.Equals(new BiomeCoordinates(coordinates.NorthWest())));

        bool HasBeenSet(BiomeCell neighbor) => neighbor != null && neighbor.type.HasValue;

        if (HasBeenSet(neighborNE) && HasBeenSet(neighborE))
            CreateUpTriangle(biomeCell);

        if (HasBeenSet(neighborSE) && HasBeenSet(neighborE))
            CreateDownTriangle(biomeCell);

        if (HasBeenSet(neighborNE) && HasBeenSet(neighborNW))
            CreateDownTriangle(neighborNW);

        if (HasBeenSet(neighborNW) && HasBeenSet(neighborW))
            CreateUpTriangle(neighborW);

        if (HasBeenSet(neighborSW) && HasBeenSet(neighborW))
            CreateDownTriangle(neighborW);

        if (HasBeenSet(neighborSE) && HasBeenSet(neighborSW))
            CreateUpTriangle(neighborSW);
    }
}
