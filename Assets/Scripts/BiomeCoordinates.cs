using System.Collections.Generic;

internal struct BiomeCoordinates
{
    public int q;
    public int r;

    public BiomeCoordinates(int column, int row)
    {
        q = column;
        r = row;
    }
    public BiomeCoordinates((int column, int row) coordinates)
    {
        q = coordinates.column;
        r = coordinates.row;
    }

    public List<BiomeCoordinates> AllNeighbors()
    {
        return new List<BiomeCoordinates>()
        {
            NorthEast(),
            East(),
            SouthEast(),
            SouthWest(),
            West(),
            NorthWest()
        };
    }

    public BiomeCoordinates NorthEast() => new BiomeCoordinates(q + (r & 1), r + 1);
    public BiomeCoordinates East() => new BiomeCoordinates(q + 1, r);
    public BiomeCoordinates SouthEast() => new BiomeCoordinates(q + (r & 1), r - 1);
    public BiomeCoordinates SouthWest() => new BiomeCoordinates(q - 1 + (r & 1), r - 1);
    public BiomeCoordinates West() => new BiomeCoordinates(q - 1, r);
    public BiomeCoordinates NorthWest() => new BiomeCoordinates(q - 1 + (r & 1), r + 1);
}