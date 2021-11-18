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
    private UX_Board[][] boards;
    public UX_Board[][] Boards { get { return boards; } }

    public void ApplyMessagesFromHost(params int[][] messages)
    {
        if (messages == null) return;
        for (int i = 0; i < messages.Length; i++)
        {
            int[] message = messages[i];
            if (message[0] == (int) SignalFromHost.Request.ADD_PIECE)
            {
                Debug.Log("Add Piece");
            }
        }
    }

    // private static List<Signal> signals = new List<Signal>();
    // public static void AddSkinTicket(Signal signal)
    // {
    //     signals.Add(signal);
    // }
    // public Signal[] Signals {
    //     get {
    //         Signal[] arr = signals.ToArray();
    //         signals.Clear();
    //         return arr;
    //     }
    //     set {
    //         foreach (Signal ticket in value)
    //         {
    //             if (ticket.SignalType == Signal.Type.ADD_PIECE)
    //             {
    //                 foreach (UX_Board board in boards[ticket.Piece.BoardIdx])
    //                 {
    //                     board.AddPiece(ticket.Piece);
    //                 }
    //             }
    //             else if (ticket.SignalType == Signal.Type.UPDATE_WAYPOINTS)
    //             {
    //                 foreach (UX_Board board in boards[ticket.Piece.BoardIdx])
    //                 {
    //                     board.UpdateWaypoints(ticket.Piece, ticket.Coords);
    //                 }
    //                 foreach (UX_Player player in players)
    //                 {
    //                     player.CalcIfWaypointsCommon();
    //                 }
    //             }
    //         }
    //     }
    // }

    /// <summary>Called once before the match begins.</summary>
    public void Init(int[] playerIDs, string[] playerNames, int[][] boardData,
        int chunkSize)
    {
        // Prep uxBoard bounds.
        float[][] boardBounds = new float[boardData.Length][];

        // Generate uxBoards.
        Transform boardGroup = new GameObject().GetComponent<Transform>();
        boardGroup.gameObject.name = "Boards";
        boards = new UX_Board[boardData.Length][];
        for (int i = 0; i < boards.Length; i++)
        {
            // Get name from boardData's char array
            char[] boardDataChars = new char[boardData[i][3]];
            for (int j = 0; j < boardData[i][3]; j++)
            {
                boardDataChars[j] = (char) boardData[i][j + 4];
            }

            // 1 real uxBoard + 8 clone uxBoards
            boards[i] = new UX_Board[9];
            Transform boardParent = new GameObject().GetComponent<Transform>();
            boardParent.parent = boardGroup;
            boardParent.gameObject.name = boardDataChars.ToString();
            boardParent.gameObject.SetActive(true);

            int boardTotalSize = boardData[i][2] * chunkSize;

            for (int j = 0; j < boards[i].Length; j++)
            {
                boards[i][j] = Instantiate(
                    baseBoard.gameObject,
                    boardParent).GetComponent<UX_Board>();
                if (j == 0)
                {
                    boards[i][j].gameObject.name = "Board - Real";
                    boards[i][j].Init(
                        boardData[i][2], boardTotalSize, playerIDs.Length, i);
                    boardBounds[i] = boards[i][j].GetBounds();
                }
                else
                {
                    boards[i][j].gameObject.name = "Board - Clone "
                        + Util.DirToString(j - 1);
                    boards[i][j].Init(
                        boardData[i][2], boardTotalSize, playerIDs.Length, i,
                        j - 1, boards[i][0]);
                }
            }
        }

        // Generate uxPlayers
        players = new UX_Player[playerIDs.Length];
        Transform playerGroup = new GameObject().GetComponent<Transform>();
        playerGroup.gameObject.name = "Players";
        for (int i = 0; i < playerIDs.Length; i++)
        {
            players[i] = Instantiate(basePlayer.gameObject, playerGroup)
                .GetComponent<UX_Player>();
            players[i].gameObject.name = playerNames[i];
            players[i].gameObject.SetActive(true);
            players[i].Init(playerIDs[i], i, boardBounds, chunkSize / 2);
        }
    }

    public void Update()
    {
        if (players != null)
        {
            foreach (UX_Player player in players)
            {
                player.QueryCamera();
                player.QueryGamepad();
            }
        }
        
    }
}
