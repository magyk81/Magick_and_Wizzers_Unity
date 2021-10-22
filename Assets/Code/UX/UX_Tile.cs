/* Copyright (C) All Rights Reserved
 * Unauthorized copying of this file, via any medium is prohibited.
 * Proprietary and confidential.
 * Written by Robin Campos <magyk81@gmail.com>, year 2021.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UX_Tile
{
    private Coord pos;
    public Coord Pos { get { return pos; } }
    private Vector3 uxPos;
    public Vector3 UX_Pos { get { return uxPos; } }
    private readonly static float LIFT_DIST = 0.05F;
    private Vector3[] uxPosAll = null;
    public Vector3[] UX_PosAll {
        get {
                if (uxPosAll == null) return real.uxPosAll;
                return uxPosAll;
            }
    }
    private UX_Tile real = null;
    private int boardIdx;

    public readonly static int LAYER = 10;

    public UX_Tile(Coord pos, int boardTotalSize, int apartOffset,
        int cloneIdx, int boardIdx)
    {
        this.pos = pos.Copy();
        this.boardIdx = boardIdx;

        // Set physical position.
        float x = pos.X + 0.5F + apartOffset, z = pos.Z + 0.5F;

        if (cloneIdx >= 0)
        {
            if (cloneIdx == Util.UP || cloneIdx == Util.UP_RIGHT
                || cloneIdx == Util.UP_LEFT) z += boardTotalSize;
            if (cloneIdx == Util.DOWN || cloneIdx == Util.DOWN_RIGHT
                || cloneIdx == Util.DOWN_LEFT) z -= boardTotalSize;
            if (cloneIdx == Util.RIGHT || cloneIdx == Util.UP_RIGHT
                || cloneIdx == Util.DOWN_RIGHT) x += boardTotalSize;
            if (cloneIdx == Util.LEFT || cloneIdx == Util.UP_LEFT
                || cloneIdx == Util.DOWN_LEFT) x -= boardTotalSize;
        }
        else
        {
            uxPosAll = new Vector3[9];
            uxPosAll[0] = new Vector3(x, LIFT_DIST, z);
        }
        
        uxPos = new Vector3(x, 0, z);
    }

    /// <summary>Called 8 times before the match begins: once for each clone
    /// needed.</summary>
    public void AddClone(UX_Tile tileClone, int cloneIdx)
    {
        tileClone.real = this;

        uxPosAll[cloneIdx + 1] = new Vector3(
            tileClone.uxPos.x, LIFT_DIST, tileClone.uxPos.z);
    }
}
