using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player
{
    private HandScript handScript;
    private CameraScript cameraScript;
    private StateEnum state = StateEnum.PIECE;
    public StateEnum State
    {
        set
        {
            StateEnum oldState = state;
            state = value;

            // If state changes, unhover everything
            if (state != oldState)
            {
                HoverChunk(null);
                HoverTile(null);
                HoverPiece(null);
            }

            cameraScript.MovesOnInput = (
                state == StateEnum.PIECE
                || state == StateEnum.DESTINATION
                || state == StateEnum.TILE);
            handScript.SetCursor(state);
            handScript.Showing = (state == StateEnum.HAND);
        }
    }
    private Gamepad gamepad = null;

    public Player(HandScript handScript, CameraScript cameraScript,
        int totalBoardSize, int idx)
    {
        this.handScript = handScript;
        this.cameraScript = cameraScript;

        cameraScript.SetBounds(-0.5F, totalBoardSize - 0.5F);

        // idx value determines if using keyboard or gamepad
        if (idx == 0)
        {
            // Set up Key bindings
            InputAction key_ESC = new InputAction();
            key_ESC.AddBinding("<Keyboard>/escape");
            key_ESC.performed += ctx => Application.Quit();
            key_ESC.Enable();

            InputAction key_H = new InputAction();
            key_H.AddBinding("<Keyboard>/h");
            key_H.performed += ctx => ToggleHand();
            key_H.Enable();

            InputAction key_LEFT = new InputAction();
            key_LEFT.AddBinding("<Keyboard>/leftarrow");
            key_LEFT.started += ctx => PressDirection(Coord.LEFT, true);
            key_LEFT.canceled += ctx => PressDirection(Coord.LEFT, false);
            key_LEFT.Enable();
            
            InputAction key_RIGHT = new InputAction();
            key_RIGHT.AddBinding("<Keyboard>/rightarrow");
            key_RIGHT.started += ctx => PressDirection(Coord.RIGHT, true);
            key_RIGHT.canceled += ctx => PressDirection(Coord.RIGHT, false);
            key_RIGHT.Enable();

            InputAction key_UP = new InputAction();
            key_UP.AddBinding("<Keyboard>/uparrow");
            key_UP.started += ctx => PressDirection(Coord.UP, true);
            key_UP.canceled += ctx => PressDirection(Coord.UP, false);
            key_UP.Enable();
            
            InputAction key_DOWN = new InputAction();
            key_DOWN.AddBinding("<Keyboard>/downarrow");
            key_DOWN.started += ctx => PressDirection(Coord.DOWN, true);
            key_DOWN.canceled += ctx => PressDirection(Coord.DOWN, false);
            key_DOWN.Enable();

            InputAction key_SPACE = new InputAction();
            key_SPACE.AddBinding("<Keyboard>/space");
            key_SPACE.performed += ctx => handScript.DrawCards(3);
            key_SPACE.Enable();
        }
        else gamepad = Gamepad.all[idx - 1];
    }

    public void PollInput()
    {
        
    }

    private void PressDirection(int dir, bool press)
    {
        if (state == StateEnum.HAND) handScript.PressDirection(dir, press);
        else cameraScript.PressDirection(dir, press);
    }

    // Toggle between hand mode and piece mode
    private void ToggleHand()
    {
        if (state == StateEnum.HAND) State = StateEnum.PIECE;
        else State = StateEnum.HAND;
    }

    private List<Piece> selectedPieces = new List<Piece>();
    public void SelectPiece(Piece piece)
    {
        selectedPieces.Add(piece);
    }

    private List<Tile> selectedTiles = new List<Tile>();
    public void SelectTile(Tile tile)
    {

    }

    public void SelectChunk(Chunk chunk)
    {

    }

    private Piece hoveredPiece = null;
    public void HoverPiece(Piece piece)
    {
        hoveredPiece = piece;
    }

    private Tile hoveredTile = null;
    public void HoverTile(Tile tile)
    {

    }

    private Chunk hoveredChunk = null;
    private void HoverChunk(Chunk chunk)
    {
        if (hoveredChunk != chunk)
        {
            hoveredChunk.PositionColliderGrid(false);
            hoveredChunk = chunk;
        }
    }

    public void Update(List<Piece> pieces)
    {
        cameraScript.UpdateRays(state);

        switch (state)
        {
            case StateEnum.PIECE:
            {
                // Check if pieces are being hovered.
                Piece pieceToHover = cameraScript.GetHoveredPiece(pieces);
                if (pieceToHover != null) hoveredPiece = pieceToHover;
                else if (hoveredPiece != null)
                {
                    // Turn off particle system before setting to null.
                    hoveredPiece.ToggleParticles(false);
                    hoveredPiece.SetDestinationObjects(false);
                    hoveredPiece = null;
                }
                if (hoveredPiece != null)
                {
                    // Position the destination gameObjects.
                    hoveredPiece.SetDestinationObjects(true);

                    // Turn on the hovered piece's particle system.
                    hoveredPiece.ToggleParticles(true);
                }
                break;
            }
            default:
            {
                break;
            }
        }
    }

    // public void Update()
    // {
    //     if (state == StateEnum.PIECE)
    //     {
    //         HoverChunk(null);

    //         // Check if pieces are being hovered.
    //         Piece pieceToHover = cameraScript.GetHoveredPiece(pieces);
    //         if (pieceToHover != null) hoveredPiece = pieceToHover;
    //         else if (hoveredPiece != null)
    //         {
    //             // Turn off particle system before setting to null.
    //             hoveredPiece.ToggleParticles(false);
    //             hoveredPiece.SetDestinationObjects(objs_dest, false);
    //             hoveredPiece = null;
    //         }

    //         // Check if there is more than 1 piece selected. If there are, see if
    //         // they have the same destinations.
    //         Piece selectedPiece = null;
    //         foreach (Piece piece in selectedPieces)
    //         {
    //             if (selectedPiece == null) selectedPiece = piece;
    //             else if (!selectedPiece.HasSameDestinationsAs(piece))
    //             {
    //                 selectedPiece = null;
    //                 break;
    //             }
    //         }

    //         if (selectedPiece != null)
    //         {
    //             // Position the destination gameObjects.
    //             selectedPiece.SetDestinationObjects(objs_dest, true);
    //         }
    //         else if (hoveredPiece != null)
    //         {
    //             // Position the destination gameObjects.
    //             hoveredPiece.SetDestinationObjects(objs_dest, true);

    //             // Turn on the hovered piece's particle system.
    //             hoveredPiece.ToggleParticles(true);
    //         }
    //     }
    //     else if (state == StateEnum.DESTINATION)
    //     {
    //         // Check if a chunk is being hovered.
    //         hoveredChunk = cameraScript.GetHoveredChunk(chunks, size);
    //         if (hoveredChunk != null)
    //         {
    //             // Position the collider grid at the hovered chunk
    //             hoveredChunk.PositionColliderGrid(true);

    //             // Check if a tile is being hovered.
    //             hoveredTile
    //                 = cameraScript.GetHoveredTile(hoveredChunk, chunkSize);
    //             if (hoveredTile != null) { /* TODO */ }
    //         }
    //     }
    //     else if (state == StateEnum.TILE)
    //     {
            
    //     }
    // }
}
