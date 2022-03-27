/* Copyright (C) All Rights Reserved
 * Unauthorized copying of this file, via any medium is prohibited.
 * Proprietary and confidential.
 * Written by Robin Campos <magyk81@gmail.com>, year 2021.
 */

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UX_Player : MonoBehaviour
{
    public enum Mode { PLAIN, WAYPOINT_PIECE, WAYPOINT_TILE, TARGET_PIECE, TARGET_CHUNK, TARGET_TILE, BOARD_SWITCH,
        HAND, DETAIL, SURRENDER, PAUSE }

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
    
    private CameraScript mCam;
    private CanvasScript mCanv;
    private UX_Hand mHand;
    private Transform[] mTileHover;
    private UX_Waypoint[][] mWaypoints;
    private UX_Waypoint[] mPotentialWaypoint;
    private Gamepad mGamepad;
    private int mHoveredWaypoint, mHoveredWaypointMax;
    private UX_Piece mHoveredPiece;
    private UX_Tile mHoveredTile;
    private List<UX_Piece> mSelectedPieces = new List<UX_Piece>();

    private Mode mMode = Mode.PAUSE;
    protected int mPlayerID, mLocalPlayerIdx;

    /// <returns>
    /// <c>True</c> if the mode has changed.
    /// </returns>
    public bool SetMode(Mode mode) {
        if (mMode == mode) return false;
        mCam.Mode = mode;
        mCanv.Mode = mode;
        Debug.Log("SetMode: " + mode);
        mMode = mode;
        return true;
    }

    /// <summary>
    /// Called once before the match begins.
    /// </summary>
    public virtual void Init(int playerID, int localPlayerIdx, float[][] boardBounds, int quadSize) {
        mPlayerID = playerID;
        mLocalPlayerIdx = localPlayerIdx;

        // Setup gamepad.
        mGamepad = new Gamepad(localPlayerIdx == 0);

        Transform tileCollParent = new GameObject().GetComponent<Transform>();
        tileCollParent.parent = GetComponent<Transform>();
        tileCollParent.gameObject.name = "Tile Colliders";

        // Generate tile colliders.
        UX_Collider[,] tileColls = new UX_Collider[quadSize, quadSize];
        for (int i = 0; i < quadSize; i++) {
            for (int j = 0; j < quadSize; j++) {
                tileColls[i, j] = Instantiate( baseColl.gameObject, tileCollParent).GetComponent<UX_Collider>();
                tileColls[i, j].SetTypeTile();
                tileColls[i, j].gameObject.layer = UX_Tile.LAYER + localPlayerIdx;
                tileColls[i, j].gameObject.name = Coord._(i, j).ToString();
            }
        }

        Transform quarterCollParent = new GameObject().GetComponent<Transform>();
        quarterCollParent.parent = GetComponent<Transform>();
        quarterCollParent.gameObject.name = "Quarter Colliders";

        // Generate quarter-chunk colliders.
        UX_Collider[] quarterColls = new UX_Collider[4];
        for (int i = 0; i < 4; i++) {
            quarterColls[i] = Instantiate( baseColl.gameObject, quarterCollParent).GetComponent<UX_Collider>();
            quarterColls[i].Quarter = i + 4;
            quarterColls[i].gameObject.layer = UX_Tile.LAYER + localPlayerIdx;
            quarterColls[i].gameObject.name = Util.DirToString(i + 4);
        }

        Transform tileVisParent = new GameObject().GetComponent<Transform>();
        tileVisParent.parent = GetComponent<Transform>();
        tileVisParent.gameObject.name = "Tile Visuals";

        // Generate visualizations.
        mTileHover = new Transform[9];
        for (int i = 0; i < 9; i++) {
            mTileHover[i] = Instantiate(baseTileHover.gameObject,tileVisParent).GetComponent<Transform>();
            mTileHover[i].gameObject.layer = UX_Tile.LAYER + localPlayerIdx;
            mTileHover[i].gameObject.name = "Tile Hover";
            mTileHover[i].gameObject.SetActive(false);
        }

        Transform waypointsParent = new GameObject().GetComponent<Transform>();
        waypointsParent.parent = GetComponent<Transform>();
        waypointsParent.gameObject.name = "Waypoints";

        // Generate waypoints.
        mWaypoints = new UX_Waypoint[Piece.MAX_WAYPOINTS][];
        for (int i = 0; i < Piece.MAX_WAYPOINTS; i++) {
            mWaypoints[i] = new UX_Waypoint[9];
            for (int j = 0; j < 9; j++) {
                mWaypoints[i][j] = Instantiate(baseWaypoint.gameObject, waypointsParent).GetComponent<UX_Waypoint>();
                if (j == 0) mWaypoints[i][j].gameObject.name = "Waypoint - Real " + i;
                else mWaypoints[i][j].gameObject.name = "Waypoint - Clone " + Util.DirToString(j - 1) + " " + i;
            }
        }

        // Generate potential waypoint.
        mPotentialWaypoint = new UX_Waypoint[9];
        for (int j = 0; j < 9; j++) {
            mPotentialWaypoint[j] = Instantiate(baseWaypoint.gameObject,waypointsParent).GetComponent<UX_Waypoint>();
            if (j == 0) mPotentialWaypoint[j].gameObject.name = "Potential Waypoint - Real";
            else mPotentialWaypoint[j].gameObject.name = "Potential Waypoint - Clone " + Util.DirToString(j - 1);
        }

        // Setup camera.
        mCam = Instantiate(
            baseCam.gameObject,
            GetComponent<Transform>()).GetComponent<CameraScript>();
        mCam.gameObject.name = "Camera";
        mCam.gameObject.SetActive(true);

        // Setup canvas.
        mCanv = Instantiate(
            baseCanv.gameObject,
            GetComponent<Transform>()).GetComponent<CanvasScript>();
        mCanv.gameObject.name = "Canvas";
        mCanv.gameObject.SetActive(true);

        // Setup hand.
        mHand = mCanv.GetComponent<UX_Hand>();

        mCam.Init(localPlayerIdx, mCanv, boardBounds, quarterColls, tileColls);
        SetMode(Mode.PLAIN);
    }

    // Called once every frame.
    protected virtual void Update() {
        if (mMode == Mode.PLAIN || mMode == Mode.BOARD_SWITCH) {
            SetMode(GetTriggerCombo());
        } else if (mMode == Mode.WAYPOINT_TILE || mMode == Mode.WAYPOINT_PIECE) {
            SetMode(GetTriggerCombo());
        }
        if (mMode != Mode.WAYPOINT_TILE && mMode != Mode.TARGET_TILE) {
            UnhoverTile();
            mHoveredTile = null;
        }
    }

    public virtual void QueryCamera() {
        if (mMode == Mode.PLAIN || mMode == Mode.WAYPOINT_PIECE || mMode == Mode.TARGET_PIECE) {
            UX_Piece detectedPiece = mCam.GetDetectedPiece();
            if (detectedPiece != null) {
                if (mHoveredPiece == null || mHoveredPiece.PieceID != detectedPiece.PieceID) {
                    if (mHoveredPiece != null) mHoveredPiece.Unhover(mLocalPlayerIdx);
                    mHoveredPiece = detectedPiece;

                    // Show that piece is hovered.
                    mHoveredPiece.Hover(mLocalPlayerIdx);

                    // UpdateWaypointDisplay();
                }
            } else if (mHoveredPiece != null) {
                // Show that piece is unhovered.
                mHoveredPiece.Unhover(mLocalPlayerIdx);
                mHoveredPiece = null;

                // UpdateWaypointDisplay();
            }
        }

        if (mMode == Mode.TARGET_TILE || mMode == Mode.WAYPOINT_TILE) {
            UX_Tile detectedTile = mCam.GetDetectedTile();
            if (detectedTile != null) {
                if (mHoveredTile == null || mHoveredTile.Pos != detectedTile.Pos) {
                    mHoveredTile = detectedTile;

                    // Show that tile is hovered.
                    Vector3[] tileHoverPos = mHoveredTile.UX_PosAll;
                    for (int i = 0; i < mTileHover.Length; i++) {
                        mTileHover[i].gameObject.SetActive(true);
                        mTileHover[i].localPosition = tileHoverPos[i];
                    }

                    // UpdateWaypointDisplay();
                }
            }
        }

        // UpdateWaypointDisplay();
    }

    private bool holdingTriggerL = false, holdingTriggerR = false;
    public virtual SignalFromClient QueryGamepad()
    {
        SignalFromClient signal = null;
        int[] gamepadInput = mGamepad.PadInput;

        // <D-pad up | Up arrow>
        if (gamepadInput[(int) Gamepad.Button.UP] == 1) {
            if (mMode == Mode.WAYPOINT_PIECE || mMode == Mode.WAYPOINT_TILE) {
                mHoveredWaypoint++;
                // UpdateWaypointDisplay();
            }
        }

        // <D-pad down | Down arrow>
        if (gamepadInput[(int) Gamepad.Button.DOWN] == 1) {
            if (mMode == Mode.BOARD_SWITCH) {
                // Switch board.
                if (mCam.BoardID != 1) mCam.BoardID = 1;
                else mCam.BoardID = 0;
            } else if (mMode == Mode.WAYPOINT_PIECE || mMode == Mode.WAYPOINT_TILE) {
                mHoveredWaypoint--;
                // UpdateWaypointDisplay();
            }
        }

        // <D-pad | Arrows>
        if (mMode == Mode.HAND) {
            int x_move = -1, y_move = -1;
            if (gamepadInput[(int) Gamepad.Button.LEFT] == 1) x_move = Util.LEFT;
            else if (gamepadInput[(int) Gamepad.Button.RIGHT] == 1) x_move = Util.RIGHT;
            if (gamepadInput[(int) Gamepad.Button.UP] == 1) y_move = Util.UP;
            else if (gamepadInput[(int) Gamepad.Button.DOWN] == 1) y_move = Util.DOWN;
            mHand.MoveCursor(x_move, y_move);
        }

        // <Left joystick | WASD> Pan the camera.
        mCam.Move(gamepadInput[(int) Gamepad.Button.L_HORIZ], gamepadInput[(int) Gamepad.Button.L_VERT]);
        
        // <A button | Space>
        if (gamepadInput[(int) Gamepad.Button.A] == 1) {
            // Select the hovered item.
            if (mMode == Mode.PLAIN) {
                if (mHoveredPiece != null) {
                    if (mSelectedPieces.Contains(mHoveredPiece)) {
                        mHoveredPiece.Unselect(mLocalPlayerIdx);
                        mSelectedPieces.Remove(mHoveredPiece);
                    // Can only select this piece if it's under this player's control.
                    } else if (mHoveredPiece.PlayerID == mPlayerID) {
                        mHoveredPiece.Select(mLocalPlayerIdx);
                        mSelectedPieces.Add(mHoveredPiece);
                    }

                    // UpdateWaypointDisplay();
                }
            } else if (mMode == Mode.WAYPOINT_TILE) { // Set waypoint on hovered tile.
                if (mSelectedPieces.Count > 0) {
                    int orderPlace = mHoveredWaypoint;
                    if (mHoveredWaypoint == -1 || mHoveredWaypoint >= Piece.MAX_WAYPOINTS)
                        orderPlace = mHoveredWaypointMax;
                    // int[] selectedPieceIDs = mSelectedPieces.Select(uxPiece => uxPiece.PieceID).ToArray();
                    // if (mSelectedPieces[0].WaypointTiles[orderPlace] != null
                    //     && mSelectedPieces[0].WaypointTiles[orderPlace].Pos == mHoveredTile.Pos) {

                    //     signal = new SignalRemoveWaypoint(
                    //         mPlayerID, mHoveredTile.BoardID, orderPlace, selectedPieceIDs);
                    // } else {
                    //     signal = SignalFromClient.AddWaypoint(
                    //         mPlayerID, mHoveredTile, orderPlace, mSelectedPieces.ToArray());
                    // }
                    // UpdateWaypointDisplay();
                }
            } else if (mMode == Mode.WAYPOINT_PIECE) {
                if (mSelectedPieces.Count > 0) {
                    int orderPlace = mHoveredWaypoint;
                    if (mHoveredWaypoint == -1 || mHoveredWaypoint >= Piece.MAX_WAYPOINTS)
                        orderPlace = mHoveredWaypointMax;
                    // if (mSelectedPieces[0].WaypointPieces[orderPlace] != null
                    //     && mSelectedPieces[0].WaypointPieces[orderPlace].PieceID == mHoveredPiece.PieceID) {
                    //     signal = SignalFromClient.RemoveWaypoint(
                    //         mHoveredPiece.BoardID, orderPlace,
                    //         mSelectedPieces.ToArray());
                    // } else {
                    //     int targetID = mHoveredPiece != null ? mHoveredPiece.PieceID : -1;
                    //     signal = SignalFromClient.AddWaypoint(
                    //         mHoveredPiece.BoardID, targetID, orderPlace,mSelectedPieces.ToArray());
                    // }
                    // UpdateWaypointDisplay();
                }
            } else if (mMode == Mode.TARGET_TILE) { // Play the selected card.
                // signal = SignalFromClient.CastSpell(mPlayCardID, mCasterPieceID, mHoveredTile);
                signal = new SignalCastSpell(mPlayerID, mHand.PlayCardID, mHand.CasterID, mCam.BoardID, mHoveredTile);
                mHand.Hide();
                SetMode(Mode.PLAIN);
            // Select the hovered card.
            } else if (mMode == Mode.HAND) { if (mHand.Select()) SetMode(Mode.TARGET_TILE); }
        }

        // <B button | L>
        if (gamepadInput[(int) Gamepad.Button.B] == 1) {
            // Unselect every piece.
            if (mMode == Mode.PLAIN) foreach (UX_Piece piece in mSelectedPieces) { piece.Unselect(mLocalPlayerIdx); }
        }

        // <X button | J>
        if (gamepadInput[(int) Gamepad.Button.X] == 1) {
            if (mMode == Mode.PLAIN) {
                if (mHoveredPiece != null) {
                    mHand.Show(mHoveredPiece);
                    SetMode(Mode.HAND);
                }
            } else if (mMode == Mode.HAND) {
                mHand.Hide();
                SetMode(Mode.PLAIN);
            }
        }

        // <Left trigger | Left shift>
        if (gamepadInput[(int) Gamepad.Button.L_TRIG] == 1) holdingTriggerL = true;
        else if (gamepadInput[(int) Gamepad.Button.L_TRIG] == -1) holdingTriggerL = false;

        // <Right trigger | Right shift>
        if (gamepadInput[(int) Gamepad.Button.R_TRIG] == 1) holdingTriggerR = true;
        else if (gamepadInput[(int) Gamepad.Button.R_TRIG] == -1) holdingTriggerR = false;
        
        return signal;
    }

    private Mode GetTriggerCombo() {
        if (holdingTriggerL && holdingTriggerR) return Mode.BOARD_SWITCH;
        else if (holdingTriggerL) return Mode.WAYPOINT_TILE;
        else if (holdingTriggerR) return Mode.WAYPOINT_PIECE;
        return Mode.PLAIN;
    }

    private void UnhoverTile() {
        if (mHoveredTile != null) {
            foreach (Transform tile in mTileHover) { tile.gameObject.SetActive(false); }
            mHoveredTile = null;
        }
    }

    private void UpdateWaypointDisplay() {
        if (mMode == Mode.WAYPOINT_PIECE || mMode == Mode.WAYPOINT_TILE) {
            // In WAYPOINT modes, show opaque waypoints if pieces are selected
            // and if those pieces all share identical waypoints.
            // Also show semitrans waypoint where new waypoint can go.
            if (mSelectedPieces.Count > 0) {
                if (AreWaypointsCommon()) ShowWaypoints(1, true);
                else ShowWaypoints(-1, true);
            // Otherwise, show semitrans waypoint on hovered piece only.
            } else if (mHoveredPiece != null) ShowWaypoints(0, false);
            else HideWaypoints();
        }
        else {
            // In PLAIN mode, show semitrans waypoints on hovered piece only.
            if (mHoveredPiece != null) ShowWaypoints(0, false);
            else HideWaypoints();
        }
    }

    // opaque == 1: visible, == 0: semi-transparent, == -1: invisible
    private void ShowWaypoints(int opaque, bool showPotential) {
        UX_Tile[] tiles = null;
        UX_Piece[] pieces = null;
        
        if (opaque == 1) {
            tiles = mSelectedPieces[0].WaypointTiles;
            pieces = mSelectedPieces[0].WaypointPieces;
        } else if (opaque == 0) {
            tiles = mHoveredPiece.WaypointTiles;
            pieces = mHoveredPiece.WaypointPieces;
        }
        int nullIdx = Piece.MAX_WAYPOINTS;

        // Show waypoints that aren't null.
        if (opaque == 0 || opaque == 1) {
            for (int i = 0; i < Piece.MAX_WAYPOINTS; i++) {
                if (tiles[i] == null && pieces[i] == null) {
                    nullIdx = i;
                    break;
                }
                Vector3[] tilePosAll = (tiles[i] != null) ? tiles[i].UX_PosAll : null;
                UX_Piece[] pieceAll = (pieces[i] != null) ? pieces[i].UX_All : null;
                for (int j = 0; j < 9; j++) {
                    mWaypoints[i][j].Show(opaque == 1, i == mHoveredWaypoint);
                    if (pieceAll != null) {
                        if (j == 0) mWaypoints[i][j].Piece = pieceAll[0];
                        else mWaypoints[i][j].Piece = pieceAll[j - 1];
                        mWaypoints[i][j].Piece = pieceAll[j];
                    } else {
                        mWaypoints[i][j].Pos = tilePosAll[j];
                        mWaypoints[i][j].Piece = null;
                    }
                }
            }
        }
        else nullIdx = 0;

        // Hide remaining waypoints.
        for (int i = nullIdx; i < Piece.MAX_WAYPOINTS; i++) {
            for (int j = 0; j < 9; j++) { mWaypoints[i][j].Hide(); }
        }

        if (showPotential) {
            // Update hovered waypoint index max to equal the number of shown
            // waypoints, but do not let it exceed the max waypoint index.
            mHoveredWaypointMax = Mathf.Min(Piece.MAX_WAYPOINTS - 1, nullIdx);

            // Update hovered waypoint index.
            if (mHoveredWaypoint < 0) mHoveredWaypoint = mHoveredWaypointMax;
            else if (mHoveredWaypoint > mHoveredWaypointMax) mHoveredWaypoint = 0;

            // Show potential waypoint.
            if (mHoveredTile != null) {
                Vector3[] tilePosAll_ = mHoveredTile.UX_PosAll;
                for (int j = 0; j < 9; j++) {
                    mPotentialWaypoint[j].Show(false, mHoveredWaypoint < nullIdx);
                    mPotentialWaypoint[j].Pos = tilePosAll_[j];
                    mPotentialWaypoint[j].Piece = null;
                }
            } else if (mHoveredPiece != null) {
                UX_Piece[] pieceAll_ = mHoveredPiece.UX_All;
                for (int j = 0; j < 9; j++) {
                    mPotentialWaypoint[j].Show(false, mHoveredWaypoint < nullIdx);
                    if (j == 0) mPotentialWaypoint[j].Piece = mHoveredPiece;
                    else mPotentialWaypoint[j].Piece = pieceAll_[j - 1];
                }
            } else { for (int j = 0; j < 9; j++) { mPotentialWaypoint[j].Hide(); } }
        }
        else { for (int j = 0; j < 9; j++) { mPotentialWaypoint[j].Hide(); } }
    }

    private void HideWaypoints() {
        for (int i = 0; i < Piece.MAX_WAYPOINTS; i++) { for (int j = 0; j < 9; j++) { mWaypoints[i][j].Hide(); } }
        for (int j = 0; j < 9; j++) { mPotentialWaypoint[j].Hide(); }
        mHoveredWaypoint = -1;
    }

    public void ResetPotentialWaypoint() { mHoveredWaypoint = -1; }

    /// <summary>Called whenever the number of waypoints changes or the number
    /// of pieces changes. Sets waypointsAreCommon to True if all selected
    /// pieces share idential waypoints, otherwise it is set to False.
    /// </summary>
    private bool AreWaypointsCommon() {
        bool waypointsAreCommon = true;

        if (mSelectedPieces.Count >= 2) {
            for (int i = 1; i < mSelectedPieces.Count; i++) {
                if (!mSelectedPieces[i].HasSameWaypoints(mSelectedPieces[0])) {
                    waypointsAreCommon = false;
                    break;
                }
            }
        }
        return waypointsAreCommon;
    }   
}