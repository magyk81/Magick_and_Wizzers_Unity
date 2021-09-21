using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UX_Tile
{
    private Coord pos;
    private int boardIdx;

    public UX_Tile(Coord pos, int boardIdx)
    {
        this.pos = pos.Copy();
        this.boardIdx = boardIdx;
    }
}
