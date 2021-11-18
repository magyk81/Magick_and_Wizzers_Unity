/* Copyright (C) All Rights Reserved
 * Unauthorized copying of this file, via any medium is prohibited.
 * Proprietary and confidential.
 * Written by Robin Campos <magyk81@gmail.com>, year 2021.
 */

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
    private UX_Hand hand;
    private Card playCard;
    private Transform[] tileHover;
    private UX_Waypoint[][] waypoints;
    private Gamepad gamepad;
    private UX_Piece hoveredPiece;
    private UX_Tile hoveredTile;
    private List<UX_Piece> selectedPieces = new List<UX_Piece>();
    public enum Mode { PLAIN, WAYPOINT_PIECE, WAYPOINT_TILE, TARGET_PIECE,
        TARGET_CHUNK, TARGET_TILE, BOARD_SWITCH, HAND, DETAIL, SURRENDER,
        PAUSE }
    private Mode mode = Mode.PAUSE;
    private int playerIdx, localPlayerIdx;

    private bool waypointsAreCommon = false;

    // Returns true if mode has changed.
    public bool SetMode(Mode mode)
    {
        if (mode == this.mode) return false;
        cam.SetMode(mode);
        canv.SetMode(mode);
        Debug.Log("SetMode: " + mode);
        this.mode = mode;
        return true;
    }

    /// <summary>Called once before the match begins.</summary>
    public void Init(int playerIdx, int localPlayerIdx, float[][] boardBounds,
        int quadSize)
    {
        this.playerIdx = playerIdx;
        this.localPlayerIdx = localPlayerIdx;

        // Setup gamepad.
        gamepad = new Gamepad(localPlayerIdx == 0);

        Transform tileCollParent = new GameObject().GetComponent<Transform>();
        tileCollParent.parent = GetComponent<Transform>();
        tileCollParent.gameObject.name = "Tile Colliders";

        // Generate tile colliders.
        UX_Collider[,] tileColls = new UX_Collider[
            quadSize, quadSize];
        for (int i = 0; i < quadSize; i++)
        {
            for (int j = 0; j < quadSize; j++)
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

        // Setup hand.
        hand = canv.GetComponent<UX_Hand>();

        cam.Init(localPlayerIdx, canv, boardBounds, quarterColls, tileColls);
        SetMode(Mode.PLAIN);
    }

    // Called once every frame.
    private void Update()
    {
        if (mode == Mode.PLAIN || mode == Mode.BOARD_SWITCH)
        {
            SetMode(GetTriggerCombo());
        }
        else if (mode == Mode.WAYPOINT_TILE || mode == Mode.WAYPOINT_PIECE)
        {
            if (SetMode(GetTriggerCombo())) UnhoverTile();
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
                }
            }
            // Show that tile is unhovered.
            else UnhoverTile();
        }

        UpdateWaypointDisplay(mode == Mode.WAYPOINT_PIECE
            || mode == Mode.WAYPOINT_TILE);
    }

    private bool holdingTriggerL = false, holdingTriggerR = false;
    public void QueryGamepad()
    {
        int[] gamepadInput = gamepad.PadInput;

        // <D-pad down | Down arrow>
        if (gamepadInput[(int) Gamepad.Button.DOWN] == 1)
        {
            if (mode == Mode.BOARD_SWITCH)
            {
                // Switch board.
                if (cam.BoardIdx != 1) cam.BoardIdx = 1;
                else cam.BoardIdx = 0;
            }
        }

        // <D-pad | Arrows>
        if (mode == Mode.HAND)
        {
            int x_move = -1, y_move = -1;
            if (gamepadInput[(int) Gamepad.Button.LEFT] == 1)
                x_move = Util.LEFT;
            else if (gamepadInput[(int) Gamepad.Button.RIGHT] == 1)
                x_move = Util.RIGHT;
            if (gamepadInput[(int) Gamepad.Button.UP] == 1)
                y_move = Util.UP;
            else if (gamepadInput[(int) Gamepad.Button.DOWN] == 1)
                y_move = Util.DOWN;
            hand.MoveCursor(x_move, y_move);
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
                    // Can only select this piece if it's under this player's
                    // control.
                    else if (hoveredPiece.Piece.PlayerIdx == playerIdx)
                    {
                        hoveredPiece.Select(localPlayerIdx);
                        selectedPieces.Add(hoveredPiece);
                    }
                    CalcIfWaypointsCommon();
                }
            }
            // Set waypoint on hovered tile.
            else if (mode == Mode.WAYPOINT_TILE)
            {
                if (selectedPieces.Count > 0)
                {
                    foreach (UX_Piece piece in selectedPieces)
                    {
                        // UX_Match.AddSkinTicket(new Signal(
                        //     piece.Piece, hoveredTile.Pos,
                        //     Signal.Type.ADD_WAYPOINT));
                    }
                }
            }
            // Play the selected card.
            else if (mode == Mode.TARGET_TILE)
            {
                // UX_Match.AddSkinTicket(new Signal(
                //     playerIdx, cam.BoardIdx, hoveredPiece.Piece, playCard,
                //     hoveredTile.Pos, Signal.Type.ADD_PIECE));
                playCard = null;
                hand.Hide();
                UnhoverTile();
                SetMode(Mode.PLAIN);
            }
            // Select the hovered card.
            else if (mode == Mode.HAND)
            {
                playCard = hand.Select();
                hand.Hide(false);
                SetMode(Mode.TARGET_TILE);
            }
        }

        // <B button | L>
        if (gamepadInput[(int) Gamepad.Button.B] == 1)
        {
            // Unselect every piece.
            if (mode == Mode.PLAIN)
            {
                foreach (UX_Piece piece in selectedPieces)
                {
                    piece.Unselect(localPlayerIdx);
                }
            }
        }

        // <X button | J>
        if (gamepadInput[(int) Gamepad.Button.X] == 1)
        {
            if (mode == Mode.PLAIN)
            {
                if (hoveredPiece != null)
                {
                    hand.Show(hoveredPiece.Piece);
                    SetMode(Mode.HAND);
                }
            }
            else if (mode == Mode.HAND)
            {
                hand.Hide();
                SetMode(Mode.PLAIN);
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

    private Mode GetTriggerCombo()
    {
        if (holdingTriggerL && holdingTriggerR) return Mode.BOARD_SWITCH;
        else if (holdingTriggerL) return Mode.WAYPOINT_TILE;
        else if (holdingTriggerR) return Mode.WAYPOINT_PIECE;
        return Mode.PLAIN;
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

    private void UpdateWaypointDisplay(bool waypointMode)
    {
        if (waypointMode)
        {
            // In WAYPOINT modes, show opaque waypoints if pieces are selected
            // and if those pieces all share identical waypoints.
            if (selectedPieces.Count > 0)
            {
                if (waypointsAreCommon) ShowWaypoints(true);
            }
            // Otherwise, show semitrans waypoint on hovered piece only.
            else if (hoveredPiece != null) ShowWaypoints(false);
            else HideWaypoints();
        }
        else
        {
            // In PLAIN mode, show semitrans waypoints on hovered piece only.
            if (hoveredPiece != null) ShowWaypoints(false);
            else HideWaypoints();
        }
    }

    private void ShowWaypoints(bool opaque)
    {
        UX_Tile[] tiles;
        if (opaque) tiles = selectedPieces[0].Waypoints;
        else tiles = hoveredPiece.Waypoints;
        int nullTileIdx = tiles.Length;

        // Show waypoints that aren't null.
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
                waypoints[i][j].Show(opaque);
                waypoints[i][j].SetPos(tilePosAll[j]);
            }
        }

        // Hide remaining waypoints.
        for (int i = nullTileIdx; i < tiles.Length; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                waypoints[i][j].Hide();
            }
        }
    }

    private void HideWaypoints()
    {
        for (int i = 0; i < Piece.MAX_WAYPOINTS; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                waypoints[i][j].Hide();
            }
        }
    }

    /// <summary>Called whenever the number of waypoints changes or the number
    /// of pieces changes. Sets waypointsAreCommon to True if all selected
    /// pieces share idential waypoints, otherwise it is set to False.
    /// </summary>
    public void CalcIfWaypointsCommon()
    {
        waypointsAreCommon = true;

        if (selectedPieces.Count >= 2)
        {
            for (int i = 1; i < selectedPieces.Count; i++)
            {
                if (!selectedPieces[i].Piece.HasSameWaypoints(
                    selectedPieces[0].Piece))
                {
                    waypointsAreCommon = false;
                    break;
                }
            }
        }
    }   
}
