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
    private static class InitInfo
    {
        public static Player[] players;
    }
    public static Player[] Players {
        set { InitInfo.players = value; } }

    private Board[] boards = new Board[2];

    private static List<SkinTicket> skinTickets = new List<SkinTicket>();
    public static void AddSkinTicket(SkinTicket ticket)
    {
        skinTickets.Add(ticket);
    }
    public SkinTicket[] SkinTickets {
        get {
            SkinTicket[] arr = skinTickets.ToArray();
            skinTickets.Clear();
            return arr;
        }
        set {
            foreach (SkinTicket ticket in value)
            {
                if (ticket.TicketType == SkinTicket.Type.ADD_WAYPOINT)
                {
                    ticket.Piece.AddWaypoint(ticket.Coord);
                }
                else if (ticket.TicketType == SkinTicket.Type.ADD_PIECE)
                {
                    if (boards[ticket.BoardIdx].AddPiece(ticket.PlayerIdx,
                        ticket.Card, ticket.Coord))
                    {
                        ticket.Piece.RemoveFromHand(ticket.Card);
                    }
                }
            }
        }
    }

    public Match()
    {
        boards[0] = new Board("Main", 0);
        boards[1] = new Board("Sheol", 1, 1);
    }
    public void InitUX(UX_Match uxMatch)
    {
        uxMatch.Init(InitInfo.players, boards);

        // Set initial masters.
        boards[0].InitMasters(InitInfo.players);
    }

    public void MainLoop()
    {
        foreach (Board board in boards) { board.Update(); }
    }
}