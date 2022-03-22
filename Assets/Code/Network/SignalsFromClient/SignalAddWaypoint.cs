public class SignalAddWaypoint : SignalFromClient {
    public readonly int PieceID, BoardID, OrderPlace;
    public readonly Coord Tile;
    public readonly int[] PieceIDs;

    public SignalAddWaypoint(params int[] intMessage) : base(intMessage) {
        Tile = Coord._(mMessage[0], mMessage[1]);
        PieceID = mMessage[2];
        BoardID = mMessage[3];
        OrderPlace = mMessage[4];
        PieceIDs = new int[mMessage[5]];
        for (int i = 0; i < mMessage[5]; i++) { PieceIDs[i] = mMessage[6 + i]; }
    }
}
