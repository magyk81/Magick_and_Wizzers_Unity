using System.Collections.Generic;
using System.Linq;

public class SignalAddGroupWaypoints : SignalFromClient {
    public readonly int BoardID;
    // If targeting piece instead, X is -1 and Z is the target piece ID.
    public readonly Coord TargetTile;
    // The pieces that are gonna get the waypoint.
    public readonly int[] PieceIDs;

    /// <remarks>
    /// Used by host to interpret a received message.
    /// </remarks>
    public SignalAddGroupWaypoints(params int[] intMessage) : base(intMessage) {
        int[] intMessageCut = intMessage.Skip(OVERHEAD_LEN).ToArray();

        TargetTile = Coord._(intMessageCut[0], intMessageCut[1]);
        BoardID = intMessageCut[2];
        PieceIDs = intMessageCut.Skip(3).ToArray();
    }

    /// <remarks>
    /// Used by client to get ready to send.
    /// </remarks>
    public SignalAddGroupWaypoints(int actingPlayerID, UX_Waypoint[] waypoints, int boardID, int[] pieceIDs) : this(
        ClientInfoToIntMessage(actingPlayerID, waypoints, boardID, pieceIDs)) { }

    private static int[] ClientInfoToIntMessage(
        int actingPlayerID, UX_Waypoint[] waypoints, int boardID, int[] pieceIDs) {
        
        List<Coord> targets = new List<Coord>();
        for (int i = 0; i < waypoints.Length; i++) {
            if (waypoints[i].Tile != null) targets.Add(waypoints[i].Tile.Pos);
            else if (waypoints[i].Piece != null) targets.Add(Coord._(-1, waypoints[i].Piece.PieceID));
            else break;
        }

        int nonArrLen = 3;
        int[] intMessage = new int[nonArrLen + pieceIDs.Length + (targets.Count * 2)];

        intMessage[0] = (int) Request.ADD_WAYPOINT;
        intMessage[1] = actingPlayerID;
        intMessage[2] = boardID;
        pieceIDs.CopyTo(intMessage, nonArrLen);
        for (int i = 0, j = nonArrLen + pieceIDs.Length; i < targets.Count; i++, j += 2) {
            intMessage[j] = targets[i].X;
            intMessage[j + 1] = targets[i].Z;
        }

        return intMessage;
    }
}
