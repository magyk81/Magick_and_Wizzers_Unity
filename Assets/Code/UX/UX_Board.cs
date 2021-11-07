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
    private Dictionary<Piece, UX_Piece> pieces
        = new Dictionary<Piece, UX_Piece>();
    private int boardIdx, cloneIdx;

    /// <summary>Called once before the match begins.</summary>
    public void Init(int size, int boardIdx,
        int cloneIdx = -1, UX_Board realBoard = null)
    {
        this.boardIdx = boardIdx;
        this.cloneIdx = cloneIdx;

        int i0 = 0, j0 = 0, iEnd = size, jEnd = size;
        int cloneOffsetX = 0, cloneOffsetZ = 0,
            apartOffset = boardIdx * distBetweenBoards;
        if (cloneIdx >= 0)
        {
            if (cloneLength <= 0 || cloneLength > size) cloneLength = size;
            if (cloneIdx == Util.UP || cloneIdx == Util.UP_LEFT
                || cloneIdx == Util.UP_RIGHT)
            {
                jEnd = cloneLength;
                cloneOffsetZ = size * Chunk.Size;
            }
            if (cloneIdx == Util.DOWN || cloneIdx == Util.DOWN_LEFT
                || cloneIdx == Util.DOWN_RIGHT)
            {
                j0 = size - cloneLength;
                cloneOffsetZ = -size * Chunk.Size;
            }
            if (cloneIdx == Util.RIGHT || cloneIdx == Util.UP_RIGHT
                || cloneIdx == Util.DOWN_RIGHT)
            {
                iEnd = cloneLength;
                cloneOffsetX = size * Chunk.Size;
            }
            if (cloneIdx == Util.LEFT || cloneIdx == Util.UP_LEFT
                || cloneIdx == Util.DOWN_LEFT)
            {
                i0 = size - cloneLength;
                cloneOffsetX = -size * Chunk.Size;
            }
        }

        tiles = new UX_Tile[size * Chunk.Size, size * Chunk.Size];
        chunks = new UX_Chunk[size, size];

        Transform chunkParent = new GameObject().GetComponent<Transform>();
        chunkParent.gameObject.name = "Chunks";
        chunkParent.parent = GetComponent<Transform>();

        // Load alternative chunk material if Sheol (temporary debugging)
        Material altChunkMat = null;
        if (boardIdx == 1)
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
                Transform chunkTra = chunks[i, j].GetComponent<Transform>();
                chunkTra.localScale = new Vector3(
                    Chunk.Size, Chunk.Size, 1);
                Coord chunkPos = Coord._(
                    (i * Chunk.Size) + cloneOffsetX + apartOffset,
                    (j * Chunk.Size) + cloneOffsetZ);
                chunkTra.localPosition = new Vector3(
                    chunkPos.X + ((float) Chunk.Size / 2F),
                    0,
                    chunkPos.Z + ((float) Chunk.Size / 2F));

                // Generate tiles.
                UX_Tile[,] chunkTiles = new UX_Tile[Chunk.Size, Chunk.Size];
                for (int a = 0; a < Chunk.Size; a++)
                {
                    for (int b = 0; b < Chunk.Size; b++)
                    {
                        Coord tilePos = Coord._(
                            i * Chunk.Size + a, j * Chunk.Size + b);
                        tiles[tilePos.X, tilePos.Z] = new UX_Tile(
                            tilePos, size * Chunk.Size, apartOffset,
                            cloneIdx, boardIdx);
                        chunkTiles[
                            tilePos.X - (i * Chunk.Size),
                            tilePos.Z - (j * Chunk.Size)]
                            = tiles[tilePos.X, tilePos.Z];
                        
                        if (cloneIdx != -1)
                        {
                            realBoard.tiles[tilePos.X, tilePos.Z].AddClone(
                                tiles[tilePos.X, tilePos.Z], cloneIdx);
                        }
                    }
                }

                chunks[i, j].Init(Coord._(i, j), boardIdx, chunkTiles);
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

    public void AddPiece(Piece piece)
    {
        UX_Piece uxPiece = Instantiate(
            basePiece.gameObject,
            pieceParent)
            .GetComponent<UX_Piece>();
        uxPiece.gameObject.name = piece.Name;
        pieces.Add(piece, uxPiece);

        uxPiece.Init(piece);
        MovePiece(piece);
    }

    /// <summary>Updates the piece's position on the board between the tiles
    /// indicated by the piece's PosPrecise info.</summary>
    public void MovePiece(Piece piece)
    {
        if (boardIdx != piece.BoardIdx)
        {
            Debug.LogError("Board " + boardIdx
                + " should not move piece from board " + piece.BoardIdx);
        }
        else
        {            
            pieces[piece].SetPos(
                tiles[piece.Pos.X, piece.Pos.Z],
                tiles[piece.Pos.X, piece.Pos.Z],
                1);
        }
    }

    public void UpdateWaypoints(Piece piece, Coord[] coords)
    {
        UX_Tile[] waypoints = new UX_Tile[coords.Length];
        for (int i = 0; i < coords.Length; i++)
        {
            if (coords[i] == Coord.Null) waypoints[i] = null;
            else waypoints[i] = tiles[coords[i].X, coords[i].Z];
        }
        pieces[piece].UpdateWaypoints(waypoints);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
