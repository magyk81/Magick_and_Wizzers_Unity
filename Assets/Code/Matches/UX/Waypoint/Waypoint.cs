/* Copyright (C) All Rights Reserved
 * Unauthorized copying of this file, via any medium is prohibited.
 * Proprietary and confidential.
 * Written by Robin Campos <magyk81@gmail.com>, year 2021.
 */

using UnityEngine;

namespace Matches.UX.Waypoints {
    public class Waypoint : MonoBehaviour {
        private readonly Vector3 LIFT_POS_OFFSET = new Vector3(0, Matches.UX.Piece.LIFT_DIST, 0);
        private const float ROT_INCREMENT = 0.1F, ROT_MAX = 360, PIVOT_INCREMENT = 0.01F, PIVOT_MAG = 1F;

        [SerializeField]
        private Material matOpaqueTile, matSemiTile, matHoveredTile, matHoveringTile,
            matOpaquePiece, matSemiPiece, matHoveredPiece, matHoveringPiece,
            matOpaqueTileGroup, matSemiTileGroup, matOpaquePieceGroup, matSemiPieceGroup;
        [SerializeField]
        private Mesh tileMesh, pieceMesh;
        [SerializeField]
        private Vector3 tileMeshRot, tileMeshSca, pieceMeshRot, pieceMeshSca;

        private Waypoint mReal = null;
        private Waypoint[] mUxAll = new Waypoint[9];
        private int mCloneID;

        private Tile mTile;
        private Piece mPiece;

        // This waypoint's transform, the target piece's transform.
        private Transform mTran, mPieceTran;
        private Renderer mRend;
        private MeshFilter mFilter;

        private Waypoint mPrev, mNext;
        private bool mOpaque, mShown = true, mHovered = false, mForGroup = false;
        private int mBoardID = 0;
        private bool mUpdatePending = false;

        private float mRot = 0;

        public Waypoint[] UX_All {
            get {
                if (mReal != null) return mReal.mUxAll;
                return mUxAll;
            }
        }

        public bool Shown { get => mShown; }
        public Tile Tile {
            get => mTile;
            set {
                if (mTile == value) return;
                mTile = value;
                mPiece = null;
                mUpdatePending = true;
            }
        }
        public Piece Piece {
            get => mPiece;
            set {
                if (mPiece == value) return;
                mPiece = value;
                // Get the transform of the real piece even if hovering a clone.
                if (mPiece != null) {
                    for (int i = 0; i < mUxAll.Length; i++) {
                        mUxAll[i].mPieceTran = mPiece.UX_All[i].GetComponent<Transform>();
                    }
                } else for (int i = 0; i < mUxAll.Length; i++) { mUxAll[i].mPieceTran = null; }
                mTile = null;
                mUpdatePending = true;
            }
        }
        public Waypoint Next {
            get => mNext;
            // Clones need to already be set.
            set {
                for (int i = 0; i < mUxAll.Length; i++) {
                    mUxAll[i].mNext = value.mUxAll[i];
                    value.mUxAll[i].mPrev = mUxAll[i];
                }
            }
        }
        public bool Opaque {
            set {
                if (mOpaque == value) return;
                mOpaque = value;
                mUpdatePending = true;
            }
        }

        public bool ForGroup {
            set {
                if (mForGroup == value) return;
                mForGroup = value;
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

        public void Init(int trailIdx) {
            mTran = GetComponent<Transform>();
            mRend = GetComponent<Renderer>();
            mFilter = GetComponent<MeshFilter>();
            name = (trailIdx == -1) ? "Hovering Waypoint" : "Waypoint";
            gameObject.SetActive(false);
        }

        // public UX_Waypoint(
        //     int idx,
        //     Transform parentTrans,
        //     GameObject baseWaypointForTile,
        //     GameObject baseWaypointForPiece,
        //     int[] boardSizes,

        //     Material matOpaqueForTile,
        //     Material matSemitransForTile,
        //     Material matHoveredForTile,
        //     Material matHoveringForTile,
        //     Material matOpaqueForPiece,
        //     Material matSemitransForPiece,
        //     Material matHoveredForPiece,
        //     Material matHoveringForPiece,

        //     Material matOpaqueForTileOnGroup,
        //     Material matSemitransForTileOnGroup,
        //     Material matOpaqueForPieceOnGroup,
        //     Material matSemitransForPieceOnGroup) {

        //     mUxForTile = new GameObject[9];
        //     mUxForPiece = new GameObject[9];
        //     mTranForTile = new Transform[9];
        //     mTranForPiece = new Transform[9];
        //     mRendForTile = new Renderer[9];
        //     mRendForPiece = new Renderer[9];
        //     for (int i = 0; i < mUxForTile.Length; i++) {
        //         mUxForTile[i] = GameObject.Instantiate(baseWaypointForTile, parentTrans);
        //         mTranForTile[i] = mUxForTile[i].GetComponent<Transform>();
        //         mRendForTile[i] = mUxForTile[i].GetComponent<Renderer>();
        //         string nameWaypointTile = (idx == -1) ? "Hovering Waypoint for tile" : "Waypoint " + idx + " for tile";
        //         if (i > 0) nameWaypointTile += " - Clone " + i;
        //         mUxForTile[i].name = nameWaypointTile;
        //         mUxForTile[i].SetActive(false);

        //         mUxForPiece[i] = GameObject.Instantiate(baseWaypointForPiece, parentTrans);
        //         mTranForPiece[i] = mUxForPiece[i].GetComponent<Transform>();
        //         mRendForPiece[i] = mUxForPiece[i].GetComponent<Renderer>();
        //         string nameWaypointPiece = (idx == -1) ? "Hovering Waypoint for piece" : "Waypoint " + idx + " for piece";
        //         if (i > 0) nameWaypointPiece += " - Clone " + i;
        //         mUxForPiece[i].name = nameWaypointPiece;
        //         mUxForPiece[i].SetActive(false);
        //     }

        //     mCloneOffsets = new Vector3[boardSizes.Length][];
        //     for (int i = 0; i < mCloneOffsets.Length; i++) {
        //         mCloneOffsets[i] = new Vector3[Util.COUNT + 1];
        //         for (int j = 0; j < mCloneOffsets[i].Length; j++) {
        //             mCloneOffsets[i][j] = Util.DirToVec3(j - 1) * boardSizes[i] * Chunk.SIZE
        //                 + (Util.DirToVec3(Util.RIGHT) * UX_Board.DIST_BETWEEN_BOARDS * i);
        //         }
        //     }
        // }

        public void SetReal() {
            mReal = this;
            mUxAll[0] = this;
            mCloneID = 0;
        }
        public void AddClone(Waypoint clone, int cloneIdx) {
            clone.mReal = this;
            mUxAll[cloneIdx] = clone;
            mCloneID = cloneIdx;
            clone.name += " - Clone " + cloneIdx;
        }

        public void Show() {
            if (mReal.mShown) return;
            mReal.mShown = true;
            mReal.mUpdatePending = true;
        }
        public void Hide() {
            if (!mReal.mShown) return;
            mReal.mShown = false;
            mReal.mUpdatePending = true;
        }

        // Should only be called once if this is the hovering waypoint.
        public void Hover() {
            if (mReal.mHovered) return;
            mReal.mHovered = true;
            mReal.mUpdatePending = true;
        }
        // Should never be called if this is the hovering waypoint.
        public void Unhover() {
            if (!mReal.mHovered) return;
            mReal.mHovered = false;
            mReal.mUpdatePending = true;
        }

        public void UpdateMaterial() {
            // mUpdatePending should only ever be true if mReal == true.
            if (mUpdatePending) {
                
                if (!mShown) {
                    foreach (Waypoint clone in UX_All) { clone.gameObject.SetActive(false); }
                } else if (mTile != null) {
                    foreach (Waypoint clone in UX_All) {
                        clone.mFilter.mesh = tileMesh;
                        clone.mTran.localEulerAngles = tileMeshRot;
                        clone.mTran.localScale = tileMeshSca;
                        if (mForGroup) {
                            if (mOpaque) clone.mRend.material = matOpaqueTileGroup;
                            else clone.mRend.material = matSemiTileGroup;
                        } else if (mHovered) {
                            if (mOpaque) clone.mRend.material = matHoveredTile;
                            else clone.mRend.material = matHoveringTile;
                        } else {
                            if (mOpaque) clone.mRend.material = matOpaqueTile;
                            else clone.mRend.material = matSemiTile;
                        }
                        clone.gameObject.SetActive(true);
                    }
                } else if (mPiece != null) {
                    foreach (Waypoint clone in UX_All) {
                        clone.mFilter.mesh = pieceMesh;
                        clone.mTran.localEulerAngles = pieceMeshRot;
                        clone.mTran.localScale = pieceMeshSca;
                        if (mForGroup) {
                            if (mOpaque) clone.mRend.material = matOpaquePieceGroup;
                            else clone.mRend.material = matSemiPieceGroup;
                        } else if (mHovered) {
                            if (mOpaque) clone.mRend.material = matHoveredPiece;
                            else clone.mRend.material = matHoveringPiece;
                        } else {
                            if (mOpaque) clone.mRend.material = matOpaquePiece;
                            else clone.mRend.material = matSemiPiece;
                        }
                        clone.gameObject.SetActive(true);
                    }
                } else { foreach (Waypoint clone in UX_All) { clone.gameObject.SetActive(false); } }

                mUpdatePending = false;
            }

            if (mShown) {
                if (mTile != null) {
                    for (int i = 0; i < mUxAll.Length; i++) {
                        mReal.UX_All[i].mTran.localPosition = mTile.UX_PosAll[i];
                    }
                } else if (mPiece != null) {
                    for (int i = 0; i < mUxAll.Length; i++) {
                        mReal.UX_All[i].mTran.localPosition = mPiece.UX_All[i].UX_Pos;
                    }
                }
            }
        }

        // Update is called once per frame
        private void Update() {
            if (mShown) {
                if (mReal.mTile != null) {
                    for (int i = 0; i < mUxAll.Length; i++) {
                        if (mForGroup) {

                        } else {
                            mRot += ROT_INCREMENT;
                            if (mRot >= ROT_MAX) mRot -= ROT_MAX;
                            mTran.eulerAngles = new Vector3(tileMeshRot.x, (int) mRot, tileMeshRot.z);
                        }
                    }
                } else if (mReal.mPiece != null) {
                    for (int i = 0; i < mUxAll.Length; i++) {
                        mTran.localPosition = mPieceTran.localPosition;
                        if (mForGroup) {

                        } else {
                            mRot += PIVOT_INCREMENT;
                            if (mRot >= ROT_MAX) mRot -= ROT_MAX;
                            mTran.eulerAngles = new Vector3(
                                pieceMeshRot.x + (Mathf.Sin(mRot) * PIVOT_MAG),
                                Mathf.Cos(mRot) * PIVOT_MAG,
                                pieceMeshRot.z
                            );
                        }
                    }
                }
            }
        }
    }
}