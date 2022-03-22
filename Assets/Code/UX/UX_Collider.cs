/* Copyright (C) All Rights Reserved
 * Unauthorized copying of this file, via any medium is prohibited.
 * Proprietary and confidential.
 * Written by Robin Campos <magyk81@gmail.com>, year 2021.
 */

using UnityEngine;

public class UX_Collider : MonoBehaviour
{
    public enum ColliderType { NONE, TILE, QUARTER, CHUNK, PIECE }

    private Transform mTran;
    private UX_Tile mTile;
    private int mQuarter = -1;
    private UX_Chunk mChunk;
    private UX_Piece mPiece;
    private BoxCollider mColl;
    private ColliderType mType = ColliderType.NONE;

    public UX_Tile Tile {
        set { mTile = value; mTran.localPosition = mTile.UX_Pos; }
        get => mTile;
    }

    public int Quarter
    {
        set { if (mQuarter == -1) { mQuarter = value; Type = ColliderType.QUARTER; } }
        get => mQuarter;
    }

    public UX_Chunk Chunk
    {
        set { Type = ColliderType.CHUNK; mChunk = value; }
        get => mChunk;
    }

    public UX_Piece Piece
    {
        set { if (mPiece == null) mPiece = value; Type = ColliderType.PIECE; }
        get => mPiece;
    }

    private ColliderType Type {
        set {
            if (value == ColliderType.PIECE) mType = value;
            else if (value == ColliderType.QUARTER) {
                if (mType != ColliderType.PIECE && mType != ColliderType.TILE) mType = value;
            } else if (value == ColliderType.CHUNK) {
                if (mType != ColliderType.PIECE && mType != ColliderType.TILE && mType != ColliderType.QUARTER)
                    mType = value;
            }
        }
    }

    public void SetTypeTile() { mType = ColliderType.TILE; }
    public bool IsType(ColliderType type) { return mType == type; }
    public void Enable() { mColl.enabled = true; }
    public void Disable() { mColl.enabled = false; }

    public void Set(Vector3 position, Vector3 scale) {
        mTran.localPosition = position;
        mTran.localScale = scale;
    }

    // Start is called before the first frame update
    private void Start() {
        mTran = GetComponent<Transform>();
        mColl = GetComponent<BoxCollider>();
        if (IsType(ColliderType.QUARTER) || IsType(ColliderType.TILE)) Disable();
        else { Disable(); Enable(); }
    }

    // Update is called once per frame
    private void Update() { }
}
