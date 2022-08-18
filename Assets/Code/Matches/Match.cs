/* Copyright (C) All Rights Reserved
 * Unauthorized copying of this file, via any medium is prohibited.
 * Proprietary and confidential.
 * Written by Robin Campos <magyk81@gmail.com>, year 2021.
 */

using System.Collections.Generic;
using UnityEngine;
using Network;
using Network.SignalsFromClient;

namespace Matches {
    public class Match {
        private readonly Player[] mPlayers;
        private readonly Board[] mBoards;

        // Match info to send to clients for UX.
        private readonly List<SignalFromHost> mSignalsToSend = new List<SignalFromHost>();

        private static readonly int STARTING_HAND_COUNT = 30;

        public void ApplyMessagesFromClient(params int[][] messages) {
            if (messages == null) return;
            for (int i = 0; i < messages.Length; i++) {
                int[] message = messages[i];
                SignalFromHost[] outcomes = null;
                SignalFromClient.Request request = (SignalFromClient.Request) message[0];
                PrintReceivedMessage(request);
                switch (request) {
                    case SignalFromClient.Request.INIT_FINISHED:
                        if (ControllerScript.ClientWasInit()) mSignalsToSend.AddRange(Init());
                        break;
                    case SignalFromClient.Request.CAST_SPELL:
                        SignalCastSpell signalCastSpell = new SignalCastSpell(message);
                        outcomes = mBoards[signalCastSpell.BoardID].CastSpell(signalCastSpell);
                        break;
                    case SignalFromClient.Request.ADD_WAYPOINT:
                        SignalAddWaypoint signalAddWaypoint = new SignalAddWaypoint(message);
                        outcomes = new SignalFromHost[] {
                            mBoards[signalAddWaypoint.BoardID].AddWaypoint(signalAddWaypoint) };
                        break;
                    case SignalFromClient.Request.ADD_GROUP_WAYPOINTS:
                        SignalAddGroupWaypoints signalAddGroupWaypoints = new SignalAddGroupWaypoints(message);
                        outcomes = mBoards[signalAddGroupWaypoints.BoardID].AddWaypoints(signalAddGroupWaypoints);
                        break;
                    case SignalFromClient.Request.REMOVE_WAYPOINT:
                        SignalRemoveWaypoint signalRemoveWaypoint = new SignalRemoveWaypoint(message);
                        outcomes = new SignalFromHost[] {
                            mBoards[signalRemoveWaypoint.BoardID].RemoveWaypoint(signalRemoveWaypoint) };
                        break;
                }
                if (outcomes != null) mSignalsToSend.AddRange(outcomes);
            }
        }

        public Match(Player[] players, Board[] boards) {
            mPlayers = players;
            mBoards = boards;
        }

        public SignalFromHost[] SignalsToSend {
            get {
                SignalFromHost[] array = mSignalsToSend.ToArray();
                mSignalsToSend.Clear();
                return array;
            }
        }
        
        public SignalFromHost[] Init() {
            return mBoards[0].InitMasters(mPlayers, STARTING_HAND_COUNT); }

        public void MainLoop() {
            foreach (Board board in mBoards) { mSignalsToSend.AddRange(board.Update()); }
        }

        private static void PrintReceivedMessage(SignalFromClient.Request request) {
            Debug.Log("Host received: \"" + Util.ToTitleCase(request.ToString()) + "\"");
        }
    }
}