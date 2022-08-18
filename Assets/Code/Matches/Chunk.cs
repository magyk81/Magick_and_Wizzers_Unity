/* Copyright (C) All Rights Reserved
 * Unauthorized copying of this file, via any medium is prohibited.
 * Proprietary and confidential.
 * Written by Robin Campos <magyk81@gmail.com>, year 2021.
 */
using System.Collections.Generic;

namespace Matches {
    public class Chunk {
        public static int SIZE = 10;
        private readonly Coord mPos, mMinTile, mMaxTile;
        private readonly Chunk[] neighbors;
        private readonly List<Piece> pieces;

        public Chunk(Coord pos) {
            mPos = pos;
            mMinTile = pos * SIZE;
            mMaxTile = pos * (SIZE + 1);

            neighbors = new Chunk[9];
            pieces = new List<Piece>();
        }

        public bool InPiecesBounds(PiecePos pos) {
            foreach (Piece piece in pieces) {
                if (piece.InBounds(pos)) return true;
            }
            return false;
        }
        public void AddPiece(Piece piece) { pieces.Add(piece); }
        public void RemovePiece(Piece piece) { pieces.Remove(piece); }
    }
}