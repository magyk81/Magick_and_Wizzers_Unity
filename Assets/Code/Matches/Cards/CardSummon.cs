/* Copyright (C) All Rights Reserved
 * Unauthorized copying of this file, via any medium is prohibited.
 * Proprietary and confidential.
 * Written by Robin Campos <magyk81@gmail.com>, year 2021.
 */

using UnityEngine;
using Matches.Terrains;

namespace Matches.Cards {
    public class CardSummon : Card {
        private readonly int mAttackPoints, mDefensePoints;
        private readonly Piece.Speed mSpeed;
        private readonly Piece.Size mSize;

        public CardSummon(string name, Texture art, int level, GameTerrain terrain,
            int attackPoints, int defensePoints, Piece.Speed speed, Piece.Size size)

            : base(name, art, level, terrain) {

            mAttackPoints = attackPoints;
            mDefensePoints = defensePoints;
            mSpeed = speed;
        }

        public Piece.Speed Speed { get => mSpeed; }
        public Piece.Size Size { get => mSize; }
    }
}