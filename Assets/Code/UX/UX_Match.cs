using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UX_Match
{
    private int[] boardSizes;
    private int[] fullSizes;
    private readonly int DIST_BETWEEN_BOARDS = 20;
    private readonly int MAX_WAYPOINTS = 5;

    private readonly UX_Chunk baseChunk;
    private readonly UX_Piece basePiece;
    private readonly GameObject waypointObj;
    private readonly CameraScript baseCam;
    private readonly Canvas baseCanvas;
    private Transform boardTra;
    
    // [boardIdx][x, z]
    private UX_Chunk[][,] chunks;
    private List<UX_Piece> pieces = new List<UX_Piece>();
    private UX_Player[] players;

    private Material debugPieceMat, debugPieceHoverMat, debugPieceSelectMat;

    public UX_Match(UX_Chunk baseChunk, UX_Piece basePiece,
        GameObject waypointObj, CameraScript baseCam, Canvas baseCanvas)
    {
        this.baseChunk = baseChunk;
        this.basePiece = basePiece;
        this.waypointObj = waypointObj;
        this.baseCam = baseCam;
        this.baseCanvas = baseCanvas;

        // Load debugging materials
        debugPieceMat = Resources.Load<Material>("Materials/Debug Piece");
        debugPieceHoverMat = Resources.Load<Material>(
            "Materials/Debug Piece Hover");
        debugPieceSelectMat = Resources.Load<Material>(
            "Materials/Debug Piece Select");
    }

    public void InitBoardObjs(int[] boardSizes, Player[] players)
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
                    chunks[i][x, z] = GameObject.Instantiate(
                        baseChunk, boardTra);
                    chunks[i][x, z].Init(fullSizes[i], DIST_BETWEEN_BOARDS,
                        i, x, z);
                }
        }

        // Generate waypoints
        GameObject[,] waypoints
            = new GameObject[players.Length, MAX_WAYPOINTS];
        GameObject waypointGroupObj = new GameObject("Waypoints");
        Transform waypointGroupTra = waypointGroupObj
            .GetComponent<Transform>();
        for (int i = 0; i < players.Length; i++)
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

        // Set physical boundaries for cameras
        int boardCount = boardSizes.Length;
        float[][] bounds = new float[boardCount][];
        for (int i = 0; i < boardCount; i++)
        {
            bounds[i] = new float[4];
            bounds[i][Util.UP]
                = chunks[i][0, boardSizes[i] - 1].GetEdge(Util.UP);
            bounds[i][Util.DOWN]
                = chunks[i][0, 0].GetEdge(Util.DOWN);
            bounds[i][Util.RIGHT]
                = chunks[i][boardSizes[i] - 1, 0].GetEdge(Util.RIGHT);
            bounds[i][Util.LEFT]
                = chunks[i][0, 0].GetEdge(Util.LEFT);
        }

        // Set group for cameras, and canvases
        Transform camGroupTra
            = new GameObject("Cameras").GetComponent<Transform>();
        Transform canvasGroupTra
            = new GameObject("Canvases").GetComponent<Transform>();

        // Generate UX_Players with their gamepads and cameras
        int localPlayerCount = 0;
        foreach (Player player in players)
        {
            if (player.PlayerType == Player.Type.LOCAL_PLAYER)
                localPlayerCount++;
        }
        this.players = new UX_Player[localPlayerCount];
        for (int i = 0, j = 0; i < players.Length; i++)
        {
            if (players[i].PlayerType == Player.Type.LOCAL_PLAYER)
            {
                Gamepad gamepad = new Gamepad(j == 0);

                GameObject camClone = GameObject.Instantiate(
                    baseCam.gameObject, camGroupTra);
                camClone.name = "Camera - Player " + (i + 1);
                camClone.GetComponent<AudioListener>().enabled = (j == 0);
                camClone.GetComponent<CameraScript>().Bounds = bounds;

                GameObject canvasClone = GameObject.Instantiate(
                    baseCanvas.gameObject, canvasGroupTra);
                canvasClone.name = "Canvas - Player " + (i + 1);
                canvasClone.GetComponent<Canvas>().worldCamera
                    = camClone.GetComponent<Camera>();
                
                camClone.GetComponent<CameraScript>().InitCamObjs(
                    canvasClone.GetComponent<CanvasScript>());

                this.players[j] = new UX_Player(players[i], gamepad,
                    camClone.GetComponent<CameraScript>());
                j++;
            } 
        }
        
        baseChunk.gameObject.SetActive(false);
        basePiece.gameObject.SetActive(false);
        waypointObj.SetActive(false);
        baseCam.gameObject.SetActive(false);
        baseCanvas.gameObject.SetActive(false);
    }

    public void AddPiece(Piece piece)
    {
        UX_Piece newUxPiece = GameObject.Instantiate(basePiece, boardTra);
        pieces.Add(newUxPiece);
        newUxPiece.Init(piece, fullSizes[piece.BoardIdx], DIST_BETWEEN_BOARDS);
        newUxPiece.UpdatePosition();
    }

    public void Update()
    {
        foreach (UX_Player player in players)
        {
            player.QueryGamepad();
            player.QueryCamera(chunks);
            player.QueryCamera(pieces);
        }
    }
}
