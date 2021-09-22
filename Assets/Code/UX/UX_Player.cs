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
    Transform baseCollParent;
    [SerializeField]
    UX_Collider baseColl;
    private CameraScript cam;
    private CanvasScript canv;
    private UX_Collider[,] tileColls;
    private UX_Collider[] quarterColls;
    private readonly Player __;
    public enum Mode { PLAIN, WAYPOINT, TARGET_PIECE, TARGET_CHUNK,
        TARGET_TILE, HAND, DETAIL, SURRENDER, PAUSE }
    private Mode mode = Mode.PLAIN;
    private Gamepad gamepad;
    
    // Board being currently viewed by this player.
    private UX_Piece hoveredPiece;
    private List<UX_Piece> selectedPieces = new List<UX_Piece>();
    // private Card cardBeingPlayed;
    // private Piece pieceBeingPlayedFrom;

    public void Init(int[][] boardBounds)
    {
        cam = Instantiate(
            baseCam.gameObject,
            GetComponent<Transform>()).GetComponent<CameraScript>();
        cam.gameObject.name = "Camera";

        canv = Instantiate(
            baseCanv.gameObject,
            GetComponent<Transform>()).GetComponent<CanvasScript>();
        canv.gameObject.name = "Canvas";

        Transform tileCollParent = Instantiate(
            baseCollParent.gameObject,
            GetComponent<Transform>()).GetComponent<Transform>();
        tileCollParent.gameObject.name = "Tile Colliders";

        tileColls = new UX_Collider[Chunk.Size / 2, Chunk.Size / 2];
        for (int i = 0; i < Chunk.Size / 2; i++)
        {
            for (int j = 0; j < Chunk.Size/ 2; j++)
            {
                tileColls[i, j] = Instantiate(
                    baseColl.gameObject,
                    tileCollParent).GetComponent<UX_Collider>();
                tileColls[i, j].gameObject.name = Coord._(i, j).ToString();
            }
        }

        Transform quarterCollParent = Instantiate(
            baseCollParent.gameObject,
            GetComponent<Transform>()).GetComponent<Transform>();
        quarterCollParent.gameObject.name = "Quarter Colliders";

        quarterColls = new UX_Collider[4];
        for (int i = 0; i < 4; i++)
        {
            quarterColls[i] = Instantiate(
                baseColl.gameObject,
                quarterCollParent).GetComponent<UX_Collider>();
            quarterColls[i].gameObject.name = Util.DirToString(i + 4);
        }

        Destroy(baseCam.gameObject);
        Destroy(baseCanv.gameObject);
        Destroy(baseCollParent.gameObject);
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

    public void QueryCamera(UX_Chunk[][,] chunks, List<UX_Piece> pieces)
    {
        // foreach (UX_Piece ux_piece in pieces) { ux_piece.Unhover(); }

        // if (mode == Mode.TARGET_TILE)
        // {
        //     // Get middle collider detected by this player's camera.
        //     Collider colliderDetected = CAM.GetDetectedCollider();

        //     // Get chunk and tile being hovered.
        //     UX_Chunk chunkDetected = null;
        //     Coord tileDetected = Coord.Null;
        //     foreach (UX_Chunk[,] ux_chunk_board in chunks)
        //     {
        //         foreach (UX_Chunk ux_chunk in ux_chunk_board)
        //         {
        //             tileDetected = ux_chunk.GetTile(colliderDetected);
        //             if (tileDetected != Coord.Null
        //                 || ux_chunk.IsCollider(colliderDetected))
        //                 chunkDetected = ux_chunk;
                    
        //             if (chunkDetected != null)
        //             {
        //                 chunkDetected.Hover(chunks);
        //                 break;
        //             }
        //         }
        //         if (chunkDetected != null) break;
        //     }

        //     // Determine if the AVAILABLE tiles need to be shown/updated.
        //     bool showAvailableRange = false;

        //     // Set influence range if it hasn't been set yet.
        //     if (influenceRange == InfluenceRange.Null)
        //     {
        //         influenceRange = InfluenceRange._(pieceBeingPlayedFrom);
        //         showAvailableRange = true;
        //     }
        //     else
        //     {
        //         // Update influence range in case the pieceBeingPlayedFrom
        //         // moved or its level changed.
        //         InfluenceRange updatedIR
        //             = influenceRange.Update(pieceBeingPlayedFrom);
        //         if (updatedIR != InfluenceRange.Null)
        //         {
        //             influenceRange = updatedIR;
        //             showAvailableRange = true;
        //         }
                    
        //     }

        //     // Show influence range tiles.
        //     if (showAvailableRange)
        //     {
        //         availableTiles = chunkDetected.ShowTiles(
        //             influenceRange.Origin,
        //             influenceRange.ValidTiles,
        //             (int) UX_Chunk.TileDispType.AVAILABLE, chunks[boardIdx]);
        //     }

        //     // Update tile display of types VALID and INVALID.
        //     // Don't need to check if tileDetected isn't null. If tileDetected
        //     // isn't null, chunkDetected isn't null either.
        //     if (tileDetected != Coord.Null)
        //     {
        //         // See whether the detected tile is within the influence range
        //         // to determine whether it should be shown as VALID or INVALID.
        //         UX_Chunk.TileDispType tileDetectedShowDisp
        //             = UX_Chunk.TileDispType.INVALID;
        //         UX_Chunk.TileDispType tileDetectedHideDisp
        //             = UX_Chunk.TileDispType.VALID;
        //         foreach (Coord availableTile in availableTiles)
        //         {
        //             if (chunkDetected.LocalCoordToBoard(tileDetected)
        //                 == availableTile)
        //             {
        //                 tileDetectedShowDisp = UX_Chunk.TileDispType.VALID;
        //                 tileDetectedHideDisp
        //                     = UX_Chunk.TileDispType.INVALID;
        //                 break;
        //             }
        //         }

        //         // Show hovered tile.
        //         chunkDetected.ShowTile(tileDetected,
        //             (int) tileDetectedShowDisp, chunks[boardIdx]);
        //         chunkDetected.HideTiles((int) tileDetectedHideDisp);
        //     }
        //     else if (chunkDetected != null)
        //     {
        //         chunkDetected.HideTiles((int) UX_Chunk.TileDispType.VALID);
        //         chunkDetected.HideTiles((int) UX_Chunk.TileDispType.INVALID);
        //     }
        // }
        // else if (mode == Mode.TARGET_PIECE || mode == Mode.PLAIN)
        // {
        //     // Get colliders detected by this player's camera.
        //     List<Collider> collidersDetected = CAM.GetDetectedColliders();

        //     // Get piece being hovered.
        //     foreach (Collider collider in collidersDetected)
        //     {
        //         foreach (UX_Piece ux_piece in pieces)
        //         {
        //             if (ux_piece.IsCollider(collider))
        //             {
        //                 hoveredPiece = ux_piece;
        //                 hoveredPiece.Hover();
        //                 return;
        //             }
        //         }
        //     }
        // }
    }

    // private void SelectPiece(UX_Piece ux_piece)
    // {
    //     if (selectedPieces.Contains(ux_piece))
    //     {
    //         selectedPieces.Remove(ux_piece);
    //         ux_piece.Unselect();
    //     }
    //     else
    //     {
    //         selectedPieces.Add(ux_piece);
    //         ux_piece.Select();
    //     }
    // }
}
