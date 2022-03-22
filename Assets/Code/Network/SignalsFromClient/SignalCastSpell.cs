public class SignalCastSpell : SignalFromClient {
    public readonly int CardID, PieceID, BoardID;
    public readonly Coord Tile;

    public SignalCastSpell(params int[] intMessage) : base(intMessage) {
        CardID = mMessage[0];
        PieceID = mMessage[1];
        BoardID = mMessage[2];
        Tile = Coord._(mMessage[3], mMessage[4]);
    }
}
