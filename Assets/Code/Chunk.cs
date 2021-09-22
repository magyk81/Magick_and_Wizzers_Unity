using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk
{
    private static class InitInfo
    {
        public static int size;
    }
    public static int Size {
        set { InitInfo.size = value; } get { return InitInfo.size; } }
    
    private UX_Chunk[] ux = new UX_Chunk[9];
    public void SetUX(UX_Chunk ux, int cloneIdx)
    {
        this.ux[cloneIdx] = ux;
    }

    private Coord pos, minTile, maxTile;

    public Chunk(Coord pos)
    {
        this.pos = pos;
        minTile = pos * InitInfo.size;
        maxTile = pos * (InitInfo.size + 1);
    }
}
