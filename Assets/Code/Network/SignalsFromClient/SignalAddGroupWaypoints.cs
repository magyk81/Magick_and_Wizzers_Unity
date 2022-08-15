using System.Collections.Generic;
using System.Linq;
using Matches;
using Matches.Waypoints;

namespace Network.SignalsFromClient {
    public class SignalAddGroupWaypoints : SignalFromClient {
        public readonly int BoardID;
        // If targeting piece instead, X is -1 and Z is the target piece ID.
        public readonly Coord[] TargetTiles;
        // The pieces that are gonna get the waypoint.
        public readonly int[] PieceIDs;

        /// <remarks>
        /// Used by host to interpret a received message.
        /// </remarks>
        public SignalAddGroupWaypoints(params int[] intMessage) : base(intMessage) {
            int[] intMessageCut = intMessage.Skip(OVERHEAD_LEN).ToArray();
            
            BoardID = intMessageCut[0];
            PieceIDs = intMessageCut.Skip(2).Take(intMessageCut[1]).ToArray();
            TargetTiles = new Coord[(intMessageCut.Length - (2 + PieceIDs.Length)) / 2];
            for (int i = 0, j = 2 + PieceIDs.Length; i < TargetTiles.Length; i++, j += 2) {
                TargetTiles[i] = Coord._(intMessageCut[j], intMessageCut[j + 1]);
            }
        }

        /// <remarks>
        /// Used by client to get ready to send.
        /// </remarks>
        public SignalAddGroupWaypoints(int actingPlayerID, Waypoint[] waypoints, int boardID, int[] pieceIDs) : this(
            ClientInfoToIntMessage(actingPlayerID, waypoints, boardID, pieceIDs)) { }

        private static int[] ClientInfoToIntMessage(
            int actingPlayerID, Waypoint[] waypoints, int boardID, int[] pieceIDs) {
            
            List<Coord> targets = new List<Coord>();
            for (int i = 0; i < waypoints.Length; i++) {
                if (waypoints[i] is Matches.UX.Waypoints.WaypointTile)
                    targets.Add((waypoints[i] as Matches.UX.Waypoints.WaypointTile).Tile);
                else if (waypoints[i] is Matches.UX.Waypoints.WaypointPiece)
                    targets.Add(Coord._(-1, (waypoints[i] as Matches.UX.Waypoints.WaypointPiece).Piece.PieceID));
                else break;
            }

            int nonArrLen = 4;
            int[] intMessage = new int[nonArrLen + pieceIDs.Length + (targets.Count * 2)];

            intMessage[0] = (int) Request.ADD_GROUP_WAYPOINTS;
            intMessage[1] = actingPlayerID;
            intMessage[2] = boardID;
            intMessage[3] = pieceIDs.Length;
            pieceIDs.CopyTo(intMessage, nonArrLen);
            for (int i = 0, j = nonArrLen + pieceIDs.Length; i < targets.Count; i++, j += 2) {
                intMessage[j] = targets[i].X;
                intMessage[j + 1] = targets[i].Z;
            }

            return intMessage;
        }
    }
}