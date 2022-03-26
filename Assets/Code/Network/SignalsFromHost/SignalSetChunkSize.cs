using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignalSetChunkSize : SignalFromHost {
    public readonly int ChunkSize;

    protected SignalSetChunkSize(params int[] intMessage) : base(intMessage) { ChunkSize = intMessage[1]; }
}
