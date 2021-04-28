using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardScript : MonoBehaviour
{
    [SerializeField]
    private int targetFramerate;
    [SerializeField]
    private GameObject BASE_CHUNK, BASE_TILE, BASE_PIECE, BASE_DEST,
        BASE_CANVAS, BASE_CAMERA;
    [SerializeField]
    private Transform selectionParticles;

    [SerializeField]
    private int size, chunkSize;
    private int TOTAL_SIZE;

    [SerializeField]
    private int playerCount;
    private Player[] players;
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

    // Start is called before the first frame update
    private void Start()
    {
        Application.targetFrameRate = targetFramerate;

        trans = GetComponent<Transform>();

        TOTAL_SIZE = size * chunkSize;

        chunks = new Chunk[size, size];
        tiles = new Tile[TOTAL_SIZE, TOTAL_SIZE];

        // Parent gameObject to keep chunks
        GameObject chunks_parent = new GameObject("Chunks");
        Transform chunks_parent_trans
            = chunks_parent.GetComponent<Transform>();
        chunks_parent_trans.SetParent(trans);

        // Set up collider grid and its parent gameObject
        GameObject[,] colliderGrid = new GameObject[chunkSize, chunkSize];
        GameObject colliderGrid_parent = new GameObject("Collider Grid");
        Transform colliderGrid_parent_trans
            = colliderGrid_parent.GetComponent<Transform>();
        colliderGrid_parent_trans.SetParent(trans);
        for (int i = 0; i < chunkSize; i++)
        {
            for (int j = 0; j < chunkSize; j++)
            {
                colliderGrid[i, j] = Instantiate(
                    BASE_TILE, colliderGrid_parent_trans);
                colliderGrid[i, j].name = "Tile [" + i + ", " + j + "]";
                colliderGrid[i, j].SetActive(false);
            }
        }

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
                chunks[i, j] = new Chunk(coords, colliderGrid, chunkSize);
                chunks[i, j].SetGameObject(BASE_CHUNK, chunks_parent_trans);

                // Set up tiles
                Tiles = chunks[i, j].Tiles;
            }
        }

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

        // Parent gameObject to keep destinations
        GameObject destinations_parent = new GameObject("Destinations");
        Transform destinations_parent_trans
            = destinations_parent.GetComponent<Transform>();
        destinations_parent_trans.SetParent(trans);
        BASE_DEST.GetComponent<Transform>()
            .SetParent(destinations_parent_trans);

        // Set up players
        players = new Player[playerCount];
        for (int i = 0; i < playerCount; i++)
        {
            // Each player gets their own camera and canvas
            GameObject canvasObj = Instantiate(BASE_CANVAS);
            canvasObj.SetActive(true);
            canvasObj.name = "Canvas " + (i + 1);

            GameObject cameraObj = Instantiate(BASE_CAMERA);
            cameraObj.SetActive(true);
            cameraObj.name = "Camera " + (i + 1);
            Camera cam = cameraObj.GetComponent<Camera>();

            canvasObj.GetComponent<Canvas>().worldCamera = cam;

            // Set up cameras to be split-screen depending on player count
            if (playerCount == 2)
            {
                if (i == 0) cam.rect = new Rect(0, 0, 0.5F, 1);
                else cam.rect = new Rect(0.5F, 0, 0.5F, 1);
            }
            else if (playerCount == 3)
            {
                if (i == 0) cam.rect = new Rect(0, 0, 1, 0.5F);
                else if (i == 1) cam.rect = new Rect(0, 0.5F, 0.5F, 0.5F);
                else if (i == 2) cam.rect = new Rect(0.5F, 0.5F, 0.5F, 0.5F);
            }
            else if (playerCount == 4)
            {
                if (i == 0) cam.rect = new Rect(0, 0.5F, 0.5F, 0.5F);
                else if (i == 1) cam.rect = new Rect(0.5F, 0.5F, 0.5F, 0.5F);
                else if (i == 2) cam.rect = new Rect(0, 0, 0.5F, 0.5F);
                else if (i == 3) cam.rect = new Rect(0.5F, 0, 0.5F, 0.5F);
            }

            players[i] = new Player(
                canvasObj.GetComponent<HandScript>(),
                cameraObj.GetComponent<CameraScript>(),
                selectionParticles,
                destinations_parent_trans,
                TOTAL_SIZE, i);
        }

        // BASE_PIECE still needed for adding new pieces
        BASE_PIECE.SetActive(false);
        Destroy(BASE_CHUNK);
        Destroy(BASE_TILE);
        Destroy(BASE_DEST);
        Destroy(BASE_CANVAS);
        Destroy(BASE_CAMERA);        

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
        pieces.Add(new Piece(tile, trans, BASE_PIECE)
            { BoardSize = TOTAL_SIZE });
    }

    public static void SetClonePositions(Transform[] trans, int boardSize)
    {
        float xPosF = trans[0].localPosition.x;
        float altitude = trans[0].localPosition.y;
        float yPosF = trans[0].localPosition.z;

        int xMult = (xPosF < boardSize / 2 - 0.5F) ? 1 : -1;
        int yMult = (yPosF < boardSize / 2 - 0.5F) ? 1 : -1;

        trans[1].localPosition = new Vector3(
            xPosF + (boardSize * xMult),
            altitude,
            yPosF);
        trans[2].localPosition = new Vector3(
            xPosF,
            altitude,
            yPosF + (boardSize * yMult));
        trans[3].localPosition = new Vector3(
            xPosF + (boardSize * xMult),
            altitude,
            yPosF + (boardSize * yMult));
    }

    private void FixedUpdate()
    {
        foreach (Piece piece in pieces) { piece.FixedUpdate(); }
    }

    // Update is called once per frame
    private void Update()
    {
        foreach (Piece piece in pieces) { piece.Update(); }

        foreach (Player player in players) { player.Update(pieces); }
    }
}
