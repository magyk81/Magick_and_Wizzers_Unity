/* Copyright (C) All Rights Reserved
 * Unauthorized copying of this file, via any medium is prohibited.
 * Proprietary and confidential.
 * Written by Robin Campos <magyk81@gmail.com>, year 2021.
 */

using UnityEngine;

public class UX_Tile {
    private readonly Coord mPos;
    private readonly Vector3 mUxPos;
    private Vector3[] uxPosAll = null;
    private UX_Tile real = null;
    private readonly int mBoardID;

    public readonly static int LAYER = 10;
    private readonly static float LIFT_DIST = 0.05F;

    public UX_Tile(Coord pos, int boardTotalSize, int apartOffset, int cloneIdx, int boardID) {
        mPos = pos.Copy();
        mBoardID = boardID;

        // Set physical position.
        float x = pos.X + 0.5F + apartOffset, z = pos.Z + 0.5F;

        if (cloneIdx > 0) {
            if (cloneIdx == Util.UP || cloneIdx == Util.UP_RIGHT || cloneIdx == Util.UP_LEFT)
                z += boardTotalSize;
            if (cloneIdx == Util.DOWN || cloneIdx == Util.DOWN_RIGHT || cloneIdx == Util.DOWN_LEFT)
                z -= boardTotalSize;
            if (cloneIdx == Util.RIGHT || cloneIdx == Util.UP_RIGHT || cloneIdx == Util.DOWN_RIGHT)
                x += boardTotalSize;
            if (cloneIdx == Util.LEFT || cloneIdx == Util.UP_LEFT || cloneIdx == Util.DOWN_LEFT)
                x -= boardTotalSize;
        } else {
            uxPosAll = new Vector3[9];
            uxPosAll[0] = new Vector3(x, LIFT_DIST, z);
        }
        
        mUxPos = new Vector3(x, 0, z);
    }

    public Coord Pos { get { return mPos; } }
    public Vector3 UX_Pos { get { return mUxPos; } }
    public Vector3[] UX_PosAll { get => (uxPosAll != null) ? uxPosAll : real.uxPosAll; }
    public int BoardID { get { return mBoardID; } }

    /// <summary>
    /// Called 8 times before the match begins: once for each clone needed.
    /// </summary>
    public void SetClone(UX_Tile tileClone, int cloneIdx) {
        tileClone.real = this;

        uxPosAll[cloneIdx] = new Vector3(tileClone.mUxPos.x, LIFT_DIST, tileClone.mUxPos.z);
    }

    public static implicit operator Coord(UX_Tile t) => t.mPos.Copy();
}
