using System.Linq;
using Matches;

namespace Network.SignalsFromClient {
    public class SignalCastSpell : SignalFromClient {
        public readonly int PlayCardID, CasterID, BoardID;
        public readonly Coord Tile;

        /// <remarks>
        /// Used by host to interpret a received message.
        /// </remarks>
        public SignalCastSpell(params int[] intMessage) : base(intMessage) {
            int[] intMessageCut = intMessage.Skip(OVERHEAD_LEN).ToArray();

            PlayCardID = intMessageCut[0];
            CasterID = intMessageCut[1];
            BoardID = intMessageCut[2];
            Tile = Coord._(intMessageCut[3], intMessageCut[4]);
        }

        /// <remarks>
        /// Used by client to get ready to send.
        /// </remarks>
        public SignalCastSpell(int actingPlayerID, int playCardID, int casterID, int boardID, Coord tile) : this(
            ClientInfoToIntMessage(actingPlayerID, playCardID, casterID, boardID, tile)) { }

        private static int[] ClientInfoToIntMessage(
            int actingPlayerID, int playCardID, int casterID, int boardID, Coord tile) {

            int[] intMessage = new int[7];

            intMessage[0] = (int) Request.CAST_SPELL;
            intMessage[1] = actingPlayerID;
            intMessage[2] = playCardID;
            intMessage[3] = casterID;
            intMessage[4] = boardID;
            intMessage[5] = tile.X;
            intMessage[6] = tile.Z;
            return intMessage;
        }
    }
}