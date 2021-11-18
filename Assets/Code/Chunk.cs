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
    private int size;
    public int Size { get { return size; } }
    private Coord pos, minTile, maxTile;

    public Chunk(Coord pos, int size)
    {
        this.pos = pos;
        this.size = size;
        minTile = pos * size;
        maxTile = pos * (size + 1);
    }
}
