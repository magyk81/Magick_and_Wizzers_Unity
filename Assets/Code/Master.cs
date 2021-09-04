using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Master : Piece
{
    private Deck deck;
    private List<Card> hand = new List<Card>();
    public Card[] Hand { get { return hand.ToArray(); } }
    private int playerIdx;
    private Coord pos;

    public Master(Player player, int playerIdx, int boardIdx, Coord initPos)
        : base("Master of " + player.Name, playerIdx, boardIdx, initPos)
    {
        player.AddMaster(this);
        
        // Debug-deck
        deck = new Deck(Card.friend_cards);
        deck.Shuffle();
    }

    public void DrawCards(int count)
    {
        for (int i = 0; i < count; i++) { hand.Add(deck.DrawCard()); }
    }
}
