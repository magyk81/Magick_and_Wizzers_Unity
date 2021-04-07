using UnityEngine;

public class Coord
{
    private readonly int x, y;
    public int X { get { return x; } }
    public int Y { get { return y; } }
    private Coord(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public static Coord _(int x, int y) { return new Coord(x, y); }
    public static Coord _(int x) { return new Coord(x, x); }
    public Coord copy() { return new Coord(x, y); }

    public override string ToString() { return x + ", " + y; }

    public Vector3 ToVec3() { return new Vector3(x, 0, y); }
    public bool Equals(Coord other) { return x == other.x && y == other.y; }

    public static Coord operator +(Coord a, Coord b) => new Coord(a.x + b.x, a.y + b.y);
    public static Coord operator +(Coord coord, int dir) => new Coord(
        coord.x + (dir == LEFT ? -1 : (dir == RIGHT ? 1 : 0)),
        coord.y + (dir == DOWN ? -1 : (dir == UP    ? 1 : 0)));
    public static Coord operator *(Coord _, int a) => new Coord(_.x * a, _.y * a);

    public static readonly int LEFT = 0, RIGHT = 1, UP = 2, DOWN = 3;
    public static string ToString(int dir)
    {
        if (dir == LEFT) return "LEFT";
        if (dir == RIGHT) return "RIGHT";
        if (dir == UP) return "UP";
        if (dir == DOWN) return "DOWN";
        return "NONE";
    }
    public static int opp(int dir)
    {
        if (dir == LEFT) return RIGHT;
        if (dir == RIGHT) return LEFT;
        if (dir == UP) return DOWN;
        if (dir == DOWN) return UP;
        return -1;
    }

    public int[] GetDistancesTo(Coord destination, int boardSize)
    {
        int[] distances = new int[4];
        if (x < destination.x)
        {
            distances[LEFT] = x + (boardSize - destination.x);
            distances[RIGHT] = destination.x - x;
        }
        else
        {
            distances[LEFT] = x - destination.x;
            distances[RIGHT] = destination.x + (boardSize - x);
        }
        if (y < destination.y)
        {
            distances[DOWN] = y + (boardSize - destination.y);
            distances[UP] = destination.y - y;
        }
        else
        {
            distances[DOWN] = y - destination.y;
            distances[UP] = destination.y + (boardSize - y);
        }
        return distances;
    }

    public int GetDirTo(Coord next)
    {
        if (x == next.x)
        {
            if (y + 1 == next.y) return UP;
            else if (y - 1 == next.y) return DOWN;
        }
        else if (y == next.y)
        {
            if (x + 1 == next.x) return RIGHT;
            else if (x - 1 == next.x) return LEFT;
        }
        return -1;
    }
}