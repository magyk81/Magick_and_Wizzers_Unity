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
    public readonly int ID;

    private readonly int SIZE;
    private readonly int TOTAL_SIZE;
    public int Size { get { return SIZE; } }
    public int TotalSize { get { return TOTAL_SIZE; } }
    private Chunk[,] chunks;
    private List<Piece> pieces = new List<Piece>();
    private Dictionary<int, Piece> piecesWithID = new Dictionary<int, Piece>();
    private string name;
    public string Name { get { return name; } }
    public Board(string name, int size, int chunkSize)
    {
        ID = IdHandler.Create(GetType());
        this.name = name;
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
    public SignalFromHost[] InitMasters(
        Player[] players, int startingHandCount)
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
        SignalFromHost[] signals = new SignalFromHost[
            players.Length * (startingHandCount + 1)];
        for (int i = 0; i < players.Length; i++)
        {
            Texture masterTex = Resources.Load<Texture>(
                "Textures/Debug_Card_Art/Master_" + players[i].Name);

            int signalIdx = i * (startingHandCount + 1);

            Master initialMaster = new Master(
                players[i], i, 0, masterStartPos[i], masterTex);
            signals[signalIdx] = AddPiece(initialMaster);

            // The 5 cards that players start with at the beginning of a match.
            SignalFromHost[] drawCardSignals = initialMaster.DrawCards(
                startingHandCount);
            for (int j = 0; j < drawCardSignals.Length; j++)
            {
                signals[j + 1 + signalIdx] = drawCardSignals[j];
            }
        }
        return signals;
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
    public SignalFromHost AddPiece(int playerID, Coord tile, Card card)
    {
        Piece piece = new Piece(playerID, ID, tile, card);
        return AddPiece(piece);
    }
    private SignalFromHost AddPiece(Piece piece)
    {
        foreach (Piece p in pieces)
        {
            if (p.Pos == piece.Pos) return null;
        }
        piece.BoardTotalSize = TOTAL_SIZE;
        pieces.Add(piece);
        piecesWithID.Add(piece.ID, piece);
        Debug.Log("piece.ID: " + piece.ID);
        return SignalFromHost.AddPiece(piece);
    }

    public SignalFromHost[] SetWaypoint(SignalFromClient signal, bool add)
    {
        List<SignalFromHost> waypointUpdates = new List<SignalFromHost>();

        Piece[] pieces = new Piece[signal.PieceIDs.Length];
        for (int i = 0; i < signal.PieceIDs.Length; i++)
        { pieces[i] = piecesWithID[signal.PieceIDs[i]]; }

        // Check to see if the pieces have different waypoints.
        bool waypointsCommon = true;
        if (signal.PieceIDs.Length > 1)
        {
            for (int i = 1; i < signal.PieceIDs.Length; i++)
            {
                if (!pieces[i - 1].HasSameWaypoints(pieces[i]))
                {
                    waypointsCommon = false;
                    break;
                }
            }
        }
        
        // If pieces have different waypoints, clear all of their waypoints
        // before adding the new ones, and no need to remove waypoints.
        if (!waypointsCommon)
        {
            for (int i = 0; i < signal.PieceIDs.Length; i++)
            {
                pieces[i].ClearWaypoints();
            }
        }
        
        // Add/remove waypoints.
        for (int i = 0; i < signal.PieceIDs.Length; i++)
        {
            if (pieces[i].PlayerID == signal.ActingPlayerID)
            {
                if (add)
                {
                    Debug.Log("signal.PieceID: " + signal.PieceID);
                    Piece waypointTarget = (signal.PieceID == -1)
                        ? null : piecesWithID[signal.PieceID];
                    pieces[i].AddWaypoint(signal.Tile, waypointTarget,
                        signal.OrderPlace);
                }
                else if (!waypointsCommon)
                    pieces[i].RemoveWaypoint(signal.OrderPlace);
                
                waypointUpdates.Add(SignalFromHost.UpdateWaypoints(pieces[i]));
            }
            else
            {
                Debug.Log("Error: Attempted to add waypoint to the piece "
                    + pieces[i].Name + " but it does not belong to player #"
                    + signal.ActingPlayerID);
            }
        }
        return waypointUpdates.ToArray();
    }

    public SignalFromHost[] CastSpell(SignalFromClient signal,
        Board boardCastedOn)
    {
        Piece caster = piecesWithID[signal.PieceID];
        if (signal.ActingPlayerID == caster.PlayerID)
        {
            Card card = Card.friend_cards[signal.CardID];


            SignalFromHost[] signals = caster.CastSpell(card, boardCastedOn,
                signal.Tile);
            for (int i = 0; i < signals.Length; i++)
            {
                if (signals[i] == null)
                {
                    Piece piece = new Piece(caster.ID, ID, signal.Tile, card);
                    AddPiece(piece);
                    signals[i] = SignalFromHost.AddPiece(piece);
                }
            }
            return signals;
        }
        return null;
    }

    public void Update()
    {
        foreach (Piece piece in pieces)
        {
            piece.Update();
        }
    }
}
