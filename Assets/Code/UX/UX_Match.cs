using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UX_Match : MonoBehaviour
{
    [SerializeField]
    private UX_Player basePlayer;
    [SerializeField]
    private UX_Board baseBoard;

    private UX_Player[] players;
    public UX_Player[] Players { get { return players; } }
    public static int localPlayerCount;
    private UX_Board[][] boards;
    public UX_Board[][] Boards { get { return boards; } }

    private static List<SkinTicket> skinTickets = new List<SkinTicket>();
    public static void AddSkinTicket(SkinTicket ticket)
    {
        skinTickets.Add(ticket);
    }
    public SkinTicket[] SkinTickets {
        get {
            SkinTicket[] arr = skinTickets.ToArray();
            skinTickets.Clear();
            return arr;
        }
        set {
            foreach (SkinTicket ticket in value)
            {
                if (ticket.TicketType == SkinTicket.Type.ADD_PIECE)
                {
                    foreach (UX_Board board in boards[ticket.Piece.BoardIdx])
                    {
                        board.AddPiece(ticket.Piece);
                    }
                }
                else if (ticket.TicketType == SkinTicket.Type.ADD_WAYPOINT)
                {
                    foreach (UX_Board board in boards[ticket.Piece.BoardIdx])
                    {
                        board.AddWaypoint(ticket.Piece, ticket.Coord);
                    }
                    foreach (UX_Player player in players)
                    {
                        player.CalcIfWaypointsCommon();
                    }
                }
            }
        }
    }

    public void Init(Player[] players, Board[] boards)
    {
        // Prep uxBoard bounds.
        float[][] boardBounds = new float[boards.Length][];

        // Calculate number of local players.
        int localPlayerCount = 0;
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].PlayerType == Player.Type.LOCAL_PLAYER)
                localPlayerCount++;
        }
        UX_Match.localPlayerCount = localPlayerCount;

        // Generate uxBoards.
        Transform boardGroup = new GameObject().GetComponent<Transform>();
        boardGroup.gameObject.name = "Boards";
        this.boards = new UX_Board[boards.Length][];
        for (int i = 0; i < boards.Length; i++)
        {
            // 1 real uxBoard + 8 clone uxBoards
            this.boards[i] = new UX_Board[9];
            Transform boardParent = new GameObject().GetComponent<Transform>();
            boardParent.parent = boardGroup;
            boardParent.gameObject.name = boards[i].Name;
            boardParent.gameObject.SetActive(true);

            for (int j = 0; j < this.boards[i].Length; j++)
            {
                this.boards[i][j] = Instantiate(
                    baseBoard.gameObject,
                    boardParent).GetComponent<UX_Board>();
                if (j == 0)
                {
                    this.boards[i][j].gameObject.name = "Board - Real";
                    this.boards[i][j].Init(boards[i].GetSize(), i);
                    boardBounds[i] = this.boards[i][j].GetBounds();
                }
                else
                {
                    this.boards[i][j].gameObject.name = "Board - Clone "
                        + Util.DirToString(j - 1);
                    this.boards[i][j].Init(
                        boards[i].GetSize(), i, j - 1, this.boards[i][0]);
                }
            }
        }

        // Generate uxPlayers
        this.players = new UX_Player[localPlayerCount];
        Transform playerGroup = new GameObject().GetComponent<Transform>();
        playerGroup.gameObject.name = "Players";
        for (int i = 0, j = 0; i < players.Length; i++)
        {
            if (players[i].PlayerType == Player.Type.LOCAL_PLAYER)
            {
                this.players[j] = Instantiate(
                    basePlayer.gameObject,
                    playerGroup
                ).GetComponent<UX_Player>();
                this.players[j].gameObject.name = players[i].Name;
                this.players[j].gameObject.SetActive(true);
                this.players[j].Init(j, boardBounds);

                j++;
            }
        }

        // Generate waypoints
        // GameObject[,] waypoints
        //     = new GameObject[players.Length, MAX_WAYPOINTS];
        // GameObject waypointGroupObj = new GameObject("Waypoints");
        // Transform waypointGroupTra = waypointGroupObj
        //     .GetComponent<Transform>();
        // for (int i = 0; i < players.Length; i++)
        // {
        //     GameObject waypointGroupPlayerObj = new GameObject(
        //         "Waypoints - Player " + i);
        //     Transform waypointGroupPlayerTra = waypointGroupPlayerObj
        //         .GetComponent<Transform>();
        //     waypointGroupPlayerTra.SetParent(waypointGroupTra);
        //     for (int j = 0; j < MAX_WAYPOINTS; j++)
        //     {
        //         waypoints[i, j] = GameObject.Instantiate(
        //             waypointObj, waypointGroupPlayerTra);
        //         waypoints[i, j].name = "Waypoint " + (j + 1);
        //         waypoints[i, j].SetActive(false);
        //     }
        // }

        // Set group for cameras, and canvases
        // Transform camGroupTra
        //     = new GameObject("Cameras").GetComponent<Transform>();
        // Transform canvasGroupTra
        //     = new GameObject("Canvases").GetComponent<Transform>();

        // Generate UX_Players with their gamepads and cameras
        // int localPlayerCount = 0;
        // foreach (Player player in players)
        // {
        //     if (player.PlayerType == Player.Type.LOCAL_PLAYER)
        //         localPlayerCount++;
        // }
        // this.players = new UX_Player[localPlayerCount];
        // for (int i = 0, j = 0; i < players.Length; i++)
        // {
        //     if (players[i].PlayerType == Player.Type.LOCAL_PLAYER)
        //     {
        //         Gamepad gamepad = new Gamepad(j == 0);

        //         GameObject camClone = GameObject.Instantiate(
        //             baseCam.gameObject, camGroupTra);
        //         camClone.name = "Camera - Player " + (i + 1);
        //         camClone.GetComponent<AudioListener>().enabled = (j == 0);
        //         camClone.GetComponent<CameraScript>().Bounds = bounds;

        //         GameObject canvasClone = GameObject.Instantiate(
        //             baseCanvas.gameObject, canvasGroupTra);
        //         canvasClone.name = "Canvas - Player " + (i + 1);
        //         canvasClone.GetComponent<Canvas>().worldCamera
        //             = camClone.GetComponent<Camera>();
                
        //         camClone.GetComponent<CameraScript>().InitCamObjs(
        //             canvasClone.GetComponent<CanvasScript>());

        //         this.players[j] = new UX_Player(players[i], gamepad,
        //             camClone.GetComponent<CameraScript>());
        //         j++;
        //     } 
        // }
        
        // baseChunk.gameObject.SetActive(false);
        // basePiece.gameObject.SetActive(false);
        // waypointObj.SetActive(false);
        // baseCam.gameObject.SetActive(false);
        // baseCanvas.gameObject.SetActive(false);
    }

    // public void AddPiece(Piece piece)
    // {
    //     UX_Piece newUxPiece = GameObject.Instantiate(basePiece, boardTra);
    //     pieces.Add(newUxPiece);
    //     newUxPiece.Init(piece, fullSizes[piece.BoardIdx], DIST_BETWEEN_BOARDS);
    //     newUxPiece.UpdatePosition();
    // }

    public void Update()
    {
        foreach (UX_Player player in players)
        {
            player.QueryCamera();
            player.QueryGamepad();
        }
    }
}
