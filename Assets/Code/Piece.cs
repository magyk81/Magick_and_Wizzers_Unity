using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece
{
    private string name;
    public string Name { get { return name; } }
    private int playerIdx, boardIdx;
    public int BoardIdx { get { return boardIdx; } }
    private Coord pos;
    public int X { get { return pos.X; } }
    public int Z { get { return pos.Z; } }
    public Piece(string name, int playerIdx, int boardIdx, Coord initPos)
    {
        this.name = name;
        this.playerIdx = playerIdx;
        pos = initPos.Copy();
    }

    // dist goes from 0 to 100
    public void Move(int dir, int dist)
    {

    }
}
