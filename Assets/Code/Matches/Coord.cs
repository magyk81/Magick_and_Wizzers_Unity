/* Copyright (C) All Rights Reserved
 * Unauthorized copying of this file, via any medium is prohibited.
 * Proprietary and confidential.
 * Written by Robin Campos <magyk81@gmail.com>, year 2021.
 */

using System;
using UnityEngine;

namespace Matches {
    public struct Coord {
        public int X { get; }
        public int Z { get; }
        private Coord(int x, int z) {
            X = x;
            Z = z;
        }
        public Coord Copy() { return new Coord(X, Z); }
        public Vector2 ToVec2() { return new Vector2(X, Z); }
        public Vector3 ToVec3() { return new Vector3(X, 0, Z); }
        public Coord ToBounds(int size) {
            if (size == -1) return Coord.Null;
            int newX = X, newZ = Z;
            while (newX > size) newX -= size;
            while (newX < 0) newX += size;
            while (newZ > size) newZ -= size;
            while (newZ < 0) newZ += size;
            return new Coord(newX, newZ);
        }
        public Coord Dir(int dir) {
            if (dir == Util.UP) { return new Coord(X, Z + 1); }
            if (dir == Util.DOWN) { return new Coord(X, Z - 1); }
            if (dir == Util.LEFT) { return new Coord(X - 1, Z); }
            if (dir == Util.RIGHT) { return new Coord(X + 1, Z); }
            if (dir == Util.UP_LEFT) { return new Coord(X - 1, Z + 1); }
            if (dir == Util.UP_RIGHT) { return new Coord(X + 1, Z + 1); }
            if (dir == Util.DOWN_LEFT) { return new Coord(X - 1, Z - 1); }
            if (dir == Util.DOWN_RIGHT) { return new Coord(X + 1, Z - 1); }
            return this;
        }
        public Coord Dir(int horiz, int vert) {
            int newX = X, newZ = Z;
            if (horiz == Util.LEFT) newX--;
            else if (horiz == Util.RIGHT) newX++;
            if (vert == Util.UP) newZ++;
            else if (vert == Util.DOWN) newZ--;
            return new Coord(newX, newZ);
        }
        public static readonly Coord Null = new Coord(-1, -1);
        public static Coord operator +(Coord a, Coord b)
            => new Coord(a.X + b.X, a.Z + b.Z);
        public static Coord operator *(Coord a, int b)
            => new Coord(a.X * b, a.Z * b);
        public static bool operator ==(Coord a, Coord b) {
            return a.X == b.X && a.Z == b.Z;
        }
        public static bool operator !=(Coord a, Coord b) {
            return a.X != b.X || a.Z != b.Z;
        }
        public static Coord Lerp(Coord a, Coord b, float dist) {
            float xDist = ((float) (b.X - a.X)) * dist,
                zDist = ((float) (b.Z - a.Z)) * dist;
            return new Coord(
                (int) Math.Round(xDist) + a.X,
                (int) Math.Round(zDist) + a.Z);
        }
        public static Coord _(int x, int z) { return new Coord(x, z); }
        public static Coord _(int[] arr, int i) {
            return new Coord(arr[i], arr[i + 1]);
        }
        public override string ToString() { return "[" + X + ", " + Z + "]"; }

        public override bool Equals(object obj) {
            return base.Equals(obj);
        }
        public override int GetHashCode() {
            return base.GetHashCode();
        }
    }
}