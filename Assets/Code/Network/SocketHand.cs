using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public abstract class SocketHand {
    private static readonly int MSG_SIZE_MAX = 100 * sizeof(int);

    protected readonly Socket mSocket;
    protected bool mConnected = false, mTerminating = false;
    protected readonly object[] mThreadLocks = new object[2];
    protected readonly List<int[]> mMessagesReceived = new List<int[]>();
    protected readonly string mIpAddress;
    protected readonly int mPort;

    private byte[] mMsgData;
    private int mMsgReceiveSize = 0;
    private List<byte> msgReceiveBytes = new List<byte>();
    private readonly List<Signal> mMessagesToSend = new List<Signal>();
    private readonly Thread mThread;

    protected SocketHand(string ipAddress, int port) {
        IPHostEntry ipHostEntry = Dns.GetHostEntry(Dns.GetHostName());

        if (ipHostEntry == null || ipAddress.Length == 0 || ipAddress.Equals("localhost")) {
            foreach (IPAddress address in ipHostEntry.AddressList) {
                if (address.AddressFamily == AddressFamily.InterNetwork) mIpAddress = address.ToString();
            }
        } else mIpAddress = ipAddress;
        mPort = port;

        Debug.Log(GetType().Name + " using IP address: " + ipAddress + ":" + port);

        mSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        
        mMsgData = new byte[MSG_SIZE_MAX];

        mThreadLocks[0] = new object(); mThreadLocks[1] = new object();
        mThread = new Thread(Run);
        mThread.Start();
    }

    public bool Connected { get { return mConnected; } }

    // Called from ControllerScript.
    public int[][] MessagesReceived {
        get {
            lock (mThreadLocks[0]) {
                if (mMessagesReceived.Count == 0) return null;
                int[][] arr = mMessagesReceived.ToArray();
                mMessagesReceived.Clear();
                return arr;
            }
        }
    }

    // Called from ControllerScript.
    public void SendSignals(params Signal[] signals) {
        lock (mThreadLocks[1]) {
            mMessagesToSend.AddRange(signals);
        }
    }

    public abstract void Terminate();

    protected int ReceiveMessages(Socket socket) {
        if (socket.Available > 0) {
            int bytesReceived;
            try { bytesReceived = socket.Receive(mMsgData); }
            catch (SocketException e) {
                Debug.Log(e);
                Terminate();
                return -1;
            }

            msgReceiveBytes.AddRange(mMsgData.Take(bytesReceived));
        } else if (msgReceiveBytes.Count == 0) return 1;

        byte[] bytesArray;
        if (mMsgReceiveSize == 0) {
            if (msgReceiveBytes.Count == 0) return 1;

            // bytesArray = msgReceiveBytes.ToArray();
            // msgReceiveSize = bytesArray[0];
            mMsgReceiveSize = msgReceiveBytes[0];
            // Debug.Log("Message size: " + msgReceiveSize);
            msgReceiveBytes = msgReceiveBytes.Skip(1).ToList();
        }

        if (msgReceiveBytes.Count < mMsgReceiveSize) return 1;

        bytesArray = msgReceiveBytes.ToArray();
        // Debug.Log("bytesArray length: " + bytesArray.Length);
        int[] completedMessage = new int[mMsgReceiveSize / sizeof(int)];
        for (int n = 0; n < mMsgReceiveSize; n += sizeof(int)) {
            completedMessage[n / sizeof(int)] = BitConverter.ToInt32(bytesArray, n);
        }
        lock (mThreadLocks[0]) { mMessagesReceived.Add(completedMessage); }
            
        msgReceiveBytes = msgReceiveBytes.Skip(mMsgReceiveSize).ToList();
        mMsgReceiveSize = 0;

        return 0;
    }

    protected int SendMessages(Socket socket) {
        int count;
        lock (mThreadLocks[1]) {
            foreach (Signal signal in mMessagesToSend) { socket.Send(signal); }
            count = mMessagesToSend.Count;
        }
        
        if (count > 0) return 0;
        return 1;
    }

    protected void ClearMessagesToSend() {
        lock(mThreadLocks[1]) { mMessagesToSend.Clear(); }
    }

    protected abstract void Run();
}
