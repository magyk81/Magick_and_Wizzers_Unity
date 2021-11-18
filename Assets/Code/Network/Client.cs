using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public class Client : SocketHand
{
    private int id = -1;
    public int ID { get { return id; } }

    public Client(string ipAddress, int port, int id) : base(ipAddress, port)
    {
        this.id = id;
    }

    private bool OpenConnection()
    {
        IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(ipAddress), port);
        
        bool connectionSuccess = false;
        try {
            socket.Connect(endPoint);
            connectionSuccess = true;
        } catch (SocketException e) {
            Debug.LogException(e);
        }

        return connectionSuccess;
    }

    protected override void Run()
    {
        while (!OpenConnection() && !terminating)
        {
            if (socket != null) socket.Close();
            Thread.Sleep(1000);
        }
        connected = true;
        Debug.Log("Client connected to host.");

        while (!terminating)
        {
            int rm = ReceiveMessages(socket);
            if (rm == 0) Debug.Log("Client received message.");
            else if (rm == -1) Terminate();

            SendMessages(socket);
            ClearMessagesToSend();
        }
    }

    public override void Terminate()
    {
        if (!terminating)
        {
            terminating = true;
            if (socket != null) socket.Close();
        }
    }
}
