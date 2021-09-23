using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece
{
    private UX_Piece[] ux = new UX_Piece[9];
    public void SetUX(UX_Piece ux, int cloneIdx)
    {
        this.ux[cloneIdx] = ux;
    }
    private string name;
    public string Name { get { return name; } }
    public enum Type { MASTER, CREATURE, ITEM, CHARM }
    public Type pieceType;
    private int playerIdx, boardIdx;
    public int PlayerIdx { get { return playerIdx; } }
    public int BoardIdx { get { return boardIdx; } }
    private Coord pos, posTo, posFrom;
    public Coord Pos { get { return pos; } }
    private float posLerp = 0;
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
    public Piece(string name, int playerIdx, int boardIdx, Coord initPos,
        Card card)
    {
        this.name = name;
        this.playerIdx = playerIdx;
        pos = initPos.Copy();
        posFrom = initPos.Copy();
        posTo = initPos.Copy();
        this.card = card;
    }
    public Piece(string name, int playerIdx, int boardIdx, Coord initPos,
        Texture art)
    {
        this.name = name;
        this.playerIdx = playerIdx;
        pos = initPos.Copy();
        posFrom = initPos.Copy();
        posTo = initPos.Copy();
        this.art = art;
    }

    // dist goes from 0 to 100
    public void Move(int dir, int dist)
    {

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

    public void UX_Move(UX_Tile[,] uxTiles, int cloneIdx)
    {
        ux[cloneIdx].SetPos(
            uxTiles[posFrom.X, posFrom.Z],
            uxTiles[posTo.X, posTo.Z],
            posLerp);
    }
}
