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

    private SignalAddPiece AddPiece(Piece piece) {
        // If the piece to be added would overlap the positions of any other piece.
        // TODO: Check for overlap, not just if the positions are equal.
        foreach (Piece p in mPieces) {
            if (p.Pos == piece.Pos) return null;
        }
        mPieces.Add(piece);
        mIdToPiece.Add(piece.ID, piece);
        return new SignalAddPiece(piece);
    }

    public SignalUpdateWaypoints AddWaypoint(SignalAddWaypoint signal) {
        // Get the piece that will be added the waypoint.
        Piece piece = mIdToPiece[signal.PieceID];

        // Add waypoint for piece if X value is -1, else add waypoint for tile.
        if (signal.TargetTile.X == -1) {
            piece.AddWaypoint(mIdToPiece[signal.TargetTile.Z], signal.OrderPlace);
        } else piece.AddWaypoint(signal.TargetTile, signal.OrderPlace);
        
        return new SignalUpdateWaypoints(piece);
    }

    public SignalUpdateWaypoints RemoveWaypoint(SignalRemoveWaypoint signal) {
        // Get the piece that will be removed the waypoint.
        Piece piece = mIdToPiece[signal.PieceID];

        // Remove waypoint for piece based solely on order place.
        piece.RemoveWaypoint(signal.OrderPlace);

        return new SignalUpdateWaypoints(piece);
    }

    public SignalFromHost[] CastSpell(SignalCastSpell signal) {
        Piece caster = mIdToPiece[signal.CasterID];
        if (signal.ActingPlayerID == caster.PlayerID) {
            Card playCard = Card.friend_cards[signal.PlayCardID];

            SignalRemoveCard signalRemoveCard = caster.CastSpell(playCard);
            if (signalRemoveCard != null) {
                SignalAddPiece signalAddPiece = AddPiece(new Piece(caster.ID, ID, signal.Tile, playCard));
                // If the spell resolved, return SignalRemoveCard and SignalAddPiece.
                if (signalAddPiece != null) return new SignalFromHost[] { signalRemoveCard, signalAddPiece };
            }
        }
        return null;
    }

    public void Update() {
        foreach (Piece piece in mPieces) {
            piece.Update();
        }
    }
}
