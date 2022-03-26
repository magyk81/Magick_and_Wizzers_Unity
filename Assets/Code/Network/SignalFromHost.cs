/* Copyright (C) All Rights Reserved
 * Unauthorized copying of this file, via any medium is prohibited.
 * Proprietary and confidential.
 * Written by Robin Campos <magyk81@gmail.com>, year 2021.
 */

using System;

public abstract class SignalFromHost : Signal {
    // Each enum value corresponds to one of the signal classes that extends this class.
    public enum Request { SET_CHUNK_SIZE, ADD_PLAYER, ADD_BOARD, ADD_PIECE, REMOVE_PIECE, MOVE_PIECE, ADD_CARD,
        REMOVE_CARD, UPDATE_WAYPOINTS }
    
    protected static int OVERHEAD_LEN = 1;

    public readonly Request SignalRequest;

    protected SignalFromHost(params int[] intMessage) : base(intMessage) {
        SignalRequest = (Request) intMessage[0];
    }
}