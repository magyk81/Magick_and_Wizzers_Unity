/* Copyright (C) All Rights Reserved
 * Unauthorized copying of this file, via any medium is prohibited.
 * Proprietary and confidential.
 * Written by Robin Campos <magyk81@gmail.com>, year 2021.
 */

public abstract class Waypoint {

    public static bool operator ==(Waypoint a, Waypoint b) { return a.Equals(b); }
    public static bool operator !=(Waypoint a, Waypoint b) { return !a.Equals(b); }

    public override bool Equals(object obj) { return Equals(obj as Waypoint); }
    public override int GetHashCode() { return base.GetHashCode(); }

    public abstract Waypoint Copy();

    protected abstract bool Equals(Waypoint other);
}
