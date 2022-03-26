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

    private UX_Player[] mPlayers;
    private UX_Board[][] mBoards;
    private Dictionary<int, int> mBoardWithPiece = new Dictionary<int, int>();
    private string[] mPlayerNames;
    // User input info to send to host.
    private List<SignalFromClient> mSignalsToSend = new List<SignalFromClient>();

    public UX_Board[][] Boards { get { return mBoards; } }
    public UX_Player[] Players { get { return mPlayers; } }
    public SignalFromClient[] SignalsToSend {
        get {
            SignalFromClient[] array = mSignalsToSend.ToArray();
            mSignalsToSend.Clear();
            return array;
        }
    }

    public void ApplyMessagesFromHost(params int[][] messages) {
        if (messages == null) return;
        for (int i = 0; i < messages.Length; i++) {
            int[] message = messages[i];
            SignalFromHost.Request request = (SignalFromHost.Request) message[0];
            switch (request) {
                case SignalFromHost.Request.ADD_PIECE:
                    Debug.Log("Add Piece");
                    SignalAddPiece signalAddPiece = new SignalAddPiece(message);
                    string pieceName = signalAddPiece.CardID >= 0
                        ? Card.friend_cards[signalAddPiece.CardID].Name : mPlayerNames[signalAddPiece.PlayerOwnID];
                    UX_Board[] boards = mBoards[signalAddPiece.BoardID];
                    UX_Piece real = boards[0].AddPiece(signalAddPiece, pieceName, mPlayers.Length);
                    real.SetReal();
                    for (int j = 1; j < boards.Length; j++) {
                        UX_Piece clone = boards[j].AddPiece(signalAddPiece, pieceName, mPlayers.Length);
                        real.AddClone(clone, j);
                    }
                    mBoardWithPiece.Add(signalAddPiece.PieceID, signalAddPiece.BoardID);
                    break;
                case SignalFromHost.Request.ADD_CARD:
                    Debug.Log("Add Card");
                    SignalAddCard signalAddCard = new SignalAddCard(message);
                    foreach (UX_Board board in mBoards[mBoardWithPiece[signalAddCard.HolderPieceID]]) {
                        board.AddCard(signalAddCard);
                    }
                    break;
                case SignalFromHost.Request.REMOVE_CARD:
                    Debug.Log("Remove Card");
                    SignalRemoveCard signalRemoveCard = new SignalRemoveCard(message);
                    foreach (UX_Board board in mBoards[mBoardWithPiece[signalRemoveCard.HolderPieceID]]) {
                        board.RemoveCard(signalRemoveCard); }
                    break;
                case SignalFromHost.Request.UPDATE_WAYPOINTS:
                    Debug.Log("Update Waypoints");
                    SignalUpdateWaypoints signalUpdateWaypoints = new SignalUpdateWaypoints(message);
                    UX_Board[] _boards = mBoards[mBoardWithPiece[signalUpdateWaypoints.PieceID]];
                    foreach (UX_Board board in _boards) {
                        board.UpdateWaypoints(signalUpdateWaypoints.PieceID, signalUpdateWaypoints.WaypointData); }
                    Players[_boards[0].PlayerWithPiece(signalUpdateWaypoints.PieceID)].ResetPotentialWaypoint();
                    break;
            }
        }
    }

    /// <summary>
    /// Called once before the match begins.
    /// </summary>
    public void Init(int[] localPlayerIDs, string[] playerNames, int[][] boardData, int chunkSize) {
        // Prep uxBoard bounds.
        float[][] boardBounds = new float[boardData.Length][];

        // Generate uxBoards.
        Transform boardGroup = new GameObject().GetComponent<Transform>();
        boardGroup.gameObject.name = "Boards";
        mBoards = new UX_Board[boardData.Length][];
        for (int i = 0; i < mBoards.Length; i++) {
            // Get name from boardData's char array.
            char[] boardDataChars = new char[boardData[i][3]];
            for (int j = 0; j < boardData[i][3]; j++) { boardDataChars[j] = (char) boardData[i][j + 4]; }

            // 1 real uxBoard + 8 clone uxBoards
            mBoards[i] = new UX_Board[9];
            Transform boardParent = new GameObject().GetComponent<Transform>();
            boardParent.parent = boardGroup;
            boardParent.gameObject.name = new string(boardDataChars);
            boardParent.gameObject.SetActive(true);

            int boardTotalSize = boardData[i][2] * chunkSize;

            for (int j = 0; j < mBoards[i].Length; j++) {
                mBoards[i][j] = Instantiate(baseBoard.gameObject,boardParent).GetComponent<UX_Board>();
                if (j == 0) {
                    mBoards[i][j].gameObject.name = "Board - Real";
                    mBoards[i][j].Init(boardData[i][2], boardTotalSize,localPlayerIDs.Length, i);
                    boardBounds[i] = mBoards[i][j].GetBounds();
                } else {
                    mBoards[i][j].gameObject.name = "Board - Clone " + Util.DirToString(j - 1);
                    mBoards[i][j].Init(
                        boardData[i][2], boardTotalSize, localPlayerIDs.Length, i, j - 1, mBoards[i][0]);
                }
            }
        }

        // Generate uxPlayers.
        mPlayers = new UX_Player[localPlayerIDs.Length];
        Transform playerGroup = new GameObject().GetComponent<Transform>();
        playerGroup.gameObject.name = "Players";
        for (int i = 0; i < localPlayerIDs.Length; i++) {
            mPlayers[i] = Instantiate(basePlayer.gameObject, playerGroup).GetComponent<UX_Player>();
            mPlayers[i].gameObject.name = playerNames[localPlayerIDs[i]];
            mPlayers[i].gameObject.SetActive(true);
            mPlayers[i].Init(localPlayerIDs[i], i, boardBounds, chunkSize / 2);
        }

        // Store all player names.
        mPlayerNames = new string[playerNames.Length];
        playerNames.CopyTo(mPlayerNames, 0);
    }

    private void Update() {
        if (mPlayers != null) {
            foreach (UX_Player player in mPlayers) {
                player.QueryCamera();
                SignalFromClient signal = player.QueryGamepad();
                if (signal != null) mSignalsToSend.Add(signal);
            }
        }
    }
}
