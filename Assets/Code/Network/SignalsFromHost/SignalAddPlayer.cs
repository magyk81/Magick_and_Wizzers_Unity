using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SignalAddPlayer : SignalFromHost {
    // The player that is to be added.
    public readonly int PlayerID;
    // The machine that the added player is from.
    public readonly int ClientID;
    public readonly bool IsBot;
    // The player's name.
    public readonly string Name;

    public SignalAddPlayer(params int[] intMessage) : base(intMessage) {
        int[] intMessageCut = intMessage.Skip(OVERHEAD_LEN).ToArray();

        PlayerID = intMessageCut[0];
        ClientID = intMessageCut[1];
        IsBot = (intMessageCut[2] == 0) ? false : true;

        char[] nameArr = new char[intMessageCut[3]];
        for (int i = 0, j = 4; i < nameArr.Length; i++, j++) { nameArr[i] = (char) intMessageCut[j]; }
        Name = nameArr.ToString();
    }
}
