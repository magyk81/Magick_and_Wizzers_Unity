/* Copyright (C) All Rights Reserved
 * Unauthorized copying of this file, via any medium is prohibited.
 * Proprietary and confidential.
 * Written by Robin Campos <magyk81@gmail.com>, year 2021.
 */

using System.Collections.Generic;
using UnityEngine;
using Network.SignalsFromHost;

namespace Matches.UX {
    public class Board : MonoBehaviour {
        public static readonly int DIST_BETWEEN_BOARDS = 200;

        [SerializeField]
        private int cloneLength;
        [SerializeField]
        Chunk baseChunk;
        [SerializeField]
        Piece basePiece;
        private Transform mPieceParent;
        private Tile[,] mTiles;
        private Chunk[,] mChunks;
        private readonly Dictionary<int, Piece> mPieces = new Dictionary<int, Piece>();
        private int mBoardID, mApartOffset;
        private float[] mBounds = new float[4];
        private Coord mOffset;

        public Chunk[,] Chunks { get => mChunks; }

        public float[] Bounds { get => mBounds; }
        public Coord Offset { get => mOffset; }


        /// <summary>
        /// Called once before the match begins.
        /// </summary>
        public void Init(int size, int layerCount, int boardID, int cloneIdx, Board realBoard) {

            mBoardID = boardID;

            int i0 = 0, j0 = 0, iEnd = size, jEnd = size;
            int cloneOffsetX = 0, cloneOffsetZ = 0,
            mApartOffset = boardID * DIST_BETWEEN_BOARDS;
            int totalSize = size * Matches.Chunk.SIZE;
            if (cloneIdx > 0) {
                if (cloneLength <= 0 || cloneLength > size) cloneLength = size;
                if (cloneIdx == Util.UP + 1 || cloneIdx == Util.UP_LEFT + 1 || cloneIdx == Util.UP_RIGHT + 1) {
                    jEnd = cloneLength;
                    cloneOffsetZ = totalSize;
                }
                if (cloneIdx == Util.DOWN + 1 || cloneIdx == Util.DOWN_LEFT + 1 || cloneIdx == Util.DOWN_RIGHT + 1) {
                    j0 = size - cloneLength;
                    cloneOffsetZ = -totalSize;
                }
                if (cloneIdx == Util.RIGHT + 1 || cloneIdx == Util.UP_RIGHT + 1 || cloneIdx == Util.DOWN_RIGHT + 1) {
                    iEnd = cloneLength;
                    cloneOffsetX = totalSize;
                } if (cloneIdx == Util.LEFT + 1 || cloneIdx == Util.UP_LEFT + 1 || cloneIdx == Util.DOWN_LEFT + 1) {
                    i0 = size - cloneLength;
                    cloneOffsetX = -totalSize;
                }
            }

            // Set offset.
            mOffset = Coord._(cloneOffsetX + mApartOffset, cloneOffsetZ);

            mTiles = new Tile[totalSize, totalSize];
            mChunks = new Chunk[size, size];

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
                    Coord chunkPos = Coord._((i * chunkSize) + mOffset.X, (j * chunkSize) + mOffset.Z);
                    chunkTra.localPosition = new Vector3(
                        chunkPos.X + ((float) chunkSize / 2F), 0, chunkPos.Z + ((float) chunkSize / 2F));

                    // Generate tiles.
                    Tile[,] chunkTiles = new Tile[chunkSize, chunkSize];
                    for (int a = 0; a < chunkSize; a++) {
                        for (int b = 0; b < chunkSize; b++) {
                            Coord tilePos = Coord._(i * chunkSize + a, j * chunkSize + b);
                            mTiles[tilePos.X, tilePos.Z] = new Tile(
                                tilePos, size * chunkSize, mApartOffset, cloneIdx, boardID);
                            chunkTiles[tilePos.X - (i * chunkSize), tilePos.Z - (j * chunkSize)] =
                                mTiles[tilePos.X, tilePos.Z];
                            
                            if (cloneIdx > 0)
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

            // Set bounds.
            mBounds[Util.UP] = mTiles[0, mTiles.GetLength(1) - 1].UX_Pos.z + 0.5F;
            mBounds[Util.RIGHT] = mTiles[mTiles.GetLength(0) - 1, 0].UX_Pos.x + 0.5F;
            mBounds[Util.DOWN] = mTiles[0, 0].UX_Pos.z - 0.5F;
            mBounds[Util.LEFT] = mTiles[0, 0].UX_Pos.x - 0.5F;

            gameObject.SetActive(true);
        }

        /// <returns>
        /// <c>null</c> if no piece is hovered
        /// </returns>
        public Piece GetHoveredPiece(float[][] rayPiecePoints) {
            // Check just the middle ray point (first in the array).
            foreach (Piece piece in mPieces.Values) {
                if (piece.Contains(rayPiecePoints[0][0] - mApartOffset, rayPiecePoints[0][1])) return piece;
            }
            // Check the other points if no piece contained the middle ray point.
            foreach (Piece piece in mPieces.Values) {
                for (int i = 1; i < rayPiecePoints.Length; i++) {
                    if (piece.Contains(rayPiecePoints[i][0] - mApartOffset, rayPiecePoints[i][1])) return piece;
                }
            }
            return null;
        }

        public Tile GetHoveredTile(float[] rayTilePoint) {
            return mTiles[(int) rayTilePoint[0] - mApartOffset, (int) rayTilePoint[1]];
        }

        public Piece GetPiece(int pieceID) { return mPieces[pieceID]; }
        public Tile GetTile(Coord coord) { return mTiles[coord.X, coord.Z]; }

        public Piece AddPiece(SignalAddPiece signal, string pieceName, int layerCount) {
            Piece uxPiece = Instantiate(basePiece.gameObject, mPieceParent).GetComponent<Piece>();
            uxPiece.gameObject.name = pieceName;
            mPieces.Add(signal.PieceID, uxPiece);
            uxPiece.Init(signal, pieceName, layerCount);
            MovePiece(signal.PieceID, signal.Tile);
            return uxPiece;
        }

        public void AddCards(SignalAddCards signal) { mPieces[signal.HolderPieceID].AddCards(signal.CardIDs); }

        public void RemoveCards(SignalRemoveCards signal) { mPieces[signal.HolderPieceID].RemoveCards(signal.CardIDs); }

        /// <summary>
        /// Updates the piece's position on the board.
        /// </summary>
        public void MovePiece(int pieceID, Coord tile) {
            mPieces[pieceID].SetPos(mTiles[tile.X, tile.Z], mTiles[tile.X, tile.Z], 1);
        }

        public void UpdateWaypoints(int pieceID, Coord[] coords) {
            Tile[] waypointTiles = new Tile[coords.Length];
            Piece[] waypointPieces = new Piece[coords.Length];
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

            Matches.Waypoints.Waypoint[] waypoints = new Matches.Waypoints.Waypoint[coords.Length];
            for (int i = 0; i < coords.Length; i++) {
                // waypoints[i] = Matches.Waypoints.Waypoint.CoordToUX(coords[i], this);
                if (coords[i] == Coord.Null) waypoints[i] = null;
                else if (coords[i].X == -1) waypoints[i] = new UX.Waypoints.WaypointPiece(GetPiece(coords[i].Z));
                else waypoints[i] = new UX.Waypoints.WaypointTile(GetTile(coords[i]));
            }

            mPieces[pieceID].Waypoints = waypoints;
        }

        // Update is called once per frame
        private void Update() { }
    }
}