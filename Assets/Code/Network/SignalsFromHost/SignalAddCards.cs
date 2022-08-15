using System.Linq;
using Matches;
using Matches.Cards;

namespace Network.SignalsFromHost {
    public class SignalAddCards : SignalFromHost {
        // Piece whose hand the card will be added to.
        public readonly int HolderPieceID;
        public readonly int[] CardIDs;

        /// <remarks>
        /// Used by client to interpret a received message.
        /// </remarks>
        public SignalAddCards(params int[] intMessage) : base(intMessage) {
            int[] intMessageCut = intMessage.Skip(OVERHEAD_LEN).ToArray();

            HolderPieceID = intMessageCut[0];
            CardIDs = new int[intMessageCut[1]];
            for (int i = 0, j = 2; i < CardIDs.Length; i++, j++) {
                CardIDs[i] = intMessageCut[j];
            }
        }

        /// <remarks>
        /// Used by host to get ready to send.
        /// </remarks>
        public SignalAddCards(Piece holderPiece, params Card[] cards)
            : this(HostInfoToIntMessage(holderPiece.ID, cards)) { }

        private static int[] HostInfoToIntMessage(int holderPieceID, Card[] cards) {
            int nonArrCount = 3;
            int[] intMessage = new int[nonArrCount + cards.Length];
            intMessage[0] = (int) Request.ADD_CARDS;
            intMessage[1] = holderPieceID;
            intMessage[2] = cards.Length;
            cards.Select(card => card.ID).ToArray().CopyTo(intMessage, nonArrCount);
            return intMessage;
        }
    }
}