using UnityEngine;

namespace Matches {
    public struct PiecePos {
        private const int LERP_MAX = 1000, LERP_MAX_DIAG = (int) (LERP_MAX * 1.414F);

        private readonly Coord mPrev, mNext, mPos, mPrevBound, mNextBound, mBound;
        private readonly int mDirNext, mLerpDist, mSize, mBoardSize;
        private readonly bool mHalfway;

        public Coord Pos { get => mPos; }
        public Coord Bound { get => mBound; }
        public bool LoopsX { get => Pos.X > Bound.X; }
        public bool LoopsZ { get => Pos.Z > Bound.Z; }
        public int BoardSize { get => mBoardSize; }

        // mSize value here gets replaced in Piece::GetPosData with Piece.Size enum value.
        public int[] Data { get => new int[] { mPrev.X, mPrev.Z, mSize, mDirNext, mLerpDist }; }

        private PiecePos(Coord prev, int size, int boardSize, int dirNext = -1, int lerpDist = 0) {
            mPrev = prev.ToBounds(boardSize);
            mNext = mPrev.Dir(dirNext).ToBounds(boardSize);
            mHalfway = lerpDist >= GetLerpMax(dirNext) / 2;
            mPos = !mHalfway ? mPrev : mNext;
            mDirNext = dirNext;
            mLerpDist = lerpDist;

            mSize = size;
            mBoardSize = boardSize;
            mPrevBound = Coord._(mPrev.X + size, mPrev.Z + size).ToBounds(boardSize);
            mNextBound = Coord._(mNext.X + size, mNext.Z + size).ToBounds(boardSize);
            mBound = (lerpDist < GetLerpMax(dirNext) / 2) ? mPrevBound : mNextBound;
        }

        public PiecePos Travel(int speed, int dirLater, out bool completedTravel) {
            // No longer moving.
            if (dirLater == -1) {
                completedTravel = false;
                if (mNext == mPrev) return this;
                Debug.Log("Stopped moving");
                return _(mPrev, mSize, mBoardSize);
            }

            // dirLater did not change, so continue in the same direction.
            if (dirLater == mDirNext) {
                int newLerpDist = speed + mLerpDist;
                int lerpMax = GetLerpMax(dirLater);
                if (newLerpDist >= lerpMax) {
                    completedTravel = true; Debug.Log("claimed new tile");
                    return new PiecePos(
                        mNext.ToBounds(mBoardSize),
                        mSize,
                        mBoardSize,
                        dirLater,
                        Mathf.Clamp(newLerpDist - lerpMax, 0, lerpMax - 1)
                    );
                }

                completedTravel = false;
                return new PiecePos(mPrev, mSize, mBoardSize, dirLater, newLerpDist);

            // dirLater changed while not stationary.
            } else if (mLerpDist > 0) {
                int lerpMax = GetLerpMax(mDirNext);
                // Made it half-way, so continue in the same direction.
                if (mHalfway) {
                    int newLerpDist = speed + mLerpDist;
                    if (newLerpDist >= lerpMax) {
                        completedTravel = true; Debug.Log("claimed new tile");
                        return new PiecePos(
                            mNext.ToBounds(mBoardSize),
                            mSize,
                            mBoardSize,
                            mDirNext,
                            Mathf.Clamp(newLerpDist - lerpMax, 0, lerpMax - 1)
                        );
                    }

                    completedTravel = false;
                    return new PiecePos(mPrev, mSize, mBoardSize, mDirNext, newLerpDist);

                // Did not make it half-way, so go in the reverse direction.
                } else {
                    int newLerpDist = mLerpDist - speed;
                    if (newLerpDist <= 0) {
                        completedTravel = true; Debug.Log("back at old tile");
                        return new PiecePos(
                            mPrev,
                            mSize,
                            mBoardSize
                        );
                    }

                    completedTravel = false;
                    return new PiecePos(mPrev, mSize, mBoardSize, mDirNext, newLerpDist);
                }
                
            }

            // dirLater changed while stationary, so start from mLerpDist 0.
            completedTravel = false;
            return new PiecePos(mPrev, mSize, mBoardSize, dirLater, speed);
        }

        public static PiecePos _(Coord coord, int size, int boardSize) {
            return new PiecePos(coord.Copy(), size, boardSize);
        }

        public static int GetLerpMax(int dirLater) { return (dirLater < Util.UP_RIGHT) ? LERP_MAX : LERP_MAX_DIAG; }

        public static bool operator ==(PiecePos a, PiecePos b) {
            return a.mPrev == b.mPrev && a.mSize == b.mSize && a.mDirNext == b.mDirNext && a.mLerpDist == b.mLerpDist;
        }
        public static bool operator !=(PiecePos a, PiecePos b) {
            return a.mPrev != b.mPrev || a.mSize != b.mSize || a.mDirNext != b.mDirNext || a.mLerpDist != b.mLerpDist;
        }

        public override bool Equals(object obj) {
            return base.Equals(obj);
        }
        public override int GetHashCode() {
            return base.GetHashCode();
        }
    }
}