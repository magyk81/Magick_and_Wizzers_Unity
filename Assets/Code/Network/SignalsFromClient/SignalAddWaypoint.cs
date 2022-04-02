using System.Linq;

public class SignalAddWaypoint : SignalFromClient {
    public readonly int BoardID, OrderPlace;
    // If targeting piece instead, X is -1 and Z is the target piece ID.
    public readonly Coord TargetTile;
    // The pieces that had the waypoint.
    public readonly int[] PieceIDs;

    /// <remarks>
    /// Used by host to interpret a received message.
    /// </remarks>
    public SignalAddWaypoint(params int[] intMessage) : base(intMessage) {
        int[] intMessageCut = intMessage.Skip(OVERHEAD_LEN).ToArray();

        TargetTile = Coord._(intMessageCut[0], intMessageCut[1]);
        BoardID = intMessageCut[2];
        OrderPlace = intMessageCut[3];
        PieceIDs = intMessageCut.Skip(4).ToArray();
    }

    /// <remarks>
    /// Used by client to get ready to send.
    /// </remarks>
    public SignalAddWaypoint(int actingPlayerID, Coord targetTile, int boardID, int orderPlace, int[] pieceIDs) : this(
        ClientInfoToIntMessage(actingPlayerID, targetTile, boardID, orderPlace, pieceIDs)) { }

    private static int[] ClientInfoToIntMessage(
        int actingPlayerID, Coord targetTile, int boardID, int orderPlace, int[] pieceIDs) {

        int nonArrCount = 6;
        int[] intMessage = new int[nonArrCount + pieceIDs.Length];

        intMessage[0] = (int) Request.ADD_WAYPOINT;
        intMessage[1] = actingPlayerID;
        intMessage[2] = targetTile.X;
        intMessage[3] = targetTile.Z;
        intMessage[4] = boardID;
        intMessage[5] = orderPlace;
        pieceIDs.CopyTo(intMessage, nonArrCount);

        return intMessage;
    }
}
