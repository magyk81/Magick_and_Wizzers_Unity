using System.Linq;
using Matches;

namespace Network.SignalsFromHost {
    public class SignalAddPiece : SignalFromHost {
        public readonly int PieceID, CardID, BoardID, PlayerOwnID;
        public readonly Piece.Size Size;
        public readonly Coord Tile;

        /// <remarks>
        /// Used by client to interpret a received message.
        /// </remarks>
        public SignalAddPiece(params int[] intMessage) : base(intMessage) {
            int[] intMessageCut = intMessage.Skip(OVERHEAD_LEN).ToArray();

            PieceID = intMessageCut[0];
            CardID = intMessageCut[1];
            BoardID = intMessageCut[2];
            PlayerOwnID = intMessageCut[3];
            Size = (Piece.Size) intMessageCut[4];
            Tile = Coord._(intMessageCut[5], intMessageCut[6]);
        }

        /// <remarks>
        /// Used by host to get ready to send.
        /// </remarks>
        public SignalAddPiece(Piece piece) : this(PieceToIntMessage(piece)) { }

        private static int[] PieceToIntMessage(Piece piece) {
            int[] intMessage = new int[8];
            intMessage[0] = (int) Request.ADD_PIECE;
            intMessage[1] = piece.ID;
            intMessage[2] = (piece.Card != null) ? piece.Card.ID : -1;
            intMessage[3] = piece.BoardID;
            intMessage[4] = piece.PlayerID;
            intMessage[5] = (int) piece.GetSize();
            intMessage[6] = piece.Pos.X;
            intMessage[7] = piece.Pos.Z;
            return intMessage;
        }
    }
}