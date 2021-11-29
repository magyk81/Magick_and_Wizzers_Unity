using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignalFromClient : Signal
{

    private SignalFromClient(params int[] intMessage) : base(intMessage) {}

    public int PlayerID
    {
        set
        {
            if (message == null)
            {
                int messageLength = 1 + ((intMessage.Length + 1)
                    * sizeof(int));
                message = new byte[messageLength];
                // Debug.Log("messageLength: " + messageLength);

                // First byte is message length.
                message[0] = (byte) (messageLength - 1);

                for (int i = 1, j = -1; i < messageLength;
                    i += sizeof(int), j++)
                {
                    byte[] intAsBytes;
                    // First integer is the acting player ID.
                    if (j == -1) intAsBytes = BitConverter.GetBytes(value);
                    else intAsBytes = BitConverter.GetBytes(intMessage[j]);
                    for (int k = 0; k < sizeof(int); k++)
                    {
                        message[i + k] = intAsBytes[k];
                    }
                }
            }
            else Debug.Log(
                "Warning: Should not assign PlayerID more than once.");
        }
    }

    public enum Request { CAST_SPELL, ADD_WAYPOINT, REMOVE_WAYPOINT }

    #region member getters
    private Request request;
    private int actingPlayerID, pieceID, boardID, cardID, orderPlace;
    private int[] pieceIDs;
    private Coord tile;
    public Request ClientRequest { get { return request; } }
    public int ActingPlayerID { get { return actingPlayerID; } }
    public int PieceID { get { return pieceID; } }
    public int[] PieceIDs { get { return pieceIDs; } }
    public int BoardID { get { return boardID; } }
    public int CardID { get { return cardID; } }
    public int OrderPlace { get { return orderPlace; } }
    public Coord Tile { get { return tile.Copy(); } }
    #endregion

    // When the host receives a message, interpret the message to populate data
    // fields.
    public static SignalFromClient FromMessage(int[] message)
    {
        SignalFromClient signal = new SignalFromClient();
        switch ((Request) message[1])
        {
            case Request.CAST_SPELL:
                signal.cardID = message[2];
                signal.pieceID = message[3];
                signal.boardID = message[4];
                signal.tile = Coord._(message[5], message[6]);
                break;
            case Request.ADD_WAYPOINT:
                signal.tile = Coord._(message[2], message[3]);
                signal.pieceID = message[4];
                signal.boardID = message[5];
                signal.orderPlace = message[6];
                signal.pieceIDs = new int[message[7]];
                for (int i = 0; i < message[7]; i++)
                { signal.pieceIDs[i] = message[8 + i]; }
                break;
            case Request.REMOVE_WAYPOINT:
                signal.boardID = message[2];
                signal.orderPlace = message[3];
                signal.pieceIDs = new int[message[4]];
                for (int i = 0; i < message[4]; i++)
                { signal.pieceIDs[i] = message[5 + i]; }
                break;
            default: signal = null; break;
        }
        if (signal != null)
        {
            signal.actingPlayerID = message[0];
            signal.request = (Request) message[1];
        }
        return signal;
    }

    public static SignalFromClient CastSpell(int cardID, int casterID,
        UX_Tile tile)
    {
        Coord tileCoord = (Coord) tile;
        return new SignalFromClient(
            (int) Request.CAST_SPELL,   // Request enum
            cardID,                     // Card ID
            casterID,                   // Piece ID that's casting the spell
            tile.BoardID,               // What board the spell is being cast
            tileCoord.X, tileCoord.Z    // Tile where it's going onto
        );
    }

    public static SignalFromClient AddWaypoint(UX_Tile tile, int pieceID,
        int orderPlace, params UX_Piece[] pieces)
    {
        Coord tileCoord = tile != null ? (Coord) tile : Coord.Null;
        int[] data = new int[7 + pieces.Length];
        data[0] = (int) Request.ADD_WAYPOINT;   // Request enum
        data[1] = tileCoord.X;                  // Tile where it's going onto
        data[2] = tileCoord.Z;
        data[3] = pieceID;                      // Target (-1 if no target)
        data[4] = tile.BoardID;                 // Board ID
        data[5] = orderPlace;                   // Its order among the others
        data[6] = pieces.Length;                // How many pieces are getting
        for (int i = 7, j = 0; j < pieces.Length; i++, j++)
        {
            data[i] = pieces[j].PieceID;
        }
        return new SignalFromClient(data);
    }

    public static SignalFromClient RemoveWaypoint(int boardID,
        int orderPlace, params UX_Piece[] pieces)
    {
        int[] data = new int[4 + pieces.Length];
        data[0] = (int) Request.REMOVE_WAYPOINT;    // Request enum
        data[1] = boardID;
        data[2] = orderPlace;
        data[3] = pieces.Length;
        for (int i = 4, j = 0; j < pieces.Length; i++, j++)
        {
            data[i] = pieces[j].PieceID;
        }
        return new SignalFromClient(data);
    }
}
