using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SignalUpdateWaypoints : SignalFromHost {
    private static readonly int WAYPOINT_DATA_LEN = 2 * Piece.MAX_WAYPOINTS;

    public readonly int PieceID;
    // Tile waypoints are [X, Z]. Piece waypoints are [-1, PieceID].
    public readonly Coord[] WaypointData = new Coord[Piece.MAX_WAYPOINTS];

    /// <remarks>
    /// Used by client to interpret a received message.
    /// </remarks>
    public SignalUpdateWaypoints(params int[] intMessage) : base(intMessage) {
        int[] intMessageCut = intMessage.Skip(OVERHEAD_LEN).ToArray();

        PieceID = intMessageCut[0];

        int[] intWaypointData = intMessageCut.Skip(1).ToArray();
        for (int i = 0; i < Piece.MAX_WAYPOINTS; i++) {
            WaypointData[i] = Coord._(intWaypointData[i * 2], intWaypointData[(i * 2) + 1]);
        }
    }

    /// <remarks>
    /// Used by host to get ready to send.
    /// </remarks>
    public SignalUpdateWaypoints(Piece piece) : this(PieceToIntMessage(piece)) { }

    private static int[] PieceToIntMessage(Piece piece) {
        int nonArrCount = 2;
        int[] intMessage = new int[nonArrCount + WAYPOINT_DATA_LEN];
        intMessage[0] = (int) Request.UPDATE_WAYPOINTS;
        intMessage[1] = piece.ID;
        piece.GetWaypointData().CopyTo(intMessage, nonArrCount);

        return intMessage;
    }
}
