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
        private readonly Chunk[] mNeighbors;
        private readonly List<Piece> pieces;

        public Chunk[] Neighbors { get => mNeighbors; }

        public Chunk(Coord pos) {
            mPos = pos;
            mMinTile = pos * SIZE;
            mMaxTile = pos * (SIZE + 1);

            mNeighbors = new Chunk[9]; mNeighbors[Util.COUNT] = this;
            pieces = new List<Piece>();
        }

        public void AddNeighbor(Chunk neighbor, int slot) { mNeighbors[slot] = neighbor; }

        public bool InPiecesBounds(int pieceID, PiecePos pos) {
            // UnityEngine.Debug.Log("chunk mPos: " + mPos);
            foreach (Piece piece in pieces) {
                // UnityEngine.Debug.Log("piece.ID: " + piece.ID);
                if (piece.ID != pieceID && piece.Intersects(pos)) {UnityEngine.Debug.Log(piece.Name); return true;}
            }
            return false;
        }
        public void AddPiece(Piece piece) { pieces.Add(piece); }
        public void RemovePiece(Piece piece) { pieces.Remove(piece); }
    }
}