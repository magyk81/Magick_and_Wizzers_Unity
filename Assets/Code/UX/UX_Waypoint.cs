/* Copyright (C) All Rights Reserved
 * Unauthorized copying of this file, via any medium is prohibited.
 * Proprietary and confidential.
 * Written by Robin Campos <magyk81@gmail.com>, year 2021.
 */

using UnityEngine;

public class UX_Waypoint {
    private Vector3 LIFT_POS_OFFSET = new Vector3(0, UX_Piece.LIFT_DIST, 0);

    private UX_Tile mTile;
    private UX_Piece mPiece;
    private GameObject[] mUxForTile, mUxForPiece;
    private Transform[] mTranForTile, mTranForPiece;
    private Transform mPieceTran, mHolderTran;
    private Renderer[] mRendForTile, mRendForPiece;
    private Vector3[][] mCloneOffsets;
    private Material mMatOpaqueForTile, mMatSemitransForTile, mMatHoveredForTile, mMatHoveringForTile,
        mMatOpaqueForPiece, mMatSemitransForPiece, mMatHoveredForPiece, mMatHoveringForPiece;

    private UX_Waypoint mPrev, mNext;
    private bool mOpaque;
    private bool mShown = true, mHovered = false;
    private int mBoardID = 0;
    private bool mUpdatePending = false;

    public bool Shown { get => mShown; }
    public UX_Tile Tile {
        get => mTile;
        set {
            if (mTile == value) return;
            mTile = value;
            mPiece = null;
            mUpdatePending = true;
        }
    }
    public UX_Piece Piece {
        get => mPiece;
        set {
            if (mPiece == value) return;
            mPiece = value;
            // Get the transform of the real piece even if hovering a clone.
            mPieceTran = mPiece.UX_All[0].GetComponent<Transform>();
            mTile = null;
            mUpdatePending = true;
        }
    }
    public UX_Waypoint Next { set { mNext = value; value.mPrev = this; } }
    public bool Opaque {
        set {
            if (mOpaque == value) return;
            mOpaque = value;
            mUpdatePending = true;
        }
    }

    public int BoardID {
        set {
            if (mBoardID == value) return;
            mBoardID = value;
            mUpdatePending = true;
        }
    }

    public UX_Waypoint(
        int idx,
        Transform parentTrans,
        GameObject baseWaypointForTile,
        GameObject baseWaypointForPiece,
        int[] boardSizes,
        Material matOpaqueForTile,
        Material matSemitransForTile,
        Material matHoveredForTile,
        Material matHoveringForTile,
        Material matOpaqueForPiece,
        Material matSemitransForPiece,
        Material matHoveredForPiece,
        Material matHoveringForPiece) {

        mUxForTile = new GameObject[9];
        mUxForPiece = new GameObject[9];
        mTranForTile = new Transform[9];
        mTranForPiece = new Transform[9];
        mRendForTile = new Renderer[9];
        mRendForPiece = new Renderer[9];
        for (int i = 0; i < mUxForTile.Length; i++) {
            mUxForTile[i] = GameObject.Instantiate(baseWaypointForTile, parentTrans);
            mTranForTile[i] = mUxForTile[i].GetComponent<Transform>();
            mRendForTile[i] = mUxForTile[i].GetComponent<Renderer>();
            string nameWaypointTile = (idx == -1) ? "Hovering Waypoint for tile" : "Waypoint " + idx + " for tile";
            if (i > 0) nameWaypointTile += " - Clone " + i;
            mUxForTile[i].name = nameWaypointTile;
            mUxForTile[i].SetActive(false);

            mUxForPiece[i] = GameObject.Instantiate(baseWaypointForPiece, parentTrans);
            mTranForPiece[i] = mUxForPiece[i].GetComponent<Transform>();
            mRendForPiece[i] = mUxForPiece[i].GetComponent<Renderer>();
            string nameWaypointPiece = (idx == -1) ? "Hovering Waypoint for piece" : "Waypoint " + idx + " for piece";
            if (i > 0) nameWaypointPiece += " - Clone " + i;
            mUxForPiece[i].name = nameWaypointPiece;
            mUxForPiece[i].SetActive(false);
        }

        mCloneOffsets = new Vector3[boardSizes.Length][];
        for (int i = 0; i < mCloneOffsets.Length; i++) {
            mCloneOffsets[i] = new Vector3[Util.COUNT + 1];
            for (int j = 0; j < mCloneOffsets[i].Length; j++) {
                mCloneOffsets[i][j] = Util.DirToVec3(j - 1) * boardSizes[i] * Chunk.SIZE
                    + (Util.DirToVec3(Util.RIGHT) * UX_Board.DIST_BETWEEN_BOARDS * i);
            }
        }

        mMatOpaqueForTile = matOpaqueForTile;
        mMatSemitransForTile = matSemitransForTile;
        mMatHoveredForTile = matHoveredForTile;
        mMatHoveringForTile = matHoveringForTile;
        mMatOpaqueForPiece = matOpaqueForPiece;
        mMatSemitransForPiece = matSemitransForPiece;
        mMatHoveredForPiece = matHoveredForPiece;
        mMatHoveringForPiece = matHoveringForPiece;
    }

    public void Show() {
        if (mShown) return;
        mShown = true;
        mUpdatePending = true;
    }
    public void Hide() {
        if (!mShown) return;
        mShown = false;
        mUpdatePending = true;
    }

    // Should only be called once if this is the hovering waypoint.
    public void Hover() {
        if (mHovered) return;
        mHovered = true;
        mUpdatePending = true;
    }
    // Should never be called if this is the hovering waypoint.
    public void Unhover() {
        if (!mHovered) return;
        mHovered = false;
        mUpdatePending = true;
    }

    // Update is called once per frame
    public void Update() {
        if (mUpdatePending) {
            
            if (!mShown) {
                for (int i = 0; i < mUxForTile.Length; i++) {
                    mUxForTile[i].SetActive(false);
                    mUxForPiece[i].SetActive(false);
                }
            } else if (mTile != null) {
                for (int i = 0; i < mUxForTile.Length; i++) {
                    if (mHovered) {
                        if (mOpaque) mRendForTile[i].material = mMatHoveredForTile;
                        else mRendForTile[i].material = mMatHoveringForTile;
                    } else {
                        if (mOpaque) mRendForTile[i].material = mMatOpaqueForTile;
                        else mRendForTile[i].material = mMatSemitransForTile;
                    }
                    mUxForTile[i].SetActive(true);
                    mUxForPiece[i].SetActive(false);
                }
            } else if (mPiece != null) {
                for (int i = 0; i < mUxForPiece.Length; i++) {
                    if (mHovered) {
                        if (mOpaque) mRendForPiece[i].material = mMatHoveredForPiece;
                        else mRendForPiece[i].material = mMatHoveringForPiece;
                    } else {
                        if (mOpaque) mRendForPiece[i].material = mMatOpaqueForPiece;
                        else mRendForPiece[i].material = mMatSemitransForPiece;
                    }
                    mUxForTile[i].SetActive(false);
                    mUxForPiece[i].SetActive(true);
                }
            } else {
                for (int i = 0; i < mUxForTile.Length; i++) {
                    mUxForTile[i].SetActive(false);
                    mUxForPiece[i].SetActive(false);
                }
            }

            mUpdatePending = false;
        }

        if (mTile != null && mUxForTile[0].activeSelf) {
            for (int i = 0; i < mTranForTile.Length; i++) {
                mTranForTile[i].localPosition = mTile.UX_PosAll[i];
            }
        } else if (mPiece != null && mUxForPiece[0].activeSelf) {
            for (int i = 0; i < mTranForPiece.Length; i++) {
                mTranForPiece[i].localPosition = mPieceTran.localPosition + mCloneOffsets[mBoardID][i];
            }
        }
    }
}
