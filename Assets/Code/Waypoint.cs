/* Copyright (C) All Rights Reserved
 * Unauthorized copying of this file, via any medium is prohibited.
 * Proprietary and confidential.
 * Written by Robin Campos <magyk81@gmail.com>, year 2021.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint
{
    private Coord tile;
    public Coord Tile {
        get { return tile; }
        set {
                tile = value;
                piece = null;
            }
    }
    private Piece piece;
    public Piece Piece {
        get { return piece; }
        set {
                piece = value;
                tile = Coord.Null;
            }
    }
    public bool IsSet { get { return tile != Coord.Null || piece != null; } }
    public Waypoint() { Reset(); }
    public void Reset()
    {
        tile = Coord.Null.Copy();
        piece = null;
    }

    public Waypoint Copy()
    {
        Waypoint copy = new Waypoint();
        copy.tile = tile;
        copy.piece = piece;
        return copy;
    }

    public static bool operator ==(Waypoint a, Waypoint b)
    {
        return a.tile == b.tile && a.piece == b.piece;
    }
    public static bool operator !=(Waypoint a, Waypoint b)
    {
        return a.tile != b.tile || a.piece != b.piece;
    }



    public override bool Equals(object obj)
    {
        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override string ToString()
    {
        return IsSet ? ((piece != null) ? piece.Name : tile.ToString()) : "[]";
    }
}
