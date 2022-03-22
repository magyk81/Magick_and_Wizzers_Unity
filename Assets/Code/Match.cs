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

    // To send to clients for UX.
    private readonly List<SignalFromHost> mSignalsToSend = new List<SignalFromHost>();

    private static readonly int STARTING_HAND_COUNT = 3;

    public void ApplyMessagesFromClient(params int[][] messages) {
        if (messages == null) return;
        for (int i = 0; i < messages.Length; i++) {
            int[] message = messages[i];
            SignalFromHost[] outcomeArr;
            SignalFromClient signal = SignalFromClient.FromMessage(message);
            switch (signal.ClientRequest) {
                case SignalFromClient.Request.CAST_SPELL:
                    Debug.Log("Cast Spell");
                    SignalCastSpell signalCastSpell = signal as SignalCastSpell;
                    outcomeArr = mBoards[signalCastSpell.BoardID].CastSpell(
                        signalCastSpell, mBoards[signalCastSpell.BoardID]);
                    if (outcomeArr != null) mSignalsToSend.AddRange(outcomeArr);
                    break;
                case SignalFromClient.Request.ADD_WAYPOINT:
                    Debug.Log("Add Waypoint");
                    SignalAddWaypoint signalAddWaypoint = signal as SignalAddWaypoint;
                    outcomeArr = mBoards[signalAddWaypoint.BoardID].AddWaypoint(signalAddWaypoint, true);
                    if (outcomeArr != null) mSignalsToSend.AddRange(outcomeArr);
                    break;
                case SignalFromClient.Request.REMOVE_WAYPOINT:
                    Debug.Log("Remove Waypoint");
                    SignalRemoveWaypoint signalRemoveWaypoint = signal as SignalRemoveWaypoint;
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

    public Match(int[][] playerData, int[][] boardData, int chunkSize)
        : this(parsePlayerData(playerData), parseBoardData(boardData, chunkSize)) { }

    public SignalFromHost[] SignalsToSend {
        get {
            SignalFromHost[] array = mSignalsToSend.ToArray();
            mSignalsToSend.Clear();
            return array;
        }
    }
    
    public SignalFromHost[] Init()
    { return mBoards[0].InitMasters(mPlayers, STARTING_HAND_COUNT); }

    public void MainLoop() {
        foreach (Board board in mBoards) { board.Update(); }
    }

    private static Player[] parsePlayerData(int[][] data) {
        Player[] players = new Player[data.Length];
        for (int i = 0; i < data.Length; i++) {
            int[] d = data[i];
            char[] name = new char[d[3]];
            for (int j = 0; j < name.Length; j++) {
                name[j] = (char) d[4 + j];
            }
            int clientID = d[1];
            bool isBot = d[2] == 1;
            players[i] = new Player(new string(name), clientID, isBot);
        }
        return players;
    }
    private static Board[] parseBoardData(int[][] data, int chunkSize) {
        Board[] boards = new Board[data.Length];
        for (int i = 0; i < data.Length; i++) {
            int[] d = data[i];
            char[] name = new char[d[2]];
            for (int j = 0; j < name.Length; j++) {
                name[j] = (char) d[3 + j];
            }
            int size = d[1];
            boards[i] = new Board(new string(name), size, chunkSize);
        }
        return boards;
    }
}