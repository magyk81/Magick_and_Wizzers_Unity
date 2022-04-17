using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UX_Bot : UX_Player {
    public override void Init(int playerID, int localPlayerIdx, float[][] boardBounds, Coord[][] boardOffsets,
        int quadSize) {

        mPlayerID = playerID;
        mLocalPlayerIdx = localPlayerIdx;
    }
}
