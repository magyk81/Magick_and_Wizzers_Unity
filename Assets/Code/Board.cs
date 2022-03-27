/* Copyright (C) All Rights Reserved
 * Unauthorized copying of this file, via any medium is prohibited.
 * Proprietary and confidential.
 * Written by Robin Campos <magyk81@gmail.com>, year 2021.
 */

using System.Collections.Generic;
using UnityEngine;

public class Board {
    public readonly int ID;

    private readonly int mSize;
    private readonly int mTotalSize;
    private readonly string mName;

    private readonly Chunk[,] mChunks;
    private readonly List<Piece> mPieces = new List<Piece>();
    private readonly Dictionary<int, Piece> mIdToPiece = new Dictionary<int, Piece>();

    public Board(string name, int size) {
        ID = IdHandler.Create(GetType());
        mName = name;
        mSize = size;
        mTotalSize = size * Chunk.SIZE;
        mChunks = new Chunk[size, size];
        for (int i = 0; i < size; i++) {
            for (int j = 0; j < size; j++) {
                mChunks[i, j] = new Chunk(Coord._(i, j));
            }
        }
    }

    public int Size { get { return mSize; } }
    public int TotalSize { get { return mTotalSize; } }
    public string Name { get { return mName; } }

    /// <summary>
    /// Adds 1 master to the boards for each player. The masters' starting positions depend on the number of players.
    /// </summary>
    public SignalFromHost[] InitMasters(Player[] players, int startingHandCount) {
        Coord[] masterStartPos = new Coord[players.Length];
        if (players.Length == 2) masterStartPos = new Coord[] {
            Coord._(mTotalSize / 4    , mTotalSize / 4),
            Coord._(mTotalSize / 4 * 3, mTotalSize / 4 * 3) };
        else if (players.Length == 3) masterStartPos = new Coord[] {
            Coord._(mTotalSize / 2    , mTotalSize / 6),
            Coord._(mTotalSize / 2    , mTotalSize / 2),
            Coord._(mTotalSize / 2    , mTotalSize / 6 * 5) };
        else if (players.Length == 4) masterStartPos = new Coord[] {
            Coord._(mTotalSize / 4    , mTotalSize / 4),
            Coord._(mTotalSize / 4 * 3, mTotalSize / 4),
            Coord._(mTotalSize / 4    , mTotalSize / 4 * 3),
            Coord._(mTotalSize / 4 * 3, mTotalSize / 4 * 3) };
        SignalFromHost[] signals = new SignalFromHost[
            players.Length * (startingHandCount + 1)];
        for (int i = 0; i < players.Length; i++) {
            Texture masterTex = Resources.Load<Texture>(
                "Textures/Debug_Card_Art/Master_" + players[i].Name);

            int signalIdx = i * (startingHandCount + 1);

            Master initialMaster = new Master(
                players[i], 0, masterStartPos[i], masterTex);
            signals[signalIdx] = AddPiece(initialMaster);

            // The 5 cards that players start with at the beginning of a match.
            SignalFromHost[] drawCardSignals = initialMaster.DrawCards(
                startingHandCount);
            for (int j = 0; j < drawCardSignals.Length; j++) {
                signals[j + 1 + signalIdx] = drawCardSignals[j];
            }
        }
        return signals;
    }

    /// <returns>
    /// The chunk that the tile belongs to.
    /// </returns>
    public Coord TileToChunk(Coord tile) {
        return Coord._(tile.X / Chunk.SIZE, tile.Z / Chunk.SIZE);
    }

    private SignalFromHost AddPiece(Piece piece) {
        foreach (Piece p in mPieces) {
            if (p.Pos == piece.Pos) return null;
        }
        mPieces.Add(piece);
        mIdToPiece.Add(piece.ID, piece);
        return new SignalAddPiece(piece);
    }

    public SignalFromHost[] AddWaypoint(SignalAddWaypoint signal, bool add) {
        List<SignalFromHost> waypointUpdates = new List<SignalFromHost>();

        Piece[] pieces = new Piece[signal.PieceIDs.Length];
        for (int i = 0; i < signal.PieceIDs.Length; i++)
        { pieces[i] = mIdToPiece[signal.PieceIDs[i]]; }

        // Check to see if the pieces have different waypoints.
        bool waypointsCommon = true;
        if (signal.PieceIDs.Length > 1) {
            for (int i = 1; i < signal.PieceIDs.Length; i++) {
                if (!pieces[i - 1].HasSameWaypoints(pieces[i])) {
                    waypointsCommon = false;
                    break;
                }
            }
        }
        
        /* If pieces have different waypoints, clear all of their waypoints before adding the new ones, and no need to
         * remove waypoints. */
        if (!waypointsCommon) {
            for (int i = 0; i < signal.PieceIDs.Length; i++) {
                pieces[i].ClearWaypoints();
            }
        }
        
        // Add/remove waypoints.
        // for (int i = 0; i < signal.PieceIDs.Length; i++) {
        //     if (pieces[i].PlayerID == signal.ActingPlayerID) {
        //         if (add) {
        //             Debug.Log("signal.PieceID: " + signal.PieceID);
        //             Piece waypointTarget = (signal.PieceID == -1) ? null : mIdToPiece[signal.PieceID];
        //             pieces[i].AddWaypoint(waypointTarget, signal.OrderPlace);
        //         } else if (!waypointsCommon) pieces[i].RemoveWaypoint(signal.OrderPlace);
                
        //         waypointUpdates.Add(new SignalUpdateWaypoints(pieces[i]));
        //     } else {
        //         Debug.Log("Error: Attempted to add waypoint to the piece " + pieces[i].Name
        //             + " but it does not belong to player #" + signal.ActingPlayerID);
        //     }
        // }
        return waypointUpdates.ToArray();
    }

    public SignalFromHost[] CastSpell(SignalCastSpell signal) {
        Piece caster = mIdToPiece[signal.CasterID];
        if (signal.ActingPlayerID == caster.PlayerID) {
            Card playCard = Card.friend_cards[signal.PlayCardID];
            SignalFromHost[] signals = caster.CastSpell(playCard, this, signal.Tile);
            for (int i = 0; i < signals.Length; i++) {
                if (signals[i] == null) { signals[i] = AddPiece(new Piece(caster.ID, ID, signal.Tile, playCard)); }
            }
            return signals;
        }
        return null;
    }

    public void Update() {
        foreach (Piece piece in mPieces) {
            piece.Update();
        }
    }
}
