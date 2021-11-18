/* Copyright (C) All Rights Reserved
 * Unauthorized copying of this file, via any medium is prohibited.
 * Proprietary and confidential.
 * Written by Robin Campos <magyk81@gmail.com>, year 2021.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerScript : MonoBehaviour
{
    private static Host host;
    private static Client client;
    private static List<int[]> matchInfo = new List<int[]>();
    public static void AddMatchInfo(int[] info) { matchInfo.Add(info); }

    private Match match;
    private UX_Match uxMatch;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;

        uxMatch = GetComponent<UX_Match>();

        // If starting from the Match scene.
        // Assume this machine is the host and there are no remote clients.
        if (client == null)
        {            
            // Magic data for debugging.
            Player[] players = new Player[2];
            players[0] = new Player("Brooke", 0, 0, false);
            players[1] = new Player("Rachel", 1, 0, true);

            Board[] boards = new Board[2];
            int chunkSize = 10;
            boards[0] = new Board("Main", 0, 2, chunkSize);
            boards[1] = new Board("Sheol", 1, 1, chunkSize);

            match = new Match(players, boards);

            string ipAddress = "localhost";
            int port = 6969;
            host = new Host(ipAddress, port, 1);
            client = new Client(ipAddress, port, 0);

            List<int> localPlayerIDs = new List<int>();
            List<string> localPlayerNames = new List<string>();
            foreach (Player player in players)
            {
                if (!player.IsBot)
                {
                    localPlayerIDs.Add(player.Idx);
                    localPlayerNames.Add(player.Name);
                }
            }
            List<int[]> boardData = new List<int[]>();
            foreach (Board board in boards)
            {
                int[] data = new int[4 + board.Name.Length];
                data[1] = board.Idx;
                data[2] = board.Size;
                data[3] = board.Name.Length;
                for (int i = 4, j = 0; j < board.Name.Length; i++, j++)
                {
                    data[i] = board.Name[j];
                }
                boardData.Add(data);
            }
            uxMatch.Init(localPlayerIDs.ToArray(), localPlayerNames.ToArray(),
                boardData.ToArray(), chunkSize);
        }
        // If this machine is not the host.
        else if (host == null)
        {

        }
        // If this machine is the host.
        else
        {
            List<int[]> playerData = new List<int[]>();
            List<int> localPlayerIDs = new List<int>();
            List<string> localPlayerNames = new List<string>();
            List<int[]> boardData = new List<int[]>();
            int chunkSize = 0;
            foreach (int[] info in matchInfo)
            {
                if (info[0] == (int) SignalFromHost.Request.ADD_PLAYER)
                {
                    playerData.Add(info);
                    if (info[2] == client.ID && info[3] == 0)
                    {
                        localPlayerIDs.Add(info[1]);

                        char[] playerName = new char[info[4]];
                        for (int i = 0, j = 5; j < info[4]; i++, j++)
                        {
                            playerName[i] = (char) info[j];
                        }
                        localPlayerNames.Add(playerName.ToString());
                    }
                }
                else if (info[0] == (int) SignalFromHost.Request.ADD_BOARD)
                    boardData.Add(info);
                else if (info[0]
                    == (int) SignalFromHost.Request.SET_CHUNK_SIZE)
                    chunkSize = info[1];
            }

            match = new Match(playerData.ToArray(), boardData.ToArray());
            uxMatch.Init(localPlayerIDs.ToArray(), localPlayerNames.ToArray(),
                boardData.ToArray(), chunkSize);
        }
        matchInfo.Clear();
    }

    // Update is called once per frame
    void Update()
    {
        if (host != null)
        {
            match.ApplyMessagesFromClient(host.MessagesReceived);
            match.MainLoop();
        }
        uxMatch.ApplyMessagesFromHost(client.MessagesReceived);
    }

    private void OnDestroy()
    {
        if (host != null) host.Terminate();
        if (client != null) client.Terminate();
        host = null;
        client = null;
    }
}
