using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Master : Piece
{
    private int playerIdx;

    public Master(Player player, int playerIdx, int boardIdx, Coord initPos)
        : base("Master of " + player.Name, playerIdx, boardIdx, initPos)
    {        
        // Debug-deck
        deck = new Deck(Card.friend_cards);
        deck.Shuffle();
    }

    public override int Level { get { return 10; } }
}
