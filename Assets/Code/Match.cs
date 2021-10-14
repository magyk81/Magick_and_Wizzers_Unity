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
            }
        }
    }

    public Match()
    {
        boards[0] = new Board("Main");
        boards[1] = new Board("Sheol", 1);
    }
    public void InitUX(UX_Match uxMatch)
    {
        uxMatch.Init(InitInfo.players, boards);

        // // Pair players with UX_Players.
        // UX_Player[] uxPlayers = uxMatch.Players;
        // for (int i = 0; i < UX_Match.localPlayerCount; i++)
        // {
        //     InitInfo.players[i].UX = uxPlayers[i];
        // }

        // // Pair boards with UX_Boards.
        // UX_Board[][] uxBoards = uxMatch.Boards;
        // for (int i = 0; i < boards.Length; i++)
        // {
        //     boards[i].UX = uxBoards[i];
        // }

        // Set initial masters.
        boards[0].InitMasters(InitInfo.players);
    }

    public void MainLoop() {}
}