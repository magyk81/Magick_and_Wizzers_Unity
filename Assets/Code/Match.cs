/* Copyright (C) All Rights Reserved
 * Unauthorized copying of this file, via any medium is prohibited.
 * Proprietary and confidential.
 * Written by Robin Campos <magyk81@gmail.com>, year 2021.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Match
{
    private Player[] players;
    private Board[] boards;
    
    private readonly int STARTING_HAND_COUNT = 3;


    private List<SignalFromHost> signalsToSend = new List<SignalFromHost>();
    public SignalFromHost[] SignalsToSend {
        get {
            SignalFromHost[] array = signalsToSend.ToArray();
            signalsToSend.Clear();
            return array;
        }
    }

    public void ApplyMessagesFromClient(params int[][] messages)
    {
        if (messages == null) return;
        for (int i = 0; i < messages.Length; i++)
        {
            int[] message = messages[i];
            SignalFromHost[] outcomeArr; SignalFromHost outcome;
            SignalFromClient signal = SignalFromClient.FromMessage(message);
            switch (signal.ClientRequest)
            {
                case SignalFromClient.Request.CAST_SPELL:
                    Debug.Log("Cast Spell");
                    outcomeArr = boards[signal.BoardID].CastSpell(
                        signal, boards[signal.BoardID]);
                    if (outcomeArr != null) signalsToSend.AddRange(outcomeArr);
                    break;
                case SignalFromClient.Request.GIVE_WAYPOINT:
                    Debug.Log("Give Waypoint");
                    outcome = boards[signal.BoardID].GiveWaypoint(signal);
                    if (outcome != null) signalsToSend.Add(outcome);
                    break;
            }
        }
    }

    // private static List<Signal> skinTickets = new List<Signal>();
    // public static void AddSkinTicket(Signal ticket)
    // {
    //     skinTickets.Add(ticket);
    // }
    // public Signal[] Signals {
    //     get {
    //         Signal[] arr = skinTickets.ToArray();
    //         skinTickets.Clear();
    //         return arr;
    //     }
    //     set {
    //         foreach (Signal ticket in value)
    //         {
    //             if (ticket.SignalType == Signal.Type.ADD_WAYPOINT)
    //             {
    //                 ticket.Piece.AddWaypoint(ticket.Coord);
    //             }
    //             else if (ticket.SignalType == Signal.Type.ADD_PIECE)
    //             {
    //                 if (boards[ticket.BoardIdx].AddPiece(ticket.PlayerIdx,
    //                     ticket.Card, ticket.Coord))
    //                 {
    //                     ticket.Piece.RemoveFromHand(ticket.Card);
    //                 }
    //             }
    //         }
    //     }
    // }

    private static Player[] parsePlayerData(int[][] data)
    {
        Player[] players = new Player[data.Length];
        for (int i = 0; i < data.Length; i++)
        {
            int[] d = data[i];
            char[] name = new char[d[4]];
            for (int j = 0; j < name.Length; j++)
            {
                name[j] = (char) d[5 + j];
            }
            int idx = d[1];
            int clientID = d[2];
            bool isBot = d[3] == 1;
            players[i] = new Player(new string(name), idx, clientID, isBot);
        }
        return players;
    }
    private static Board[] parseBoardData(int[][] data, int chunkSize)
    {
        Board[] boards = new Board[data.Length];
        for (int i = 0; i < data.Length; i++)
        {
            int[] d = data[i];
            char[] name = new char[d[3]];
            for (int j = 0; j < name.Length; j++)
            {
                name[j] = (char) d[4 + j];
            }
            int idx = d[1];
            int size = d[2];
            boards[i] = new Board(new string(name), idx, size, chunkSize);
        }
        return boards;
    }
    public Match(int[][] playerData, int[][] boardData, int chunkSize) : this(
            parsePlayerData(playerData),
            parseBoardData(boardData, chunkSize)) { }

    public Match(Player[] players, Board[] boards)
    {
        this.players = players;
        this.boards = boards;
    }
    public SignalFromHost[] Init()
    { return boards[0].InitMasters(players, STARTING_HAND_COUNT); }

    public void MainLoop()
    {
        foreach (Board board in boards) { board.Update(); }
    }
}