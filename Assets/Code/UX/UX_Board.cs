/* Copyright (C) All Rights Reserved
 * Unauthorized copying of this file, via any medium is prohibited.
 * Proprietary and confidential.
 * Written by Robin Campos <magyk81@gmail.com>, year 2021.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UX_Board : MonoBehaviour
{
    [SerializeField]
    private int distBetweenBoards, cloneLength;
    [SerializeField]
    UX_Chunk baseChunk;
    [SerializeField]
    UX_Piece basePiece;
    private Transform pieceParent;
    private UX_Tile[,] tiles;
    private UX_Chunk[,] chunks;
    public UX_Chunk[,] Chunks { get { return chunks; } }
    private Dictionary<int, UX_Piece> pieces
        = new Dictionary<int, UX_Piece>();
    private int boardID;

    /// <summary>Called once before the match begins.</summary>
    public void Init(int size, int totalSize, int layerCount, int boardID,
        int cloneIdx = -1, UX_Board realBoard = null)
    {
        this.boardID = boardID;

        int i0 = 0, j0 = 0, iEnd = size, jEnd = size;
        int cloneOffsetX = 0, cloneOffsetZ = 0,
            apartOffset = boardID * distBetweenBoards;
        if (cloneIdx >= 0)
        {
            if (cloneLength <= 0 || cloneLength > size) cloneLength = size;
            if (cloneIdx == Util.UP || cloneIdx == Util.UP_LEFT
                || cloneIdx == Util.UP_RIGHT)
            {
                jEnd = cloneLength;
                cloneOffsetZ = totalSize;
            }
            if (cloneIdx == Util.DOWN || cloneIdx == Util.DOWN_LEFT
                || cloneIdx == Util.DOWN_RIGHT)
            {
                j0 = size - cloneLength;
                cloneOffsetZ = -totalSize;
            }
            if (cloneIdx == Util.RIGHT || cloneIdx == Util.UP_RIGHT
                || cloneIdx == Util.DOWN_RIGHT)
            {
                iEnd = cloneLength;
                cloneOffsetX = totalSize;
            }
            if (cloneIdx == Util.LEFT || cloneIdx == Util.UP_LEFT
                || cloneIdx == Util.DOWN_LEFT)
            {
                i0 = size - cloneLength;
                cloneOffsetX = -totalSize;
            }
        }

        tiles = new UX_Tile[totalSize, totalSize];
        chunks = new UX_Chunk[size, size];

        Transform chunkParent = new GameObject().GetComponent<Transform>();
        chunkParent.gameObject.name = "Chunks";
        chunkParent.parent = GetComponent<Transform>();

        // Load alternative chunk material if Sheol (temporary debugging)
        Material altChunkMat = null;
        if (boardID == 1)
        {
            altChunkMat = Resources.Load<Material>(
                "Materials/Debug Chunk Sheol");
        }

        // Generate chunks.
        for (int i = i0; i < iEnd; i++)
        {
            for (int j = j0; j < jEnd; j++)
            {
                chunks[i, j] = Instantiate(baseChunk, chunkParent);

                // Set alternative chunk material if it's been loaded.
                // Material set in prefab is used otherwise.
                if (altChunkMat != null)
                {
                    chunks[i, j].GetComponent<MeshRenderer>().material
                        = altChunkMat;
                }
                
                // Position and scale chunks.
                int chunkSize = totalSize / size;
                Transform chunkTra = chunks[i, j].GetComponent<Transform>();
                chunkTra.localScale = new Vector3(
                    chunkSize, chunkSize, 1);
                Coord chunkPos = Coord._(
                    (i * chunkSize) + cloneOffsetX + apartOffset,
                    (j * chunkSize) + cloneOffsetZ);
                chunkTra.localPosition = new Vector3(
                    chunkPos.X + ((float) chunkSize / 2F),
                    0,
                    chunkPos.Z + ((float) chunkSize / 2F));

                // Generate tiles.
                UX_Tile[,] chunkTiles = new UX_Tile[chunkSize, chunkSize];
                for (int a = 0; a < chunkSize; a++)
                {
                    for (int b = 0; b < chunkSize; b++)
                    {
                        Coord tilePos = Coord._(
                            i * chunkSize + a, j * chunkSize + b);
                        tiles[tilePos.X, tilePos.Z] = new UX_Tile(
                            tilePos, size * chunkSize, apartOffset,
                            cloneIdx, boardID);
                        chunkTiles[
                            tilePos.X - (i * chunkSize),
                            tilePos.Z - (j * chunkSize)]
                            = tiles[tilePos.X, tilePos.Z];
                        
                        if (cloneIdx != -1)
                        {
                            realBoard.tiles[tilePos.X, tilePos.Z].AddClone(
                                tiles[tilePos.X, tilePos.Z], cloneIdx);
                        }
                    }
                }

                chunks[i, j].Init(Coord._(i, j), boardID, chunkTiles, layerCount);
                chunks[i, j].gameObject.SetActive(true);
            }
        }

        pieceParent = new GameObject().GetComponent<Transform>();
        pieceParent.gameObject.name = "Pieces";
        pieceParent.parent = GetComponent<Transform>();

        gameObject.SetActive(true);
    }

    public float[] GetBounds()
    {
        float[] bounds = new float[4];
        bounds[Util.UP] = tiles[0, tiles.GetLength(1) - 1].UX_Pos.z + 0.5F;
        bounds[Util.RIGHT] = tiles[tiles.GetLength(0) - 1, 0].UX_Pos.x + 0.5F;
        bounds[Util.DOWN] = tiles[0, 0].UX_Pos.z - 0.5F;
        bounds[Util.LEFT] = tiles[0, 0].UX_Pos.x - 0.5F;
        return bounds;
    }

    public void AddPiece(SignalFromHost signal, string pieceName,
        int layerCount)
    {
        UX_Piece uxPiece = Instantiate(
            basePiece.gameObject,
            pieceParent)
            .GetComponent<UX_Piece>();
        uxPiece.gameObject.name = pieceName;
        pieces.Add(signal.PieceID, uxPiece);
        uxPiece.Init(signal, pieceName, layerCount);
        MovePiece(signal.PieceID, signal.Tile);
    }

    public void AddCard(SignalFromHost signal)
    {
        pieces[signal.PieceID].AddCard(signal.CardID);
    }

    public void RemoveCard(SignalFromHost signal)
    {
        pieces[signal.PieceID].RemoveCard(signal.CardID);
    }

    /// <summary>Updates the piece's position on the board between the tiles
    /// indicated by the piece's PosPrecise info.</summary>
    public void MovePiece(int pieceID, Coord tile)
    {
        pieces[pieceID].SetPos(
            tiles[tile.X, tile.Z],
            tiles[tile.X, tile.Z],
            1);
    }

    public void UpdateWaypoints(int pieceID, Coord[] coords)
    {
        UX_Tile[] waypoints = new UX_Tile[coords.Length];
        for (int i = 0; i < coords.Length; i++)
        {
            if (coords[i] == Coord.Null) waypoints[i] = null;
            else waypoints[i] = tiles[coords[i].X, coords[i].Z];
        }
        pieces[pieceID].UpdateWaypoints(waypoints);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
