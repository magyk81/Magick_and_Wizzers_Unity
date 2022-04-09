using System.Linq;

public class SignalAddWaypoint : SignalFromClient {
    public readonly int BoardID, OrderPlace;
    // If targeting piece instead, X is -1 and Z is the target piece ID.
    public readonly Coord TargetTile;
    // The piece that's gonna get the waypoint.
    public readonly int PieceID;

    /// <remarks>
    /// Used by host to interpret a received message.
    /// </remarks>
    public SignalAddWaypoint(params int[] intMessage) : base(intMessage) {
        int[] intMessageCut = intMessage.Skip(OVERHEAD_LEN).ToArray();

        TargetTile = Coord._(intMessageCut[0], intMessageCut[1]);
        BoardID = intMessageCut[2];
        OrderPlace = intMessageCut[3];
        PieceID = intMessageCut[4];
    }

    /// <remarks>
    /// Used by client to get ready to send.
    /// </remarks>
    public SignalAddWaypoint(int actingPlayerID, Coord targetTile, int boardID, int orderPlace, int pieceID) : this(
        ClientInfoToIntMessage(actingPlayerID, targetTile, boardID, orderPlace, pieceID)) { }

    private static int[] ClientInfoToIntMessage(
        int actingPlayerID, Coord targetTile, int boardID, int orderPlace, int pieceID) {
        int[] intMessage = new int[7];

        intMessage[0] = (int) Request.ADD_WAYPOINT;
        intMessage[1] = actingPlayerID;
        intMessage[2] = targetTile.X;
        intMessage[3] = targetTile.Z;
        intMessage[4] = boardID;
        intMessage[5] = orderPlace;
        intMessage[6] = pieceID;

        return intMessage;
    }
}
