using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public class Host : SocketHand
{
    private Socket[] clientSockets;
    private int clientCount;
    private int connClientCount = 0;

    public Host(string ipAddress, int port, int clientCount)
        : base(ipAddress, port)
    {
        this.clientCount = clientCount;
    }

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
        clientSockets = new Socket[clientCount];
        if (clientSockets.Length <= 0)
        {
            Debug.LogError("Error: clientSockets.Length is "
                + clientSockets.Length);
            return;
        }

        while (!terminating && !connected)
        {
            IPEndPoint endPoint = new IPEndPoint(
                IPAddress.Parse(ipAddress), port);
            
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
                    + " clients.");
            }
            else if (clientSockets.Length == 2)
                Debug.Log("Host connected to both clients.");
            else Debug.Log("Host connected to single client.");
        }

        while (!terminating)
        {
            int sm = 1;
            for (int i = 0; i < clientSockets.Length; i++)
            {
                sm = SendMessages(clientSockets[i]);

                int rm = ReceiveMessages(clientSockets[i]);
                if (rm == 0)
                {
                    /*Debug.Log(
                        "Host received message from client #" + i + ".");*/
                }
                else if (rm == -1) Terminate();
            }
            if (sm == 0) ClearMessagesToSend();
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
}
