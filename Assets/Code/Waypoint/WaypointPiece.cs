public class WaypointPiece : Waypoint {
    private readonly Piece mPiece;

    public WaypointPiece(Piece piece) { mPiece = piece; }

    public static bool operator ==(WaypointPiece a, WaypointPiece b) { return a.mPiece == b.mPiece; }
    public static bool operator !=(WaypointPiece a, WaypointPiece b) { return a.mPiece != b.mPiece; }

    public Piece Piece { get => mPiece; }

    public override bool Equals(object obj) { return base.Equals(obj); }
    public override int GetHashCode() { return base.GetHashCode(); }
    public override string ToString() { return mPiece.Name; }

    public override Waypoint Copy() {
        return new WaypointPiece(mPiece);
    }

    protected override bool Equals(Waypoint other) {
        if (other is WaypointPiece) return mPiece == (other as WaypointPiece).mPiece;
        return false;
    }
}
