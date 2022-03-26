using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SignalAddBoard : SignalFromHost {
    public readonly int BoardID;
    // How many chunks long the board is.
    public readonly int Size;
    public readonly string Name;

    public SignalAddBoard(params int[] intMessage) : base(intMessage) {
        int[] intMessageCut = intMessage.Skip(OVERHEAD_LEN).ToArray();

        BoardID = intMessageCut[0];
        Size = intMessageCut[1];

        char[] nameArr = new char[intMessageCut[2]];
        for (int i = 0, j = 3; i < nameArr.Length; i++, j++) { nameArr[i] = (char) intMessageCut[j]; }
        Name = nameArr.ToString();
    }
}
