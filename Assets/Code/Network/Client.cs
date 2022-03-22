using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public class Client : SocketHand {
    private int mId = -1;

    public Client(string ipAddress, int port, int id) : base(ipAddress, port) { mId = id; }

    public int ID { get => mId; }
    public override void Terminate() {
        if (!mTerminating) {
            mTerminating = true;
            if (mSocket != null) mSocket.Close();
        }
    }

    protected override void Run() {
        while (!OpenConnection() && !mTerminating) {
            if (mSocket != null) mSocket.Close();
            Thread.Sleep(1000);
        }
        mConnected = true;
        Debug.Log("Client connected to host.");

        while (!mTerminating) {
            int rm = ReceiveMessages(mSocket);
            if (rm == 0) { /*Debug.Log("Client received message.");*/ } else if (rm == -1) Terminate();

            SendMessages(mSocket);
            ClearMessagesToSend();
        }
    }

    private bool OpenConnection() {
        IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(mIpAddress), mPort);
        
        bool connectionSuccess = false;
        try {
            mSocket.Connect(endPoint);
            connectionSuccess = true;
        } catch (SocketException e) {
            Debug.LogException(e);
        }

        return connectionSuccess;
    }
}
