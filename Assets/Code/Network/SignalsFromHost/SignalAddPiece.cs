using System.Linq;
using Matches;

namespace Network.SignalsFromHost {
    public class SignalAddPiece : SignalFromHost {
        public readonly int PieceID, CardID, BoardID, PlayerOwnID;
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
            Tile = Coord._(intMessageCut[4], intMessageCut[5]);
        }

        /// <remarks>
        /// Used by host to get ready to send.
        /// </remarks>
        public SignalAddPiece(Piece piece) : this(PieceToIntMessage(piece)) { }

        private static int[] PieceToIntMessage(Piece piece) {
            int[] intMessage = new int[7];
            intMessage[0] = (int) Request.ADD_PIECE;
            intMessage[1] = piece.ID;
            intMessage[2] = (piece.Card != null) ? piece.Card.ID : -1;
            intMessage[3] = piece.BoardID;
            intMessage[4] = piece.PlayerID;
            intMessage[5] = piece.Pos.X;
            intMessage[6] = piece.Pos.Z;
            return intMessage;
        }
    }
}