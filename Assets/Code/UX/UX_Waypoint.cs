/* Copyright (C) All Rights Reserved
 * Unauthorized copying of this file, via any medium is prohibited.
 * Proprietary and confidential.
 * Written by Robin Campos <magyk81@gmail.com>, year 2021.
 */

using UnityEngine;

public class UX_Waypoint : MonoBehaviour {
    private Vector3 LIFT_POS_OFFSET = new Vector3(0, UX_Piece.LIFT_DIST, 0);

    private UX_Tile mTile;
    private UX_Piece mHolderPiece, mPiece;
    private Transform mTran, mPieceTran;
    private Renderer mRend;
    private MeshFilter mFilter;
    private UX_Waypoint mReal = null;
    private UX_Waypoint[] mUxAll;
    private Coord[] mClonePosOffset;

    [SerializeField]
    private Material matOpaque, matSemitrans, matHovered, matHovering;
    private bool mOpaque;
    private int mBoardID = 0;
    private bool mShown = true, mHovered = false;

    public UX_Waypoint[] UX_All { get => mReal.mUxAll; }
    public int ClonePosOffsetCount { set => mClonePosOffset = new Coord[value]; }
    public bool Shown { get => mShown; }
    public UX_Tile Tile {
        get => mTile;
        set {
            foreach (UX_Waypoint waypoint in UX_All) {
                waypoint.mTile = value;
                waypoint.mPiece = null;
                waypoint.SetPosToTile();
            }
        }
    }
    public UX_Piece Piece {
        get => mPiece;
        set {
            foreach (UX_Waypoint waypoint in UX_All) {
                waypoint.mPiece = value;
                waypoint.mTile = null;
                waypoint.mPieceTran = value.gameObject.GetComponent<Transform>();
                waypoint.UpdatePosToPiece();
            }
            
        }
    }
    // public int Current
    // public UX_Piece HolderPiece {
    //     set {
    //         if (value != null) {
    //             mHolderPiece = value;
                
    //         }
    //         if (value != null) {
    //             mPieceTran = value.GetComponent<Transform>();
    //             RendMesh = meshForPiece;
    //             UpdatePosToPiece();
    //         } else {
    //             mPieceTran = null;
    //             RendMesh = meshForTile;
    //         }
    //     }
    // }

    public bool Opaque {
        set {
            if (mOpaque == value) return;
            foreach (UX_Waypoint waypoint in UX_All) {
                waypoint.mOpaque = value;
            }
            UpdateMat();
        }
    }

    public int BoardID {
        set {
            foreach (UX_Waypoint waypoint in UX_All) {
                waypoint.mBoardID = value;
                waypoint.SetPosToTile();
            }
        }
    }

    public void Init() {
        mTran = GetComponent<Transform>();
        mRend = GetComponent<Renderer>();
        mFilter = GetComponent<MeshFilter>();
    }

    public void Show(bool opaque, bool hovered) {
        // bool onP = mPieceTran != null;
        // if (opaque) {
        //     if (hovered) rendMaterial = onP ? opaqueYellowPMat : opaqueYellowMat;
        //     else rendMaterial = onP ? opaqueRedPMat : opaqueRedMat;
        // } else {
        //     if (hovered) rendMaterial = onP ? semitransYellowPMat : semitransYellowMat;
        //     else rendMaterial = onP ? semitransRedPMat : semitransRedMat;
        // }
        UpdatePosToPiece();
        gameObject.SetActive(true);
    }

    public void Show() {
        if (mShown) return;
        Debug.Log("Show");
        foreach (UX_Waypoint waypoint in UX_All) {
            waypoint.gameObject.SetActive(true);
        }
        mShown = true;
    }

    public void Hide() {
        if (!mShown) return;
        Debug.Log("Hide");
        foreach (UX_Waypoint waypoint in UX_All) {
            waypoint.gameObject.SetActive(false);
            waypoint.mTile = null;
            waypoint.mPiece = null;
        }
        mShown = false;
    }

    // Should only be called once if this is the hovering waypoint.
    public void Hover() {
        if (mHovered) return;
        mHovered = true;
        UpdateMat();
    }
    // Should never be called if this is the hovering waypoint.
    public void Unhover() {
        if (!mHovered) return;
        mHovered = false;
        UpdateMat();
    }

    public bool ForPiece() { return mPieceTran != null; }

    public void SetReal() {
        mReal = this;
        mUxAll = new UX_Waypoint[9];
        mUxAll[0] = this;
    }
    public void AddClone(UX_Waypoint waypoint, int cloneIdx) {
        waypoint.mReal = this;
        mUxAll[cloneIdx] = waypoint;
    }
    public void AddClonePosOffset(Coord offset, int boardID) {
        mClonePosOffset[boardID] = offset.Copy();
    }

    public Vector3 GetPosForLines() { return mTran.localPosition + LIFT_POS_OFFSET; }
    public Vector3 GetCoordForLines() {
        if (mTile != null) return mTile.UX_Pos;
        if (mPiece != null) return mPiece.UX_Pos;
        return Vector3.zero;
    }

    private Material rendMaterial {
        set {
            if (mRend.material != value) {
                foreach (UX_Waypoint waypoint in UX_All) { waypoint.mRend.material = value; }
            }
        }
    }

    // private Mesh RendMesh {
    //     set {
    //         if (mFilter.mesh != value) {
    //             mFilter.mesh = value;
    //             if (value == meshForTile) {
    //                 mTran.localEulerAngles = new Vector3(0, 0, 0);
    //                 mTran.localScale = new Vector3(1, 1, 1);
    //             } else if (value == meshForPiece) {
    //                 mTran.localEulerAngles = new Vector3(90, 0, 0);
    //                 mTran.localScale = new Vector3(2, 2, 1);
    //             }
    //         }
    //     }
    // }

    // Start is called before the first frame update
    private void Start() {}

    // Update is called once per frame
    private void Update() { UpdatePosToPiece(); }

    private void UpdateMat() {
        if (mOpaque) {
            if (mHovered) foreach (UX_Waypoint waypoint in UX_All) { rendMaterial = matHovered; }
            else foreach (UX_Waypoint waypoint in UX_All) { rendMaterial = matOpaque; }
        } else {
            if (mHovered) foreach (UX_Waypoint waypoint in UX_All) { rendMaterial = matHovering; }
            else foreach (UX_Waypoint waypoint in UX_All) { rendMaterial = matSemitrans; }
        }
    }

    private void SetPosToTile() {
        if (mTile != null) mTran.localPosition = mTile.UX_Pos + mClonePosOffset[mBoardID].ToVec3(); }
    private void UpdatePosToPiece() { if (mPiece != null) mTran.localPosition = mPieceTran.localPosition; }
}
