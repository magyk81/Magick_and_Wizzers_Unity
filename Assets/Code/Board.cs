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
    private static class InitInfo
    {
        public static int size;
    }
    private int customSize = 0;
    public static int Size { set { InitInfo.size = value; } }
    public int GetSize()
    {
        if (customSize > 0) return customSize;
        else return InitInfo.size;
    }

    private readonly int TOTAL_SIZE;
    private Chunk[,] chunks;
    private List<Piece> pieces = new List<Piece>();
    private string name;
    public string Name { get { return name; } }
    public Board(string name, int customSize = 0)
    {
        this.name = name;
        this.customSize = customSize;
        TOTAL_SIZE = Chunk.Size * InitInfo.size;
        chunks = new Chunk[GetSize(), GetSize()];
        for (int i = 0; i < GetSize(); i++)
        {
            for (int j = 0; j < GetSize(); j++)
            {
                chunks[i, j] = new Chunk(Coord._(i, j));
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
                players[i], i, 0, masterStartPos[i], masterTex);
            AddPiece(initialMaster);

            // The 5 cards that players start with at the beginning of a match.
            initialMaster.DrawCards(20);
        }
    }

    /// <returns>The chunk that the tile belongs to.
    /// </returns>
    public Coord TileToChunk(Coord tile)
    {
        return Coord._(tile.X / Chunk.Size, tile.Z / Chunk.Size);
    }

    public void AddPiece(Piece piece)
    {
        pieces.Add(piece);
        Match.AddSkinTicket(new SkinTicket(piece, SkinTicket.Type.ADD_PIECE));
    }
}
