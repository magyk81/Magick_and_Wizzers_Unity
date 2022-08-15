using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Matches.UX.Waypoints {
    public class WaypointTrail {
        private Matches.Waypoints.Waypoint[] mWaypoints;
        private int mIdxMax = 0;
        private int mIdx = 0;
        private bool mFull = false;
        public Matches.Waypoints.Waypoint[] Waypoints {
            get => mWaypoints;
            set {
                mWaypoints = value;
                mFull = (mWaypoints[Matches.Piece.MAX_WAYPOINTS - 1] != null);
                bool wasAtMax = mIdx == mIdxMax;
                if (mFull) mIdxMax = Matches.Piece.MAX_WAYPOINTS - 1;
                else {
                    for (int i = 0; i < mWaypoints.Length; i++) {
                        if (mWaypoints[i] == null) { mIdxMax = i; break; }
                    }
                }
                if (wasAtMax) mIdx = mIdxMax;
            }
        }
        public int Idx {
            get => mIdx;
            set {
                if (value > mIdxMax) mIdx = mIdxMax;
                else if (value < 0) mIdx = 0;
                else mIdx = value;
            }
        }

        public WaypointTrail() { mWaypoints = new Matches.Waypoints.Waypoint[Matches.Piece.MAX_WAYPOINTS]; }
            
        public bool AtMax() { return mIdxMax == mIdx && !mFull; }
        public bool NextToMax() { return mIdxMax == mIdx + 1 || mFull; }
        // public void Update(UX_Tile[] tiles, UX_Piece[] pieces) {
        //     int idxMaxPrev = mIdxMax;
        //     mIdxMax = 1;
        //     for (int i = 0; i < mWaypointTiles.Length; i++) {
        //         mWaypointTiles[i] = tiles[i];
        //         if (i > 0 && tiles[i] != null) mIdxMax = i + 1;
        //     }
        //     for (int i = 0; i < mWaypointPieces.Length; i++) {
        //         mWaypointPieces[i] = pieces[i];
        //         if (i > 0 && pieces[i] != null && i >= mIdxMax) mIdxMax = i + 1;
        //     }
        //     if (mIdxMax >= Piece.MAX_WAYPOINTS) {
        //         mIdxMax = Piece.MAX_WAYPOINTS - 1;
        //         mFull = true;
        //     } else mFull = false;
        //     // The setter for mIdx keeps it in bounds.
        //     Idx += mIdxMax - idxMaxPrev;
        // }
        
        // SetTile and SetPiece only used by group waypoints, so assume mIdxMax is not relevant.
        public void SetTile(Tile tile, int waypointIdx) { mWaypoints[waypointIdx] = new WaypointTile(tile); }
        public void SetPiece(Piece piece, int waypointIdx) { mWaypoints[waypointIdx] = new WaypointPiece(piece); }
        public void Clear() {
            for (int i = 0; i < mWaypoints.Length; i++) { mWaypoints[i] = null; }
            Idx = 0;
        }
    }
}