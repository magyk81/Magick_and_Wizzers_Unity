using System;
using System.Linq;
using UnityEngine;

public abstract class SignalFromClient : Signal {
    public enum Request { CAST_SPELL, ADD_WAYPOINT, REMOVE_WAYPOINT }

    protected readonly int[] mMessage;

    private Request mRequest;
    private int mActingPlayerID;
    // private readonly int mPieceID, mBoardID, mCardID, mOrderPlace;
    // private int[] mPieceIDs;
    // private readonly Coord mTile;

    protected SignalFromClient(params int[] intMessage) : base(intMessage) {
        mMessage = intMessage.Skip(2).ToArray();
    }

    public int PlayerID {
        set {
            if (mByteMessage == null) {
                int messageLength = 1 + ((mIntMessage.Length + 1) * sizeof(int));
                mByteMessage = new byte[messageLength];
                // Debug.Log("messageLength: " + messageLength);

                // First byte is message length.
                mByteMessage[0] = (byte) (messageLength - 1);

                for (int i = 1, j = -1; i < messageLength; i += sizeof(int), j++) {
                    byte[] intAsBytes;
                    // First integer is the acting player ID.
                    if (j == -1) intAsBytes = BitConverter.GetBytes(value);
                    else intAsBytes = BitConverter.GetBytes(mIntMessage[j]);
                    for (int k = 0; k < sizeof(int); k++) {
                        mByteMessage[i + k] = intAsBytes[k];
                    }
                }
            }
            else Debug.Log("Warning: Should not assign PlayerID more than once.");
        }
    }
    public Request ClientRequest { get { return mRequest; } }
    public int ActingPlayerID { get { return mActingPlayerID; } }
    // public int PieceID { get { return mPieceID; } }
    // public int[] PieceIDs { get { return mPieceIDs; } }
    // public int BoardID { get { return mBoardID; } }
    // public int CardID { get { return mCardID; } }
    // public int OrderPlace { get { return mOrderPlace; } }
    // public Coord Tile { get { return mTile.Copy(); } }

    // When the host receives a message, interpret the message to populate data fields.
    public static SignalFromClient FromMessage(int[] message) {
        SignalFromClient signal;

        switch ((Request) message[1]) {
            case Request.CAST_SPELL: signal = new SignalCastSpell(message); break;
            case Request.ADD_WAYPOINT: signal = new SignalAddWaypoint(message); break;
            case Request.REMOVE_WAYPOINT: signal = new SignalRemoveWaypoint(message); break;
            default: signal = null; break;
        }

        // switch ((Request) message[1])
        // {
        //     case Request.CAST_SPELL:
        //         signal.cardID = message[2];
        //         signal.pieceID = message[3];
        //         signal.boardID = message[4];
        //         signal.tile = Coord._(message[5], message[6]);
        //         break;
        //     case Request.ADD_WAYPOINT:
        //         signal.tile = Coord._(message[2], message[3]);
        //         signal.pieceID = message[4];
        //         signal.boardID = message[5];
        //         signal.orderPlace = message[6];
        //         signal.pieceIDs = new int[message[7]];
        //         for (int i = 0; i < message[7]; i++)
        //         { signal.pieceIDs[i] = message[8 + i]; }
        //         break;
        //     case Request.REMOVE_WAYPOINT:
        //         signal.boardID = message[2];
        //         signal.orderPlace = message[3];
        //         signal.pieceIDs = new int[message[4]];
        //         for (int i = 0; i < message[4]; i++)
        //         { signal.pieceIDs[i] = message[5 + i]; }
        //         break;
        //     default: signal = null; break;
        // }
        if (signal != null) {
            signal.mActingPlayerID = message[0];
            signal.mRequest = (Request) message[1];
        }
        return signal;
    }

    public static SignalFromClient CastSpell(int cardID, int casterID, UX_Tile tile) {
        Coord tileCoord = (Coord) tile;
        return new SignalCastSpell(
            (int) Request.CAST_SPELL,   // Request enum
            cardID,                     // Card ID
            casterID,                   // Piece ID that's casting the spell
            tile.BoardID,               // What board the spell is being cast
            tileCoord.X, tileCoord.Z    // Tile where it's going onto
        );
    }

    public static SignalFromClient AddWaypoint(UX_Tile tile, int orderPlace, params UX_Piece[] pieces) {
        return AddWaypoint(tile.BoardID, tile.Pos, -1, orderPlace, pieces);
    }
    public static SignalFromClient AddWaypoint(int boardID, int pieceID, int orderPlace, params UX_Piece[] pieces) {
        return AddWaypoint(boardID, Coord.Null, pieceID, orderPlace, pieces);
    }
    public static SignalFromClient RemoveWaypoint(int boardID, int orderPlace, params UX_Piece[] pieces) {
        int[] data = new int[4 + pieces.Length];
        data[0] = (int) Request.REMOVE_WAYPOINT;    // Request enum
        data[1] = boardID;
        data[2] = orderPlace;
        data[3] = pieces.Length;
        for (int i = 4, j = 0; j < pieces.Length; i++, j++) { data[i] = pieces[j].PieceID; }
        return new SignalRemoveWaypoint(data);
    }

    private static SignalFromClient AddWaypoint(
        int boardID, Coord tile, int pieceID, int orderPlace, params UX_Piece[] pieces) {

        int[] data = new int[7 + pieces.Length];
        data[0] = (int) Request.ADD_WAYPOINT;   // Request enum
        data[1] = tile.X;                       // Tile where it's going onto
        data[2] = tile.Z;
        data[3] = pieceID;                      // Target (-1 if no target)
        data[4] = boardID;                      // Board ID
        data[5] = orderPlace;                   // Its order among the others
        data[6] = pieces.Length;                // How many pieces are getting
        for (int i = 7, j = 0; j < pieces.Length; i++, j++) { data[i] = pieces[j].PieceID; }
        return new SignalAddWaypoint(data);
    }
}
