using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UX_Board : MonoBehaviour
{
    [SerializeField]
    private int distBetweenBoards, cloneLength;
    [SerializeField]
    UX_Chunk baseChunk;
    [SerializeField]
    UX_Piece basePiece;
    private UX_Tile[,] tiles;
    private UX_Chunk[,] chunks;
    private List<UX_Piece> pieces = new List<UX_Piece>();
    private bool isClone;
    private int boardIdx;

    public void Init(int size, int boardIdx, int cloneIdx = -1)
    {
        this.boardIdx = boardIdx;
        isClone = (cloneIdx >= 0);

        int i0 = 0, j0 = 0, iEnd = size, jEnd = size;
        if (isClone)
        {
            if (cloneLength <= 0) cloneLength = size;
            if (cloneIdx == Util.UP || cloneIdx == Util.UP_LEFT
                || cloneIdx == Util.UP_RIGHT)
                jEnd = size - cloneLength;
            if (cloneIdx == Util.DOWN || cloneIdx == Util.DOWN_LEFT
                || cloneIdx == Util.DOWN_RIGHT) j0 = cloneLength;
            if (cloneIdx == Util.RIGHT || cloneIdx == Util.UP_RIGHT
                || cloneIdx == Util.DOWN_RIGHT)
                iEnd = size - cloneLength;
            if (cloneIdx == Util.LEFT || cloneIdx == Util.UP_LEFT
                || cloneIdx == Util.DOWN_LEFT) i0 = cloneLength;
        }

        tiles = new UX_Tile[size * Chunk.Size, size * Chunk.Size];
        chunks = new UX_Chunk[size, size];

        // Generate chunks.
        for (int i = i0; i < iEnd; i++)
        {
            for (int j = j0; j < jEnd; j++)
            {
                chunks[i, j] = Instantiate(baseChunk);

                // Generate tiles.
                for (int a = 0; a < Chunk.Size; a++)
                {
                    for (int b = 0; b < Chunk.Size; b++)
                    {
                        Coord tilePos = Coord._(
                            i * Chunk.Size + a, j * Chunk.Size + b);
                        tiles[tilePos.X, tilePos.Z]
                            = new UX_Tile(tilePos, boardIdx);
                    }
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
