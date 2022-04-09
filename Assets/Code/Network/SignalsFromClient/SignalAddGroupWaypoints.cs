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
    public SignalAddGroupWaypoints(int actingPlayerID, Coord[] targetTiles, int boardID, int[] pieceIDs) : this(
        ClientInfoToIntMessage(actingPlayerID, targetTiles, boardID, pieceIDs)) { }

    private static int[] ClientInfoToIntMessage(
        int actingPlayerID, Coord[] targetTiles, int boardID, int[] pieceIDs) {
        int nonArrLen = 3;
        int[] intMessage = new int[nonArrLen + pieceIDs.Length + (targetTiles.Length * 2)];

        intMessage[0] = (int) Request.ADD_WAYPOINT;
        intMessage[1] = actingPlayerID;
        intMessage[2] = boardID;
        pieceIDs.CopyTo(intMessage, nonArrLen);
        for (int i = 0, j = nonArrLen + pieceIDs.Length; i < targetTiles.Length; i++, j += 2) {
            intMessage[j] = targetTiles[i].X;
            intMessage[j + 1] = targetTiles[i].Z;
        }

        return intMessage;
    }
}
