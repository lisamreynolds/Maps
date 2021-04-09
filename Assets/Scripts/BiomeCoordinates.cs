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
            new BiomeCoordinates(NorthEast()),
            new BiomeCoordinates(East()),
            new BiomeCoordinates(SouthEast()),
            new BiomeCoordinates(SouthWest()),
            new BiomeCoordinates(West()),
            new BiomeCoordinates(NorthWest())
        };
    }

    public (int, int) NorthEast() => (q + (r & 1), r + 1);
    public (int, int) East() => (q + 1, r);
    public (int, int) SouthEast() => (q + (r & 1), r - 1);
    public (int, int) SouthWest() => (q - 1 + (r & 1), r - 1);
    public (int, int) West() => (q - 1, r);
    public (int, int) NorthWest() => (q - 1 + (r & 1), r + 1);
}