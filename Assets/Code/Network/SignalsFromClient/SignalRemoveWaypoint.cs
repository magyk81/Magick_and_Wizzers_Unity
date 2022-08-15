using System.Linq;

namespace Network.SignalsFromClient {
    public class SignalRemoveWaypoint : SignalFromClient {
        public readonly int BoardID, OrderPlace;
        public readonly int PieceID;

        /// <remarks>
        /// Used by host to interpret a received message.
        /// </remarks>
        public SignalRemoveWaypoint(params int[] intMessage) : base(intMessage) {
            int[] intMessageCut = intMessage.Skip(OVERHEAD_LEN).ToArray();

            BoardID = intMessageCut[0];
            OrderPlace = intMessageCut[1];
            PieceID = intMessageCut[2];
        }

        /// <remarks>
        /// Used by client to get ready to send.
        /// </remarks>
        public SignalRemoveWaypoint(int actingPlayerID, int boardID, int orderPlace, int pieceID) : this(
            ClientInfoToIntMessage(actingPlayerID, boardID, orderPlace, pieceID)) { }

        private static int[] ClientInfoToIntMessage(int actingPlayerID, int boardID, int orderPlace, int pieceID) {
            int[] intMessage = new int[5];

            intMessage[0] = (int) Request.REMOVE_WAYPOINT;
            intMessage[1] = actingPlayerID;
            intMessage[2] = boardID;
            intMessage[3] = orderPlace;
            intMessage[4] = pieceID;

            return intMessage;
        }
    }
}