using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UX_Player : MonoBehaviour
{
    [SerializeField]
    CameraScript baseCam;
    [SerializeField]
    CanvasScript baseCanv;
    [SerializeField]
    UX_Collider baseColl;
    [SerializeField]
    Transform baseTileHover;
    [SerializeField]
    UX_Waypoint baseWaypoint;
    private CameraScript cam;
    private CanvasScript canv;
    private Transform[] tileHover;
    private UX_Waypoint[][] waypoints;
    private Gamepad gamepad;
    private UX_Piece hoveredPiece;
    private UX_Tile hoveredTile;
    private List<UX_Piece> selectedPieces = new List<UX_Piece>();
    public enum Mode { PLAIN, WAYPOINT_PIECE, WAYPOINT_TILE, TARGET_PIECE,
        TARGET_CHUNK, TARGET_TILE, HAND, DETAIL, SURRENDER, PAUSE }
    private Mode mode = Mode.PAUSE;
    private int localPlayerIdx;
    public void SetMode(Mode mode)
    {
        if (mode == this.mode) return;
        cam.SetMode(mode);
        canv.SetMode(mode);
        Debug.Log("SetMode: " + mode);
        this.mode = mode;
    }

    public void Init(int localPlayerIdx, float[][] boardBounds)
    {
        this.localPlayerIdx = localPlayerIdx;

        // Setup gamepad.
        gamepad = new Gamepad(localPlayerIdx == 0);

        Transform tileCollParent = new GameObject().GetComponent<Transform>();
        tileCollParent.parent = GetComponent<Transform>();
        tileCollParent.gameObject.name = "Tile Colliders";

        // Generate tile colliders.
        UX_Collider[,] tileColls = new UX_Collider[
            Chunk.Size / 2, Chunk.Size / 2];
        for (int i = 0; i < Chunk.Size / 2; i++)
        {
            for (int j = 0; j < Chunk.Size/ 2; j++)
            {
                tileColls[i, j] = Instantiate(
                    baseColl.gameObject,
                    tileCollParent).GetComponent<UX_Collider>();
                tileColls[i, j].SetTypeTile();
                tileColls[i, j].gameObject.layer
                    = UX_Tile.LAYER + localPlayerIdx;
                tileColls[i, j].gameObject.name = Coord._(i, j).ToString();
            }
        }

        Transform quarterCollParent
            = new GameObject().GetComponent<Transform>();
        quarterCollParent.parent = GetComponent<Transform>();
        quarterCollParent.gameObject.name = "Quarter Colliders";

        // Generate quarter-chunk colliders.
        UX_Collider[] quarterColls = new UX_Collider[4];
        for (int i = 0; i < 4; i++)
        {
            quarterColls[i] = Instantiate(
                baseColl.gameObject,
                quarterCollParent).GetComponent<UX_Collider>();
            quarterColls[i].Quarter = i + 4;
            quarterColls[i].gameObject.layer
                = UX_Tile.LAYER + localPlayerIdx;
            quarterColls[i].gameObject.name = Util.DirToString(i + 4);
        }

        Transform tileVisParent
            = new GameObject().GetComponent<Transform>();
        tileVisParent.parent = GetComponent<Transform>();
        tileVisParent.gameObject.name = "Tile Visuals";

        // Generate visualizations.
        tileHover = new Transform[9];
        for (int i = 0; i < 9; i++)
        {
            tileHover[i] = Instantiate(baseTileHover.gameObject,
                tileVisParent).GetComponent<Transform>();
            tileHover[i].gameObject.layer
                    = UX_Tile.LAYER + localPlayerIdx;
            tileHover[i].gameObject.name = "Tile Hover";
            tileHover[i].gameObject.SetActive(false);
        }

        Transform waypointsParent = new GameObject().GetComponent<Transform>();
        waypointsParent.parent = GetComponent<Transform>();
        waypointsParent.gameObject.name = "Waypoints";

        // Generate waypoints.
        waypoints = new UX_Waypoint[Piece.MAX_WAYPOINTS][];
        for (int i = 0; i < Piece.MAX_WAYPOINTS; i++)
        {
            waypoints[i] = new UX_Waypoint[9];
            for (int j = 0; j < 9; j++)
            {
                waypoints[i][j] = Instantiate(baseWaypoint.gameObject,
                    waypointsParent).GetComponent<UX_Waypoint>();
                if (j == 0)
                    waypoints[i][j].gameObject.name = "Waypoint - Real " + i;
                else
                {
                    waypoints[i][j].gameObject.name = "Waypoint - Clone "
                        + Util.DirToString(j - 1) + " " + i;
                }
                waypoints[i][j].gameObject.SetActive(false);
            }
        }

        // Setup camera.
        cam = Instantiate(
            baseCam.gameObject,
            GetComponent<Transform>()).GetComponent<CameraScript>();
        cam.gameObject.name = "Camera";
        cam.gameObject.SetActive(true);

        // Setup canvas.
        canv = Instantiate(
            baseCanv.gameObject,
            GetComponent<Transform>()).GetComponent<CanvasScript>();
        canv.gameObject.name = "Canvas";
        canv.gameObject.SetActive(true);

        cam.Init(localPlayerIdx, canv, boardBounds, quarterColls, tileColls);
        SetMode(Mode.PLAIN);
    }

    private void Update()
    {
        if (mode == Mode.PLAIN)
        {
            if (holdingTriggerL && !holdingTriggerR)
                SetMode(Mode.WAYPOINT_TILE);
            else if (!holdingTriggerL && holdingTriggerR)
                SetMode(Mode.WAYPOINT_PIECE);
        }
        else if (mode == Mode.WAYPOINT_TILE)
        {
            if (!holdingTriggerL && holdingTriggerR)
            {
                UnhoverTile();
                SetMode(Mode.WAYPOINT_PIECE);
            }
            else if (!holdingTriggerL && !holdingTriggerR)
            {
                UnhoverTile();
                SetMode(Mode.PLAIN);
            }
        }
        else if (mode == Mode.WAYPOINT_PIECE)
        {
            if (holdingTriggerL && !holdingTriggerR)
                SetMode(Mode.WAYPOINT_TILE);
            else if (!holdingTriggerL && !holdingTriggerR)
                SetMode(Mode.PLAIN);
        }
    }

    public void QueryCamera()
    {
        if (mode == Mode.PLAIN || mode == Mode.WAYPOINT_PIECE
            || mode == Mode.TARGET_PIECE)
        {
            UX_Piece detectedPiece = cam.GetDetectedPiece();
            if (detectedPiece != null)
            {
                hoveredPiece = detectedPiece;

                // Show that piece is hovered.
                hoveredPiece.Hover(localPlayerIdx);
            }
            else if (hoveredPiece != null)
            {
                // Show that piece is unhovered.
                hoveredPiece.Unhover(localPlayerIdx);
                hoveredPiece = null;
            }
        }
        if (mode == Mode.TARGET_TILE || mode == Mode.WAYPOINT_TILE)
        {
            UX_Tile detectedTile = cam.GetDetectedTile();
            if (detectedTile != null)
            {
                if (hoveredTile == null || hoveredTile.Pos != detectedTile.Pos)
                {
                    hoveredTile = detectedTile;

                    // Show that tile is hovered.
                    Vector3[] tileHoverPos = hoveredTile.UX_PosAll;
                    for (int i = 0; i < tileHover.Length; i++)
                    {
                        tileHover[i].gameObject.SetActive(true);
                        tileHover[i].localPosition = tileHoverPos[i];
                    }
                    
                    Debug.Log(hoveredTile.Pos);
                }
            }
            // Show that tile is unhovered.
            else UnhoverTile();

            // Show waypoints.
            if (selectedPieces.Count == 0)
            {
                if (hoveredPiece != null)
                {
                    UX_Tile[] tiles = hoveredPiece.Waypoints;
                    int nullTileIdx = tiles.Length;
                    for (int i = 0; i < tiles.Length; i++)
                    {
                        if (tiles[i] == null)
                        {
                            nullTileIdx = i;
                            break;
                        }
                        Vector3[] tilePosAll = tiles[i].UX_PosAll;
                        for (int j = 0; j < 9; j++)
                        {
                            waypoints[i][j].gameObject.SetActive(true);
                            waypoints[i][j].GetComponent<Transform>()
                                .localPosition = tilePosAll[j];
                        }
                    }

                    // Hide remaining waypoints.
                    for (int i = nullTileIdx; i < tiles.Length; i++)
                    {
                        for (int j = 0; j < 9; j++)
                        {
                            waypoints[i][j].gameObject.SetActive(false);
                        }
                    }
                }
                // Hide all waypoints.
                else
                {
                    for (int i = 0; i < Piece.MAX_WAYPOINTS; i++)
                    {
                        for (int j = 0; j < 9; j++)
                        {
                            waypoints[i][j].gameObject.SetActive(true);
                        }
                    }
                }
            }
            else
            {

            }
        }
    }

    private bool holdingTriggerL = false, holdingTriggerR = false;
    public void QueryGamepad()
    {
        int[] gamepadInput = gamepad.PadInput;

        // <D-pad down | Down arrow>
        if (gamepadInput[(int) Gamepad.Button.DOWN] == 1)
        {
            // Switch board.
            if (holdingTriggerL && holdingTriggerR)
            {
                if (cam.BoardIdx != 1) cam.BoardIdx = 1;
                else cam.BoardIdx = 0;
            }
        }

        // <Left joystick | WASD> Pan the camera.
        cam.Move(
            gamepadInput[(int) Gamepad.Button.L_HORIZ],
            gamepadInput[(int) Gamepad.Button.L_VERT]);
        
        // <A button | Space>
        if (gamepadInput[(int) Gamepad.Button.A] == 1)
        {
            // Select the hovered item.
            if (mode == Mode.PLAIN)
            {
                if (hoveredPiece != null)
                {
                    if (selectedPieces.Contains(hoveredPiece))
                    {
                        hoveredPiece.Unselect(localPlayerIdx);
                        selectedPieces.Remove(hoveredPiece);
                    }
                    else
                    {
                        hoveredPiece.Select(localPlayerIdx);
                        selectedPieces.Add(hoveredPiece);
                    }
                }
            }
            // Set waypoint on hovered tile.
            else if (mode == Mode.WAYPOINT_TILE)
            {
                if (selectedPieces.Count > 0)
                {
                    foreach (UX_Piece piece in selectedPieces)
                    {
                        UX_Match.AddSkinTicket(new SkinTicket(
                            piece.Piece, hoveredTile.Pos,
                            SkinTicket.Type.ADD_WAYPOINT));
                    }
                }
            }
        }

        // <Left trigger | Left shift>
        if (gamepadInput[(int) Gamepad.Button.L_TRIG] == 1)
            holdingTriggerL = true;
        else if (gamepadInput[(int) Gamepad.Button.L_TRIG] == -1)
            holdingTriggerL = false;

        // <Right trigger | Right shift>
        if (gamepadInput[(int) Gamepad.Button.R_TRIG] == 1)
            holdingTriggerR = true;
        else if (gamepadInput[(int) Gamepad.Button.R_TRIG] == -1)
            holdingTriggerR = false;
    }

    private void UnhoverTile()
    {
        if (hoveredTile != null)
        {
            foreach (Transform tile in tileHover)
            {
                tile.gameObject.SetActive(false);
            }
            hoveredTile = null;
        }
    }

    // public void QueryGamepad()
    // {
    //     int[] padInput = GAMEPAD.PadInput;

    //     Mode oldMode = mode;

    //     if (mode == Mode.PLAIN || mode == Mode.TARGET_CHUNK
    //         || mode == Mode.TARGET_PIECE || mode == Mode.TARGET_TILE)
    //     {
    //         // Move camera
    //         int x_move = 0, z_move = 0,
    //             l_horiz = padInput[(int) Gamepad.Button.L_HORIZ],
    //             l_vert = padInput[(int) Gamepad.Button.L_VERT];
    //         if (l_horiz != 0) x_move = l_horiz;
    //         if (l_vert != 0) z_move = l_vert;
    //         CAM.Move(x_move, z_move);
            
    //         if (mode == Mode.PLAIN)
    //         {
    //             if (hoveredPiece != null)
    //             {
    //                 // Select piece if hovering a piece
    //                 bool a_button = padInput[(int) Gamepad.Button.A] > 0;
    //                 if (a_button) SelectPiece(hoveredPiece);
    //             }
                    
    //             if (hoveredPiece != null && __.Idx == hoveredPiece._.PlayerIdx)
    //             {
    //                 CAM.SetHandCards(hoveredPiece._);

    //                 // Go to HAND mode if hovering a master
    //                 bool x_button = padInput[(int) Gamepad.Button.X] > 0;
    //                 if (x_button)
    //                 {
    //                     mode = Mode.HAND;
    //                     pieceBeingPlayedFrom = hoveredPiece._;
    //                 }
    //             }
    //         }
    //     }

    //     if (mode == Mode.HAND)
    //     {
    //         // Go to PLAIN mode
    //         bool b_button = padInput[(int) Gamepad.Button.B] > 0;
    //         if (b_button) mode = Mode.PLAIN;

    //         // Play the hovered card
    //         bool a_button = padInput[(int) Gamepad.Button.A] > 0;
    //         if (a_button)
    //         {
    //             cardBeingPlayed
    //                 = CAM.GetHandPiece().GetCardFromHand(CAM.GetHandCard());
    //             if (cardBeingPlayed != null)
    //             {
    //                 CAM.DisplayPlayCard(CAM.GetHandCard());
    //                 mode = cardBeingPlayed.GetPlayMode();
    //             }
    //         }

    //         int x_move = -1, y_move = -1;
    //         if (padInput[(int) Gamepad.Button.LEFT] == 1) x_move = Util.LEFT;
    //         else if (padInput[(int) Gamepad.Button.RIGHT] == 1)
    //             x_move = Util.RIGHT;
    //         if (padInput[(int) Gamepad.Button.DOWN] == 1) y_move = Util.DOWN;
    //         else if (padInput[(int) Gamepad.Button.UP] == 1) y_move = Util.UP;
    //         CAM.HandMove(x_move, y_move);
    //     }

    //     if (oldMode != mode)
    //     {
    //         CAM.SetMode(mode);
    //         Debug.Log("Set Mode: " + mode);
    //     }
    // }


   
}
