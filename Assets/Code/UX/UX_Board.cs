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
    private Transform pieceParent;
    private UX_Tile[,] tiles;
    private UX_Chunk[,] chunks;
    public UX_Chunk[,] Chunks { get { return chunks; } }
    private List<UX_Piece> pieces = new List<UX_Piece>();
    private readonly static float PIECE_LIFT_DIST = 0.1F;
    private int boardIdx, cloneIdx;

    public void Init(int size, int boardIdx, int cloneIdx = -1)
    {
        this.boardIdx = boardIdx;

        int i0 = 0, j0 = 0, iEnd = size, jEnd = size;
        int cloneOffsetX = 0, cloneOffsetZ = 0,
            apartOffset = boardIdx * distBetweenBoards;
        if (cloneIdx >= 0)
        {
            if (cloneLength <= 0 || cloneLength > size) cloneLength = size;
            if (cloneIdx == Util.UP || cloneIdx == Util.UP_LEFT
                || cloneIdx == Util.UP_RIGHT)
            {
                jEnd = cloneLength;
                cloneOffsetZ = size * Chunk.Size;
            }
            if (cloneIdx == Util.DOWN || cloneIdx == Util.DOWN_LEFT
                || cloneIdx == Util.DOWN_RIGHT)
            {
                j0 = size - cloneLength;
                cloneOffsetZ = -size * Chunk.Size;
            }
            if (cloneIdx == Util.RIGHT || cloneIdx == Util.UP_RIGHT
                || cloneIdx == Util.DOWN_RIGHT)
            {
                iEnd = cloneLength;
                cloneOffsetX = size * Chunk.Size;
            }
            if (cloneIdx == Util.LEFT || cloneIdx == Util.UP_LEFT
                || cloneIdx == Util.DOWN_LEFT)
            {
                i0 = size - cloneLength;
                cloneOffsetX = -size * Chunk.Size;
            }
        }

        tiles = new UX_Tile[size * Chunk.Size, size * Chunk.Size];
        chunks = new UX_Chunk[size, size];

        Transform chunkParent = new GameObject().GetComponent<Transform>();
        chunkParent.gameObject.name = "Chunks";
        chunkParent.parent = GetComponent<Transform>();

        // Change chunk material if Sheol (temporary)
        if (boardIdx == 1)
        {
            Material sheolMat = Resources.Load<Material>(
                "Materials/Debug Chunk Sheol");
            baseChunk.GetComponent<MeshRenderer>().material = sheolMat;
        }

        // Generate chunks.
        for (int i = i0; i < iEnd; i++)
        {
            for (int j = j0; j < jEnd; j++)
            {
                chunks[i, j] = Instantiate(baseChunk, chunkParent);

                // Position and scale chunks.
                Transform chunkTra = chunks[i, j].GetComponent<Transform>();
                chunkTra.localScale = new Vector3(
                    Chunk.Size, Chunk.Size, 1);
                Coord chunkPos = Coord._(
                    (i * Chunk.Size) + cloneOffsetX + apartOffset,
                    (j * Chunk.Size) + cloneOffsetZ);
                chunkTra.localPosition = new Vector3(
                    chunkPos.X + ((float) Chunk.Size / 2F),
                    0,
                    chunkPos.Z + ((float) Chunk.Size / 2F));

                // Generate tiles.
                UX_Tile[,] chunkTiles = new UX_Tile[Chunk.Size, Chunk.Size];
                for (int a = 0; a < Chunk.Size; a++)
                {
                    for (int b = 0; b < Chunk.Size; b++)
                    {
                        Coord tilePos = Coord._(
                            i * Chunk.Size + a, j * Chunk.Size + b);
                        tiles[tilePos.X, tilePos.Z] = new UX_Tile(
                            tilePos, size * Chunk.Size, cloneIdx, boardIdx);
                        chunkTiles[
                            tilePos.X - (i * Chunk.Size),
                            tilePos.Z - (j * Chunk.Size)]
                            = tiles[tilePos.X, tilePos.Z];
                    }
                }

                chunks[i, j].Init(Coord._(i, j), boardIdx, chunkTiles);
                chunks[i, j].gameObject.SetActive(true);
            }
        }

        pieceParent = new GameObject().GetComponent<Transform>();
        pieceParent.gameObject.name = "Pieces";
        pieceParent.parent = GetComponent<Transform>();

        gameObject.SetActive(true);
    }

    public void AddPiece(Piece piece)
    {
        UX_Piece uxPiece = Instantiate(
            basePiece.gameObject,
            pieceParent)
            .GetComponent<UX_Piece>();
        uxPiece.gameObject.name = piece.Name;
        piece.SetUX(uxPiece, cloneIdx + 1);
        pieces.Add(uxPiece);

        UX_Tile tile = tiles[piece.Pos.X, piece.Pos.Z];
        if (tile != null)
        {
            // Position the piece.
            uxPiece.GetComponent<Transform>().localPosition = new Vector3(
                tile.UX_Pos.x, PIECE_LIFT_DIST, tile.UX_Pos.z);
            uxPiece.gameObject.SetActive(true);
        }       
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
