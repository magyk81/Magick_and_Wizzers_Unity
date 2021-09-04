using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Util
{
    public static int UP = 0, RIGHT = 1, DOWN = 2, LEFT = 3,
        UP_RIGHT = 4, UP_LEFT = 5, DOWN_RIGHT = 6, DOWN_LEFT = 7;
    
    public static string DirToString(int dir)
    {
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

    public static string ReplaceSpaces(string s)
    {
        char[] chars = new char[s.Length];
        for (int i = 0; i < s.Length; i++)
        {
            if (s[i] == ' ') chars[i] = '_';
            else chars[i] = s[i];
        }
        return new string(chars);
    }
}
