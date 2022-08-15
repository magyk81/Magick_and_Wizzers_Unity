/* Copyright (C) All Rights Reserved
 * Unauthorized copying of this file, via any medium is prohibited.
 * Proprietary and confidential.
 * Written by Robin Campos <magyk81@gmail.com>, year 2021.
 */

namespace Network {
    public abstract class SignalFromHost : Signal {
        // Each enum value corresponds to one of the signal classes that extends this class.
        public enum Request { INIT, ADD_PIECE, REMOVE_PIECE, MOVE_PIECE, ADD_CARDS,
            REMOVE_CARDS, UPDATE_WAYPOINTS }
        
        protected static int OVERHEAD_LEN = 1;

        public readonly Request SignalRequest;

        protected SignalFromHost(params int[] intMessage) : base(intMessage) {
            SignalRequest = (Request) intMessage[0];
        }
    }
}