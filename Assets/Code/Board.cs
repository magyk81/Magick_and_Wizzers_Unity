/* Copyright (C) All Rights Reserved
 * Unauthorized copying of this file, via any medium is prohibited.
 * Proprietary and confidential.
 * Written by Robin Campos <magyk81@gmail.com>, year 2021.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board
{
    private readonly int SIZE;
    private readonly int TOTAL_SIZE;
    public int Size { get { return SIZE; } }
    public int TotalSize { get { return TOTAL_SIZE; } }
    private Chunk[,] chunks;
    private List<Piece> pieces = new List<Piece>();
    private string name;
    public string Name { get { return name; } }
    private int idx;
    public int Idx { get { return idx; } }
    public Board(string name, int idx, int size, int chunkSize)
    {
        this.name = name;
        this.idx = idx;
        SIZE = size;
        TOTAL_SIZE = size * chunkSize;
        chunks = new Chunk[size, size];
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                chunks[i, j] = new Chunk(Coord._(i, j), chunkSize);
            }
        }
    }

    /// <summary>Adds 1 master to the boards for each player. The masters'
    ///     starting positions depend on the number of players.</summary>
    public void InitMasters(Player[] players)
    {
        Coord[] masterStartPos = new Coord[players.Length];
        if (players.Length == 2) masterStartPos = new Coord[] {
            Coord._(TOTAL_SIZE / 4    , TOTAL_SIZE / 4),
            Coord._(TOTAL_SIZE / 4 * 3, TOTAL_SIZE / 4 * 3) };
        else if (players.Length == 3) masterStartPos = new Coord[] {
            Coord._(TOTAL_SIZE / 2    , TOTAL_SIZE / 6),
            Coord._(TOTAL_SIZE / 2    , TOTAL_SIZE / 2),
            Coord._(TOTAL_SIZE / 2    , TOTAL_SIZE / 6 * 5) };
        else if (players.Length == 4) masterStartPos = new Coord[] {
            Coord._(TOTAL_SIZE / 4    , TOTAL_SIZE / 4),
            Coord._(TOTAL_SIZE / 4 * 3, TOTAL_SIZE / 4),
            Coord._(TOTAL_SIZE / 4    , TOTAL_SIZE / 4 * 3),
            Coord._(TOTAL_SIZE / 4 * 3, TOTAL_SIZE / 4 * 3) };
        for (int i = 0; i < players.Length; i++)
        {
            Texture masterTex = Resources.Load<Texture>(
                "Textures/Debug_Card_Art/Master_" + players[i].Name);

            Master initialMaster = new Master(
                players[i], i, 0, masterTex);
            AddPiece(initialMaster, masterStartPos[i]);

            // The 5 cards that players start with at the beginning of a match.
            initialMaster.DrawCards(20);
        }
    }

    /// <returns>The chunk that the tile belongs to.
    /// </returns>
    public Coord TileToChunk(Coord tile)
    {
        int chunkSize = chunks[0, 0].Size;
        return Coord._(tile.X / chunkSize, tile.Z / chunkSize);
    }

    /// <returns>True if the piece was successfully added. False otherwise.
    /// </returns>
    public bool AddPiece(int playerIdx, Card card, Coord tile)
    {
        Piece piece = new Piece(playerIdx, idx, card);
        return AddPiece(piece, tile);
    }
    private bool AddPiece(Piece piece, Coord tile)
    {
        foreach (Piece p in pieces)
        {
            if (p.Pos == piece.Pos) return false;
        }
        piece.Pos = tile;
        piece.BoardTotalSize = TOTAL_SIZE;
        pieces.Add(piece);
        // Match.AddSkinTicket(new Signal(piece, Signal.Type.ADD_PIECE));
        return true;
    }

    public void Update()
    {
        foreach (Piece piece in pieces)
        {
            piece.Update();
        }
    }
}
