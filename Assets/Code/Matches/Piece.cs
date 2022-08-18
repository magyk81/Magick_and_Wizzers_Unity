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
        public readonly int ID;

        protected Deck mDeck;
        protected Type mPieceType = Type.CREATURE;
        protected string mName;   

        private PiecePos mPos;
        private Speed mSpeed;
        private Size mSize;
        private int mDirLater = -1;
        private Coord mWaypointTargetPos = Coord.Null;

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

        public SignalFromHost[] Update(params Chunk[] chunks) {
            List<SignalFromHost> outcomes = new List<SignalFromHost>();

            // Try travelling and save the new spot.
            PiecePos newPos = mPos.Travel(GetSpeedInt(), mDirLater);
            bool overlap = false;
            if (newPos.Pos != mPos.Pos) {
                foreach (Chunk chunk in chunks) {
                    if (chunk != null && chunk.InPiecesBounds(mPos)) {
                        overlap = true;
                        break;
                    }
                }
            }
            if (!overlap) mPos = newPos;

            // Update mWaypointTargetPos and dirLater.
            UpdateWaypointTargetPos();

            Debug.Log(Pos);

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

        public bool InBounds(PiecePos pos) {
            bool inX = false, inZ = false;
            if (mPos.LoopsX) {
                if (pos.Bound.X >= Pos.X || pos.Pos.X <= mPos.Bound.X) inX = true;
            } else {
                if (pos.Bound.X >= Pos.X && pos.Pos.X <= mPos.Bound.X) inX = true;
            }
            if (mPos.LoopsZ) {
                if (pos.Bound.Z >= Pos.Z || pos.Pos.Z <= mPos.Bound.Z) inZ = true;
            } else {
                if (pos.Bound.Z >= Pos.Z && pos.Pos.Z <= mPos.Bound.Z) inZ = true;
            }
            return inX && inZ;
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
            Queue<int> emptySlots = new Queue<int>();
            for (int i = 0; i < mWaypoints.Length - 1; i++) {
                if (mWaypoints[i] == null) emptySlots.Enqueue(i);
                else if (emptySlots.Count > 0) {
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
        }

        private void UpdateWaypointTargetPos() {
            if (mWaypoints[0] != null) {
                if (mWaypoints[0] is WaypointPiece)  {}

                Coord pos;

                if (mWaypoints[0] is WaypointPiece) {
                    pos = (mWaypoints[0] as WaypointPiece).Piece.Pos;
                } else { // mWaypoints[0] is WaypointTile
                    pos = (mWaypoints[0] as WaypointTile).Tile;
                }

                if (pos != mWaypointTargetPos) {
                    mWaypointTargetPos = pos;
                    mDirLater = CalcTravelDir();
                }
            }
        }

        private bool IsAdjacent(Coord dest) {
            // if (dest.X == 0 && pos.X ==)
            return false;
        }

        private bool InBounds(Coord dest) {
            bool inX = false, inZ = false;
            if (mPos.LoopsX) {
                if (dest.X >= Pos.X || dest.X <= mPos.Bound.X) inX = true;
            } else {
                if (dest.X >= Pos.X && dest.X <= mPos.Bound.X) inX = true;
            }
            if (mPos.LoopsZ) {
                if (dest.Z >= Pos.Z || dest.Z <= mPos.Bound.Z) inZ = true;
            } else {
                if (dest.Z >= Pos.Z && dest.Z <= mPos.Bound.Z) inZ = true;
            }
            return inX && inZ;
        }

        private int CalcTravelDir() {
            int xDir = -1, zDir = -1;
            Coord pos = Pos, dest = mWaypointTargetPos;
            
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
            return 0;
        }
    }
}