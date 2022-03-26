using System;
using System.Linq;
using UnityEngine;

public abstract class SignalFromClient : Signal {
    // Each enum value corresponds to one of the signal classes that extends this class.
    public enum Request { CAST_SPELL, ADD_WAYPOINT, REMOVE_WAYPOINT }

    protected static int OVERHEAD_LEN = 2;

    public readonly Request SignalRequest;
    public readonly int ActingPlayerID;

    protected SignalFromClient(params int[] intMessage) : base(intMessage) {
        SignalRequest = (Request) intMessage[0];
        ActingPlayerID = intMessage[1];
    }
}
