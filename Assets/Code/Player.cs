using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    public enum Interface { PLAIN, WAYPOINT, HAND, DETAIL, SURRENDER, PAUSE }
    private Interface currInterface = Interface.PLAIN;
    private Piece pieceHovered;
    private List<Piece> pieceSelected = new List<Piece>();
    public Player() { }

    public void Input(int input)
    {
        if (currInterface == Interface.PLAIN)
        {
            if (input == 0) {}
        }
    }

    public void HoverPiece(GameObject obj)
    {
        
    }
}
