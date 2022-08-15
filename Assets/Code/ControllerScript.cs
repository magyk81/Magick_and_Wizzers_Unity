/* Copyright (C) All Rights Reserved
 * Unauthorized copying of this file, via any medium is prohibited.
 * Proprietary and confidential.
 * Written by Robin Campos <magyk81@gmail.com>, year 2021.
 */

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Matches;
using Network;
using Network.SignalsFromHost;

public class ControllerScript : MonoBehaviour {
    private Host mHost;
    private Client mClient;
    private Match mMatch;
    private Matches.UX.Match mUxMatch;

    private static int sClientsToInit;

    /**
     * Data stored in sInfoForInit is as follows:
     * [0] - Players and boards to be passed into "SignalInit(int[] intMessage)"
     * [1] - Game mode and special rules
     * [2] - [0] Client count (including local client) | [1] Client ID (local is always 0) | [2] Port | [3] IP address
     */
    private static List<int[]> sInfoForInit = new List<int[]>();

    public void AddInfoForInit(int[] info) { sInfoForInit.Add(info); }

    public static bool ClientWasInit() { return --sClientsToInit == 0; }

    // Start is called before the first frame update
    private void Start() {
        Application.targetFrameRate = 60;

        // Every new entity to start from zero.
        IdHandler.Reset();

        mUxMatch = GetComponent<Matches.UX.Match>();

        /* Starting from the Match scene in Unity Editor, which is only for debugging.
         * Assume this machine is the host and there are no remote clients. */
        if (sInfoForInit.Count == 0) {
            // Local client is the only client.
            sClientsToInit = 1;
            
            // Remote connection setup. IP address and port number are magic data.
            string ipAddress = "localhost";
            int port = 6969;
            mHost = new Host(ipAddress, port, sClientsToInit);
            // The local client is always ID 0.
            mClient = new Client(ipAddress, port, 0);
            mUxMatch.ClientID = 0;

            // Players are magic data.
            Player[] players = new Player[2];
            players[0] = new Player("Brooke", 0, false);
            players[1] = new Player("Rachel", 0, true);

            // Boards are magic data.
            Board[] boards = new Board[2];
            boards[0] = new Board("Main", 2);
            boards[1] = new Board("Sheol", 1);

            mMatch = new Match(players, boards);
            mHost.SendSignals(new SignalInit(players, boards));
        }

        // This machine is not the host.
        else if (sInfoForInit[0].Length == 0) {
            // The local client ID is stored in sInfoForInit. This machine is not the host, so it's never 0.
            mUxMatch.ClientID = sInfoForInit[2][1];
            string ipAddress = new string(sInfoForInit[2].Skip(3).Select(item => (char) item).ToArray());
            mClient = new Client(ipAddress, sInfoForInit[2][2], sInfoForInit[2][1]);
        }

        // This machine is the host.
        else {
            // Assume sInfoForInit has been populated with data before this scene was loaded.
            SignalInit signalInit = new SignalInit(sInfoForInit[0]);

            // Number of clients (including the local one) is stored in sInfoForInit.
            sClientsToInit = sInfoForInit[2][0];

            // Remote connection setup.
            string ipAddress = new string(sInfoForInit[2].Skip(3).Select(item => (char) item).ToArray());
            int port = sInfoForInit[2][2];
            mHost = new Host(ipAddress, port, sClientsToInit);
            // The local client ID is stored in sInfoForInit. This machine is the host, so it's always 0.
            mUxMatch.ClientID = sInfoForInit[2][1];
            mClient = new Client(ipAddress, port, sInfoForInit[2][1]);

            Player[] players = new Player[signalInit.PlayerCount];
            for (int i = 0; i < players.Length; i++) {
                players[i] = new Player(
                    signalInit.PlayerNames[i], signalInit.PlayerClientIDs[i], signalInit.PlayerIsBot[i]);
            }

            Board[] boards = new Board[signalInit.BoardCount];
            for (int i = 0; i < boards.Length; i++) {
                boards[i] = new Board(signalInit.BoardNames[i], signalInit.BoardSizes[i]);
            }

            mMatch = new Match(players, boards);
            mHost.SendSignals(signalInit);
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
            /* Host receives signals from the clients and uses them to update the match.
             * Also generates signals to send to the clients regarding how the received signals affected the match. */
            mMatch.ApplyMessagesFromClient(mHost.MessagesReceived);

            // Match runs 1 tick.
            mMatch.MainLoop();

            // Host sends signals about the match to the clients.
            mHost.SendSignals(mMatch.SignalsToSend);
        }

        // Client receives signals from the host and uses them to update UX.
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
