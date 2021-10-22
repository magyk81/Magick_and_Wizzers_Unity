/* Copyright (C) All Rights Reserved
 * Unauthorized copying of this file, via any medium is prohibited.
 * Proprietary and confidential.
 * Written by Robin Campos <magyk81@gmail.com>, year 2021.
 */

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

    private Coord pos, minTile, maxTile;

    public Chunk(Coord pos)
    {
        this.pos = pos;
        minTile = pos * InitInfo.size;
        maxTile = pos * (InitInfo.size + 1);
    }
}
