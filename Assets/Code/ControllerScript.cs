/* Copyright (C) All Rights Reserved
 * Unauthorized copying of this file, via any medium is prohibited.
 * Proprietary and confidential.
 * Written by Robin Campos <magyk81@gmail.com>, year 2021.
 */

using System.Collections.Generic;
using UnityEngine;

public class ControllerScript : MonoBehaviour {
    private Host mHost;

    /// <remarks>
    /// This instance of <c>Client</c> is only used for debugging.
    /// </remarks>
    private Client mClient;
    private Match mMatch;
    private UX_Match mUxMatch;

    private static List<int[]> sInfoForInit = new List<int[]>();

    public void AddInfoForInit(int[] info) { sInfoForInit.Add(info); }

    // Start is called before the first frame update
    private void Start() {
        Application.targetFrameRate = 60;

        // Every new entity to start from zero.
        IdHandler.Reset();

        mUxMatch = GetComponent<UX_Match>();

        /* If starting from the Match scene in Unity Editor, which is only for debugging.
         * Assume this machine is the host and there are no remote clients. */
        if (mClient == null) {            
            // Player and boards are magic data.
            Player[] players = new Player[2];
            players[0] = new Player("Brooke", 0, false);
            players[1] = new Player("Rachel", 0, true);

            Board[] boards = new Board[2];
            int chunkSize = 10;
            boards[0] = new Board("Main", 2, chunkSize);
            boards[1] = new Board("Sheol", 1, chunkSize);

            mMatch = new Match(players, boards);

            // Remote connection setup.
            string ipAddress = "localhost";
            int port = 6969;
            mHost = new Host(ipAddress, port, 1);
            mClient = new Client(ipAddress, port, 0);

            List<int> localPlayerIDs = new List<int>();
            List<string> playerNames = new List<string>();
            foreach (Player player in players) {
                if (!player.IS_BOT) localPlayerIDs.Add(player.ID);
                playerNames.Add(player.NAME);
            }

            List<int[]> boardData = new List<int[]>();
            foreach (Board board in boards) {
                int[] data = new int[4 + board.Name.Length];
                data[1] = board.ID;
                data[2] = board.Size;
                data[3] = board.Name.Length;
                for (int i = 4, j = 0; j < board.Name.Length; i++, j++) {
                    data[i] = board.Name[j];
                }
                boardData.Add(data);
            }

            // Setup the UX stuff.
            mUxMatch.Init(localPlayerIDs.ToArray(), playerNames.ToArray(),
                boardData.ToArray(), chunkSize);

            // Signal that begins the match.
            mHost.SendSignals(mMatch.Init());
        }

        // If this machine is not the host.
        else if (mHost == null) {

        }

        // If this machine is the host.
        else {
            List<int[]> playerData = new List<int[]>();
            List<int> localPlayerIDs = new List<int>();
            List<string> localPlayerNames = new List<string>();
            List<int[]> boardData = new List<int[]>();
            int chunkSize = 0;
            foreach (int[] info in sInfoForInit) {
                if (info[0] == (int) SignalFromHost.Request.ADD_PLAYER) {
                    playerData.Add(info);
                    if (info[2] == mClient.ID && info[3] == 0) {
                        localPlayerIDs.Add(info[1]);

                        char[] playerName = new char[info[4]];
                        for (int i = 0, j = 5; j < info[4]; i++, j++) {
                            playerName[i] = (char) info[j];
                        }
                        localPlayerNames.Add(new string(playerName));
                    }
                }
                else if (info[0] == (int) SignalFromHost.Request.ADD_BOARD) boardData.Add(info);
                else if (info[0] == (int) SignalFromHost.Request.SET_CHUNK_SIZE) chunkSize = info[1];
            }

            mMatch = new Match(playerData.ToArray(), boardData.ToArray(),
                chunkSize);
            mUxMatch.Init(localPlayerIDs.ToArray(), localPlayerNames.ToArray(),
                boardData.ToArray(), chunkSize);
            mHost.SendSignals(mMatch.Init());
        }
        sInfoForInit.Clear();
    }

    // Update is called once per frame
    private void Update()
    {
        // Client sends signals about player input to the host.
        mClient.SendSignals(mUxMatch.SignalsToSend);

        // If this machine is the host.
        if (mHost != null)
        {
            // Host sends signals to update the clients.
            mHost.SendSignals(mMatch.SignalsToSend);

            // Host uses signals received from the clients to update the match.
            mMatch.ApplyMessagesFromClient(mHost.MessagesReceived);

            // Match runs 1 tick.
            mMatch.MainLoop();
        }

        // Client uses signals received from host to update UX.
        mUxMatch.ApplyMessagesFromHost(mClient.MessagesReceived);
    }

    private void OnDestroy()
    {
        if (mHost != null) mHost.Terminate();
        if (mClient != null) mClient.Terminate();
        mHost = null;
        mClient = null;
    }
}
