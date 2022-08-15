using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Matches {
    public struct PiecePos {
        public Coord Prev { get; }
        public Coord Next { get; }
        public int LerpDist { get; }

        private PiecePos(Coord prev, Coord next, int lerpDist) {
            Prev = prev;
            Next = next;
            LerpDist = lerpDist;
        }

        public static PiecePos _(params int[] vals) {
            return new PiecePos(Coord._(vals[0], vals[1]), Coord._(vals[2], vals[3]), vals[4]);
        }
    }
}