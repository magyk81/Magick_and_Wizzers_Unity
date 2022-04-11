/* Copyright (C) All Rights Reserved
 * Unauthorized copying of this file, via any medium is prohibited.
 * Proprietary and confidential.
 * Written by Robin Campos <magyk81@gmail.com>, year 2021.
 */

public abstract class Waypoint {
    public enum WaypointType { Tile, Piece }

    public abstract Waypoint Copy();

    protected abstract bool Equals(Waypoint other);
}
