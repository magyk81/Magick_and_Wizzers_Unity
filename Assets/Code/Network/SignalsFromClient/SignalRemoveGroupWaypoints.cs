using System.Linq;

namespace Network.SignalsFromClient {
    public class SignalRemoveGroupWaypoints : SignalFromClient {
        public readonly int BoardID;
        public readonly int[] PieceIDs;

        /// <remarks>
        /// Used by host to interpret a received message.
        /// </remarks>
        public SignalRemoveGroupWaypoints(params int[] intMessage) : base(intMessage) {
            int[] intMessageCut = intMessage.Skip(OVERHEAD_LEN).ToArray();

            BoardID = intMessageCut[0];
            for (int i = 1; i < intMessageCut.Length; i++) { PieceIDs[i - 1] = intMessageCut[i]; }
        }

        /// <remarks>
        /// Used by client to get ready to send.
        /// </remarks>
        public SignalRemoveGroupWaypoints(int actingPlayerID, int boardID, int[] pieceIDs) : this(
            ClientInfoToIntMessage(actingPlayerID, boardID, pieceIDs)) { }

        private static int[] ClientInfoToIntMessage(int actingPlayerID, int boardID, int[] pieceIDs) {
            int nonArrLen = 3;
            int[] intMessage = new int[nonArrLen + pieceIDs.Length];

            intMessage[0] = (int) Request.REMOVE_WAYPOINT;
            intMessage[1] = actingPlayerID;
            intMessage[2] = boardID;
            pieceIDs.CopyTo(intMessage, nonArrLen);

            return intMessage;
        }
    }
}