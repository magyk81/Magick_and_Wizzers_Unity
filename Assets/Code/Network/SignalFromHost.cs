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

    public enum Request { ADD_PIECE, REMOVE_PIECE, MOVE_PIECE,
        ADD_CARD, REMOVE_CARD,
        ADD_WAYPOINT, REMOVE_WAYPOINT }

    public static SignalFromHost AddPiece(int playerID, int cardID,
        int pieceID, Coord tile)
    {
        return new SignalFromHost(
            (int) Request.ADD_PIECE,    // Request enum
            playerID,                   // Player that piece belongs to
            cardID,                     // Card ID
            pieceID,                    // Piece ID
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

    public static SignalFromHost AddWaypoint(int pieceID, Coord tile,
        int orderPlace)
    {
        return new SignalFromHost(
            (int) Request.ADD_WAYPOINT, // Request enum
            tile.X, tile.Z,             // Tile where it's going onto
            orderPlace                  // If it's the 1st, 2nd, 3rd, etc.
        );
    }
}