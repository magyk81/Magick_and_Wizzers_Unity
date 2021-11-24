using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public abstract class SocketHand
{
    protected Socket socket; private Thread thread;
    protected bool connected = false, terminating = false;
    public bool Connected { get { return connected; } }
    protected object[] threadLocks = new object[2];

    private readonly int MSG_SIZE_MAX = 100 * sizeof(int);
    private byte[] msgData;
    private int msgReceiveSize = 0;
    private List<byte> msgReceiveBytes = new List<byte>();

    protected List<int[]> messagesReceived = new List<int[]>();
    private List<Signal> messagesToSend = new List<Signal>();
    
    protected string ipAddress;
    protected int port;

    protected SocketHand(string ipAddress, int port)
    {
        IPHostEntry ipHostEntry = Dns.GetHostEntry(Dns.GetHostName());

        if (ipHostEntry == null || ipAddress.Length == 0
            || ipAddress.Equals("localhost"))
        {
            foreach (IPAddress address in ipHostEntry.AddressList)
            {
                if (address.AddressFamily == AddressFamily.InterNetwork)
                    this.ipAddress = address.ToString();
            }
        }
        else this.ipAddress = ipAddress;
        this.port = port;

        Debug.Log("Sender using IP address: " + ipAddress + ":" + port);

        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream,
            ProtocolType.Tcp);
        
        msgData = new byte[MSG_SIZE_MAX];

        threadLocks[0] = new object(); threadLocks[1] = new object();
        thread = new Thread(Run);
        thread.Start();
    }

    protected abstract void Run();
    public abstract void Terminate();

    protected int ReceiveMessages(Socket socket)
    {
        if (socket.Available > 0)
        {
            int bytesReceived;
            try { bytesReceived = socket.Receive(msgData); }
            catch (SocketException e) {
                Debug.Log(e);
                Terminate();
                return -1;
            }

            msgReceiveBytes.AddRange(msgData.Take(bytesReceived));
        }
        else if (msgReceiveBytes.Count == 0) return 1;

        byte[] bytesArray;
        if (msgReceiveSize == 0)
        {
            if (msgReceiveBytes.Count == 0) return 1;

            // bytesArray = msgReceiveBytes.ToArray();
            // msgReceiveSize = bytesArray[0];
            msgReceiveSize = msgReceiveBytes[0];
            // Debug.Log("Message size: " + msgReceiveSize);
            msgReceiveBytes = msgReceiveBytes.Skip(1).ToList();
        }

        if (msgReceiveBytes.Count < msgReceiveSize) return 1;

        bytesArray = msgReceiveBytes.ToArray();
        // Debug.Log("bytesArray length: " + bytesArray.Length);
        int[] completedMessage = new int[msgReceiveSize / sizeof(int)];
        for (int n = 0; n < msgReceiveSize; n += sizeof(int))
        {
            completedMessage[n / sizeof(int)]
                = BitConverter.ToInt32(bytesArray, n);
        }
        lock (threadLocks[0]) { messagesReceived.Add(completedMessage); }
            
        msgReceiveBytes = msgReceiveBytes.Skip(msgReceiveSize).ToList();
        msgReceiveSize = 0;

        return 0;
    }

    protected int SendMessages(Socket socket)
    {
        int count;
        lock (threadLocks[1])
        {
            foreach (Signal signal in messagesToSend)
            {
                // Debug.Log("Signal set to send: " + signal);
                socket.Send(signal);
            }
            count = messagesToSend.Count;
        }
        
        if (count > 0) return 0;
        return 1;
    }

    protected void ClearMessagesToSend()
    {
        lock(threadLocks[1]) { messagesToSend.Clear(); }
    }

    // Called from ControllerScript.
    public int[][] MessagesReceived {
        get {
            lock (threadLocks[0])
            {
                if (messagesReceived.Count == 0) return null;
                int[][] arr = messagesReceived.ToArray();
                messagesReceived.Clear();
                return arr;
            }
        }
    }

    // Called from ControllerScript.
    public void SendSignals(params Signal[] signals) {
        lock (threadLocks[1])
        {
            messagesToSend.AddRange(signals);
        }
    }
}
