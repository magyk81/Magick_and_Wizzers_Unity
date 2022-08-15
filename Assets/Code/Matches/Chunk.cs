/* Copyright (C) All Rights Reserved
 * Unauthorized copying of this file, via any medium is prohibited.
 * Proprietary and confidential.
 * Written by Robin Campos <magyk81@gmail.com>, year 2021.
 */

namespace Matches {
    public class Chunk {
        public static int SIZE = 10;
        private readonly Coord mPos, mMinTile, mMaxTile;

        public Chunk(Coord pos) {
            mPos = pos;
            mMinTile = pos * SIZE;
            mMaxTile = pos * (SIZE + 1);
        }
    }
}