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
    public readonly int ID;
    private static int ID_count = 0;

    private Coord pos;
    public Coord Pos {
        get { return pos; }
        set { pos = value; }
    }

    private string name;
    public string Name { get { return name; } }
    public enum Type { MASTER, CREATURE, ITEM, CHARM }
    public Type pieceType;
    private int playerIdx, boardIdx, boardTotalSize;
    public int PlayerIdx { get { return playerIdx; } }
    public int BoardIdx { get { return boardIdx; } }
    public int BoardTotalSize { set { boardTotalSize = value; } }

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
        ID = ++ID_count;

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
        ID = ++ID_count;

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

        // Match.AddSkinTicket(new Signal(
        //     this, waypointTiles, Signal.Type.UPDATE_WAYPOINTS));
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
