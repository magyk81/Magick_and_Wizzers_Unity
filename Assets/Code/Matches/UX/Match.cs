/* Copyright (C) All Rights Reserved
 * Unauthorized copying of this file, via any medium is prohibited.
 * Proprietary and confidential.
 * Written by Robin Campos <magyk81@gmail.com>, year 2021.
 */

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Matches.Cards;
using Network;
using Network.SignalsFromClient;
using Network.SignalsFromHost;

namespace Matches.UX {
    public class Match : MonoBehaviour
    {
        private int mClientID = -1;

        [SerializeField]
        private Player basePlayer;
        [SerializeField]
        private Bot basePlayerBot;
        [SerializeField]
        private Board baseBoard;

        private Player[] mPlayers;
        private Board[][] mBoards;
        private Dictionary<int, int> mBoardWithPiece = new Dictionary<int, int>();
        // User input info to send to host.
        private List<SignalFromClient> mSignalsToSend = new List<SignalFromClient>();

        public int ClientID { set { if (mClientID < 0) mClientID = value; } }
        // [BoardID][CloneID]
        public Board[][] Boards { get { return mBoards; } }
        public Player[] Players { get { return mPlayers; } }
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
                PrintReceivedMessage(request);
                switch (request) {
                    case SignalFromHost.Request.INIT:
                        Init(new SignalInit(message));
                        mSignalsToSend.Add(new SignalInitFinished());
                        break;
                    case SignalFromHost.Request.ADD_PIECE:
                        SignalAddPiece signalAddPiece = new SignalAddPiece(message);
                        string pieceName = (signalAddPiece.CardID >= 0)
                            ? Card.friend_cards[signalAddPiece.CardID].Name
                            : mPlayers[signalAddPiece.PlayerOwnID].gameObject.name;
                        Board[] boards = mBoards[signalAddPiece.BoardID];
                        Piece real = boards[0].AddPiece(signalAddPiece, pieceName, mPlayers.Length);
                        real.SetReal();
                        for (int j = 1; j < boards.Length; j++) {
                            Piece clone = boards[j].AddPiece(signalAddPiece, pieceName, mPlayers.Length);
                            real.AddClone(clone, j);
                        }
                        mBoardWithPiece.Add(signalAddPiece.PieceID, signalAddPiece.BoardID);
                        break;
                    case SignalFromHost.Request.ADD_CARDS:
                        SignalAddCards signalAddCards = new SignalAddCards(message);
                        foreach (Board board in mBoards[mBoardWithPiece[signalAddCards.HolderPieceID]]) {
                            board.AddCards(signalAddCards);
                        }
                        break;
                    case SignalFromHost.Request.REMOVE_CARDS:
                        SignalRemoveCards signalRemoveCards = new SignalRemoveCards(message);
                        foreach (Board board in mBoards[mBoardWithPiece[signalRemoveCards.HolderPieceID]]) {
                            board.RemoveCards(signalRemoveCards);
                        }
                        break;
                    case SignalFromHost.Request.UPDATE_WAYPOINTS:
                        SignalUpdateWaypoints signalUpdateWaypoints = new SignalUpdateWaypoints(message);
                        Board[] _boards = mBoards[mBoardWithPiece[signalUpdateWaypoints.PieceID]];
                        // Piece on board needs to have its waypoints updated before they are updated on the Player.
                        foreach (Board board in _boards) {
                            board.UpdateWaypoints(signalUpdateWaypoints.PieceID, signalUpdateWaypoints.WaypointData);
                        }
                        Players[_boards[0].GetPiece(signalUpdateWaypoints.PieceID).PlayerID].UpdateWaypoints();
                        break;
                }
            }
        }

        /// <summary>
        /// Called once at the beginning of the match when the INIT signal is received.
        /// </summary>
        private void Init(SignalInit signal) {
            /* Which players are local to this machine.
            * Note: Bots are always on the host machine. If this is the host machine, then they count as local,
            * but they use UX_Bot instead of UX_Player. */
            Dictionary<int, int> localToID = new Dictionary<int, int>();
            int localPlayerCount = 0;
            for (int i = 0; i < signal.PlayerCount; i++) {
                // It's local to this machine.
                if (signal.PlayerClientIDs[i] == mClientID) {
                    localToID.Add(localPlayerCount++, i);
                }
            }

            // Generate uxBoards.
            Transform boardGroup = new GameObject().GetComponent<Transform>();
            boardGroup.gameObject.name = "Boards";
            mBoards = new Board[signal.BoardCount][];
            for (int i = 0; i < mBoards.Length; i++) {
                // 1 real uxBoard + 8 clone uxBoards
                mBoards[i] = new Board[9];
                Transform boardParent = new GameObject().GetComponent<Transform>();
                boardParent.parent = boardGroup;
                // Get name from boardData's char array.
                boardParent.gameObject.name = signal.BoardNames[i];
                boardParent.gameObject.SetActive(true);

                for (int j = 0; j < mBoards[i].Length; j++) {
                    mBoards[i][j] = Instantiate(baseBoard.gameObject,boardParent).GetComponent<Board>();
                    if (j == 0) mBoards[i][j].gameObject.name = "Real";
                    else mBoards[i][j].gameObject.name = "Clone " + Util.DirToString(j - 1);
                    mBoards[i][j].Init(signal.BoardSizes[i], localPlayerCount, i, j, mBoards[i][0]);
                }
            }

            // Generate uxPlayers.
            mPlayers = new Player[localPlayerCount];
            Transform playerGroup = new GameObject().GetComponent<Transform>();
            playerGroup.gameObject.name = "Players";
            for (int i = 0; i < localPlayerCount; i++) {
                int playerID = localToID[i];
                if (signal.PlayerIsBot[playerID])
                    mPlayers[i] = Instantiate(basePlayerBot.gameObject, playerGroup).GetComponent<Bot>();
                else mPlayers[i] = Instantiate(basePlayer.gameObject, playerGroup).GetComponent<Player>();
                mPlayers[i].gameObject.name = signal.PlayerNames[playerID];
                mPlayers[i].gameObject.SetActive(true);
                // mPlayers[i].Init(playerID, i, boardBounds, boardOffsets, signal.BoardSizes);
                mPlayers[i].Init(playerID, i, mBoards);
            }
        }

        private static void PrintReceivedMessage(SignalFromHost.Request request) {
            Debug.Log("Client received: \"" + Util.ToTitleCase(request.ToString()) + "\"");
        }

        private void Update() {
            if (mPlayers != null) {
                foreach (Player player in mPlayers) {
                    if (!(player is Bot)) {
                        float[][] rayPoints = player.QueryRays();
                        player.HoveredTile = mBoards[player.BoardID][0].GetHoveredTile(rayPoints[Util.COUNT + 1]);
                        player.HoveredPiece = mBoards[player.BoardID][0].GetHoveredPiece(
                            rayPoints.Take(Util.COUNT + 1).ToArray());
                        SignalFromClient signal = player.QueryGamepad();
                        if (signal != null) mSignalsToSend.Add(signal);
                    }
                }
            }
        }
    }
}