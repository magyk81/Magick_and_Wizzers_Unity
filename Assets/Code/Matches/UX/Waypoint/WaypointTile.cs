using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Matches.UX.Waypoints {
    public class WaypointTile : Matches.Waypoints.Waypoint {
        private readonly Tile mTile;

        public WaypointTile(UX.Tile tile) { mTile = tile; }

        public static bool operator ==(WaypointTile a, WaypointTile b) { return a.mTile == b.mTile; }
        public static bool operator !=(WaypointTile a, WaypointTile b) { return a.mTile != b.mTile; }

        public Tile Tile { get => mTile; }

        public override bool Equals(object obj) { return base.Equals(obj); }
        public override int GetHashCode() { return base.GetHashCode(); }
        public override string ToString() { return mTile.ToString(); }

        public override Matches.Waypoints.Waypoint Copy() {
            return new WaypointTile(mTile);
        }

        protected override bool Equals(Matches.Waypoints.Waypoint other) {
            if (other is WaypointTile) return mTile == (other as WaypointTile).mTile;
            return false;
        }
    }
}