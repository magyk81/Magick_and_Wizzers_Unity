namespace Network.SignalsFromHost {
    public class SignalRemovePiece : SignalFromHost {
        public readonly int PieceID;

        public SignalRemovePiece(params int[] intMessage) : base(intMessage) { PieceID = intMessage[1]; }
    }
}