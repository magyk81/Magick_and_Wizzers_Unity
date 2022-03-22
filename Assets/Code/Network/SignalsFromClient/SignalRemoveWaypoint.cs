public class SignalRemoveWaypoint : SignalFromClient {
    public readonly int BoardID, OrderPlace;
    public readonly int[] PieceIDs;

    public SignalRemoveWaypoint(params int[] intMessage) : base(intMessage) {
        BoardID = mMessage[0];
        OrderPlace = mMessage[1];
        PieceIDs = new int[mMessage[2]];
        for (int i = 0; i < mMessage[2]; i++) { PieceIDs[i] = mMessage[3 + i]; }
    }
}
