/* Copyright (C) All Rights Reserved
 * Unauthorized copying of this file, via any medium is prohibited.
 * Proprietary and confidential.
 * Written by Robin Campos <magyk81@gmail.com>, year 2021.
 */

using System.Collections.Generic;
using UnityEngine;

public class Match {
    private readonly Player[] mPlayers;
    private readonly Board[] mBoards;

    // Match info to send to clients for UX.
    private readonly List<SignalFromHost> mSignalsToSend = new List<SignalFromHost>();

    private static readonly int STARTING_HAND_COUNT = 3;

    public void ApplyMessagesFromClient(params int[][] messages) {
        if (messages == null) return;
        for (int i = 0; i < messages.Length; i++) {
            int[] message = messages[i];
            SignalFromHost[] outcomes;
            SignalFromClient.Request request = (SignalFromClient.Request) message[0];
            PrintReceivedMessage(request);
            switch (request) {
                case SignalFromClient.Request.INIT_FINISHED:
                    if (ControllerScript.ClientWasInit()) mSignalsToSend.AddRange(Init());
                    break;
                case SignalFromClient.Request.CAST_SPELL:
                    SignalCastSpell signalCastSpell = new SignalCastSpell(message);
                    outcomes = mBoards[signalCastSpell.BoardID].CastSpell(signalCastSpell);
                    if (outcomes != null) mSignalsToSend.AddRange(outcomes);
                    break;
                case SignalFromClient.Request.ADD_WAYPOINT:
                    SignalAddWaypoint signalAddWaypoint = new SignalAddWaypoint(message);
                    outcomes = mBoards[signalAddWaypoint.BoardID].AddWaypoint(signalAddWaypoint, true);
                    if (outcomes != null) mSignalsToSend.AddRange(outcomes);
                    break;
                case SignalFromClient.Request.REMOVE_WAYPOINT:
                    SignalRemoveWaypoint signalRemoveWaypoint = new SignalRemoveWaypoint(message);
                    // outcomeArr = mBoards[signalRemoveWaypoint.BoardID].SetWaypoint(signalRemoveWaypoint, false);
                    // if (outcomeArr != null) mSignalsToSend.AddRange(outcomeArr);
                    break;
            }
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
        foreach (Board board in mBoards) { board.Update(); }
    }

    private static void PrintReceivedMessage(SignalFromClient.Request request) {
        Debug.Log("Host received: \"" + Util.ToTitleCase(request.ToString()) + "\"");
    }
}