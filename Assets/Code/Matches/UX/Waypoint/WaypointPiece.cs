using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Matches.UX.Waypoints {
    public class WaypointPiece : Matches.Waypoints.Waypoint {
        private readonly Piece mPiece;

        public WaypointPiece(Piece piece) { mPiece = piece; }

        public static bool operator ==(WaypointPiece a, WaypointPiece b) { return a.mPiece == b.mPiece; }
        public static bool operator !=(WaypointPiece a, WaypointPiece b) { return a.mPiece != b.mPiece; }

        public Piece Piece { get => mPiece; }

        public override bool Equals(object obj) { return base.Equals(obj); }
        public override int GetHashCode() { return base.GetHashCode(); }
        public override string ToString() { return mPiece.name; }

        public override Matches.Waypoints.Waypoint Copy() {
            return new WaypointPiece(mPiece);
        }

        protected override bool Equals(Matches.Waypoints.Waypoint other) {
            if (other is WaypointPiece) return mPiece == (other as WaypointPiece).mPiece;
            return false;
        }
    }
}