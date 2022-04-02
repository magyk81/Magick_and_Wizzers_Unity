/* Copyright (C) All Rights Reserved
 * Unauthorized copying of this file, via any medium is prohibited.
 * Proprietary and confidential.
 * Written by Robin Campos <magyk81@gmail.com>, year 2021.
 */

using System.Collections.Generic;
using UnityEngine;

public class Piece {
    public readonly int ID;

    protected Deck mDeck;
    protected Type mPieceType = Type.CREATURE;
    protected string mName;   

    private Coord mPos; 
    private readonly int mPlayerID, mBoardID;
    private readonly Card mCard;
    private readonly List<Card> mHand = new List<Card>();
    private readonly Waypoint[] mWaypoints = new Waypoint[MAX_WAYPOINTS];
    // public Waypoint NextWaypoint { get { return waypoints[0]; } }

    public static readonly int MAX_WAYPOINTS = 5;

    public enum Type { MASTER, CREATURE, ITEM, CHARM }

    // Non-master type piece
    public Piece(int playerID, int boardID, Coord tile, Card card) {
        ID = IdHandler.Create(GetType());

        if (card != null) mName = card.Name;
        mPlayerID = playerID;
        mBoardID = boardID;
        mPos = tile.Copy();
        mCard = card;
    }

    public Coord Pos { get => mPos; set { mPos = value; } }
    public string Name { get => mName; }
    public Type PieceType { get => mPieceType; }
    public int PlayerID { get => mPlayerID; }
    public int BoardID { get => mBoardID; }
    public Card Card { get => mCard; }
    public Card[] Hand { get => mHand.ToArray(); }
    public virtual Texture Art { get => mCard.Art; }
    public virtual int Level { get => mCard.Level; }

    public void Update() {

    }

    /// <param name="dist">Goes from 0 to 100.</param>
    public void Move(int dir, int dist) {

    }

    public void AddWaypoint(Coord tile, int orderPlace) {
        mWaypoints[orderPlace] = new WaypointTile(tile);
        ArrangeWaypointOrder();
    }
    public void AddWaypoint(Piece piece, int orderPlace) {
        mWaypoints[orderPlace] = new WaypointPiece(piece);
        ArrangeWaypointOrder();
    }
    public void RemoveWaypoint(int orderPlace) {
        mWaypoints[orderPlace] = null;
        ArrangeWaypointOrder();
    }
    public void ClearWaypoints() {
        for (int i = 0; i < mWaypoints.Length; i++) {
            mWaypoints[i] = null;
        }
    }

    public int[] GetWaypointData()
    {
        int[] data = new int[2 * MAX_WAYPOINTS];
        for (int i = 0; i < data.Length; i += 2) {
            Waypoint waypoint = mWaypoints[i / 2];
            if (waypoint == null) { // No waypoint.
                data[i] = -1;
                data[i + 1] = -1;
            } else if (waypoint is WaypointPiece) { // Waypoint on piece.
                data[i] = -1;
                data[i + 1] = (waypoint as WaypointPiece).Piece.ID;
            } else { // Waypoint on tile.
                Coord tile = (waypoint as WaypointTile).Tile;
                data[i] = tile.X;
                data[i + 1] = tile.Z;
            }
        }
        return data;
    }

    public bool HasSameWaypoints(Piece piece)
    {
        for (int i = 0; i < mWaypoints.Length; i++) {
            if (mWaypoints[i] != piece.mWaypoints[i]) return false;
        }
        return true;
    }

    public SignalFromHost[] DrawCards(int count) {
        SignalFromHost[] signals = new SignalFromHost[count];
        for (int i = 0; i < count; i++)
        { signals[i] = AddToHand(mDeck.DrawCard()); }
        return signals;
    }

    public void RemoveFromHand(Card card) { mHand.Remove(card); }

    /// <returns>
    /// SignalRemoveCardd object if the card get successfully removed from the holder piece's hand.
    /// </returns>
    public SignalRemoveCard CastSpell(Card card) {
        // Skipped a lot of steps here.

        /* Remove function returns true if removal was successful, false otherwise. Signals the board to resolve
         * the spell if a SignalRemoveCard gets returned. */
        if (mHand.Remove(card)) { return new SignalRemoveCard(this, card); }
        return null;
    }

    /// <summary>
    /// Move all items in the array to the left, removing any gaps.
    /// </summary>
    private void ArrangeWaypointOrder() {
        Queue<int> emptySlots = new Queue<int>();
        for (int i = 0; i < mWaypoints.Length - 1; i++) {
            if (mWaypoints[i] == null) emptySlots.Enqueue(i);
            else if (emptySlots.Count > 0) {
                mWaypoints[emptySlots.Dequeue()] = mWaypoints[i];
                mWaypoints[i] = null;
                emptySlots.Enqueue(i);
            }
        }

        string _ = "";
        for (int i = 0; i < Piece.MAX_WAYPOINTS; i++) {
            _ += mWaypoints[i].ToString() + ", ";
        }
        Debug.Log(_);
    }
    
    private SignalFromHost AddToHand(Card card) {
        mHand.Add(card);
        return new SignalAddCard(this, card);
    }
}
