/* Copyright (C) All Rights Reserved
 * Unauthorized copying of this file, via any medium is prohibited.
 * Proprietary and confidential.
 * Written by Robin Campos <magyk81@gmail.com>, year 2021.
 */

using UnityEngine;
using Matches.Cards;

namespace Matches {
    public class Master : Piece {
        private readonly Texture mArt;

        public Master(Player player, int boardID, int boardSize, Coord tile, Texture art)
            : base(player.ID, boardID, boardSize, tile, null) {

            mName = "Master - " + player.Name;
            mPieceType = Type.MASTER;
            mArt = art;

            // For debugging.
            mDeck = new Deck(Card.friend_cards);
            mDeck.Shuffle();
        }

        public override Texture Art { get => mArt; }
        public override int Level { get { return 10; } }
    }
}