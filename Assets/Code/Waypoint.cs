using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint
{
    private Coord tile;
    public Coord Tile { get { return tile; } set { tile = value; } }
    private Piece piece;
    public Piece Piece { set { piece = value; } }
    public bool IsSet { get { return tile != Coord.Null && piece == null; } }
    public Waypoint()
    {
        Reset();
    }
    public void Reset()
    {
        tile = Coord.Null.Copy();
        piece = null;
    }
}
