using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public class Host : SocketHand {
    private Socket[] mClientSockets;
    private readonly int mClientCount;
    private int mConnClientCount = 0;

    public Host(string ipAddress, int port, int clientCount) : base(ipAddress, port) { mClientCount = clientCount; }

    public override void Terminate() {
        if (!mTerminating) {
            mTerminating = true;
            if (mSocket != null) {
                // socket.Shutdown(SocketShutdown.Both);
                mSocket.Close();
            }
            for (int i = 0; i < mClientSockets.Length; i++) {
                if (mClientSockets[i] != null) mClientSockets[i].Close();
            }
        }
    }

    protected override void Run() {
        mClientSockets = new Socket[mClientCount];
        if (mClientSockets.Length <= 0) {
            Debug.LogError("Error: clientSockets.Length is " + mClientSockets.Length);
            return;
        }

        while (!mTerminating && !mConnected) {
            IPEndPoint endPoint = new IPEndPoint(
                IPAddress.Parse(mIpAddress), mPort);
            
            try {
                mSocket.Bind(endPoint);
                mSocket.Listen(10);
            } catch (SocketException e) {
                Debug.LogException(e);
                continue;
            }

            Debug.Log("Host waiting for a connection [" + 1 + "/" + mClientSockets.Length + "]...");
            while (!mConnected && !mTerminating) {
                if (OpenConnection(mConnClientCount)) {
                    mConnClientCount++;
                    if (mConnClientCount == mClientSockets.Length) mConnected = true;
                    else Debug.Log("Host waiting for a connection [" + (mConnClientCount + 1) + "/"
                        + mClientSockets.Length + "]...");
                } else Thread.Sleep(1000);
            }

            if (mClientSockets.Length > 2) {
                Debug.Log("Host connected to all " + mClientSockets.Length + " clients.");
            } else if (mClientSockets.Length == 2) Debug.Log("Host connected to both clients.");
            else Debug.Log("Host connected to single client.");
        }

        while (!mTerminating) {
            int sm = 1;
            for (int i = 0; i < mClientSockets.Length; i++) {
                sm = SendMessages(mClientSockets[i]);

                int rm = ReceiveMessages(mClientSockets[i]);
                // If rm == 0, that means Host received message from this Client.
                if (rm == -1) Terminate();
            }
            if (sm == 0) ClearMessagesToSend();
        }
    }

    private bool OpenConnection(int idx) {
        bool connectionSuccess = false;
        try {
            mClientSockets[idx] = mSocket.Accept();
            if (mClientSockets[idx] != null) connectionSuccess = true;
        } catch (SocketException e) {
            Debug.LogException(e);
        }

        return connectionSuccess;
    }
}
