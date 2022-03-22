/* Copyright (C) All Rights Reserved
 * Unauthorized copying of this file, via any medium is prohibited.
 * Proprietary and confidential.
 * Written by Robin Campos <magyk81@gmail.com>, year 2021.
 */

using UnityEngine;

public class Util {
    public static readonly int UP = 0, RIGHT = 1, DOWN = 2, LEFT = 3,
        UP_RIGHT = 4, UP_LEFT = 5, DOWN_RIGHT = 6, DOWN_LEFT = 7;
    
    public static string DirToString(int dir) {
        if (dir == 0) return "UP";
        else if (dir == 1) return "RIGHT";
        else if (dir == 2) return "DOWN";
        else if (dir == 3) return "LEFT";
        else if (dir == 4) return "UP_RIGHT";
        else if (dir == 5) return "UP_LEFT";
        else if (dir == 6) return "DOWN_RIGHT";
        else if (dir == 7) return "DOWN_LEFT";
        return "NONE";
    }

    public static string ReplaceSpaces(string str) {
        char[] chars = new char[str.Length];
        for (int i = 0; i < str.Length; i++)
        {
            if (str[i] == ' ') chars[i] = '_';
            else chars[i] = str[i];
        }
        return new string(chars);
    }

    public static int AddDirs(int horiz, int vert) {
        if (horiz == -1) return vert;
        if (vert == -1) return horiz;
        if (vert == UP) {
            if (horiz == LEFT) return UP_LEFT;
            return UP_RIGHT;
        } else {
            if (horiz == LEFT) return DOWN_LEFT;
            return DOWN_RIGHT;
        }
    }
    public static int[] DiagToDirs(int diag) {
        if (diag == UP_LEFT) return new int[] { LEFT, UP };
        if (diag == DOWN_LEFT) return new int[] { LEFT, DOWN };
        if (diag == UP_RIGHT) return new int[] { RIGHT, UP };
        if (diag == DOWN_RIGHT) return new int[] { RIGHT, DOWN };
        return null;
    }
    public static bool InDiag(int diag, int straight) {
        if (diag == UP_LEFT) return straight == UP || straight == LEFT;
        else if (diag == UP_RIGHT) return straight == UP || straight == RIGHT;
        else if (diag == DOWN_LEFT) return straight == DOWN || straight == LEFT;
        else if (diag == DOWN_RIGHT) return straight == DOWN || straight == RIGHT;
        Debug.LogWarning("diag value \"" + diag + "\" not valid");
        return false;
    }

    public static int[] GetDists(Coord a, Coord b, int totalSize) {
        int[] dists = new int[4];
        if (a.X > b.X) {
            dists[Util.LEFT] = b.X - a.X;
            dists[Util.RIGHT] = totalSize + dists[Util.LEFT];
        } else if (a.X < b.X) {
            dists[Util.RIGHT] = b.X - a.X;
            dists[Util.LEFT] = totalSize + dists[Util.LEFT];
        } else {
            dists[Util.LEFT] = 0;
            dists[Util.RIGHT] = 0;
        }
        if (a.Z > b.Z) {
            dists[Util.DOWN] = b.Z - a.Z;
            dists[Util.UP] = totalSize + dists[Util.LEFT];
        } else if (a.Z < b.Z) {
            dists[Util.UP] = b.Z - a.Z;
            dists[Util.DOWN] = totalSize + dists[Util.LEFT];
        } else {
            dists[Util.UP] = 0;
            dists[Util.DOWN] = 0;
        }
        dists[Util.DOWN] *= -1;
        dists[Util.LEFT] *= -1;
        return dists;
    }
}
