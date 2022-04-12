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
    protected int mPlayerID, mLocalPlayerIdx;

    [SerializeField]
    CameraScript baseCam;
    [SerializeField]
    CanvasScript baseCanv;
    [SerializeField]
    Transform baseTileHover;
    [SerializeField]
    UX_Waypoint baseWaypointForTiles, baseWaypointForPieces;

    private enum Mode { SELECT_PIECE_PLAIN, SELECT_PIECE_GROUP, SELECT_PIECE_SPELLCAST, SELECT_PIECE_WAYPOINT,
        SELECT_TILE_SPELLCAST, SELECT_TILE_WAYPOINT, SELECT_CHUNK_SPELLCAST, SELECT_BOARD, HAND, DETAIL, SURRENDER,
        PAUSE
    }
    
    private CameraScript mCam;
    private CanvasScript mCanv;
    private UX_Hand mHand;
    private Transform[] mTileHover;
    // Need multiple of each (Piece.MAX_WAYPOINTS), x 9 clones
    private UX_Waypoint[][] mWaypointsForTiles, mWaypointsForPieces;
    // Only need one of each, 9 clones.
    private UX_Waypoint[] mWaypointHoveringForTiles, mWaypointHoveringForPieces;
    private Gamepad mGamepad;
    private int mHoveredWaypointIdx = 0;
    private UX_Tile mHoveredTile;
    private List<UX_Piece> mSelectedPieces = new List<UX_Piece>();
    // Pieces hovered by group selection before the user depresses the Select button.
    private List<UX_Piece> mHoveredPieces = new List<UX_Piece>();
    private Coord[] mWaypointTargetsForGroup = new Coord[Piece.MAX_WAYPOINTS];
    private bool mShowingWaypoints = false;

    private Mode mMode = Mode.SELECT_PIECE_PLAIN;
    private Mode mModePrev = Mode.SELECT_PIECE_PLAIN;

    public int BoardID { get => mCam.BoardID; }
    public int HoveredWaypointIdx {
        get => mHoveredWaypointIdx;
        set {
            if (mSelectedPieces.Count == 1) {
                mSelectedPieces[0].WaypointIdx = value; Debug.Log(value);
                mHoveredWaypointIdx = mSelectedPieces[0].WaypointIdx;
            } else {
                if (value >= Piece.MAX_WAYPOINTS) mHoveredWaypointIdx = Piece.MAX_WAYPOINTS - 1;
                else if (value < 0) mHoveredWaypointIdx = 0;
                else mHoveredWaypointIdx = value;
            }
        }
    }
    public UX_Piece HoveredPiece {
        set {
            if (mMode != Mode.SELECT_PIECE_GROUP) {
                if (mHoveredPieces.Count == 0) HoverPiece(value);
                else if (mHoveredPieces.Count == 1) {
                    if (value == null || mHoveredPieces[0].PieceID != value.PieceID) {
                        mHoveredPieces[0].Unhover(mLocalPlayerIdx);
                        mHoveredPieces.Clear();
                        HoverPiece(value);
                    }
                } else {
                    foreach (UX_Piece piece in mHoveredPieces) { piece.Unhover(mLocalPlayerIdx); }
                    mHoveredPieces.Clear();
                    HoverPiece(value);
                }
            }
        }
    }
    public UX_Tile HoveredTile {
        set {
            if (value != null && (mHoveredTile == null || mHoveredTile.Pos != value.Pos)) {
                // Hovered tile should never be null, except when assigned the first time.
                mHoveredTile = value;

                // Show that tile is hovered.
                Vector3[] tileHoverPos = mHoveredTile.UX_PosAll;
                for (int i = 0; i < mTileHover.Length; i++) { mTileHover[i].localPosition = tileHoverPos[i]; }
            }
        }
    }

    /// <summary>
    /// Called once before the match begins.
    /// </summary>
    public virtual void Init(int playerID, int localPlayerIdx, float[][] boardBounds, Coord[][] boardOffsets,
        int quadSize) {

        mPlayerID = playerID;
        mLocalPlayerIdx = localPlayerIdx;

        // Setup gamepad.
        mGamepad = new Gamepad(localPlayerIdx == 0);

        Transform tileVisParent = new GameObject().GetComponent<Transform>();
        tileVisParent.parent = GetComponent<Transform>();
        tileVisParent.gameObject.name = "Tile Visuals";

        // Generate visualizations.
        mTileHover = new Transform[9];
        for (int i = 0; i < 9; i++) {
            mTileHover[i] = Instantiate(baseTileHover.gameObject, tileVisParent).GetComponent<Transform>();
            mTileHover[i].gameObject.layer = UX_Tile.LAYER + localPlayerIdx;
            mTileHover[i].gameObject.name = "Tile Hover";
            mTileHover[i].gameObject.SetActive(false);
        }

        Transform waypointsParent = new GameObject().GetComponent<Transform>();
        waypointsParent.parent = GetComponent<Transform>();
        waypointsParent.gameObject.name = "Waypoints";

        // Generate waypoints.
        mWaypointsForTiles = new UX_Waypoint[Piece.MAX_WAYPOINTS][];
        for (int i = 0; i < mWaypointsForTiles.Length; i++) {
            mWaypointsForTiles[i] = new UX_Waypoint[9];
            for (int j = 0; j < mWaypointsForTiles[i].Length; j++) {
                mWaypointsForTiles[i][j] = Instantiate(baseWaypointForTiles.gameObject, waypointsParent)
                    .GetComponent<UX_Waypoint>();
                mWaypointsForTiles[i][j].ClonePosOffsetCount = boardOffsets.Length;
                for (int boardID = 0; boardID < boardOffsets.Length; boardID++) {
                    mWaypointsForTiles[i][j].AddClonePosOffset(boardOffsets[boardID][j], boardID);
                }
                string name = "Waypoint for Tiles " + i;
                if (j == 0) {
                    mWaypointsForTiles[i][j].gameObject.name = name;
                    mWaypointsForTiles[i][j].SetReal();
                } else {
                    mWaypointsForTiles[i][j].gameObject.name = name + " - Clone " + Util.DirToString(j - 1);
                    mWaypointsForTiles[i][0].AddClone(mWaypointsForTiles[i][j], j);
                }
            }
        }
        mWaypointsForPieces = new UX_Waypoint[Piece.MAX_WAYPOINTS][];
        for (int i = 0; i < mWaypointsForPieces.Length; i++) {
            mWaypointsForPieces[i] = new UX_Waypoint[9];
            for (int j = 0; j < mWaypointsForPieces[i].Length; j++) {
                mWaypointsForPieces[i][j] = Instantiate(baseWaypointForPieces.gameObject, waypointsParent)
                    .GetComponent<UX_Waypoint>();
                mWaypointsForPieces[i][j].ClonePosOffsetCount = boardOffsets.Length;
                for (int boardID = 0; boardID < boardOffsets.Length; boardID++) {
                    mWaypointsForPieces[i][j].AddClonePosOffset(boardOffsets[boardID][j], boardID);
                }
                string name = "Waypoint for Pieces " + i;
                if (j == 0) {
                    mWaypointsForPieces[i][j].gameObject.name = name;
                    mWaypointsForPieces[i][j].SetReal();
                } else {
                    mWaypointsForPieces[i][j].gameObject.name = name + " - Clone " + Util.DirToString(j - 1);
                    mWaypointsForPieces[i][0].AddClone(mWaypointsForPieces[i][j], j);
                }
            }
        }
        mWaypointHoveringForTiles = new UX_Waypoint[9];
        for (int j = 0; j < 9; j++) {
            mWaypointHoveringForTiles[j] = Instantiate(baseWaypointForTiles.gameObject, waypointsParent)
                .GetComponent<UX_Waypoint>();
            mWaypointHoveringForTiles[j].ClonePosOffsetCount = boardOffsets.Length;
            for (int boardID = 0; boardID < boardOffsets.Length; boardID++) {
                mWaypointHoveringForTiles[j].AddClonePosOffset(boardOffsets[boardID][j], boardID);
            }
            string name = "Waypoint (Hovered) for Tiles";
            if (j == 0) {
                mWaypointHoveringForTiles[j].gameObject.name = name;
                mWaypointHoveringForTiles[j].SetReal();
            } else {
                mWaypointHoveringForTiles[j].gameObject.name = name + " - Clone " + Util.DirToString(j - 1);
                mWaypointHoveringForTiles[0].AddClone(mWaypointHoveringForTiles[j], j);
            }
        }
        mWaypointHoveringForPieces = new UX_Waypoint[9];
        for (int j = 0; j < 9; j++) {
            mWaypointHoveringForPieces[j] = Instantiate(baseWaypointForPieces.gameObject, waypointsParent)
                .GetComponent<UX_Waypoint>();
            mWaypointHoveringForPieces[j].ClonePosOffsetCount = boardOffsets.Length;
            for (int boardID = 0; boardID < boardOffsets.Length; boardID++) {
                mWaypointHoveringForPieces[j].AddClonePosOffset(boardOffsets[boardID][j], boardID);
            }
            string name = "Waypoint (Hovered) for Pieces";
            if (j == 0) {
                mWaypointHoveringForPieces[j].gameObject.name = name;
                mWaypointHoveringForPieces[j].SetReal();
            } else {
                mWaypointHoveringForPieces[j].gameObject.name = name + " - Clone " + Util.DirToString(j - 1);
                mWaypointHoveringForPieces[0].AddClone(mWaypointHoveringForPieces[j], j);
            }
        }

        // Hide hovered tile.
        HideTileHover();

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

        mCam.Init(localPlayerIdx, mCanv, boardBounds);
    }

    public float[][] QueryRays() { return mCam.RayHitPoints; }

    public SignalFromClient QueryGamepad() {
        SignalFromClient signal = null;

        // 1 -> button pressed, -1 -> button depressed
        int[] gamepadInput = mGamepad.PadInput;

        // <Left joystick | WASD> Pan the camera.
        mCam.Move(gamepadInput[(int) Gamepad.Button.L_HORIZ], gamepadInput[(int) Gamepad.Button.L_VERT]);

        // <Right joystick | Square brackets> Rotate the camera.
        // mCam.Rotate(gameInput[(int) Gamepad.Button.R_HORIZ]);

        // <Right joystick | Plus/Minus> Zoom the camera.
        // mCam.Zoom(gameInput[(int) Gamepad.Button.R_VERT]);

        // <D-pad | Arrows>
        if (gamepadInput[(int) Gamepad.Button.LEFT] == 1) {
            if (mMode == Mode.HAND) mHand.MoveCursor(Util.LEFT, -1);
            else if (mMode == Mode.SURRENDER) { /* mSurrenderMenu.MoveCursor(Util.LEFT); */ }
            else if (mMode == Mode.PAUSE) { /* mPauseMenu.MoveCursor(Util.LEFT); */ }
            else if (mMode == Mode.SELECT_BOARD) { /* mCam.BoardID = mBoardMenu.Select(Util.LEFT); */ }
            else if (mMode == Mode.DETAIL) { /* mDetailMenu.MoveCursor(Util.LEFT); */ }
            else if (InSelectPieceMode()) {
                /* mCam.SnapToPieceFrom(mHoveredTile, Util.LEFT); */ }
            else if (InSelectTileMode()) { /* mCam.GoToTile(mHoveredTile, Util.LEFT); */ }
            else if (mMode == Mode.SELECT_CHUNK_SPELLCAST) { /* mCam.GoToChunk(mHoveredTile, Util.LEFT); */ }
        } else if (gamepadInput[(int) Gamepad.Button.RIGHT] == 1) {
            if (mMode == Mode.HAND) mHand.MoveCursor(Util.RIGHT, -1);
            else if (mMode == Mode.SURRENDER) { /* mSurrenderMenu.MoveCursor(Util.RIGHT) */ }
            else if (mMode == Mode.PAUSE) { /* mPauseMenu.MoveCursor(Util.RIGHT); */ }
            else if (mMode == Mode.SELECT_BOARD) { /* mCam.BoardID = mBoardMenu.Select(Util.RIGHT); */ }
            else if (mMode == Mode.DETAIL) { /* mDetailMenu.MoveCursor(Util.RIGHT); */ }
            else if (InSelectPieceMode()) {
                /* mCam.SnapToPieceFrom(mHoveredTile, Util.RIGHT); */ }
            else if (InSelectTileMode()) { /* mCam.GoToTile(mHoveredTile, Util.RIGHT); */ }
            else if (mMode == Mode.SELECT_CHUNK_SPELLCAST) { /* mCam.GoToChunk(mHoveredTile, Util.RIGHT); */ }
        } else if (gamepadInput[(int) Gamepad.Button.UP] == 1) {
            if (mMode == Mode.HAND) mHand.MoveCursor(-1, Util.UP);
            else if (mMode == Mode.PAUSE) { /* mPauseMenu.MoveCursor(Util.UP); */ }
            else if (mMode == Mode.SELECT_BOARD) { /* mCam.BoardID = mBoardMenu.Select(Util.UP); */ }
            else if (mMode == Mode.DETAIL) { /* mDetailMenu.MoveCursor(Util.UP); */ }
            else if (InSelectPieceMode()) {
                /* mCam.SnapToPieceFrom(mHoveredTile, Util.UP); */ }
            else if (InSelectTileMode()) { /* mCam.GoToTile(mHoveredTile, Util.UP); */ }
            else if (mMode == Mode.SELECT_CHUNK_SPELLCAST) { /* mCam.GoToChunk(mHoveredTile, Util.UP); */ }
        } else if (gamepadInput[(int) Gamepad.Button.DOWN] == 1) {
            if (mMode == Mode.HAND) mHand.MoveCursor(-1, Util.DOWN);
            else if (mMode == Mode.PAUSE) { /* mPauseMenu.MoveCursor(Util.DOWN); */ }
            else if (mMode == Mode.SELECT_BOARD) { /* mCam.BoardID = mBoardMenu.Select(Util.DOWN); */ }
            else if (mMode == Mode.DETAIL) { /* mDetailMenu.MoveCursor(Util.DOWN); */ }
            else if (InSelectPieceMode()) {
                /* mCam.SnapToPieceFrom(mHoveredTile, Util.DOWN); */ }
            else if (InSelectTileMode()) { /* mCam.GoToTile(mHoveredTile, Util.DOWN); */ }
            else if (mMode == Mode.SELECT_CHUNK_SPELLCAST) { /* mCam.GoToChunk(mHoveredTile, Util.DOWN); */ }
        }

        // <A button | Space> (button pressed)
        else if (gamepadInput[(int) Gamepad.Button.A] == 1) {
            if (mMode == Mode.SELECT_PIECE_PLAIN) SetMode(Mode.SELECT_PIECE_GROUP);
            else if (mMode == Mode.HAND) {
                /* Try selecting the hovered card in the hand.
                 * Switch to the appropriate mode depending on what kind of entity the card's spell targets. */
                SetModeFromPlayCard(mHand.Select());
            } else if (mMode == Mode.SELECT_TILE_SPELLCAST) {
                signal = new SignalCastSpell(
                    mPlayerID, mHand.PlayCardID, mHand.CasterID, mCam.BoardID, mHoveredTile.Pos);
                mHand.HideAll();
                SetMode(Mode.SELECT_PIECE_PLAIN);
            } else if (mMode == Mode.SELECT_TILE_WAYPOINT) {
                if (mSelectedPieces.Count == 1) {
                    signal = new SignalAddWaypoint(mPlayerID, mHoveredTile.Pos, mCam.BoardID, HoveredWaypointIdx,
                        mSelectedPieces[0].PieceID);
                } else if (mSelectedPieces.Count > 1) {
                    // Send different type of signal if for a group of selected pieces.
                    signal = new SignalAddGroupWaypoints(mPlayerID, mWaypointTargetsForGroup, mCam.BoardID,
                        mSelectedPieces.Select(piece => piece.PieceID).ToArray());
                }
                
            }
        }

        // <A button | Space> (button depressed)
        else if (gamepadInput[(int) Gamepad.Button.A] == -1) {
            if (mMode == Mode.SELECT_PIECE_GROUP) {
                // If only one piece was hovered (typical for non-group selection).
                if (mHoveredPieces.Count == 1) {
                    UX_Piece hoveredPiece = mHoveredPieces[0];
                    // Unselect the hovered piece if it's already selected.
                    if (mSelectedPieces.Contains(hoveredPiece)) {
                        hoveredPiece.Unselect(mLocalPlayerIdx);
                        mSelectedPieces.Remove(hoveredPiece);
                    /* Select the hovered piece if it isn't selected.
                     * Can only select this piece if it's belongs to this player. */
                    } else if (hoveredPiece.PlayerID == mPlayerID) {
                        hoveredPiece.Select(mLocalPlayerIdx);
                        mSelectedPieces.Add(hoveredPiece);
                    }
                } else if (mHoveredPieces.Count > 0) {
                    // All hovered pieces that are not selected become selected.
                    foreach (UX_Piece hoveredPiece in mHoveredPieces) {
                        // Can only select this piece if it's belongs to this player.
                        if (!mSelectedPieces.Contains(hoveredPiece) && hoveredPiece.PlayerID == mPlayerID) {
                            hoveredPiece.Select(mLocalPlayerIdx);
                            mSelectedPieces.Add(hoveredPiece);
                        }
                    }
                }
                SetMode(Mode.SELECT_PIECE_PLAIN);
            }
        }

        //     } else if (mMode == Mode.WAYPOINT_TILE) { // Set waypoint on hovered tile.
        //         if (mSelectedPieces.Count > 0) {
        //             int orderPlace = mHoveredWaypoint;
        //             if (mHoveredWaypoint == -1 || mHoveredWaypoint >= Piece.MAX_WAYPOINTS)
        //                 orderPlace = mHoveredWaypointMax;
        //             // int[] selectedPieceIDs = mSelectedPieces.Select(uxPiece => uxPiece.PieceID).ToArray();
        //             // if (mSelectedPieces[0].WaypointTiles[orderPlace] != null
        //             //     && mSelectedPieces[0].WaypointTiles[orderPlace].Pos == mHoveredTile.Pos) {

        //             //     signal = new SignalRemoveWaypoint(
        //             //         mPlayerID, mHoveredTile.BoardID, orderPlace, selectedPieceIDs);
        //             // } else {
        //             //     signal = SignalFromClient.AddWaypoint(
        //             //         mPlayerID, mHoveredTile, orderPlace, mSelectedPieces.ToArray());
        //             // }
        //         }
        //     } else if (mMode == Mode.WAYPOINT_PIECE) {
        //         if (mSelectedPieces.Count > 0) {
        //             int orderPlace = mHoveredWaypoint;
        //             if (mHoveredWaypoint == -1 || mHoveredWaypoint >= Piece.MAX_WAYPOINTS)
        //                 orderPlace = mHoveredWaypointMax;
        //             // if (mSelectedPieces[0].WaypointPieces[orderPlace] != null
        //             //     && mSelectedPieces[0].WaypointPieces[orderPlace].PieceID == mHoveredPiece.PieceID) {
        //             //     signal = SignalFromClient.RemoveWaypoint(
        //             //         mHoveredPiece.BoardID, orderPlace,
        //             //         mSelectedPieces.ToArray());
        //             // } else {
        //             //     int targetID = mHoveredPiece != null ? mHoveredPiece.PieceID : -1;
        //             //     signal = SignalFromClient.AddWaypoint(
        //             //         mHoveredPiece.BoardID, targetID, orderPlace,mSelectedPieces.ToArray());
        //             // }
        //         }
        //     } else if (mMode == Mode.TARGET_TILE) { // Play the selected card.
        //         // signal = SignalFromClient.CastSpell(mPlayCardID, mCasterPieceID, mHoveredTile);
        //         signal = new SignalCastSpell(mPlayerID, mHand.PlayCardID, mHand.CasterID, mCam.BoardID, mHoveredTile);
        //         mHand.Hide();
        //         SetMode(Mode.PLAIN);
        //     // Select the hovered card.
        //     } else if (mMode == Mode.HAND) { if (mHand.Select()) SetMode(Mode.TARGET_TILE); }
        // }

        // <B button | R>
        else if (gamepadInput[(int) Gamepad.Button.B] == 1) {
            // Unselect every piece.
            if (mMode == Mode.SELECT_PIECE_PLAIN) {
                foreach (UX_Piece piece in mSelectedPieces) { piece.Unselect(mLocalPlayerIdx); }
            // Go back to mode that was saved before entering high-level menu.
            } else if (InMenuMode()) SetMode(mModePrev);
            // Go to Plain mode if viewing the hand.
            else if (mMode == Mode.HAND) { mHand.HideAll(); SetMode(Mode.SELECT_PIECE_PLAIN); }
            // Go to Plain mode if viewing the board selection menu.
            else if (mMode == Mode.SELECT_BOARD) { /* mBoardMenu.Hide(); */ SetMode(Mode.SELECT_PIECE_PLAIN); }
            else if (mMode == Mode.SELECT_CHUNK_SPELLCAST || InSelectPieceMode() || InSelectTileMode())
                SetMode(Mode.SELECT_PIECE_PLAIN);
            else if (mMode == Mode.DETAIL) { /* mDetailMenu.Hide(); */ SetMode(mModePrev); }
            else if (mMode == Mode.SURRENDER) { /* mSurrenderMenu.Hide(); */ SetMode(mModePrev); }
            else if (mMode == Mode.PAUSE) { /* mPauseMenu.Hide(); */ SetMode(mModePrev); }
        }

        // <X button | F>
        else if (gamepadInput[(int) Gamepad.Button.X] == 1) {
            // View the hand of a hovered piece if it's the only piece being hovered.
            if (mMode == Mode.SELECT_PIECE_PLAIN && mHoveredPieces.Count == 1) {
                mHand.Show(mHoveredPieces[0]);
                SetMode(Mode.HAND);
            // Go to Plain mode if viewing the hand.
            } else if (mMode == Mode.HAND) { mHand.HideAll(); SetMode(Mode.SELECT_PIECE_PLAIN); }
        }

        // <Bumpers | Q/E>
        else if (gamepadInput[(int) Gamepad.Button.L_BUMP] == 1) {
            if (InWaypointMode()) HoveredWaypointIdx--;
            else if (mMode == Mode.SELECT_PIECE_PLAIN && mSelectedPieces.Count > 0) {
                HoveredWaypointIdx = mWaypointsForTiles.Length - 1;
                SetMode(Mode.SELECT_TILE_WAYPOINT);
            }
        } else if (gamepadInput[(int) Gamepad.Button.R_BUMP] == 1) {
            if (InWaypointMode()) HoveredWaypointIdx++;
            else if (mMode == Mode.SELECT_PIECE_PLAIN && mSelectedPieces.Count > 0) {
                HoveredWaypointIdx = 0;
                SetMode(Mode.SELECT_TILE_WAYPOINT);
            }
        }

        // <Left trigger | Left shift>
        else if (gamepadInput[(int) Gamepad.Button.L_TRIG] == 1) {
            if (InWaypointMode()) {
                if (mSelectedPieces.Count == 1) {
                    signal = new SignalRemoveWaypoint(mPlayerID, mCam.BoardID, HoveredWaypointIdx,
                        mSelectedPieces[0].PieceID);
                } else if (mSelectedPieces.Count > 1) {
                    // Send different type of signal if for a group of selected pieces.
                    signal = new SignalRemoveGroupWaypoints(mPlayerID, mCam.BoardID,
                        mSelectedPieces.Select(piece => piece.PieceID).ToArray());
                }
            }
        }

        // <Right trigger | Right shift>
        else if (gamepadInput[(int) Gamepad.Button.R_TRIG] == 1) {
            if (InWaypointMode()) {
                // Toggle between targeting tiles and pieces when setting waypoints.
                if (mMode == Mode.SELECT_TILE_WAYPOINT) SetMode(Mode.SELECT_PIECE_WAYPOINT);
                else if (mMode == Mode.SELECT_TILE_WAYPOINT) SetMode(Mode.SELECT_PIECE_WAYPOINT);
            }
        }
        
        return signal;
    }

    public void UpdateWaypoints() {
        if (InWaypointMode() || mHoveredPieces.Count == 1) ShowWaypoints();
    }

    // Called once every frame.
    protected virtual void Update() {

    }

    private void HoverPiece(UX_Piece piece) {
        if (piece != null) {
            mHoveredPieces.Add(piece);
            piece.Hover(mLocalPlayerIdx);
        } else if (mSelectedPieces.Count == 0) HideWaypoints();
    }

    private void UnhoverTile() {
        if (mHoveredTile != null) {
            foreach (Transform tile in mTileHover) { tile.gameObject.SetActive(false); }
            mHoveredTile = null;
        }
    }

    private void UnhoverChunk() {

    }

    private void ShowWaypoints() {
        // If modifying the selected piece(s)'s waypoints.
        if (InWaypointMode()) {
            // if (mSelectedPieces.Count >= 1) {
            //     // Show the hovering waypoint.
            //     if (mMode == Mode.SELECT_TILE_WAYPOINT) {
            //         // Setting stuff for the real UX_Waypoint sets it for its 8 other clones.
            //         mWaypointHoveringForTiles[0].Show(); mShowingWaypoints = true;
            //         mWaypointHoveringForPieces[0].Hide();
            //     } else if (mMode == Mode.SELECT_PIECE_WAYPOINT) {
            //         // Setting stuff for the real UX_Waypoint sets it for its 8 other clones.
            //         mWaypointHoveringForPieces[0].Show(); mShowingWaypoints = true;
            //         mWaypointHoveringForTiles[0].Hide();
            //     }
            // }
            if (mSelectedPieces.Count == 1) {
                /* Show the opaque waypoints + (maybe) the hovered waypoint
                 * that are already held by the selected piece. */
                UX_Tile[] waypointTiles = mSelectedPieces[0].WaypointTiles;
                UX_Piece[] waypointPieces = mSelectedPieces[0].WaypointPieces;
                for (int i = 0; i < waypointTiles.Length; i++) {
                    if (waypointTiles[i] != null) {
                        // Setting stuff for the real UX_Waypoint sets it for its 8 other clones.
                        mWaypointsForTiles[i][0].Tile = waypointTiles[i];
                        mWaypointsForTiles[i][0].Opaque = true; // Specifies material.
                        if (HoveredWaypointIdx == i) mWaypointsForTiles[i][0].Hover(); // Specifies material.
                        else mWaypointsForTiles[i][0].Unhover(); // Specifies material.
                        mWaypointsForTiles[i][0].Show(); mShowingWaypoints = true;
                        mWaypointsForPieces[i][0].Hide();
                    } else if (waypointPieces[i] != null) {
                        // Setting stuff for the real UX_Waypoint sets it for its 8 other clones.
                        mWaypointsForPieces[i][0].Piece = waypointPieces[i];
                        mWaypointsForPieces[i][0].Opaque = true; // Specifies material.
                        if (HoveredWaypointIdx == i) mWaypointsForPieces[i][0].Hover(); // Specifies material.
                        else mWaypointsForPieces[i][0].Unhover(); // Specifies material.
                        mWaypointsForPieces[i][0].Show(); mShowingWaypoints = true;
                        mWaypointsForTiles[i][0].Hide();
                    } else { // No waypoint of any type in this slot.
                        // If the waypoint hovered index is out of bounds.
                        // if (HoveredWaypointIdx > i) {
                        //     HoveredWaypointIdx = i;
                        //     if (mMode == Mode.SELECT_TILE_WAYPOINT) {
                        //         mWaypointsForTiles[i][0].Hover();
                        //         mWaypointsForPieces[i][0].Unhover();
                        //     } else if (mMode == Mode.SELECT_PIECE_WAYPOINT) {
                        //         mWaypointsForPieces[i][0].Hover();
                        //         mWaypointsForTiles[i][0].Unhover();
                        //     }
                        // }
                        break;
                    }
                }
            }
        /* If just hovering over a single piece. Doesn't matter if pieces are selected.
         * Don't bother with multiple hovered pieces. */
        } else if (mHoveredPieces.Count == 1) {
            // Show just the waypoints as transparent (no hovering waypoint, no hovered waypoint).
            UX_Tile[] waypointTiles = mHoveredPieces[0].WaypointTiles;
            UX_Piece[] waypointPieces = mHoveredPieces[0].WaypointPieces;
            for (int i = 0; i < waypointTiles.Length; i++) {
                if (waypointTiles[i] != null) {
                    // Setting stuff for the real UX_Waypoint sets it for its 8 other clones.
                    mWaypointsForTiles[i][0].Tile = waypointTiles[0];
                    mWaypointsForTiles[i][0].Opaque = true; // Specifies material.
                    mWaypointsForTiles[i][0].Unhover(); // Specifies material.
                    mWaypointsForTiles[i][0].Show(); mShowingWaypoints = true;
                    mWaypointsForPieces[i][0].Hide();
                } else if (waypointPieces[i] != null) {
                    // Setting stuff for the real UX_Waypoint sets it for its 8 other clones.
                    mWaypointsForPieces[i][0].Piece = waypointPieces[0];
                    mWaypointsForPieces[i][0].Opaque = true; // Specifies material.
                    mWaypointsForPieces[i][0].Unhover(); // Specifies material.
                    mWaypointsForPieces[i][0].Show(); mShowingWaypoints = true;
                    mWaypointsForTiles[i][0].Hide();
                }
            }
        }
    }

    private void HideWaypoints() {
        if (!mShowingWaypoints) return;

        // Setting stuff for the real UX_Waypoint sets it for its 8 other clones.
        for (int i = 0; i < mWaypointsForTiles.Length; i++) {
            mWaypointsForTiles[i][0].Hide();
            mWaypointsForPieces[i][0].Hide();
        }
        for (int i = 0; i < mWaypointHoveringForTiles.Length; i++) {
            mWaypointHoveringForTiles[i].Hide();
            mWaypointHoveringForPieces[i].Hide();
        }
        mShowingWaypoints = false;
    }

    private void ShowTileHover() {
        for (int i = 0; i < mTileHover.Length; i++) { mTileHover[i].gameObject.SetActive(true); }
    }
    private void HideTileHover() {
        for (int i = 0; i < mTileHover.Length; i++) { mTileHover[i].gameObject.SetActive(false); }
    }

    private bool InMenuMode() { return mMode == Mode.DETAIL || mMode == Mode.SURRENDER || mMode == Mode.PAUSE; }
    private bool InSelectTileMode() {
        return mMode == Mode.SELECT_TILE_SPELLCAST || mMode == Mode.SELECT_TILE_WAYPOINT;
    }
    private bool InSelectPieceMode() {
        return mMode == Mode.SELECT_PIECE_PLAIN || mMode == Mode.SELECT_PIECE_GROUP
            || mMode == Mode.SELECT_PIECE_SPELLCAST || mMode == Mode.SELECT_PIECE_WAYPOINT;
    }
    private bool InWaypointMode() {
        return mMode == Mode.SELECT_TILE_WAYPOINT || mMode == Mode.SELECT_PIECE_WAYPOINT;
    }

    /// <returns>
    /// <c>True</c> if the mode has changed.
    /// </returns>
    private bool SetMode(Mode mode) {
        if (mMode == mode) return false;

        mModePrev = mMode;
        mMode = mode;

        if (InMenuMode() || mMode == Mode.HAND || mMode == Mode.SELECT_BOARD) mCanv.SetCrosshair(2);
        else if (InSelectPieceMode() && mMode != Mode.SELECT_PIECE_GROUP) mCanv.SetCrosshair(0);
        else mCanv.SetCrosshair(1);
        mCanv.SetDarkScreen(InMenuMode() || mMode == Mode.HAND);

        if (InSelectTileMode()) ShowTileHover(); else HideTileHover();

        Debug.Log("SetMode: " + mode);
        return true;
    }

    /// <remarks>
    /// 0 -> chunk, 1 -> tile, 2 -> piece
    /// </remarks>
    private bool SetModeFromPlayCard(int option) {
        switch (option) {
            case 0: return SetMode(Mode.SELECT_CHUNK_SPELLCAST);
            case 1: return SetMode(Mode.SELECT_TILE_SPELLCAST);
            case 2: return SetMode(Mode.SELECT_PIECE_SPELLCAST);
        }
        return false;
    }
}