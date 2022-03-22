/* Copyright (C) All Rights Reserved
 * Unauthorized copying of this file, via any medium is prohibited.
 * Proprietary and confidential.
 * Written by Robin Campos <magyk81@gmail.com>, year 2021.
 */

using System.Collections.Generic;
using UnityEngine;

public class UX_Board : MonoBehaviour {
    [SerializeField]
    private int distBetweenBoards, cloneLength;
    [SerializeField]
    UX_Chunk baseChunk;
    [SerializeField]
    UX_Piece basePiece;
    private Transform mPieceParent;
    private UX_Tile[,] mTiles;
    private UX_Chunk[,] mChunks;
    private readonly Dictionary<int, UX_Piece> mPieces = new Dictionary<int, UX_Piece>();
    private int mBoardID;

    public UX_Chunk[,] Chunks { get => mChunks; }

    /// <summary>
    /// Called once before the match begins.
    /// </summary>
    public void Init(int size, int totalSize, int layerCount, int boardID, int cloneIdx = -1,
        UX_Board realBoard = null) {

        mBoardID = boardID;

        int i0 = 0, j0 = 0, iEnd = size, jEnd = size;
        int cloneOffsetX = 0, cloneOffsetZ = 0,
            apartOffset = boardID * distBetweenBoards;
        if (cloneIdx >= 0) {
            if (cloneLength <= 0 || cloneLength > size) cloneLength = size;
            if (cloneIdx == Util.UP || cloneIdx == Util.UP_LEFT || cloneIdx == Util.UP_RIGHT) {
                jEnd = cloneLength;
                cloneOffsetZ = totalSize;
            }
            if (cloneIdx == Util.DOWN || cloneIdx == Util.DOWN_LEFT || cloneIdx == Util.DOWN_RIGHT) {
                j0 = size - cloneLength;
                cloneOffsetZ = -totalSize;
            }
            if (cloneIdx == Util.RIGHT || cloneIdx == Util.UP_RIGHT || cloneIdx == Util.DOWN_RIGHT) {
                iEnd = cloneLength;
                cloneOffsetX = totalSize;
            } if (cloneIdx == Util.LEFT || cloneIdx == Util.UP_LEFT || cloneIdx == Util.DOWN_LEFT) {
                i0 = size - cloneLength;
                cloneOffsetX = -totalSize;
            }
        }

        mTiles = new UX_Tile[totalSize, totalSize];
        mChunks = new UX_Chunk[size, size];

        Transform chunkParent = new GameObject().GetComponent<Transform>();
        chunkParent.gameObject.name = "Chunks";
        chunkParent.parent = GetComponent<Transform>();

        // Load alternative chunk material if Sheol (temporary debugging)
        Material altChunkMat = null;
        if (boardID == 1) {
            altChunkMat = Resources.Load<Material>("Materials/Debug Chunk Sheol");
        }

        // Generate chunks.
        for (int i = i0; i < iEnd; i++) {
            for (int j = j0; j < jEnd; j++) {
                mChunks[i, j] = Instantiate(baseChunk, chunkParent);

                // Set alternative chunk material if it's been loaded.
                // Material set in prefab is used otherwise.
                if (altChunkMat != null) mChunks[i, j].GetComponent<MeshRenderer>().material = altChunkMat;
                
                // Position and scale chunks.
                int chunkSize = totalSize / size;
                Transform chunkTra = mChunks[i, j].GetComponent<Transform>();
                chunkTra.localScale = new Vector3(chunkSize, chunkSize, 1);
                Coord chunkPos = Coord._((i * chunkSize) + cloneOffsetX + apartOffset, (j * chunkSize) + cloneOffsetZ);
                chunkTra.localPosition = new Vector3(
                    chunkPos.X + ((float) chunkSize / 2F), 0, chunkPos.Z + ((float) chunkSize / 2F));

                // Generate tiles.
                UX_Tile[,] chunkTiles = new UX_Tile[chunkSize, chunkSize];
                for (int a = 0; a < chunkSize; a++) {
                    for (int b = 0; b < chunkSize; b++) {
                        Coord tilePos = Coord._(i * chunkSize + a, j * chunkSize + b);
                        mTiles[tilePos.X, tilePos.Z] = new UX_Tile(
                            tilePos, size * chunkSize, apartOffset, cloneIdx, boardID);
                        chunkTiles[tilePos.X - (i * chunkSize), tilePos.Z - (j * chunkSize)] =
                            mTiles[tilePos.X, tilePos.Z];
                        
                        if (cloneIdx != -1)
                            realBoard.mTiles[tilePos.X, tilePos.Z].SetClone(mTiles[tilePos.X, tilePos.Z], cloneIdx);
                    }
                }

                mChunks[i, j].Init(Coord._(i, j), boardID, chunkTiles, layerCount);
                mChunks[i, j].gameObject.SetActive(true);
            }
        }

        mPieceParent = new GameObject().GetComponent<Transform>();
        mPieceParent.gameObject.name = "Pieces";
        mPieceParent.parent = GetComponent<Transform>();

        gameObject.SetActive(true);
    }

    public float[] GetBounds() {
        float[] bounds = new float[4];
        bounds[Util.UP] = mTiles[0, mTiles.GetLength(1) - 1].UX_Pos.z + 0.5F;
        bounds[Util.RIGHT] = mTiles[mTiles.GetLength(0) - 1, 0].UX_Pos.x + 0.5F;
        bounds[Util.DOWN] = mTiles[0, 0].UX_Pos.z - 0.5F;
        bounds[Util.LEFT] = mTiles[0, 0].UX_Pos.x - 0.5F;
        return bounds;
    }

    public int PlayerWithPiece(int pieceID) { return mPieces[pieceID].PlayerID; }

    public UX_Piece AddPiece(SignalFromHost signal, string pieceName, int layerCount) {
        UX_Piece uxPiece = Instantiate(basePiece.gameObject, mPieceParent).GetComponent<UX_Piece>();
        uxPiece.gameObject.name = pieceName;
        mPieces.Add(signal.PieceID, uxPiece);
        uxPiece.Init(signal, pieceName, layerCount);
        MovePiece(signal.PieceID, signal.Tile);
        return uxPiece;
    }

    public void AddCard(SignalFromHost signal) { mPieces[signal.PieceID].AddCard(signal.CardID); }

    public void RemoveCard(SignalFromHost signal) { mPieces[signal.PieceID].RemoveCard(signal.CardID); }

    /// <summary>Updates the piece's position on the board between the tiles
    /// indicated by the piece's PosPrecise info.</summary>
    public void MovePiece(int pieceID, Coord tile) {
        mPieces[pieceID].SetPos(mTiles[tile.X, tile.Z], mTiles[tile.X, tile.Z], 1);
    }

    public void UpdateWaypoints(int pieceID, Coord[] coords) {
        UX_Tile[] waypointTiles = new UX_Tile[coords.Length];
        UX_Piece[] waypointPieces = new UX_Piece[coords.Length];
        for (int i = 0; i < coords.Length; i++) {
            if (coords[i] == Coord.Null) {
                waypointTiles[i] = null;
                waypointPieces[i] = null;
            } else if (coords[i].X == -1) {
                waypointTiles[i] = null;
                waypointPieces[i] = mPieces[coords[i].Z];
            } else {
                waypointTiles[i] = mTiles[coords[i].X, coords[i].Z];
                waypointPieces[i] = null;
            }
        }

        mPieces[pieceID].UpdateWaypoints(waypointTiles, waypointPieces);
    }

    // Update is called once per frame
    private void Update() { }
}
