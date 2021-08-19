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
    private readonly GameObject waypointObj, cameraObj;
    private readonly UX_Chunk baseChunk;
    private readonly UX_Piece basePiece;
    private Transform boardTra;
    
    // [boardIdx][x, z]
    private UX_Chunk[][,] chunks;
    private GameObject[] cameras;
    private List<UX_Piece> pieces = new List<UX_Piece>();
    private GameObject[,] waypoints;

    private Material debugPieceMat, debugPieceHoverMat, debugPieceSelectMat;

    public UX_Match(UX_Chunk baseChunk, UX_Piece basePiece,
        GameObject waypointObj, GameObject cameraObj)
    {
        gamepads = new Gamepad[MAX_GAMEPADS];
        gamepads[0] = new Gamepad(true);
        this.baseChunk = baseChunk;
        this.basePiece = basePiece;
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

        chunks = new UX_Chunk[boardSizes.Length][,];

        for (int i = 0; i < boardSizes.Length; i++)
        {
            GameObject boardObj = new GameObject("Board " + (i + 1));
            boardTra = boardObj.GetComponent<Transform>();
            chunks[i] = new UX_Chunk[boardSizes[i], boardSizes[i]];

            fullSizes[i] = boardSizes[i] * Board.CHUNK_SIZE;
            for (int x = 0; x < boardSizes[i]; x++)
                for (int z = 0; z < boardSizes[i]; z++)
                {
                    chunks[i][x, z] = GameObject.Instantiate(baseChunk, boardTra);
                    chunks[i][x, z].Init(fullSizes[i], DIST_BETWEEN_BOARDS,
                        i, x, z);
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
            cameras[i].GetComponent<CameraScript>().Setup(i);
        }

        baseChunk.gameObject.SetActive(false);
        basePiece.gameObject.SetActive(false);
        waypointObj.SetActive(false);
    }

    public void AddPiece(Piece piece)
    {
        UX_Piece newUxPiece = GameObject.Instantiate(basePiece, boardTra);
        pieces.Add(newUxPiece);
        newUxPiece.Init(piece, fullSizes[piece.BoardIdx], DIST_BETWEEN_BOARDS);
        newUxPiece.UpdatePosition();
    }

    // QueryGamepad is called once per frame
    int QueryGamepad(int idx)
    {
        if (gamepads[idx] != null) return gamepads[idx].GetInput();
        return -1;
    }
}
