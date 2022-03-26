using System.Linq;

public class SignalRemoveWaypoint : SignalFromClient {
    public readonly int BoardID, OrderPlace;
    public readonly int[] PieceIDs;

    /// <remarks>
    /// Used by host to interpret a received message.
    /// </remarks>
    public SignalRemoveWaypoint(params int[] intMessage) : base(intMessage) {
        int[] intMessageCut = intMessage.Skip(OVERHEAD_LEN).ToArray();

        BoardID = intMessageCut[0];
        OrderPlace = intMessageCut[1];
        PieceIDs = intMessageCut.Skip(2).ToArray();
    }

    /// <remarks>
    /// Used by client to get ready to send.
    /// </remarks>
    public SignalRemoveWaypoint(int actingPlayerID, int boardID, int orderPlace, int[] pieceIDs) : this(
        ClientInfoToIntMessage(actingPlayerID, boardID, orderPlace, pieceIDs)) { }

    private static int[] ClientInfoToIntMessage(int actingPlayerID, int boardID, int orderPlace, int[] pieceIDs) {
        int nonArrCount = 4;
        int[] intMessage = new int[nonArrCount + pieceIDs.Length];

        intMessage[0] = (int) Request.REMOVE_WAYPOINT;
        intMessage[1] = actingPlayerID;
        intMessage[2] = boardID;
        intMessage[3] = orderPlace;
        pieceIDs.CopyTo(intMessage, nonArrCount);

        return intMessage;
    }
}
