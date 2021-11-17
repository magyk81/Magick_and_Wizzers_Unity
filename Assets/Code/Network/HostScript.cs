using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public class HostScript : SocketScript
{
    private Socket[] clientSockets;
    private int connClientCount = 0;

    [SerializeField]
    private int debugClientCount;
    [SerializeField]
    private KeyCode debugKey = KeyCode.None;

    private bool OpenConnection(int idx)
    {
        bool connectionSuccess = false;
        try {
            clientSockets[idx] = socket.Accept();
            if (clientSockets[idx] != null) connectionSuccess = true;
        } catch (SocketException e) {
            Debug.LogException(e);
        }

        return connectionSuccess;
    }

    protected override void Run()
    {
        clientSockets = new Socket[ControllerScript.PlayerCount > 0
            ? ControllerScript.PlayerCount : debugClientCount];
        if (clientSockets.Length <= 0)
        {
            Debug.LogError("Error: clientSockets.Length is "
                + clientSockets.Length);
            return;
        }

        while (!terminating && !connected)
        {
            IPEndPoint endPoint
                = new IPEndPoint(IPAddress.Parse(ipAddress), port);
            
            try {
                socket.Bind(endPoint);
                socket.Listen(10);
            } catch (SocketException e) {
                Debug.LogException(e);
                continue;
            }

            Debug.Log("Host waiting for a connection [" + 1 + "/"
                + clientSockets.Length + "]...");
            while (!connected && !terminating)
            {
                if (OpenConnection(connClientCount))
                {
                    connClientCount++;
                    if (connClientCount == clientSockets.Length)
                        connected = true;
                    else Debug.Log("Host waiting for a connection ["
                        + (connClientCount + 1) + "/" + clientSockets.Length
                        + "]...");
                }
                else Thread.Sleep(1000);
            }

            if (clientSockets.Length > 2)
            {
                Debug.Log("Host connected to all " + clientSockets.Length
                    + " client sockets.");
            }
            else if (clientSockets.Length == 2)
                Debug.Log("Host connected to both client sockets.");
            else Debug.Log("Host connected to single client socket.");
        }

        while (!terminating)
        {
            int sm = 1;
            for (int i = 0; i < clientSockets.Length; i++)
            {
                sm = SendMessages(clientSockets[i]);

                int rm = ReceiveMessages(clientSockets[i]);
                if (rm == 0)
                    Debug.Log("Host received message from client #" + i + ".");
                else if (rm == -1) Terminate();
            }
            if (sm == 0) ClearMessagesToSend();
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (debugKey != KeyCode.None && connected && !terminating)
        {
            if (Input.GetKeyDown(debugKey))
            {
                // Generate random data.
                System.Random rand = new System.Random();
                int GetRandInt() { return rand.Next() % 100; };
                int randPlayerID = GetRandInt(),
                    randCardID = GetRandInt(),
                    randPieceID = GetRandInt();
                Coord randTile = Coord._(GetRandInt(), GetRandInt());

                SignalFromHost signal = SignalFromHost.AddPiece(
                    randPlayerID, randCardID, randPieceID, randTile);

                SendSignals(signal);
            }
        }
    }

    public override void Terminate()
    {
        if (!terminating)
        {
            terminating = true;
            if (socket != null)
            {
                // socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }
            for (int i = 0; i < clientSockets.Length; i++)
            {
                if (clientSockets[i] != null) clientSockets[i].Close();
            }
        }
        
    }

    private void OnDestroy() { Terminate(); }
}
