/* Copyright (C) All Rights Reserved
 * Unauthorized copying of this file, via any medium is prohibited.
 * Proprietary and confidential.
 * Written by Robin Campos <magyk81@gmail.com>, year 2021.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignalFromHost : Signal
{
    private SignalFromHost(params int[] intMessage) : base(intMessage)
    {
        if (intMessage.Length == 0) return;

        // Set the message (byte array).
        int messageLength = 1 + (intMessage.Length * sizeof(int));
        message = new byte[messageLength];

        // First byte is message length.
        message[0] = (byte) (messageLength - 1);

        for (int i = 1, j = 0; i < messageLength;
            i += sizeof(int), j++)
        {
            byte[] intAsBytes = BitConverter.GetBytes(intMessage[j]);
            for (int k = 0; k < sizeof(int); k++)
            {
                message[i + k] = intAsBytes[k];
            }
        }
    }

    public enum Request { SET_CHUNK_SIZE, ADD_PLAYER, ADD_BOARD,
        ADD_PIECE, REMOVE_PIECE, MOVE_PIECE,
        ADD_CARD, REMOVE_CARD,
        UPDATE_WAYPOINTS }
    
    #region member getters
    private Request request;
    private bool isBot;
    private int clientID, playerID, boardID, pieceID, cardID, size, chunkSize,
        tileLerp, orderPlace, pieceType;
    private Coord tile = Coord.Null, tileNext = Coord.Null,
        tilePrev = Coord.Null;
    private Coord[] tiles;
    private string name, artPath;
    public Request HostRequest { get { return request; } }
    public bool IsBot { get { return isBot; } }
    public int ClientID { get { return clientID; } }
    public int PlayerID { get { return playerID; } }
    public int BoardID { get { return boardID; } }
    public int PieceID { get { return pieceID; } }
    public int CardID { get { return cardID; } }
    public int Size { get { return size; } }
    public int ChunkSize { get { return chunkSize; } }
    public int TileLerp { get { return tileLerp; } }
    public int OrderPlace { get { return orderPlace; } }
    public int PieceType { get { return pieceType; } }
    public Coord Tile { get { return tile.Copy(); } }
    public Coord TileNext { get { return tileNext.Copy(); } }
    public Coord TilePrev { get { return tilePrev.Copy(); } }
    public Coord[] Tiles { get { return tiles; } }
    public string Name { get { return name; } }
    public string ArtPath { get { return artPath; } }
    #endregion

    // When a client receives a message, interpret the message to populate data
    // fields.
    public static SignalFromHost FromMessage(int[] message)
    {
        SignalFromHost signal = new SignalFromHost();
        char[] name;
        switch ((Request) message[0])
        {
            case Request.SET_CHUNK_SIZE:
                signal.chunkSize = message[1];
                break;
            case Request.ADD_PLAYER:
                signal.playerID = message[1];
                signal.clientID = message[2];
                signal.isBot = message[3] == 1;
                name = new char[message[4]];
                for (int i = 0; i < name.Length; i++)
                { name[i] = (char) message[5 + i]; }
                signal.name = new string(name);
                break;
            case Request.ADD_BOARD:
                signal.boardID = message[1];
                signal.size = message[2];
                name = new char[message[3]];
                for (int i = 0; i < name.Length; i++)
                { name[i] = (char) message[4 + i]; }
                signal.name = new string(name);
                break;
            case Request.ADD_PIECE:
                signal.playerID = message[1];
                signal.cardID = message[2];
                signal.pieceID = message[3];
                signal.boardID = message[4];
                signal.pieceType = message[5];
                signal.tile = Coord._(message[6], message[7]);
                break;
            case Request.REMOVE_PIECE:
                signal.pieceID = message[1];
                break;
            case Request.MOVE_PIECE:
                signal.pieceID = message[1];
                signal.tilePrev = Coord._(message[2], message[3]);
                signal.tileNext = Coord._(message[4], message[5]);
                signal.tileLerp = message[6];
                break;
            case Request.ADD_CARD:
                signal.pieceID = message[1];
                signal.cardID = message[2];
                break;
            case Request.REMOVE_CARD:
                signal.pieceID = message[1];
                signal.cardID = message[2];
                break;
            case Request.UPDATE_WAYPOINTS:
                signal.pieceID = message[1];
                signal.tiles = new Coord[Piece.MAX_WAYPOINTS];
                for (int i = 0; i < Piece.MAX_WAYPOINTS * 2; i += 2)
                { signal.tiles[i / 2] = Coord._(message, i + 2); }
                break;
            default: signal = null; break;
        }
        if (signal != null) signal.request = (Request) message[0];
        return signal;
    }

    // Only used before match starts.
    public static SignalFromHost SetChunkSize(int chunkSize)
    {
        return new SignalFromHost(
            (int) Request.SET_CHUNK_SIZE,   // Request enum
            chunkSize                       // How many tiles long is a chunk
        );
    }

    // Only used before match starts.
    public static SignalFromHost AddPlayer(int playerID, int clientID,
        bool isBot, string name)
    {
        int[] data = new int[5 + name.Length];
        data[0] = (int) Request.ADD_PLAYER; // Request enum
        data[1] = playerID;                 // Player ID
        data[2] = clientID;                 // What machine she's from
        data[3] = isBot ? 1 : 0;            // If it's a bot
        data[4] = name.Length;              // How many chars in the name
        for (int i = 5, j = 0; j < name.Length; i++, j++)
        {
            data[i] = name[j];
        }
        return new SignalFromHost(data);
    }

    // Only used before match starts.
    public static SignalFromHost AddBoard(int boardID, int size, string name)
    {
        int[] data = new int[4 + name.Length];
        data[0] = (int) Request.ADD_BOARD;  // Request enum
        data[1] = boardID;                  // Board ID
        data[2] = size;                     // How many chunks long is a board
        data[3] = name.Length;              // How many chars in the name
        for (int i = 4, j = 0; j < name.Length; i++, j++)
        {
            data[i] = name[j];
        }
        return new SignalFromHost(data);
    }

    public static SignalFromHost AddPiece(Piece piece)
    {
        int pieceCardID = (piece.Card == null) ? -1 : piece.Card.ID;
        return AddPiece(piece.PlayerID, pieceCardID, piece.ID, piece.BoardID,
            piece.PieceType, piece.Pos);
    }
    public static SignalFromHost AddPiece(int playerID, int cardID,
        int pieceID, int boardID, Piece.Type pieceType, Coord tile)
    {
        return new SignalFromHost(
            (int) Request.ADD_PIECE,    // Request enum
            playerID,                   // Player that piece belongs to
            cardID,                     // Card ID
            pieceID,                    // Piece ID
            boardID,                    // Board ID
            (int) pieceType,            // Master, Creature, Item, etc...
            tile.X, tile.Z              // Tile where it's going onto
        );
    }

    public static SignalFromHost RemovePiece(int pieceID)
    {
        return new SignalFromHost(
            (int) Request.REMOVE_CARD,  // Request enum
            pieceID                     // Piece ID
        );
    }

    public static SignalFromHost MovePiece(int pieceID, Coord tilePrev,
        Coord tileNext, int tileLerp)
    {
        return new SignalFromHost(
            (int) Request.MOVE_PIECE,   // Request enum
            pieceID,                    // Piece ID
            tilePrev.X, tilePrev.Z,     // Location
            tileNext.X, tileNext.Z,
            tileLerp                    // Distance between prev and next
        );
    }

    public static SignalFromHost AddCard(int pieceID, int cardID)
    {
        return new SignalFromHost(
            (int) Request.ADD_CARD,     // Request enum
            pieceID,                    // Piece ID
            cardID                      // Card ID
        );
    }

    public static SignalFromHost RemoveCard(int pieceID, int cardID)
    {
        return new SignalFromHost(
            (int) Request.REMOVE_CARD,  // Request enum
            pieceID,                    // Piece ID
            cardID                      // Card ID
        );
    }

    public static SignalFromHost UpdateWaypoints(Piece piece)
    {
        int[] data = new int[(2 * Piece.MAX_WAYPOINTS) + 2];
        data[0] = (int) Request.UPDATE_WAYPOINTS;   // Request enum
        data[1] = piece.ID;                         // Piece ID
        piece.GetWaypointData().CopyTo(data, 2);    // List of tile coords
        return new SignalFromHost(data);
    }
}