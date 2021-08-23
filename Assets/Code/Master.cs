using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Master : Piece
{
    private Deck deck;
    private int playerIdx;
    private Coord pos;

    public Master(Player player, int playerIdx, int boardIdx, Coord initPos)
        : base("Master of " + player.Name, playerIdx, boardIdx, initPos)
    {
        player.AddMaster(this);
    }
}
