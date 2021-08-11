using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    public enum Interface { PLAIN, WAYPOINT, HAND, DETAIL, SURRENDER, PAUSE }
    private Interface currInterface = Interface.PLAIN;
    private List<Piece> pieces = new List<Piece>();
    public Player(){}

    public void Input(int input)
    {
        if (currInterface == Interface.PLAIN)
        {
            if (input == 0) {}
        }
    }
}
