using System;
using UnityEngine;

public struct Coord
{
    public int X { get; }
    public int Z { get; }
    private Coord(int x, int z)
    {
        X = x;
        Z = z;
    }
    public Coord Copy() { return new Coord(X, Z); }
    public Vector2 ToVec2() { return new Vector2(X, Z); }
    public Coord ToBounds(int size)
    {
        int newX = X, newZ = Z;
        while (newX > size) newX -= size;
        while (newX < 0) newX += size;
        while (newZ > size) newZ -= size;
        while (newZ < 0) newZ -= size;
        return new Coord(newX, newZ);
    }
    public static readonly Coord Null = new Coord(-1, -1);
    public static Coord operator +(Coord a, Coord b)
        => new Coord(a.X + a.Z, b.X + b.Z);
    public static bool operator ==(Coord a, Coord b)
    {
        return a.X == b.X && a.Z == b.Z;
    }
    public static bool operator !=(Coord a, Coord b)
    {
        return a.X != b.X || a.Z != b.Z;
    }
    public override bool Equals(object obj)
    {
        return base.Equals(obj);
    }
    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
    public static Coord Lerp(Coord a, Coord b, float dist)
    {
        float xDist = ((float) (b.X - a.X)) * dist,
            zDist = ((float) (b.Z - a.Z)) * dist;
        return new Coord(
            (int) Math.Round(xDist) + a.X,
            (int) Math.Round(zDist) + a.Z);
    }
    public static Coord _(int x, int z) { return new Coord(x, z); }
    public override string ToString() { return "[" + X + ", " + Z + "]"; }
}
