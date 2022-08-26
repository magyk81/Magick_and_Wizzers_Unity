using System.Linq;
using Matches;

namespace Network.SignalsFromHost {
    public class SignalMovePiece : SignalFromHost {
        private static readonly int PIECE_POS_DATA_LEN = 5;

        public readonly int PieceID, DirNext;
        public readonly Coord Position;
        public readonly Piece.Size Size;
        public readonly float Lerp;

        /// <remarks>
        /// Used by client to interpret a received message.
        /// </remarks>
        public SignalMovePiece(params int[] intMessage) : base(intMessage) {
            int[] intMessageCut = intMessage.Skip(OVERHEAD_LEN).ToArray();

            PieceID = intMessageCut[0];
            // Position = PiecePos.ClientData(intMessageCut.Skip(1).ToArray());
            Position = Coord._(intMessageCut[1], intMessageCut[2]);
            Size = (Piece.Size) intMessageCut[3];
            DirNext = intMessageCut[4];
            Lerp = ((float) intMessageCut[5]) / PiecePos.LERP_MAX;
        }

        /// <remarks>
        /// Used by host to get ready to send.
        /// </remarks>
        public SignalMovePiece(Piece piece) : this(PieceToIntMessage(piece)) { }

        private static int[] PieceToIntMessage(Piece piece) {
            int nonArrCount = 2;
            int[] intMessage = new int[nonArrCount + PIECE_POS_DATA_LEN];
            intMessage[0] = (int) Request.MOVE_PIECE;
            intMessage[1] = piece.ID;
            piece.GetPosData().CopyTo(intMessage, nonArrCount);

            return intMessage;
        }
    }
}