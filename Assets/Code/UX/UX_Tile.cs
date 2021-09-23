using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UX_Tile
{
    private Coord pos;
    public Coord Pos { get { return pos; } }
    private Vector3 uxPos;
    public Vector3 UX_Pos { get { return uxPos; } }
    private int boardIdx;

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
        
        uxPos = new Vector3(x, 0, z);
    }
}
