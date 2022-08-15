namespace Matches.UX {
    public class Bot : Player {
        public override void Init(int playerID, int localPlayerIdx, Board[][] boards) {
                
            mPlayerID = playerID;
            mLocalPlayerIdx = localPlayerIdx;
        }
    }
}