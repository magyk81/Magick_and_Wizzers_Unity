using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Master : Piece
{
    private int playerIdx;

    public Master(Player player, int playerIdx, int boardIdx, Coord initPos,
        Texture art)
        : base(player.Name, playerIdx, boardIdx, initPos, art)
    {
        pieceType = Type.MASTER;

        // Debug-deck
        deck = new Deck(Card.friend_cards);
        deck.Shuffle();
    }

    public override int Level { get { return 10; } }
}
