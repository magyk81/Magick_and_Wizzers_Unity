using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SignalAddCard : SignalFromHost {
    // Piece whose hand the card will be added to.
    public readonly int HolderPieceID;
    public readonly int CardID;

    public SignalAddCard(params int[] intMessage) : base(intMessage) {
        int[] intMessageCut = intMessage.Skip(OVERHEAD_LEN).ToArray();

        HolderPieceID = intMessageCut[0];
        CardID = intMessageCut[1];
    }
}
