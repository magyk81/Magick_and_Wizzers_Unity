using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coord
{
    private readonly int x, z;
    public int X { get { return x; } }
    public int Z { get { return z; } }
    private Coord(int x, int z)
    {
        this.x = x;
        this.z = z;
    }
    public Coord Copy() { return new Coord(x, z); }
    public static Coord _(int x, int z) { return new Coord(x, z); }
}
