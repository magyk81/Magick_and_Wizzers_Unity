/* Copyright (C) All Rights Reserved
 * Unauthorized copying of this file, via any medium is prohibited.
 * Proprietary and confidential.
 * Written by Robin Campos <magyk81@gmail.com>, year 2021.
 */

using System.Collections.Generic;
using UnityEngine;

public class UX_Piece : MonoBehaviour
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
    private UX_Tile[] mWaypointTiles = new UX_Tile[Piece.MAX_WAYPOINTS];
    private UX_Piece[] mWaypointPieces = new UX_Piece[Piece.MAX_WAYPOINTS];
    private UX_Piece mReal = null;
    private UX_Piece[] mUxAll = null;
    private List<int> mHand = new List<int>();
    private Coord mPosPrev, mPosNext;
    private float mPosLerp;
    private int mSize = 1;
    private float[] mBounds = new float[4];
    private int mPieceID, mBoardID, mPlayerID;
    private bool mIsHovered, mIsSelected;

    public UX_Piece[] UX_All {
        get {
            if (mReal != null) return mReal.mUxAll;
            return mUxAll;
        }
    }
    public UX_Tile[] WaypointTiles { get => mWaypointTiles; }
    public UX_Piece[] WaypointPieces { get => mWaypointPieces; }

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

    /// <summary>
    /// Called once before the match begins.
    /// </summary>
    public void Init(SignalAddPiece signal, string pieceName, int layerCount) {
        mPieceID = signal.PieceID;
        mBoardID = signal.BoardID;
        mUxAll = new UX_Piece[9];

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
    public void AddClone(UX_Piece piece, int cloneIdx) {
        piece.mReal = this;
        mUxAll[cloneIdx] = piece;
    }

    public void SetPos(UX_Tile a, UX_Tile b, float lerp) {
        if (a == null || b == null) gameObject.SetActive(false);
        else {
            Vector3 pos = Vector3.Lerp(a.UX_Pos, b.UX_Pos, lerp);
            mTran.localPosition = new Vector3(pos.x, LIFT_DIST, pos.z);
            gameObject.SetActive(true);

            mPosPrev = a.Pos; mPosNext = b.Pos; mPosLerp = lerp;
            mBounds[0] = mPosPrev.X + ((lerp - 1) * mSize); mBounds[1] = mPosPrev.X + (lerp * mSize);
            mBounds[2] = mPosPrev.Z + ((lerp - 1) * mSize); mBounds[3] = mPosPrev.Z + (lerp * mSize);
        }
    }

    public void UpdateWaypoints(UX_Tile[] tiles, UX_Piece[] pieces) {
        for (int i = 0; i < mWaypointTiles.Length; i++) { mWaypointTiles[i] = tiles[i]; }
        for (int i = 0; i < mWaypointPieces.Length; i++) { mWaypointPieces[i] = pieces[i]; }
    }

    // Called from UX_Player, so apply to every clone.
    public void Hover(int localPlayerID) {
        foreach (UX_Piece piece in UX_All) {
            piece.mHover[localPlayerID].SetActive(true);
            piece.mFrame[localPlayerID].SetActive(false);
            piece.mIsHovered = true;
        }
    }

    // Called from UX_Player, so apply to every clone.
    public void Unhover(int localPlayerID) {
        foreach (UX_Piece piece in UX_All) {
            piece.mHover[localPlayerID].SetActive(false);
            piece.mFrame[localPlayerID].SetActive(true);
            piece.mIsHovered = false;
        }
    }

    // Called from UX_Player, so apply to every clone.
    public void Select(int localPlayerCount) {
        foreach (UX_Piece piece in UX_All) {
            piece.mSelect[localPlayerCount].SetActive(true);
            piece.mIsSelected = true;
        }
    }

    // Called from UX_Player, so apply to every clone.
    public void Unselect(int localPlayerCount) {
        foreach (UX_Piece piece in UX_All) {
            piece.mSelect[localPlayerCount].SetActive(false);
            piece.mIsSelected = false;
        }
    }

    public void AddCard(int cardID) { mHand.Add(cardID); }
    public void RemoveCard(int cardID) { mHand.Remove(cardID); }

    public bool Contains(float x, float z) {
        return mBounds[0] < x && mBounds[1] > x && mBounds[2] < z && mBounds[3] > z;
    }

    private void SetActive(GameObject[] obj, bool active) {
        if (obj != null) { for (int i = 0; i < obj.Length; i++) { obj[i].SetActive(active); } }
    }

    // Cleans memory if/when application is stopped.
    private void OnDestroy() { if (mArtMat != null) Destroy(mArtMat); }
}
