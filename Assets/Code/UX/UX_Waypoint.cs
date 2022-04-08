/* Copyright (C) All Rights Reserved
 * Unauthorized copying of this file, via any medium is prohibited.
 * Proprietary and confidential.
 * Written by Robin Campos <magyk81@gmail.com>, year 2021.
 */

using UnityEngine;

public class UX_Waypoint : MonoBehaviour {
    private UX_Tile mTile;
    private UX_Piece mHolderPiece, mPiece;
    private Transform mTran, mPieceTran;
    private Renderer mRend;
    private MeshFilter mFilter;

    [SerializeField]
    private Material matOpaque, matSemitrans, matHovered, matHovering;

    private bool mOpaque;

    public UX_Tile Tile {
        get => mTile;
        set {
            mTile = value;
            mPiece = null;
            mTran.localPosition = mTile.UX_Pos;
        }
    }
    public UX_Piece Piece {
        get => mPiece;
        set {
            mPiece = value;
            mTile = null;
            mPieceTran = value.gameObject.GetComponent<Transform>();
        }
    }
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
        set => mOpaque = value;
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

    public void Show() {}

    public void Hide() {
        gameObject.SetActive(false);
        mTile = null;
        mPiece = null;
    }

    public void Hover() { }
    public void Unhover() { }

    public bool ForPiece() { return mPieceTran != null; }

    private Material rendMaterial { set { if (mRend.material != value) mRend.material = value; } }

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
    private void Start() {
        mTran = GetComponent<Transform>();
        mRend = GetComponent<Renderer>();
        mFilter = GetComponent<MeshFilter>();
        Hide();
    }

    // Update is called once per frame
    private void Update() { UpdatePosToPiece(); }

    private void UpdatePosToPiece() { if (mPiece != null) mTran.localPosition = mPieceTran.localPosition; }
}
