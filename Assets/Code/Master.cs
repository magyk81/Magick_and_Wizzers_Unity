using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Master : Piece
{
    private Deck deck;
    private int playerIdx;
    private Coord pos;

    public Master(int playerIdx, int boardIdx, Coord initPos)
        : base("Player " + playerIdx + " Master", playerIdx, boardIdx, initPos)
    {}
}
