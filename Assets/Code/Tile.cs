using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile
{
    private readonly GameObject[] objs;
    private Tile[] neighbors = new Tile[4];
    public Tile[] Neighbors {
        set {
                neighbors[Coord.LEFT] = value[Coord.LEFT];
                neighbors[Coord.RIGHT] = value[Coord.RIGHT];
                neighbors[Coord.UP] = value[Coord.UP];
                neighbors[Coord.DOWN] = value[Coord.DOWN];
        }
    }
    private readonly Coord COORD;
    public Coord Coord {
        get { return COORD.copy(); }
    }

    public int X { get { return COORD.X; } }
    public int Y { get { return COORD.Y; } }

    public Tile(Coord[] coords, int chunkSize)
    {
        COORD = coords[0];
    }

    public bool IsNeighbor(Tile tile)
    {
        foreach (Tile _ in neighbors) { if (_ == tile) return true; }
        return false;
    }

    public static Tile operator +(Tile tile, int dir)
        => tile.neighbors[dir];
    public static Tile operator -(Tile tile, int dir)
        => tile.neighbors[Coord.opp(dir)];
}
