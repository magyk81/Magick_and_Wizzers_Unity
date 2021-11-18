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

    public void ApplyMessagesFromClient(params int[][] messages)
    {
        if (messages == null) return;
        for (int i = 0; i < messages.Length; i++)
        {
            int[] message = messages[i];
            if (message[1] == (int) SignalFromClient.Request.CAST_SPELL)
            {
                Debug.Log("Cast Spell");
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
        return players;
    }
    private static Board[] parseBoardData(int[][] data)
    {
        Board[] boards = new Board[data.Length];
        return boards;
    }
    public Match(int[][] playerData, int[][] boardData)
        : this(parsePlayerData(playerData), parseBoardData(boardData)) { }

    public Match(Player[] players, Board[] boards)
    {
        this.players = players;
        this.boards = boards;

        // Set initial masters.
        boards[0].InitMasters(players);
    }

    public void MainLoop()
    {
        foreach (Board board in boards) { board.Update(); }
    }
}