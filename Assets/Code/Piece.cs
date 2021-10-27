/* Copyright (C) All Rights Reserved
 * Unauthorized copying of this file, via any medium is prohibited.
 * Proprietary and confidential.
 * Written by Robin Campos <magyk81@gmail.com>, year 2021.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq.Expressions;

public class Piece
{
    #region movement

    // Set to true if the piece goes on a new tile.
    private bool posChanged = true;
    public bool PosChanged { get {
        bool temp = posChanged;
        posChanged = false;
        return temp; }
    }

    // If a piece starts moving onto a new tile, that tile is pos. If a piece
    // is not completely on the new tile, the previous tile is posPrev. If a
    // piece is completely on the new tile and has stopped moving, then pos and
    // posPrev should be equal.
    private Coord pos, posPrev;
    public Coord Pos {
        get { return pos; }
        set { pos = value.Copy(); posPrev = value.Copy(); }
    }
    public Coord PosPrev { get { return posPrev; } }
    private int posPrevDist = 0;
    private readonly int POS_DIST_MAX = 1000;
    public float GetPosLerp() { return ((float) posPrevDist) / POS_DIST_MAX; }

    List<Coord> path = new List<Coord>();
    private void SetPath()
    {
        if (waypoints[0].IsSet)
        {
            path.Clear();
            int[] dists = Util.GetDists(
                pos, waypoints[0].Tile, boardTotalSize);
            Debug.Log("left: " + dists[Util.LEFT]
                + ", right: " + dists[Util.RIGHT]
                + ", up: " + dists[Util.UP]
                + ", down: " + dists[Util.DOWN]);
            int[] pathDirs = GetPathDirs(dists);
            Debug.Log("pathDirs[0]: " + Util.DirToString(pathDirs[0])
                + ", pathDirs[1]: " + Util.DirToString(pathDirs[1]));
            return;

            int[] diagDirs = Util.DiagToDirs(pathDirs[0]);
            System.Func<int, int, int> horizInc = (int posX, int idx) => posX;
            System.Func<int, int, int> vertInc = (int posZ, int idx) => posZ;
            if (diagDirs == null)
            {
                // If diagDirs == null, that meanst pathDirs[0] is a straight
                // direction and pathDirs[1] is -1.

                if (pathDirs[0] == Util.LEFT)
                    horizInc = (posX, idx) => posX - idx;
                else if (pathDirs[0] == Util.RIGHT)
                    horizInc = (posX, idx) => posX + idx;
                if (pathDirs[0] == Util.DOWN)
                    vertInc = (posZ, idx) => posZ - idx;
                else if (pathDirs[0] == Util.UP)
                    vertInc = (posZ, idx) => posZ + idx;

                // No diagonal travel, just straight travel.
                for (int i = 1; i < dists[pathDirs[0]]; i++)
                {
                    path.Add(Coord._(horizInc(pos.X, i), vertInc(pos.Z, i)));
                }
            }
            else
            {
                if (Util.InDiag(pathDirs[0], Util.LEFT))
                    horizInc = (posX, idx) => posX - idx;
                else if (Util.InDiag(pathDirs[0], Util.RIGHT))
                    horizInc = (posX, idx) => posX + idx;
                if (Util.InDiag(pathDirs[0], Util.DOWN))
                    vertInc = (posZ, idx) => posZ - idx;
                else if (Util.InDiag(pathDirs[0], Util.UP))
                    vertInc = (posZ, idx) => posZ + idx;
                
                // Diagonal travel.
                int diagDist = Mathf.Min(
                    pathDirs[diagDirs[0]], pathDirs[diagDirs[1]]);
                
                for (int i = 1; i < diagDist; i++)
                {
                    path.Add(Coord._(horizInc(pos.X, i), vertInc(pos.Z, i)));
                }

                if (pathDirs[1] != -1)
                {
                    bool straighIsHoriz = pathDirs[1] != Util.UP
                        && pathDirs[1] != Util.DOWN;

                    if (straighIsHoriz)
                    {
                        if (pathDirs[1] == Util.LEFT)
                            horizInc = (posX, idx) => posX - idx;
                        else if (pathDirs[1] == Util.RIGHT)
                            horizInc = (posX, idx) => posX + idx;
                        
                        if (Util.InDiag(pathDirs[0], Util.UP))
                            vertInc = (posZ, idx) => posZ + diagDist;
                        else vertInc = (posZ, idx) => posZ - diagDist;
                    }
                    else
                    {
                        if (pathDirs[1] == Util.DOWN)
                            vertInc = (posZ, idx) => posZ - idx;
                        else if (pathDirs[1] == Util.UP)
                            vertInc = (posZ, idx) => posZ + idx;

                        if (Util.InDiag(pathDirs[0], Util.RIGHT))
                            horizInc = (posX, idx) => posX + diagDist;
                        else horizInc = (posX, idx) => posX - diagDist;
                    }

                    // Straight travel after diagonal.
                    //int straightDist = dists[pathDirs[1]] - diagDist;

                    for (int i = diagDist; i < dists[pathDirs[1]]; i++)
                    {
                        path.Add(Coord._(horizInc(pos.X, i), vertInc(pos.Z, i)));
                    }
                }
            }

            string _ = "path: ";
            foreach (Coord c in path) { _ += c + ", "; }
            Debug.Log(_);
        }
    }
    private static int[] GetPathDirs(int[] dists)
    {
        int[] dirs = { -1, -1 };
        int horiz = -1, vert = -1;
        if (dists[Util.LEFT] != 0 || dists[Util.RIGHT] != 0)
        {
            if (dists[Util.LEFT] > dists[Util.RIGHT]) horiz = Util.RIGHT;
            else horiz = Util.LEFT;
        }
        if (dists[Util.UP] != 0 || dists[Util.DOWN] != 0)
        {
            if (dists[Util.UP] > dists[Util.DOWN]) vert = Util.DOWN;
            else vert = Util.UP;
        }

        //if (Util.DiagToDirs())
        dirs[0] = Util.AddDirs(horiz, vert);
        if (horiz == -1) dirs[1] = vert;
        else if (vert == -1 || dists[horiz] > dists[vert]) dirs[1] = horiz;
        else if (dists[horiz] < dists[vert]) dirs[1] = vert;
        return dirs;
    }

    #endregion

    // private Coord nextTile = Coord.Null;
    // public bool IsNextTileSet() { return nextTile != Coord.Null; }
    // public void SetNextTile(int horiz, int vert)
    // {
    //     if (IsNextTileSet()) nextTile = nextTile.Dir(horiz, vert);
    //     else nextTile = pos.Dir(horiz, vert);
    // }
    // public struct PosPrecise
    // {
    //     public Coord To { get; }
    //     public Coord From { get; }
    //     public float Lerp { get; }
    //     private readonly int DIST, DIST_MAX;

    //     private PosPrecise(Coord to, Coord from, int dist)
    //     {
    //         DIST = dist;
    //         DIST_MAX = 500;

    //         To = to;
    //         From = from;
    //         Lerp = ((float) dist) / DIST_MAX;
    //     }
    //     public static PosPrecise _(Coord init)
    //     {
    //         return new PosPrecise(init, init, 0);
    //     }
    //     public bool IsDone() { return To == From; }
    //     public PosPrecise Dir(int dir)
    //     {
    //         Coord to = From.Dir(dir);
    //         if (to == To) return this;
    //         return new PosPrecise(to, From, 0);
    //     }
    //     public PosPrecise Step(Coord next, int dist)
    //     {
    //         int newDist = DIST + dist;
    //         if (newDist > DIST_MAX)
    //         {
    //             if (next == Coord.Null) return _(To);
    //             else return new PosPrecise(next, To, newDist - DIST_MAX);
    //         }
    //         return new PosPrecise(To, From, newDist);
    //     }
    // }

    private string name;
    public string Name { get { return name; } }
    public enum Type { MASTER, CREATURE, ITEM, CHARM }
    public Type pieceType;
    private int playerIdx, boardIdx, boardTotalSize;
    public int PlayerIdx { get { return playerIdx; } }
    public int BoardIdx { get { return boardIdx; } }
    public int BoardTotalSize { set { boardTotalSize = value; } }
    // private Coord pos;
    // public Coord Pos
    // {
    //     get { return pos; }
    //     set
    //     {
    //         pos = value.Copy();
    //         posPrec = PosPrecise._(value);
    //     }
    // }

    // private PosPrecise posPrec;
    // public PosPrecise PosPrec { get { return posPrec; } }
    private Card card;
    public Card Card { get { return card; } }
    private Texture art;
    public Texture Art
    {
        get
        {
            if (card != null) return card.Art;
            return art;
        }
    }
    protected Deck deck;
    private List<Card> hand = new List<Card>();
    public Card[] Hand { get { return hand.ToArray(); } }
    private Waypoint[] waypoints;
    // public Waypoint NextWaypoint { get { return waypoints[0]; } }
    public static readonly int MAX_WAYPOINTS = 5;
    public Piece(int playerIdx, int boardIdx, Card card)
    {
        this.name = card.Name;
        this.playerIdx = playerIdx;
        this.boardIdx = boardIdx;
        this.card = card;
        
        waypoints = new Waypoint[MAX_WAYPOINTS];
        for (int i = 0; i < MAX_WAYPOINTS; i++)
        {
            waypoints[i] = new Waypoint();
        }
    }
    public Piece(string name, int playerIdx, int boardIdx, Texture art)
    {
        this.name = name;
        this.playerIdx = playerIdx;
        this.art = art;

        waypoints = new Waypoint[MAX_WAYPOINTS];
        for (int i = 0; i < MAX_WAYPOINTS; i++)
        {
            waypoints[i] = new Waypoint();
        }
    }

    public void Update()
    {
        // if (!posPrec.IsDone() || NextWaypoint.IsSet)
        // {
        //     Coord tileTo = posPrec.To;
        //     posPrec = posPrec.Step(nextTile, 1);

        //     Debug.Log(posPrec.To + ", " + posPrec.From + ", " + posPrec.Lerp);

        //     // Did we step onto a new tile.
        //     if (tileTo != posPrec.To)
        //         // Set it to null so that Board will give it a new value.
        //         nextTile = Coord.Null;

        //     if (posPrec.Lerp > 0.5F) pos = posPrec.To;
        //     else pos = posPrec.From;
        // }
        


    }

    /// <param name="dist">Goes from 0 to 100.</param>
    public void Move(int dir, int dist)
    {

    }

    public void AddWaypoint(Coord tile)
    {
        for (int i = 0; i < MAX_WAYPOINTS; i++)
        {
            if (!waypoints[i].IsSet)
            {
                if (i > 0 && waypoints[i - 1].Tile == tile) return;
                waypoints[i].Tile = tile;
                UpdateWaypoints();
                break;
            }
        }
    }
    public void RemoveWaypoint(Coord tile)
    {
        for (int i = 0; i < MAX_WAYPOINTS; i++)
        {
            if (waypoints[i].IsSet && waypoints[i].Tile == tile)
            {
                waypoints[i].Reset();
                UpdateWaypoints();
                break;
            }
        }
    }
    private void UpdateWaypoints()
    {
        // Move all waypoints to the left, removing any gaps.
        for (int i = 0; i < Piece.MAX_WAYPOINTS - 1; i++)
        {
            if (!waypoints[i].IsSet)
            {
                for (int j = i + 1; j < Piece.MAX_WAYPOINTS; j++)
                {
                    if (waypoints[j].IsSet)
                    {
                        waypoints[i] = waypoints[j];
                        waypoints[j].Reset();
                    }
                }
            }
        }

        // Convert waypoints to tiles for ticket.
        Coord[] waypointTiles = new Coord[Piece.MAX_WAYPOINTS];
        for (int i = 0; i < Piece.MAX_WAYPOINTS; i++)
        {
            waypointTiles[i] = waypoints[i].Tile;
        }

        // Path may be different now that waypoints have changed.
        SetPath();

        Match.AddSkinTicket(new SkinTicket(
            this, waypointTiles, SkinTicket.Type.UPDATE_WAYPOINTS));
    }
    public bool HasSameWaypoints(Piece piece)
    {
        for (int i = 0; i < waypoints.Length; i++)
        {
            if (waypoints[i] != piece.waypoints[i]) return false;
        }
        return true;
    }
    public virtual int Level { get
    {
        if (card != null) return card.Level;
        return 0;
    } }
    public void DrawCards(int count)
    {
        for (int i = 0; i < count; i++) { AddToHand(deck.DrawCard()); }
    }

    private void AddToHand(Card card) { hand.Add(card); }
    public void RemoveFromHand(Card card) { hand.Remove(card); }
}
