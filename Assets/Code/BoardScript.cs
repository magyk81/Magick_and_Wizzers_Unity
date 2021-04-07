using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardScript : MonoBehaviour
{
    [SerializeField]
    private GameObject BASE_CHUNK, BASE_PIECE, BASE_DEST;

    [SerializeField]
    private int size, chunkSize;
    private int TOTAL_SIZE;

    [SerializeField]
    private CameraScript cameraScript;
    private Transform trans;
    private Chunk[,] chunks;
    private Tile[,] tiles;
    public Tile[,] Tiles {
        set { // Assume "value" is chunk.Tiles
            for (int i = 0; i < chunkSize; i++) {
                for (int j = 0; j < chunkSize; j++) {
                    Tile _ = value[i, j];
                    tiles[_.X, _.Y] = _;
                }
            }
        }
    }

    [SerializeField]
    private HandScript handScript;

    // Start is called before the first frame update
    private void Start()
    {
        trans = GetComponent<Transform>();

        TOTAL_SIZE = size * chunkSize;

        chunks = new Chunk[size, size];
        tiles = new Tile[TOTAL_SIZE, TOTAL_SIZE];

        // Parent gameObject to keep chunks
        GameObject chunks_parent = new GameObject("Chunks");
        Transform chunks_parent_trans
            = chunks_parent.GetComponent<Transform>();
        chunks_parent_trans.SetParent(trans);

        // Set up chunks
        for (int j = 0; j < size; j++)
        {
            for (int i = 0; i < size; i++)
            {
                Coord[] coords;

                // Chunks along middle rows/columns
                if (size % 2 == 1 && (i == size / 2 || j == size / 2))
                {
                    // No clones needed for middle chunk
                    if (i == size / 2 && j == size / 2) coords = new Coord[1];

                    // 1 clone needed if not middle chunk
                    else
                    {
                        coords = new Coord[2];
                        float mid = size / 2F;
                        int xCoord = i, yCoord = j;

                        // clone in the z direction
                        if (i == size / 2)
                        {
                            if (j < mid) yCoord = j + size;
                            else yCoord = j - size;
                        }
                        // clone in the x direction
                        else // (j == size / 2)
                        {
                            if (i < mid) xCoord = i + size;
                            else xCoord = i - size;
                        }

                        coords[1] = Coord._(xCoord, yCoord);
                    }
                }

                // 3 clones needed for every other chunk
                else
                {
                    coords = new Coord[4];

                    float mid = size / 2F;
                    int xCoord = i, yCoord = j;
                    if (i < mid) xCoord = i + size;
                    else if (i >= mid) xCoord = i - size;
                    if (j < mid) yCoord = j + size;
                    else if (j >= mid) yCoord = j - size;

                    coords[1] = Coord._(xCoord, j);
                    coords[2] = Coord._(i, yCoord);
                    coords[3] = Coord._(xCoord, yCoord);
                }

                coords[0] = Coord._(i, j);
                chunks[i, j] = new Chunk(coords, chunkSize);
                chunks[i, j].SetGameObject(BASE_CHUNK, chunks_parent_trans);

                // Set up tiles
                Tiles = chunks[i, j].Tiles;
            }
        }
        BASE_CHUNK.SetActive(false);
        BASE_PIECE.SetActive(false);
        BASE_DEST.SetActive(false);


        // Set neighbors for chunks and their tiles
        for (int i = 0; i < TOTAL_SIZE; i++)
        {
            for (int j = 0; j < TOTAL_SIZE; j++)
            {
                if (i < size && j < size)
                {
                    Chunk[] chunkNeighbors = new Chunk[4];
                    if (i == 0) chunkNeighbors[Coord.LEFT] = chunks[size - 1, j];
                    else chunkNeighbors[Coord.LEFT] = chunks[i - 1, j];
                    if (i == size - 1) chunkNeighbors[Coord.RIGHT] = chunks[0, j];
                    else chunkNeighbors[Coord.RIGHT] = chunks[i + 1, j];
                    if (j == 0) chunkNeighbors[Coord.UP] = chunks[i, size - 1];
                    else chunkNeighbors[Coord.UP] = chunks[i, j - 1];
                    if (j == size - 1) chunkNeighbors[Coord.DOWN] = chunks[i, 0];
                    else chunkNeighbors[Coord.DOWN] = chunks[i, j + 1];
                    chunks[i, j].Neighbors = chunkNeighbors;
                }
                
                Tile[] neighbors = new Tile[4];
                neighbors[Coord.LEFT] = GetTile(i - 1, j);
                neighbors[Coord.RIGHT] = GetTile(i + 1, j);
                neighbors[Coord.UP] = GetTile(i, j + 1);
                neighbors[Coord.DOWN] = GetTile(i, j - 1);
                tiles[i, j].Neighbors = neighbors;
            }
        }        

        cameraScript.SetBounds(-0.5F, TOTAL_SIZE - 0.5F);

        // Testing
        AddPiece(tiles[0, 0]);
        pieces[0].AddDestination(tiles[2, 2]);
        pieces[0].AddDestination(tiles[1, 1]);
        pieces[0].AddDestination(tiles[0, 0]);
    }

    Tile GetTile(int x, int y)
    {
        while (x >= TOTAL_SIZE) x -= TOTAL_SIZE;
        while (x < 0) x += TOTAL_SIZE;
        while (y >= TOTAL_SIZE) y -= TOTAL_SIZE;
        while (y < 0) y += TOTAL_SIZE;
        return tiles[x, y];
    }
    Tile GetTile(Coord coord) { return GetTile(coord.X, coord.Y); }
    Chunk GetChunk(int x, int y)
    {
        while (x >= size) x -= size;
        while (x < 0) x += size;
        while (y >= size) y -= size;
        while (y < 0) y += size;
        return chunks[x, y];
    }
    Chunk GetChunk(Coord coord) { return GetChunk(coord.X, coord.Y); }

    bool AreNeighbors(Coord a, Coord b)
    {
        return GetTile(a).IsNeighbor(GetTile(b));
    }

    private List<Piece> pieces = new List<Piece>();
    public void AddPiece(Tile tile)
    {
        pieces.Add(new Piece(tile, trans, BASE_PIECE, BASE_DEST)
            { BoardSize = TOTAL_SIZE });
    }

    private void FixedUpdate()
    {
        foreach (Piece piece in pieces) { piece.FixedUpdate(); }
    }

    // Update is called once per frame
    private void Update()
    {
        foreach (Piece piece in pieces) { piece.Update(); }
    }
}
