using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece
{
    public struct PosPrecise
    {
        public Coord To { get; }
        public Coord From { get; }
        public float Lerp { get; }
        private PosPrecise(Coord to, Coord from, float lerp)
        {
            To = to;
            From = from;
            Lerp = lerp;
        }
        public static PosPrecise _(Coord init)
        {
            return new PosPrecise(init, init, 0);
        }
        public PosPrecise Dir(int dir)
        {
            Coord to = From.Dir(dir);
            if (to == To) return this;
            return new PosPrecise(to, From, 0);
        }
    }
    // private UX_Piece[] ux = new UX_Piece[9];
    // public void SetUX(UX_Piece ux, int cloneIdx)
    // {
    //     this.ux[cloneIdx] = ux;
    // }
    private string name;
    public string Name { get { return name; } }
    public enum Type { MASTER, CREATURE, ITEM, CHARM }
    public Type pieceType;
    private int playerIdx, boardIdx;
    public int PlayerIdx { get { return playerIdx; } }
    public int BoardIdx { get { return boardIdx; } }
    private Coord pos;
    public Coord Pos { get { return pos; } }
    private PosPrecise posPrec;
    public PosPrecise PosPrec { get { return posPrec; } }
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
    private Waypoint[] waypoints;
    public static readonly int MAX_WAYPOINTS = 5;
    public Piece(string name, int playerIdx, int boardIdx, Coord initPos,
        Card card)
    {
        this.name = name;
        this.playerIdx = playerIdx;
        this.boardIdx = boardIdx;
        pos = initPos.Copy();
        posPrec = PosPrecise._(initPos);
        this.card = card;
        
        waypoints = new Waypoint[MAX_WAYPOINTS];
        for (int i = 0; i < MAX_WAYPOINTS; i++)
        {
            waypoints[i] = new Waypoint();
        }
    }
    public Piece(string name, int playerIdx, int boardIdx, Coord initPos,
        Texture art)
    {
        this.name = name;
        this.playerIdx = playerIdx;
        pos = initPos.Copy();
        posPrec = PosPrecise._(initPos);
        this.art = art;

        waypoints = new Waypoint[MAX_WAYPOINTS];
        for (int i = 0; i < MAX_WAYPOINTS; i++)
        {
            waypoints[i] = new Waypoint();
        }
    }

    // dist goes from 0 to 100
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
                break;
            }
        }
        Match.AddSkinTicket(new SkinTicket(
            this, tile, SkinTicket.Type.ADD_WAYPOINT));
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
    public Card GetCardFromHand(int idx)
    {
        return hand[idx];
    }
    private void AddToHand(Card card) { hand.Add(card); }

    // public void UX_Move(UX_Tile[,] uxTiles, int cloneIdx)
    // {
    //     ux[cloneIdx].SetPos(
    //         uxTiles[posFrom.X, posFrom.Z],
    //         uxTiles[posTo.X, posTo.Z],
    //         posLerp);
    // }
}
