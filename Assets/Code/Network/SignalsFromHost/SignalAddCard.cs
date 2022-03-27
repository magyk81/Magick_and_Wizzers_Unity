using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SignalAddCard : SignalFromHost {
    // Piece whose hand the card will be added to.
    public readonly int HolderPieceID;
    public readonly int CardID;

    /// <remarks>
    /// Used by client to interpret a received message.
    /// </remarks>
    public SignalAddCard(params int[] intMessage) : base(intMessage) {
        int[] intMessageCut = intMessage.Skip(OVERHEAD_LEN).ToArray();

        HolderPieceID = intMessageCut[0];
        CardID = intMessageCut[1];
    }

    /// <remarks>
    /// Used by host to get ready to send.
    /// </remarks>
    public SignalAddCard(Piece holderPiece, Card card) : this(HostInfoToIntMessage(holderPiece.ID, card.ID)) { }

    private static int[] HostInfoToIntMessage(int holderPieceID, int cardID) {
        int[] intMessage = new int[3];
        intMessage[0] = (int) Request.ADD_CARD;
        intMessage[1] = holderPieceID;
        intMessage[2] = cardID;
        return intMessage;
    }
}
