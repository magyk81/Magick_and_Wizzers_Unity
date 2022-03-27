using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/**
 * This signal should only be sent once at the beginning of the match.
 */
public class SignalInit : SignalFromHost {
    public readonly int PlayerCount;
    public readonly int[] PlayerIDs;
    public readonly int[] PlayerClientIDs;
    public readonly bool[] PlayerIsBot;
    public readonly string[] PlayerNames;
    public readonly int BoardCount;
    public readonly int[] BoardSizes;
    public readonly string[] BoardNames;

    /// <remarks>
    /// Used by client to interpret a received message.
    /// </remarks>
    public SignalInit(int[] intMessage) : base(intMessage) {
        int[] intMessageCut = intMessage.Skip(OVERHEAD_LEN).ToArray();
        int j = 0;
        PlayerCount = intMessageCut[j]; j++;
        PlayerIDs = new int[PlayerCount];
        PlayerClientIDs = new int[PlayerCount];
        PlayerIsBot = new bool[PlayerCount];
        PlayerNames = new string[PlayerCount];
        for (int i = 0; i < PlayerCount; i++) {
            PlayerIDs[i] = intMessageCut[j]; j++;
            PlayerClientIDs[i] = intMessageCut[j]; j++;
            PlayerIsBot[i] = (intMessageCut[j] != 0); j++;
            int nameLen = intMessageCut[j]; j++;
            PlayerNames[i] = new string(intMessageCut.Skip(j).Take(nameLen).Select(item => (char) item).ToArray());
            j += nameLen;
        }
        BoardCount = intMessageCut[j]; j++;
        BoardSizes = new int[BoardCount];
        BoardNames = new string[BoardCount];
        for (int i = 0; i < BoardCount; i++) {
            BoardSizes[i] = intMessageCut[j]; j++;
            int nameLen = intMessageCut[j]; j++;
            BoardNames[i] = new string(intMessageCut.Skip(j).Take(nameLen).Select(item => (char) item).ToArray());
            j += nameLen;
        }
    }

    /// <remarks>
    /// Used by host to get ready to send.
    /// </remarks>
    public SignalInit(Player[] players, Board[] boards) : base(HostInfoToIntMessage(players, boards)) { }

    /// <remarks>
    /// Used to pass the integer message to another scene without using a socket.
    /// </remarks>
    public int[] IntMessage { get => mIntMessage; }

    private static int[] HostInfoToIntMessage(Player[] players, Board[] boards) {
        List<int> message = new List<int>();
        message.Add((int) Request.INIT);
        message.Add(players.Length);
        foreach (Player player in players) {
            message.Add(player.ID);
            message.Add(player.ClientID);
            message.Add(player.IsBot ? 1 : 0);
            message.Add(player.Name.Length);
            message.AddRange(player.Name.Select(item => (int) item));
        }
        message.Add(boards.Length);
        foreach (Board board in boards) {
            message.Add(board.Size);
            message.Add(board.Name.Length);
            message.AddRange(board.Name.Select(item => (int) item));
        }
        return message.ToArray();
    }
}
