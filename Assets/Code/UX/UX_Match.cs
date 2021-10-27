/* Copyright (C) All Rights Reserved
 * Unauthorized copying of this file, via any medium is prohibited.
 * Proprietary and confidential.
 * Written by Robin Campos <magyk81@gmail.com>, year 2021.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UX_Match : MonoBehaviour
{
    [SerializeField]
    private UX_Player basePlayer;
    [SerializeField]
    private UX_Board baseBoard;

    private UX_Player[] players;
    public UX_Player[] Players { get { return players; } }
    public static int localPlayerCount;
    private UX_Board[][] boards;
    public UX_Board[][] Boards { get { return boards; } }

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
                if (ticket.TicketType == SkinTicket.Type.ADD_PIECE)
                {
                    foreach (UX_Board board in boards[ticket.Piece.BoardIdx])
                    {
                        board.AddPiece(ticket.Piece);
                    }
                }
                else if (ticket.TicketType == SkinTicket.Type.UPDATE_WAYPOINTS)
                {
                    foreach (UX_Board board in boards[ticket.Piece.BoardIdx])
                    {
                        board.UpdateWaypoints(ticket.Piece, ticket.Coords);
                    }
                    foreach (UX_Player player in players)
                    {
                        player.CalcIfWaypointsCommon();
                    }
                }
            }
        }
    }

    /// <summary>Called once before the match begins.</summary>
    public void Init(Player[] players, Board[] boards)
    {
        // Prep uxBoard bounds.
        float[][] boardBounds = new float[boards.Length][];

        // Calculate number of local players.
        int localPlayerCount = 0;
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].PlayerType == Player.Type.LOCAL_PLAYER)
                localPlayerCount++;
        }
        UX_Match.localPlayerCount = localPlayerCount;

        // Generate uxBoards.
        Transform boardGroup = new GameObject().GetComponent<Transform>();
        boardGroup.gameObject.name = "Boards";
        this.boards = new UX_Board[boards.Length][];
        for (int i = 0; i < boards.Length; i++)
        {
            // 1 real uxBoard + 8 clone uxBoards
            this.boards[i] = new UX_Board[9];
            Transform boardParent = new GameObject().GetComponent<Transform>();
            boardParent.parent = boardGroup;
            boardParent.gameObject.name = boards[i].Name;
            boardParent.gameObject.SetActive(true);

            for (int j = 0; j < this.boards[i].Length; j++)
            {
                this.boards[i][j] = Instantiate(
                    baseBoard.gameObject,
                    boardParent).GetComponent<UX_Board>();
                if (j == 0)
                {
                    this.boards[i][j].gameObject.name = "Board - Real";
                    this.boards[i][j].Init(boards[i].GetSize(), i);
                    boardBounds[i] = this.boards[i][j].GetBounds();
                }
                else
                {
                    this.boards[i][j].gameObject.name = "Board - Clone "
                        + Util.DirToString(j - 1);
                    this.boards[i][j].Init(
                        boards[i].GetSize(), i, j - 1, this.boards[i][0]);
                }
            }
        }

        // Generate uxPlayers
        this.players = new UX_Player[localPlayerCount];
        Transform playerGroup = new GameObject().GetComponent<Transform>();
        playerGroup.gameObject.name = "Players";
        for (int i = 0, j = 0; i < players.Length; i++)
        {
            if (players[i].PlayerType == Player.Type.LOCAL_PLAYER)
            {
                this.players[j] = Instantiate(
                    basePlayer.gameObject,
                    playerGroup
                ).GetComponent<UX_Player>();
                this.players[j].gameObject.name = players[i].Name;
                this.players[j].gameObject.SetActive(true);
                this.players[j].Init(i, j, boardBounds);

                j++;
            }
        }
    }

    public void Update()
    {
        foreach (UX_Player player in players)
        {
            player.QueryCamera();
            player.QueryGamepad();
        }
    }
}
