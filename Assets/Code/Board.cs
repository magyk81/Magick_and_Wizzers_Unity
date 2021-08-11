using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board
{
    public static readonly int CHUNK_SIZE = 5;
    private readonly int SIZE;
    private readonly int TILE_MAX;
    private List<Piece> pieces = new List<Piece>();
    public Board(int size) { SIZE = size; TILE_MAX = size * CHUNK_SIZE; }

    public int GetSize() { return SIZE; }

    void MovePiece(int idx, int dir, int dist) { pieces[idx].Move(dir, dist); }
}