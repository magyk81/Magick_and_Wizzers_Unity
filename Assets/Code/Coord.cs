using System;

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
}
