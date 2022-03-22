public class WaypointTile : Waypoint {
    private readonly Coord mTile;

    public WaypointTile(Coord tile) {
        mTile = tile;
    }

    public static bool operator ==(WaypointTile a, WaypointTile b) { return a.mTile == b.mTile; }
    public static bool operator !=(WaypointTile a, WaypointTile b) { return a.mTile != b.mTile; }

    public Coord Tile { get => mTile; }

    public override bool Equals(object obj) { return base.Equals(obj); }
    public override int GetHashCode() { return base.GetHashCode(); }
    public override string ToString() { return mTile.ToString(); }

    public override Waypoint Copy() {
        return new WaypointTile(mTile);
    }

    protected override bool Equals(Waypoint other) {
        if (other is WaypointTile) return mTile == (other as WaypointTile).mTile;
        return false;
    }
}
