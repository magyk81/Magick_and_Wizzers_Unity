/* Copyright (C) All Rights Reserved
 * Unauthorized copying of this file, via any medium is prohibited.
 * Proprietary and confidential.
 * Written by Robin Campos <magyk81@gmail.com>, year 2021.
 */

using System.Globalization;
using UnityEngine;

public static class Util {
    public const int UP = 0, RIGHT = 1, DOWN = 2, LEFT = 3,
        UP_RIGHT = 4, UP_LEFT = 5, DOWN_RIGHT = 6, DOWN_LEFT = 7, COUNT = 8;
    
    private static readonly TextInfo TEXT_INFO = new CultureInfo("en-US", false).TextInfo;

    public static int DirOpp(int dir) {
        if (dir == UP) return DOWN;
        if (dir == RIGHT) return LEFT;
        if (dir == DOWN) return UP;
        if (dir == LEFT) return RIGHT;
        if (dir == UP_RIGHT) return DOWN_LEFT;
        if (dir == UP_LEFT) return DOWN_RIGHT;
        if (dir == DOWN_RIGHT) return UP_LEFT;
        if (dir == DOWN_LEFT) return UP_RIGHT;
        return -1;
    }
    public static int DirAdd(int a, int b) {
        if (a == UP) {
            if (b == UP || b == -1) return UP;
            if (b == DOWN) return -1;
            if (b == LEFT) return UP_LEFT;
            if (b == RIGHT) return UP_RIGHT;
        } else if (a == DOWN) {
            if (b == UP) return -1;
            if (b == DOWN || b == -1) return DOWN;
            if (b == LEFT) return DOWN_LEFT;
            if (b == RIGHT) return DOWN_RIGHT;
        } else if (a == RIGHT) {
            if (b == UP) return DOWN_LEFT;
            if (b == DOWN) return DOWN_RIGHT;
            if (b == LEFT) return -1;
            if (b == RIGHT || b == -1) return RIGHT;
        } else if (a == LEFT) {
            if (b == UP) return UP_LEFT;
            if (b == DOWN) return DOWN_LEFT;
            if (b == LEFT || b == -1) return LEFT;
            if (b == RIGHT) return -1;
        }
        return -1;
    }
    ///
    /// <returns>The clone ID that the dir moves it to.</returns>
    ///
    public static int DirAddToCloneID(int cloneID, int dir) {
        if (cloneID == 0) return dir;
        if (cloneID == UP - 1) {
            if (dir == UP) return -1;
            if (dir == RIGHT) return UP_RIGHT + 1;
            if (dir == DOWN) return 0; // center clone
            if (dir == LEFT) return UP_LEFT;
            if (dir == UP_RIGHT) return -1;
            if (dir == UP_LEFT) return -1;
            if (dir == DOWN_RIGHT) return RIGHT + 1;
            if (dir == DOWN_LEFT) return LEFT + 1;
        } else if (cloneID == DOWN - 1) {
            if (dir == UP) return 0; // center clone
            if (dir == RIGHT) return DOWN_RIGHT + 1;
            if (dir == DOWN) return -1;
            if (dir == LEFT) return DOWN_LEFT + 1;
            if (dir == UP_RIGHT) return RIGHT + 1;
            if (dir == UP_LEFT) return LEFT + 1;
            if (dir == DOWN_RIGHT) return -1;
            if (dir == DOWN_LEFT) return -1;
        } else if (cloneID == RIGHT - 1) {
            if (dir == UP) return UP_RIGHT + 1;
            if (dir == RIGHT) return -1;
            if (dir == DOWN) return DOWN_RIGHT + 1;
            if (dir == LEFT) return 0; // center clone
            if (dir == UP_RIGHT) return -1;
            if (dir == UP_LEFT) return UP + 1;
            if (dir == DOWN_RIGHT) return -1;
            if (dir == DOWN_LEFT) return DOWN + 1;
        } else if (cloneID == LEFT - 1) {
            if (dir == UP) return UP_LEFT + 1;
            if (dir == RIGHT) return 0; // center clone
            if (dir == DOWN) return DOWN_LEFT + 1;
            if (dir == LEFT) return -1;
            if (dir == UP_RIGHT) return UP + 1;
            if (dir == UP_LEFT) return -1;
            if (dir == DOWN_RIGHT) return DOWN + 1;
            if (dir == DOWN_LEFT) return -1;
        } else if (cloneID == UP_RIGHT - 1) {
            if (dir == UP) return -1;
            if (dir == RIGHT) return -1;
            if (dir == DOWN) return RIGHT + 1;
            if (dir == LEFT) return UP + 1;
            if (dir == UP_RIGHT) return -1;
            if (dir == UP_LEFT) return -1;
            if (dir == DOWN_RIGHT) return -1;
            if (dir == DOWN_LEFT) return 0; // center clone
        } else if (cloneID == UP_LEFT - 1) {
            if (dir == UP) return -1;
            if (dir == RIGHT) return UP + 1;
            if (dir == DOWN) return LEFT + 1;
            if (dir == LEFT) return -1;
            if (dir == UP_RIGHT) return -1;
            if (dir == UP_LEFT) return -1;
            if (dir == DOWN_RIGHT) return 0; // center clone
            if (dir == DOWN_LEFT) return -1;
        } else if (cloneID == DOWN_RIGHT - 1) {
            if (dir == UP) return RIGHT + 1;
            if (dir == RIGHT) return -1;
            if (dir == DOWN) return -1;
            if (dir == LEFT) return DOWN + 1;
            if (dir == UP_RIGHT) return -1;
            if (dir == UP_LEFT) return 0; // center clone
            if (dir == DOWN_RIGHT) return -1;
            if (dir == DOWN_LEFT) return -1;
        } else if (cloneID == DOWN_LEFT - 1) {
            if (dir == UP) return LEFT + 1;
            if (dir == RIGHT) return DOWN + 1;
            if (dir == DOWN) return -1;
            if (dir == LEFT) return -1;
            if (dir == UP_RIGHT) return 0; // center clone
            if (dir == UP_LEFT) return -1;
            if (dir == DOWN_RIGHT) return -1;
            if (dir == DOWN_LEFT) return -1;
        }
        return -1;
    }
    
    public static Vector3 DirToVec3(int dir) {
        if (dir == -1) return Vector3.zero;
        if (dir == UP) return new Vector3(0, 0, 1);
        if (dir == RIGHT) return new Vector3(1, 0, 0);
        if (dir == DOWN) return new Vector3(0, 0, -1);
        if (dir == LEFT) return new Vector3(-1, 0, 0);
        if (dir == UP_RIGHT) return DirToVec3(UP) + DirToVec3(RIGHT);
        if (dir == UP_LEFT) return DirToVec3(UP) + DirToVec3(LEFT);
        if (dir == DOWN_RIGHT) return DirToVec3(DOWN) + DirToVec3(RIGHT);
        if (dir == DOWN_LEFT) return DirToVec3(DOWN) + DirToVec3(LEFT);
        return Vector3.zero;
    }

    public static string DirToString(int dir) {
        if (dir == UP) return "UP";
        if (dir == RIGHT) return "RIGHT";
        if (dir == DOWN) return "DOWN";
        if (dir == LEFT) return "LEFT";
        if (dir == UP_RIGHT) return "UP_RIGHT";
        if (dir == UP_LEFT) return "UP_LEFT";
        if (dir == DOWN_RIGHT) return "DOWN_RIGHT";
        if (dir == DOWN_LEFT) return "DOWN_LEFT";
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

    public static int[] GetDists(Matches.Coord a, Matches.Coord b, int totalSize) {
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

    public static string ToTitleCase(string str) {
        return TEXT_INFO.ToTitleCase(str.Replace('_', ' ').ToLower());
    }
}
