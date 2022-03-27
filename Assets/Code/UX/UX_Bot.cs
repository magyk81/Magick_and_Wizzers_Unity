using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UX_Bot : UX_Player {
    public override void Init(int playerID, int localPlayerIdx, float[][] boardBounds, int quadSize) {
        mPlayerID = playerID;
        mLocalPlayerIdx = localPlayerIdx;
    }
    public override void QueryCamera() { }
    public override SignalFromClient QueryGamepad() { return null; }

    protected override void Update() {
        // Nothing
    }
}
