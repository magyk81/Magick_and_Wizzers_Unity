namespace Network.SignalsFromClient {
    public class SignalInitFinished : SignalFromClient {
        // ActingPlayerID is not important in this case. Can be -1.
        public SignalInitFinished() : base(new int[] { (int) Request.INIT_FINISHED, -1 }) { }
    }
    }