using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SignalMovePiece : SignalFromHost {
    public readonly int PieceID;
    public readonly PiecePos Position;

    public SignalMovePiece(params int[] intMessage) : base(intMessage) {
        int[] intMessageCut = intMessage.Skip(OVERHEAD_LEN).ToArray();

        PieceID = intMessageCut[0];
        Position = PiecePos._(intMessageCut.Skip(1).ToArray());
    }
}
