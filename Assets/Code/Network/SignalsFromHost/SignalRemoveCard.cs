using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SignalRemoveCard : SignalFromHost {
    // The piece whose hand the card is to be removed from.
    public readonly int HolderPieceID;
    public readonly int CardID;

    /// <remarks>
    /// Used by client to interpret a received message.
    /// </remarks>
    public SignalRemoveCard(params int[] intMessage) : base(intMessage) {
        int[] intMessageCut = intMessage.Skip(OVERHEAD_LEN).ToArray();

        HolderPieceID = intMessageCut[0];
        CardID = intMessageCut[1];
    }

    /// <remarks>
    /// Used by host to get ready to send.
    /// </remarks>
    public SignalRemoveCard(Piece holderPiece, Card card) : this(HostInfoToIntMessage(holderPiece.ID, card.ID)) { }
    private static int[] HostInfoToIntMessage(int holderPieceID, int cardID) {
        int[] intMessage = new int[3];
        intMessage[0] = (int) Request.REMOVE_CARD;
        intMessage[1] = holderPieceID;
        intMessage[2] = cardID;
        return intMessage;
    }
}
