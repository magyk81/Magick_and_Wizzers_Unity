/* Copyright (C) All Rights Reserved
 * Unauthorized copying of this file, via any medium is prohibited.
 * Proprietary and confidential.
 * Written by Robin Campos <magyk81@gmail.com>, year 2021.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Master : Piece
{
    private int playerIdx;

    public Master(Player player, int playerIdx, int boardIdx, Coord tile,
        Texture art)
        : base(player.Name, playerIdx, boardIdx, tile, art)
    {
        pieceType = Type.MASTER;

        // Debug-deck
        deck = new Deck(Card.friend_cards);
        deck.Shuffle();
    }

    public override int Level { get { return 10; } }
}
