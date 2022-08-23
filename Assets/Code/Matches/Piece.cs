/* Copyright (C) All Rights Reserved
 * Unauthorized copying of this file, via any medium is prohibited.
 * Proprietary and confidential.
 * Written by Robin Campos <magyk81@gmail.com>, year 2021.
 */

using System.Collections.Generic;
using UnityEngine;
using Matches.Cards;
using Matches.Waypoints;
using Network;
using Network.SignalsFromHost;

namespace Matches {
    public class Piece {
        private const int MOVE_ON_WITHIN_RANGE = 3;

        public readonly int ID;

        protected Deck mDeck;
        protected Type mPieceType = Type.CREATURE;
        protected string mName;   

        private PiecePos mPos;
        private Speed mSpeed;
        private Size mSize;
        private int mDirLater = -1;

        private readonly int mPlayerID, mBoardID;
        private Chunk mChunk;
        private readonly Card mCard;
        private readonly List<Card> mHand = new List<Card>();
        private readonly Waypoint[] mWaypoints = new Waypoint[MAX_WAYPOINTS];
        // public Waypoint NextWaypoint { get { return waypoints[0]; } }

        public static readonly int MAX_WAYPOINTS = 5;

        public enum Type { MASTER, CREATURE, ITEM, CHARM }
        public enum Speed { IMMOBILE, SLOW, NORMAL, FAST, SHOOT }
        public enum Size { TINY, SMALL, MEDIUM, LARGE, HUGE, GARGANTUAN, COLOSSAL }

        public Piece(int playerID, int boardID, int boardSize, Coord tile, CardSummon card) {
            ID = IdHandler.Create(GetType());

            if (card != null) mName = card.Name;
            mPlayerID = playerID;
            mBoardID = boardID;
            mSize = (card != null) ? card.Size : Size.MEDIUM;
            mPos = PiecePos._(tile.Copy(), GetSizeInt() - 1, boardSize);
            mCard = card;
            mSpeed = (card != null) ? card.Speed : Speed.NORMAL;
        }

        public Coord Pos { get => mPos.Pos; }
        public string Name { get => mName; }
        public Type PieceType { get => mPieceType; }
        public int PlayerID { get => mPlayerID; }
        public int BoardID { get => mBoardID; }
        public Chunk Chunk {
            set {
                if (mChunk != value) {
                    if (mChunk != null) mChunk.RemovePiece(this);
                    if (value != null) value.AddPiece(this);
                    mChunk = value;
                }
            }
        }
        public Card Card { get => mCard; }
        public Card[] Hand { get => mHand.ToArray(); }
        public virtual Texture Art { get => mCard.Art; }
        public virtual int Level { get => mCard.Level; }

        public Waypoint[] Waypoints {
            set {
                for (int i = 0; i < MAX_WAYPOINTS; i++) {
                    if (i < value.Length) mWaypoints[i] = value[i];
                    else mWaypoints[i] = null;
                }
            }
        }

        public SignalFromHost[] Update() {
            List<SignalFromHost> outcomes = new List<SignalFromHost>();

            // Save the old spot before travelling.
            PiecePos oldPos = mPos;

            // completedTravel set to true if it finishes claiming a new tile after this travel.
            bool completedTravel;
            PiecePos newPos = mPos.Travel(GetSpeedInt(), mDirLater, out completedTravel);

            // overlap gets set to true if there is another piece in the way of travelling in this direction.
            bool overlap = false;
            if (newPos.Pos != mPos.Pos) {
                foreach (Chunk chunk in mChunk.Neighbors) {
                    if (chunk.InPiecesBounds(ID, mPos)) {
                        overlap = true; Debug.Log("overlap == true");
                        break;
                    }
                }
                if (!overlap) Debug.Log("oldPos: " + oldPos.Pos + ", newPos: " + newPos.Pos);
            }
            if (!overlap) mPos = newPos;
            else mPos = oldPos;

            // Check if target piece is dead or invisible to update the waypoints.

            // Claimed a new tile.
            if (completedTravel) {
                bool targetPosUpdated = false;

                if (mWaypoints[0] != null) {
                    if (mWaypoints[0] is WaypointPiece) {
                        // New tile is within range of the target waypoint piece.
                        if ((mWaypoints[0] as WaypointPiece).Piece.IsWithinRange(mPos, 1)) {
                            // Attack or use ability on target piece.
                        }
                    } else {
                        // New tile is within range of the target waypoint tile.
                        if (overlap && IsWithinRange((mWaypoints[0] as WaypointTile).Tile, MOVE_ON_WITHIN_RANGE)
                            || IsWithinRange((mWaypoints[0] as WaypointTile).Tile, 1)) {

                            RemoveWaypoint(0);
                            targetPosUpdated = true;
                            outcomes.Add(new SignalUpdateWaypoints(this));
                        }
                    }
                }

                if (!targetPosUpdated) UpdateWaypointTargetPos();
            }

            return outcomes.ToArray();
        }

        public void AddWaypoint(Coord tile, int orderPlace) {
            mWaypoints[orderPlace] = new WaypointTile(tile);
            ArrangeWaypointOrder();
        }
        public void AddWaypoint(Piece piece, int orderPlace) {
            mWaypoints[orderPlace] = new WaypointPiece(piece);
            ArrangeWaypointOrder();
        }
        public void RemoveWaypoint(int orderPlace) {
            mWaypoints[orderPlace] = null;
            ArrangeWaypointOrder();
        }
        public void ClearWaypoints() {
            for (int i = 0; i < mWaypoints.Length; i++) {
                mWaypoints[i] = null;
            }

            // Update dirLater.
            UpdateWaypointTargetPos();
        }

        public int[] GetWaypointData()
        {
            int[] data = new int[2 * MAX_WAYPOINTS];
            for (int i = 0; i < data.Length; i += 2) {
                Waypoint waypoint = mWaypoints[i / 2];
                if (waypoint == null) { // No waypoint.
                    data[i] = -1;
                    data[i + 1] = -1;
                } else if (waypoint is WaypointPiece) { // Waypoint on piece.
                    data[i] = -1;
                    data[i + 1] = (waypoint as WaypointPiece).Piece.ID;
                } else { // Waypoint on tile.
                    Coord tile = (waypoint as WaypointTile).Tile;
                    data[i] = tile.X;
                    data[i + 1] = tile.Z;
                }
            }
            return data;
        }

        public bool HasSameWaypoints(Piece piece)
        {
            for (int i = 0; i < mWaypoints.Length; i++) {
                if (mWaypoints[i] != piece.mWaypoints[i]) return false;
            }
            return true;
        }

        public bool Intersects(PiecePos pos) {
            bool inX = false, inZ = false;

            if (mPos.LoopsX && pos.LoopsX)  inX = true;
            else if (mPos.LoopsX) {
                if (pos.Pos.X <= mPos.Bound.X || pos.Bound.X >= mPos.Pos.X) inX = true;
            } else if (pos.LoopsX) {
                if (mPos.Pos.X <= pos.Bound.X || mPos.Bound.X >= pos.Pos.X) inX = true;
            } else {
                if (pos.Bound.X >= Pos.X && pos.Pos.X <= mPos.Bound.X) inX = true;
            }

            if (mPos.LoopsZ && pos.LoopsZ) inZ = true;
            else if (mPos.LoopsZ) {
                if (pos.Pos.Z <= mPos.Bound.Z || pos.Bound.Z >= mPos.Pos.Z) inZ = true;
            } else if (pos.LoopsZ) {
                if (mPos.Pos.Z <= pos.Bound.Z || mPos.Bound.Z >= pos.Pos.Z) inZ = true;
            } else {
                if (pos.Bound.Z >= Pos.Z && pos.Pos.Z <= mPos.Bound.Z) inZ = true;
            }

            return inX && inZ;
        }

        public bool IsWithinRange(PiecePos pos, int dist) {
            // if (dest.X == 0 && pos.X ==)
            return false;
        }

        public bool IsWithinRange(Coord tile, int dist) {
            int distX = 0, distZ = 0;

            if (mPos.LoopsX) {
                distX = Mathf.Min(Pos.X - tile.X, tile.X - mPos.Bound.X);
            } else {
                if (tile.X < Pos.X)
                    distX = Mathf.Min(Pos.X - tile.X, tile.X + mPos.BoardSize - mPos.Bound.X);
                else if (tile.X > mPos.Bound.X)
                    distX = Mathf.Min(tile.X - mPos.Bound.X, mPos.BoardSize - tile.X + Pos.X);
            }

            if (mPos.LoopsZ) {
                distZ = Mathf.Min(Pos.Z - tile.Z, tile.Z - mPos.Bound.Z);
            } else {
                if (tile.Z < Pos.Z)
                    distZ = Mathf.Min(Pos.Z - tile.Z, tile.Z + mPos.BoardSize - mPos.Bound.Z);
                else if (tile.Z > mPos.Bound.Z)
                    distZ = Mathf.Min(tile.Z - mPos.Bound.Z, mPos.BoardSize - tile.Z + Pos.Z);
            }

            return distX + distZ <= dist;
        }

        public SignalFromHost DrawCards(int count) {
            return AddToHand(mDeck.DrawCards(count));
        }

        public void RemoveFromHand(Card card) { mHand.Remove(card); }

        /// <returns>
        /// SignalRemoveCardd object if the card get successfully removed from the holder piece's hand.
        /// </returns>
        public SignalRemoveCards CastSpell(Card card) {
            // Skipped a lot of steps here.

            /* Remove function returns true if removal was successful, false otherwise. Signals the board to resolve
            * the spell if a SignalRemoveCard gets returned. */
            if (mHand.Remove(card)) { return new SignalRemoveCards(this, card); }
            return null;
        }

        /// <summary>
        /// Move all items in the array to the left, removing any gaps.
        /// </summary>
        private void ArrangeWaypointOrder() {
            // Automatically resolve a waypoint if it's first and occupying this piece.
            if (mWaypoints[0] != null && AutoResolveWaypoint(mWaypoints[0])) mWaypoints[0] = null;

            Queue<int> emptySlots = new Queue<int>();
            for (int i = 0; i < mWaypoints.Length - 1; i++) {
                if (mWaypoints[i] == null) emptySlots.Enqueue(i);
                else if (emptySlots.Count > 0) {

                    // Automatically resolve a waypoint if it's first and occupying this piece.
                    if (emptySlots.Peek() != 0 || !AutoResolveWaypoint(mWaypoints[i]))
                        mWaypoints[emptySlots.Dequeue()] = mWaypoints[i];

                    mWaypoints[i] = null;
                    emptySlots.Enqueue(i);
                }
            }

            string _ = "";
            for (int i = 0; i < Piece.MAX_WAYPOINTS; i++) {
                if (mWaypoints[i] == null) _ += "[ , ], ";
                else _ += mWaypoints[i].ToString() + ", ";
            }
            Debug.Log(_);

            // Update dirLater.
            UpdateWaypointTargetPos();
        }

        private bool AutoResolveWaypoint(Waypoint waypoint) {
            if (waypoint is WaypointTile) {
                if (IsWithinRange((waypoint as WaypointTile).Tile, 0)) {
                    Debug.Log("Resolving waypoint: " + (waypoint as WaypointTile).Tile);
                    // Resolve it.
                } else return false;
            } else {
                if (IsWithinRange((waypoint as WaypointPiece).Piece.Pos, 0)) {
                    // Resolve it.
                    Debug.Log("Resolving waypoint: " + (waypoint as WaypointPiece).Piece.mName);
                } else return false;
            }
            return true;
        }

        /// <remarks>
        /// Call this whenever the waypoints are updated and when a new tile is claimed.
        /// </remarks>
        private void UpdateWaypointTargetPos() {
            if (mWaypoints[0] != null) {
                Coord pos;

                if (mWaypoints[0] is WaypointPiece) pos = (mWaypoints[0] as WaypointPiece).Piece.Pos;
                else /* mWaypoints[0] is WaypointTile */ pos = (mWaypoints[0] as WaypointTile).Tile;

                mDirLater = CalcTravelDir(pos);
            } else mDirLater = -1;
        }

        private int CalcTravelDir(Coord dest) {
            int xDir = -1, zDir = -1;
            Coord pos = Pos;
            
            if (pos.X < dest.X) xDir = Util.RIGHT;
            else if (pos.X > dest.X) xDir = Util.LEFT;

            if (Mathf.Abs(dest.X - pos.X) > mPos.BoardSize / 2) xDir = Util.DirOpp(xDir);

            if (pos.Z < dest.Z) zDir = Util.UP;
            else if (pos.Z > dest.Z) zDir = Util.DOWN;

            if (Mathf.Abs(dest.Z - pos.Z) > mPos.BoardSize / 2) zDir = Util.DirOpp(zDir);

            return Util.AddDirs(xDir, zDir);
        }
        // public static int CalcTravelDirsChangeOnly(Coord pos, Coord dest, int boardSize) {
        //     int size = boardSize * Chunk.SIZE;
        //     int xDir = -1, zDir = -1;
        //     if (pos.X < dest.X) xDir = Util.RIGHT;
        //     else if (pos.X > dest.X) xDir = Util.LEFT;
        //     if (Mathf.Abs(dest.X - pos.X) > size / 2) xDir = Util.DirOpp(xDir); else xDir = -1;
        //     if (pos.Z < dest.Z) zDir = Util.UP;
        //     else if (pos.Z > dest.Z) zDir = Util.DOWN;
        //     if (Mathf.Abs(dest.Z - pos.Z) > size / 2) zDir = Util.DirOpp(zDir); else zDir = -1;
        //     return Util.AddDirs(xDir, zDir);
        // }
        
        private SignalFromHost AddToHand(Card[] cards) {
            mHand.AddRange(cards);
            return new SignalAddCards(this, cards);
        }

        private int GetSpeedInt() {
            if (mSpeed == Speed.SLOW) return 1;
            if (mSpeed == Speed.NORMAL) return 2;
            if (mSpeed == Speed.FAST) return 4;
            if (mSpeed == Speed.SHOOT) return 8;
            return 0;
        }
        private int GetSizeInt() {
            if (mSize == Size.HUGE) return 2;
            if (mSize == Size.GARGANTUAN) return 3;
            if (mSize == Size.COLOSSAL) return 4;
            return 1;
        }
    }
}