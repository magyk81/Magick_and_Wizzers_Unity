using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece
{
    private string name;
    public string Name { get { return name; } }
    private int playerIdx, boardIdx;
    public int PlayerIdx { get { return playerIdx; } }
    public int BoardIdx { get { return boardIdx; } }
    private Coord pos;
    public int X { get { return pos.X; } }
    public int Z { get { return pos.Z; } }
    public Coord Pos { get { return pos; } }
    private Card card;
    public Card Card { get { return card; } }
    protected Deck deck;
    private List<Card> hand = new List<Card>();
    private bool handUpdated = false;
    public Card[] Hand { get
    {
        bool _handUpdated = handUpdated;
        handUpdated = false;
        if (_handUpdated) return hand.ToArray();
        return null;
    } }
    public Piece(string name, int playerIdx, int boardIdx, Coord initPos,
        Card card = null)
    {
        this.name = name;
        this.playerIdx = playerIdx;
        pos = initPos.Copy();
        this.card = card;
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
    private void AddToHand(Card card)
    {
        hand.Add(card);
        handUpdated = true;
    }
}
