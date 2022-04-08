/* Copyright (C) All Rights Reserved
 * Unauthorized copying of this file, via any medium is prohibited.
 * Proprietary and confidential.
 * Written by Robin Campos <magyk81@gmail.com>, year 2021.
 */

using UnityEngine;

public class UX_Chunk : MonoBehaviour {
    private int mBoardIdx;
    private Coord mPos;
    private UX_Tile[,] mTiles;
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
        Transform tra = GetComponent<Transform>();

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

    private void Update() { }
}
