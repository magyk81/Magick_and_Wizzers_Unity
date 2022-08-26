/* Copyright (C) All Rights Reserved
 * Unauthorized copying of this file, via any medium is prohibited.
 * Proprietary and confidential.
 * Written by Robin Campos <magyk81@gmail.com>, year 2021.
 */

using System.Collections.Generic;
using UnityEngine;
using Matches.Cards;
using Matches.UX.Waypoints;
using Network.SignalsFromHost;

namespace Matches.UX {
    public class Piece : MonoBehaviour
    {
        public readonly static float LIFT_DIST = 0.1F;
        public readonly static int LAYER = 6;

        private enum Part {ART, FRAME, ATTACK, DEFENSE, LIFE, HOVER, SELECT, TARGET, COUNT };

        [SerializeField]
        private GameObject art, frame, attackBar, defenseBar, lifeBar, hover, select,
            target;

        private GameObject[] mArt, mFrame, mAttackBar, mDefenseBar, mLifeBar, mHover, mSelect, mTarget;
        private Transform mTran;
        private Material mArtMat;
        private Piece mReal = null;
        private Piece[] mUxAll = new Piece[9];
        private List<int> mHand = new List<int>();
        private float[] mPos;
        private Matches.Piece.Size mSize;
        private int mSizeInt = 1;
        private float[] mBounds = new float[4];
        private int mPieceID, mBoardID, mPlayerID;
        private bool mIsHovered, mIsSelected;
        private WaypointTrail mTrail = new WaypointTrail();

        public Piece[] UX_All {
            get {
                if (mReal != null) return mReal.mUxAll;
                return mUxAll;
            }
        }
        public Matches.Waypoints.Waypoint[] Waypoints { set => mTrail.Waypoints = value; }
        public WaypointTrail Trail { get => mTrail; }
        public int WaypointIdx { get => mTrail.Idx; set => mTrail.Idx = value; }
        public Card[] Hand {
            get {
                Card[] hand = new Card[mHand.Count];
                for (int i = 0; i < hand.Length; i++) { hand[i] = Card.friend_cards[mHand[i]]; }
                return hand;
            }
        }

        public int PieceID { get => mPieceID; }
        public int BoardID { get => mBoardID; }
        public int PlayerID { get => mPlayerID; }
        public bool IsHovered { get => mIsHovered; }
        public bool IsSelected { get => mIsSelected; }
        public Vector3 UX_Pos { get => mTran.localPosition; }
        // public Tile UX_TileAvg { get => mTileAvg; }

        /// <summary>
        /// Called once before the match begins.
        /// </summary>
        public void Init(SignalAddPiece signal, string pieceName, int layerCount) {
            mPieceID = signal.PieceID;
            mBoardID = signal.BoardID;

            mTran = GetComponent<Transform>();
            bool fromCard = signal.CardID >= 0;
                
            if (fromCard) {
                mAttackBar = new GameObject[layerCount];
                for (int i = 0; i < layerCount; i++) {
                    mAttackBar[i] = Instantiate(attackBar, mTran);
                    mAttackBar[i].layer = LAYER + i;
                    mAttackBar[i].name = "Attack Bar - view " + i;
                }
                SetActive(mAttackBar, true);
                mDefenseBar = new GameObject[layerCount];
                for (int i = 0; i < layerCount; i++) {
                    mDefenseBar[i] = Instantiate(defenseBar, mTran);
                    mDefenseBar[i].layer = LAYER + i;
                    mDefenseBar[i].name = "Defense Bar - view " + i;
                }
                SetActive(mDefenseBar, true);
            } else { // It's a Master.
                mLifeBar = new GameObject[layerCount];
                for (int i = 0; i < layerCount; i++) {
                    mLifeBar[i] = Instantiate(lifeBar, mTran);
                    mLifeBar[i].layer = LAYER + i;
                    mLifeBar[i].name = "Life Bar - view " + i;
                }
                SetActive(mLifeBar, true);
            }

            mArt = new GameObject[layerCount];
            mArtMat = new Material(art.GetComponent<MeshRenderer>().sharedMaterial);
            mArtMat.name = "Piece Art Material - " + pieceName;
            Texture cardArt = signal.CardID == -1 ? null : Card.friend_cards[signal.CardID].Art;
            if (cardArt != null) mArtMat.mainTexture = cardArt;
            for (int i = 0; i < layerCount; i++) {
                mArt[i] = Instantiate(art, mTran);
                mArt[i].GetComponent<MeshRenderer>().material = mArtMat;
                mArt[i].layer = LAYER + i;
                mArt[i].name = "Art - view " + i;
            }
            SetActive(mArt, true);
            mFrame = new GameObject[layerCount];
            for (int i = 0; i < layerCount; i++) {
                mFrame[i] = Instantiate(frame, mTran);
                mFrame[i].layer = LAYER + i;
                mFrame[i].name = "Frame - view " + i;
            }
            SetActive(mFrame, true);

            mHover = new GameObject[layerCount];
            for (int i = 0; i < layerCount; i++) {
                mHover[i] = Instantiate(hover, mTran);
                mHover[i].layer = LAYER + i;
                mHover[i].name = "Hover Crown - view " + i;
            }
            SetActive(mHover, false);
            mSelect = new GameObject[layerCount];
            for (int i = 0; i < layerCount; i++) {
                mSelect[i] = Instantiate(select, mTran);
                mSelect[i].layer = LAYER + i;
                mSelect[i].name = "Select Crown - view " + i;
            }
            SetActive(mSelect, false);
            mTarget = new GameObject[layerCount];
            for (int i = 0; i < layerCount; i++) {
                mTarget[i] = Instantiate(target, mTran);
                mTarget[i].layer = LAYER + i;
                mTarget[i].name = "Target Crown - view " + i;
            }
            SetActive(mTarget, false);

            if (fromCard) gameObject.name = "Piece - " + pieceName;
            else gameObject.name = "Master - " + pieceName;
        }

        public void SetReal() {
            mReal = this;
            mUxAll[0] = this;
        }
        public void AddClone(Piece piece, int cloneIdx) {
            piece.mReal = this;
            mUxAll[cloneIdx] = piece;
        }

        public void SetSize(Matches.Piece.Size size) {
            mSize = size;
            mSizeInt = Matches.Piece.SizeToInt(size);
        }
        public void SetPos(Tile tile, int dirNext, float lerp) {
            if (tile == null) gameObject.SetActive(false);
            else {
                Vector3 center = Util.DirToVec3(dirNext, lerp);
                if (mSizeInt == 1) center += new Vector3(tile.UX_Pos.x, LIFT_DIST, tile.UX_Pos.z);
                else if (mSizeInt == 2) center += new Vector3(tile.UX_Pos.x + 0.5F, LIFT_DIST, tile.UX_Pos.z + 0.5F);
                else if (mSizeInt == 3) center += new Vector3(tile.UX_Pos.x + 1F, LIFT_DIST, tile.UX_Pos.z + 1F);
                else if (mSizeInt == 4) center += new Vector3(tile.UX_Pos.x + 1.5F, LIFT_DIST, tile.UX_Pos.z + 1.5F);
                else Debug.LogError("Invalid mSizeInt: " + mSizeInt);

                mTran.localPosition = new Vector3(center.x, LIFT_DIST, center.z);
                gameObject.SetActive(true);

                mBounds[0] = (center.x - ((float) mSizeInt) / 2); mBounds[1] = (center.x + ((float) mSizeInt) / 2);
                mBounds[2] = (center.z - ((float) mSizeInt) / 2); mBounds[3] = (center.z + ((float) mSizeInt) / 2);
            }
        }

        public bool WaypointIdxAtMax() { return mTrail.AtMax(); }
        public bool WaypointIdxNextToMax() { return mTrail.NextToMax(); }

        // Called from UX_Player, so apply to every clone.
        public void Hover(int localPlayerID) {
            foreach (Piece piece in UX_All) {
                piece.mHover[localPlayerID].SetActive(true);
                piece.mFrame[localPlayerID].SetActive(false);
                piece.mIsHovered = true;
            }
        }

        // Called from UX_Player, so apply to every clone.
        public void Unhover(int localPlayerID) {
            foreach (Piece piece in UX_All) {
                piece.mHover[localPlayerID].SetActive(false);
                piece.mFrame[localPlayerID].SetActive(true);
                piece.mIsHovered = false;
            }
        }

        // Called from UX_Player, so apply to every clone.
        public void Select(int localPlayerCount) {
            foreach (Piece piece in UX_All) {
                piece.mSelect[localPlayerCount].SetActive(true);
                piece.mIsSelected = true;
            }
        }

        // Called from UX_Player, so apply to every clone.
        public void Unselect(int localPlayerCount) {
            foreach (Piece piece in UX_All) {
                piece.mSelect[localPlayerCount].SetActive(false);
                piece.mIsSelected = false;
            }
        }

        public void AddCards(params int[] cardIDs) { mHand.AddRange(cardIDs); }
        public void RemoveCards(params int[] cardIDs) { foreach (int id in cardIDs) { mHand.Remove(id); } }

        public bool Contains(float x, float z) {
            return mBounds[0] < x && mBounds[1] > x && mBounds[2] < z && mBounds[3] > z;
        }

        private void SetActive(GameObject[] obj, bool active) {
            if (obj != null) { for (int i = 0; i < obj.Length; i++) { obj[i].SetActive(active); } }
        }

        // Cleans memory if/when application is stopped.
        private void OnDestroy() { if (mArtMat != null) Destroy(mArtMat); }
    }
}