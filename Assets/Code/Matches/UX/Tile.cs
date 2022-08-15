/* Copyright (C) All Rights Reserved
 * Unauthorized copying of this file, via any medium is prohibited.
 * Proprietary and confidential.
 * Written by Robin Campos <magyk81@gmail.com>, year 2021.
 */

using UnityEngine;

namespace Matches.UX {
    public class Tile {
        public readonly static float LIFT_DIST = 0.05F;

        private readonly Coord mPos;
        private readonly Vector3 mUxPos;
        private Vector3[] mUxPosAll = null;
        private Tile mReal = null;
        private readonly int mBoardID;

        public readonly static int LAYER = 10;

        public Tile(Coord pos, int boardTotalSize, int apartOffset, int cloneIdx, int boardID) {
            mPos = pos.Copy();
            mBoardID = boardID;

            // Set physical position.
            float x = pos.X + 0.5F + apartOffset, z = pos.Z + 0.5F;

            if (cloneIdx > 0) {
                if (cloneIdx == Util.UP + 1 || cloneIdx == Util.UP_RIGHT + 1 || cloneIdx == Util.UP_LEFT + 1)
                    z += boardTotalSize;
                if (cloneIdx == Util.DOWN + 1 || cloneIdx == Util.DOWN_RIGHT + 1 || cloneIdx == Util.DOWN_LEFT + 1)
                    z -= boardTotalSize;
                if (cloneIdx == Util.RIGHT + 1 || cloneIdx == Util.UP_RIGHT + 1 || cloneIdx == Util.DOWN_RIGHT + 1)
                    x += boardTotalSize;
                if (cloneIdx == Util.LEFT + 1 || cloneIdx == Util.UP_LEFT + 1 || cloneIdx == Util.DOWN_LEFT + 1)
                    x -= boardTotalSize;
            } else {
                mUxPosAll = new Vector3[9];
                mUxPosAll[0] = new Vector3(x, LIFT_DIST, z);
            }
            
            mUxPos = new Vector3(x, 0, z);
        }

        public Coord Pos { get { return mPos; } }
        public Vector3 UX_Pos { get { return mUxPos; } }
        public Vector3[] UX_PosAll { get => (mUxPosAll != null) ? mUxPosAll : mReal.mUxPosAll; }
        public int BoardID { get { return mBoardID; } }

        /// <summary>
        /// Called 8 times before the match begins: once for each clone needed.
        /// </summary>
        public void SetClone(Tile tileClone, int cloneIdx) {
            tileClone.mReal = this;

            mUxPosAll[cloneIdx] = new Vector3(tileClone.mUxPos.x, LIFT_DIST, tileClone.mUxPos.z);
        }

        public override string ToString() { return Pos.ToString(); }

        public static implicit operator Coord(Tile t) => t.mPos.Copy();
    }
}