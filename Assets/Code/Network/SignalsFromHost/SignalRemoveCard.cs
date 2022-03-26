using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SignalRemoveCard : SignalFromHost {
    // The piece whose hand the card is to be removed from.
    public readonly int HolderPieceID;
    public readonly int CardID;

    public SignalRemoveCard(params int[] intMessage) : base(intMessage) {
        int[] intMessageCut = intMessage.Skip(OVERHEAD_LEN).ToArray();

        HolderPieceID = intMessageCut[0];
        CardID = intMessageCut[1];
    }
}
