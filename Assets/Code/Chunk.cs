/* Copyright (C) All Rights Reserved
 * Unauthorized copying of this file, via any medium is prohibited.
 * Proprietary and confidential.
 * Written by Robin Campos <magyk81@gmail.com>, year 2021.
 */

public class Chunk {
    private readonly int mSize;
    private readonly Coord mPos, mMinTile, mMaxTile;

    public Chunk(Coord pos, int size) {
        mPos = pos;
        mSize = size;
        mMinTile = pos * size;
        mMaxTile = pos * (size + 1);
    }

    public int Size { get => mSize; }
}
