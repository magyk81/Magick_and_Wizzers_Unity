using UnityEngine;

namespace Matches {
    public struct PiecePos {
        private const int LERP_MAX = 1000;

        private readonly Coord mPrev, mNext, mPrevBound, mNextBound;
        private readonly int mDirNext, mLerpDist, mSize, mBoardSize;

        public Coord Pos { get => (mLerpDist < LERP_MAX / 2) ? mPrev : mNext; }
        public Coord Bound { get => (mLerpDist < LERP_MAX / 2) ? mPrevBound : mNextBound; }
        public bool LoopsX { get => Pos.X > Bound.X; }
        public bool LoopsZ { get => Pos.Z > Bound.Z; }
        public int BoardSize { get => mBoardSize; }

        private PiecePos(Coord prev, int size, int boardSize, int dirNext = -1, int lerpDist = 0) {

            mPrev = prev.ToBounds(boardSize);
            mNext = mPrev.Dir(dirNext).ToBounds(boardSize);
            mDirNext = dirNext;
            mLerpDist = lerpDist;

            mSize = size;
            mBoardSize = boardSize;
            mPrevBound = Coord._(mPrev.X + size, mPrev.Z + size).ToBounds(boardSize);
            mNextBound = Coord._(mNext.X + size, mNext.Z + size).ToBounds(boardSize);
        }

        public PiecePos Travel(int speed, int dirLater) {
            int newLerpDist = speed + mLerpDist;
            if (newLerpDist >= LERP_MAX) {
                return new PiecePos(
                    mNext.ToBounds(mBoardSize),
                    mSize,
                    mBoardSize,
                    dirLater,
                    Mathf.Clamp(newLerpDist - LERP_MAX, 0, LERP_MAX - 1)
                );
            }
            return new PiecePos(mPrev, mSize, mBoardSize, mDirNext, newLerpDist);
        }

        public static PiecePos _(Coord coord, int size, int boardSize) {
            return new PiecePos(coord.Copy(), size, boardSize);
        }

        public static PiecePos _(params int[] vals) {
            return new PiecePos(Coord._(vals[0], vals[1]), vals[2], vals[3], vals[4], vals[5]);
        }
    }
}