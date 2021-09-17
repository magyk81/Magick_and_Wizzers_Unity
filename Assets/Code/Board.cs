using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board
{
    public static readonly int CHUNK_SIZE = 10;
    private readonly int SIZE;
    private readonly int TILE_MAX;
    private List<Piece> pieces = new List<Piece>();
    public Board(int size) { SIZE = size; TILE_MAX = size * CHUNK_SIZE; }

    public int GetSize() { return SIZE; }
    public int GetTileMax() { return TILE_MAX; }
    public Coord TileToChunk(Coord tile)
    {
        int x = SIZE, z = SIZE;
        while (tile.X < x * CHUNK_SIZE) x--;
        while (tile.Z < z * CHUNK_SIZE) z--;
        return Coord._(x, z);
    }

    // Should only be called from Match.
    public void AddPiece(Piece piece) { pieces.Add(piece); }
    void MovePiece(int idx, int dir, int dist) { pieces[idx].Move(dir, dist); }
}
