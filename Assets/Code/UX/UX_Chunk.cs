/* Copyright (C) All Rights Reserved
 * Unauthorized copying of this file, via any medium is prohibited.
 * Proprietary and confidential.
 * Written by Robin Campos <magyk81@gmail.com>, year 2021.
 */

using UnityEngine;

public class UX_Chunk : MonoBehaviour {
    [SerializeField]
    private UX_Collider baseCollider;
    private int mBoardIdx;
    private Coord mPos;
    private UX_Tile[,] mTiles;
    private UX_Collider[] mColliders;
    private Vector3[] mQuarterPos;
    private Vector3 mQuarterSize;
    private Coord[][] mTileCollIdxOffset;

    /// <summary>
    /// Called once before the match begins.
    /// </summary>
    public void Init(Coord pos, int boardIdx, UX_Tile[,] tiles, int layerCount) {
        mBoardIdx = boardIdx;
        mPos = pos.Copy();
        mTiles = tiles;

        gameObject.name = "Chunk " + pos;

        // Generate colliders.
        Transform tra = GetComponent<Transform>();
        mColliders = new UX_Collider[layerCount];
        for (int i = 0; i < layerCount; i++) {
            UX_Collider coll = Instantiate(baseCollider, tra).GetComponent<UX_Collider>();
            coll.GetComponent<Transform>().eulerAngles = new Vector3(90, 0, 0);
            coll.Chunk = this;
            coll.gameObject.layer = UX_Tile.LAYER + i;
            coll.gameObject.name = "Collider - view " + i;
            mColliders[i] = coll;
        }

        // Set quarter collider postions and size.
        mQuarterPos = new Vector3[4];
        
        Vector3 traPos = tra.localPosition, offset = tra.localScale / 4F;
        mQuarterPos[Util.UP_RIGHT - 4] = new Vector3(traPos.x + offset.x, 0, traPos.z + offset.y);
        mQuarterPos[Util.DOWN_LEFT - 4] = new Vector3(traPos.x - offset.x, 0, traPos.z - offset.y);
        mQuarterPos[Util.UP_LEFT - 4] = new Vector3(traPos.x - offset.x, 0, traPos.z + offset.y);
        mQuarterPos[Util.DOWN_RIGHT - 4] = new Vector3(traPos.x + offset.x, 0, traPos.z - offset.y);
        mQuarterSize = new Vector3(tiles.GetLength(0) / 2, tiles.GetLength(0) / 2, 1);
        
        // Set tile collider idx offsets.
        mTileCollIdxOffset = new Coord[4][];
        int halfSize = tiles.GetLength(0) / 2;
        mTileCollIdxOffset[Util.UP_RIGHT - 4] = new Coord[2] { Coord._(halfSize, halfSize), Coord._(0, 0) };
        mTileCollIdxOffset[Util.UP_LEFT - 4] = new Coord[2] { Coord._(0, halfSize), Coord._(-halfSize, 0) };
        mTileCollIdxOffset[Util.DOWN_RIGHT - 4] = new Coord[2] { Coord._(halfSize, 0), Coord._(0, -halfSize) };
        mTileCollIdxOffset[Util.DOWN_LEFT - 4] = new Coord[2] { Coord._(0, 0), Coord._(-halfSize, -halfSize) };
    }

    /// <summary>Disables the large collider and enables the 4
    /// quarter-sized colliders to take its place.</summary>
    public void SetQuarterColliders(UX_Collider[] quarterColls, int localPlayerIdx) {
        mColliders[localPlayerIdx].Disable();
        for (int i = 0; i < 4; i++) {
            quarterColls[i].Set(mQuarterPos[i], mQuarterSize);
            quarterColls[i].Chunk = this;
            quarterColls[i].Enable();
        }
    }

    /// <summary>Called when the quarter-sized collider has been disabled. This
    /// method enables the many tile-sized colliders to take its place.
    /// </summary>
    public void SetTileColliders(int quarter, UX_Collider[,] tileColls, int localPlayerIdx) {
        mColliders[localPlayerIdx].Disable();
        Coord[] idxOffset = mTileCollIdxOffset[quarter - 4];
        for (int i = idxOffset[0].X, _i = 0; i < mTiles.GetLength(0) + idxOffset[1].X; i++, _i++) {
            for (int j = idxOffset[0].Z, _j = 0; j < mTiles.GetLength(1) + idxOffset[1].Z; j++, _j++) {
                tileColls[_i, _j].Tile = mTiles[i, j];
                tileColls[_i, _j].Chunk = this;
                tileColls[_i, _j].Enable();
            }
        }
    }

    private void Update() { }
}
