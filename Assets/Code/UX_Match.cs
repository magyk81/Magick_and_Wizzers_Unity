using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UX_Match
{
    private readonly int MAX_GAMEPADS = 6;
    private int[] boardSizes;
    private int[] fullSizes;
    private readonly int DIST_BETWEEN_BOARDS = 20;
    private readonly int MAX_WAYPOINTS = 5;

    Gamepad[] gamepads;
    private readonly GameObject chunkObj, pieceObj, waypointObj, cameraObj;
    
    // [board][x, z, clone]
    private GameObject[][,,] chunks;
    private GameObject[] cameras;
    private List<GameObject[]> pieces = new List<GameObject[]>();
    private GameObject[,] waypoints;

    private Material debugPieceMat, debugPieceHoverMat, debugPieceSelectMat;

    public UX_Match(GameObject chunkObj,
        GameObject pieceObj, GameObject waypointObj, GameObject cameraObj)
    {
        gamepads = new Gamepad[MAX_GAMEPADS];
        gamepads[0] = new Gamepad(true);
        this.chunkObj = chunkObj;
        this.pieceObj = pieceObj;
        this.waypointObj = waypointObj;
        this.cameraObj = cameraObj;

        // Load debugging materials
        debugPieceMat = Resources.Load<Material>("Materials/Debug Piece");
        debugPieceHoverMat = Resources.Load<Material>(
            "Materials/Debug Piece Hover");
        debugPieceSelectMat = Resources.Load<Material>(
            "Materials/Debug Piece Select");
    }

    public void InitBoardObjs(int[] boardSizes, int playerCount)
    {
        // Generate chunks
        this.boardSizes = boardSizes;
        fullSizes = new int[boardSizes.Length];
        chunks = new GameObject[boardSizes.Length][,,];
        for (int i = 0; i < boardSizes.Length; i++)
        {
            GameObject boardObj = new GameObject("Board " + (i + 1));
            Transform boardTra = boardObj.GetComponent<Transform>();
            chunks[i] = new GameObject[boardSizes[i], boardSizes[i], 9];

            fullSizes[i] = boardSizes[i] * Board.CHUNK_SIZE;
            for (int x = 0; x < boardSizes[i]; x++)
                for (int z = 0; z < boardSizes[i]; z++)
                {
                    GameObject chunkParent = new GameObject(
                        "Chunk [" + x + ", " + z + "]");
                    Transform chunkTra = chunkParent
                        .GetComponent<Transform>();
                    chunkTra.SetParent(boardTra);
                    for (int c = 0; c < 9; c++)
                        InstantiateChunk(chunkTra, i, x, z, c);
                }

            
        }

        // Generate waypoints
        waypoints = new GameObject[playerCount, MAX_WAYPOINTS];
        GameObject waypointGroupObj = new GameObject("Waypoints");
        Transform waypointGroupTra = waypointGroupObj
            .GetComponent<Transform>();
        for (int i = 0; i < playerCount; i++)
        {
            GameObject waypointGroupPlayerObj = new GameObject(
                "Waypoints - Player " + i);
            Transform waypointGroupPlayerTra = waypointGroupPlayerObj
                .GetComponent<Transform>();
            waypointGroupPlayerTra.SetParent(waypointGroupTra);
            for (int j = 0; j < MAX_WAYPOINTS; j++)
            {
                waypoints[i, j] = GameObject.Instantiate(
                    waypointObj, waypointGroupPlayerTra);
                waypoints[i, j].name = "Waypoint " + (j + 1);
                waypoints[i, j].SetActive(false);
            }
        }

        // Generate cameras
        GameObject cameraGroupObj = new GameObject("Cameras");
        Transform cameraGroupTra = cameraGroupObj.GetComponent<Transform>();
        cameras = new GameObject[playerCount];
        for (int i = 0; i < playerCount; i++)
        {
            cameraObj.SetActive(false);
            cameras[i] = GameObject.Instantiate(cameraObj, cameraGroupTra);
            cameras[i].name = "Camera " + (i + 1);
            if (i > 0)
                cameras[i].GetComponent<AudioListener>().enabled = false;
            cameras[i].GetComponent<Camera>().rect = new Rect(
                ((float) i) / playerCount, 0, 1F / playerCount, 1);
            cameras[i].SetActive(true);
        }

        chunkObj.SetActive(false);
        pieceObj.SetActive(false);
        waypointObj.SetActive(false);
    }

    private void InstantiateChunk(Transform boardTra,
        int i, int x, int z, int c)
    {
        chunks[i][x, z, c] = GameObject.Instantiate(chunkObj, boardTra);
        chunks[i][x, z, c].GetComponent<Transform>()
            .localScale = new Vector3(Board.CHUNK_SIZE, Board.CHUNK_SIZE, 1);
        int _c = c - 1;
        if (c == 0) chunks[i][x, z, c].name = "Real Chunk";
        else chunks[i][x, z, c].name = "Clone Chunk - " + Util.DirToString(_c);

        int _x = x * Board.CHUNK_SIZE, _z = z * Board.CHUNK_SIZE;
        if (_c == Util.UP || _c == Util.UP_LEFT || _c == Util.UP_RIGHT)
            _z += fullSizes[i];
        else if (_c == Util.DOWN || _c == Util.DOWN_LEFT
            || _c == Util.DOWN_RIGHT) _z -= fullSizes[i];
        if (_c == Util.RIGHT || _c == Util.UP_RIGHT || _c == Util.DOWN_RIGHT)
            _x += fullSizes[i];
        else if (_c == Util.LEFT || _c == Util.UP_LEFT || _c == Util.DOWN_LEFT)
            _x -= fullSizes[i];

        // Move the chunk far away if it's another board.
        _x += i * DIST_BETWEEN_BOARDS * Board.CHUNK_SIZE;

        chunks[i][x, z, c].GetComponent<Transform>()
            .localPosition = new Vector3(_x, 0, _z);
    }

    public int[] GetBoardCenter(int boardIdx)
    {
        if (boardIdx < boardSizes.Length && boardIdx >= 0)
        {
            int size = boardSizes[boardIdx];
            Transform tra = chunks[boardIdx][size, size, 0]
                .GetComponent<Transform>();
            int x = (int) tra.localPosition.x;
            int z = (int) tra.localPosition.z;
            return new int[] { x, z };
        }
        else return null;
    }

    public void AddPiece(Piece piece)
    {
        // [0] is the real piece, [10] is the parent,
        // [1] thru [9] are the clones.
        GameObject[] pieceGroup = new GameObject[10];
        pieceGroup[9] = new GameObject("Piece: " + piece.Name);
        Transform pieceParentTra = pieceGroup[9].GetComponent<Transform>();
        for (int i = 0; i < 9; i++)
        {
            pieceGroup[i] = GameObject.Instantiate(pieceObj, pieceParentTra);
            int _c = i - 1;
            if (i == 0) pieceGroup[i].name = "Real Piece";
            else pieceGroup[i].name = "Clone Piece - " + Util.DirToString(_c);

            int fullSize = fullSizes[piece.BoardIdx];
            float _x = piece.X - (Board.CHUNK_SIZE / 2),
                _z = piece.Z - (Board.CHUNK_SIZE / 2);
            if (Board.CHUNK_SIZE % 2 == 0) { _x += 0.5F; _z += 0.5F; }
            if (_c == Util.UP || _c == Util.UP_LEFT || _c == Util.UP_RIGHT)
                _z += fullSize;
            else if (_c == Util.DOWN || _c == Util.DOWN_LEFT
                || _c == Util.DOWN_RIGHT) _z -= fullSize;
            if (_c == Util.RIGHT || _c == Util.UP_RIGHT
                || _c == Util.DOWN_RIGHT) _x += fullSize;
            else if (_c == Util.LEFT || _c == Util.UP_LEFT
                || _c == Util.DOWN_LEFT) _x -= fullSize;

            // Move the piece to another board.
            _x += piece.BoardIdx * DIST_BETWEEN_BOARDS * Board.CHUNK_SIZE;

            pieceGroup[i].GetComponent<Transform>()
                .localPosition = new Vector3(_x, 0.1F, _z);
            pieceGroup[i].SetActive(true);
        }
    }

    // QueryGamepad is called once per frame
    int QueryGamepad(int idx)
    {
        if (gamepads[idx] != null) return gamepads[idx].GetInput();
        return -1;
    }
}
