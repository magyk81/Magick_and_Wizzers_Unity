using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UX_Player
{
    private readonly Player __;
    public enum Mode { PLAIN, WAYPOINT, HAND, DETAIL, SURRENDER, PAUSE }
    private Mode mode = Mode.PLAIN;
    private readonly Gamepad GAMEPAD;
    private readonly CameraScript CAM;
    private UX_Piece hoveredPiece;
    private List<UX_Piece> selectedPieces = new List<UX_Piece>();
    private Card cardBeingPlayed;
    public UX_Player(Player __, Gamepad gamepad,
        CameraScript cam)
    {
        this.__ = __;
        GAMEPAD = gamepad;
        CAM = cam;
    }

    public void QueryGamepad()
    {
        int[] padInput = GAMEPAD.PadInput;

        Mode oldMode = mode;

        if (mode == Mode.PLAIN)
        {
            // Move camera
            int x_move = 0, z_move = 0,
                l_horiz = padInput[(int) Gamepad.Button.L_HORIZ],
                l_vert = padInput[(int) Gamepad.Button.L_VERT];
            if (l_horiz != 0) x_move = l_horiz;
            if (l_vert != 0) z_move = l_vert;
            CAM.Move(x_move, z_move);
            
            if (cardBeingPlayed != null)
            {

            }
            else
            {
                if (hoveredPiece != null)
                {
                    // Select piece if hovering a piece
                    bool a_button = padInput[(int) Gamepad.Button.A] > 0;
                    if (a_button) SelectPiece(hoveredPiece);
                }
                
                if (hoveredPiece != null && __.HasMaster(hoveredPiece._))
                {
                    CAM.SetHandCards(hoveredPiece._);

                    // Go to HAND mode if hovering a master
                    bool x_button = padInput[(int) Gamepad.Button.X] > 0;
                    if (x_button) mode = Mode.HAND;
                }
            }
        }

        if (mode == Mode.HAND)
        {
            // Go to PLAIN mode
            bool b_button = padInput[(int) Gamepad.Button.B] > 0;
            if (b_button) mode = Mode.PLAIN;

            // Play the hovered card
            bool a_button = padInput[(int) Gamepad.Button.A] > 0;
            if (a_button)
            {
                cardBeingPlayed
                    = CAM.GetHandPiece().GetCardFromHand(CAM.GetHandCard());
                if (cardBeingPlayed != null)
                {
                    CAM.DisplayPlayCard(CAM.GetHandCard());
                    mode = Mode.PLAIN;
                }
            }

            int x_move = -1, y_move = -1;
            if (padInput[(int) Gamepad.Button.LEFT] == 1) x_move = Util.LEFT;
            else if (padInput[(int) Gamepad.Button.RIGHT] == 1)
                x_move = Util.RIGHT;
            if (padInput[(int) Gamepad.Button.DOWN] == 1) y_move = Util.DOWN;
            else if (padInput[(int) Gamepad.Button.UP] == 1) y_move = Util.UP;
            CAM.HandMove(x_move, y_move);
        }

        if (oldMode != mode)
        {
            CAM.SetMode(mode);
            Debug.Log("Set Mode: " + mode);
        }
    }

    public void QueryCamera(UX_Chunk[][,] chunks)
    {
        // Hover tiles

        if (cardBeingPlayed == null) return;

        UX_Chunk chunkDetected = null;
        
    }

    public void QueryCamera(List<UX_Piece> pieces)
    {
        foreach (UX_Piece ux_piece in pieces)
        {
            ux_piece.Unhover();
        }

        if (cardBeingPlayed != null) return;

        // Hover pieces

        // Get colliders detected by this player's camera.
        List<Collider> collidersDetected = CAM.GetDetectedColliders();
        foreach (Collider collider in collidersDetected)
        {
            foreach (UX_Piece ux_piece in pieces)
            {
                if (ux_piece.IsCollider(collider))
                {
                    hoveredPiece = ux_piece;
                    hoveredPiece.Hover();
                    return;
                }
            }
        }
    }

    private void SelectPiece(UX_Piece ux_piece)
    {
        if (selectedPieces.Contains(ux_piece))
        {
            selectedPieces.Remove(ux_piece);
            ux_piece.Unselect();
        }
        else
        {
            selectedPieces.Add(ux_piece);
            ux_piece.Select();
        }
    }
}
