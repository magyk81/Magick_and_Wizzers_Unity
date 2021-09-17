using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UX_Player
{
    private readonly Player __;
    public enum Mode { PLAIN, WAYPOINT, TARGET_PIECE, TARGET_CHUNK,
        TARGET_TILE, HAND, DETAIL, SURRENDER, PAUSE }
    private Mode mode = Mode.PLAIN;
    private readonly Gamepad GAMEPAD;
    private readonly CameraScript CAM;
    
    // Board being currently viewed by this player.
    private int boardIdx = 0;
    private UX_Piece hoveredPiece;
    private List<UX_Piece> selectedPieces = new List<UX_Piece>();
    private Card cardBeingPlayed;
    private Piece pieceBeingPlayedFrom;

    private struct InfluenceRange
    {
        public Coord Origin { get; }
        public Coord[] ValidTiles { get; }
        public int Range { get; }
        private InfluenceRange(Coord origin, int range)
        {
            Origin = origin.Copy();
            Range = range;

            if (range >= 0)
            {
                List<Coord> coords = new List<Coord>();
                for (int i = -range; i <= range; i++)
                {
                    for (int j = -range; j <= range; j++)
                    {
                        int dist = Mathf.RoundToInt(
                            Mathf.Sqrt((i * i) + (j * j)));
                        if (dist <= range) coords.Add(Coord._(i, j));
                    }
                }
                ValidTiles = coords.ToArray();
            }
            else ValidTiles = null;
        }

        public InfluenceRange Update(Piece piece)
        {
            // Avoid recalculating anything if the member variables have the
            // same values as before.
            if (piece.Pos == Origin && piece.Level == Range)
                return InfluenceRange.Null;
            else return _(piece);
        }
        public static InfluenceRange _(Piece piece)
        {
            return new InfluenceRange(piece.Pos.Copy(), piece.Level);
        }
        public readonly static InfluenceRange Null
            = new InfluenceRange(Coord.Null, -1);
        public static bool operator ==(InfluenceRange a, InfluenceRange b)
        {
            return a.Origin == b.Origin && a.Range == b.Range;
        }
        public static bool operator !=(InfluenceRange a, InfluenceRange b)
        {
            return a.Origin != b.Origin || a.Range != b.Range;
        }
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
    private InfluenceRange influenceRange = InfluenceRange.Null;

    public UX_Player(Player __, Gamepad gamepad,
        CameraScript cam)
    {
        this.__ = __;
        GAMEPAD = gamepad;
        CAM = cam;

        CAM.SetMode(mode);
    }

    public void QueryGamepad()
    {
        int[] padInput = GAMEPAD.PadInput;

        Mode oldMode = mode;

        if (mode == Mode.PLAIN || mode == Mode.TARGET_CHUNK
            || mode == Mode.TARGET_PIECE || mode == Mode.TARGET_TILE)
        {
            // Move camera
            int x_move = 0, z_move = 0,
                l_horiz = padInput[(int) Gamepad.Button.L_HORIZ],
                l_vert = padInput[(int) Gamepad.Button.L_VERT];
            if (l_horiz != 0) x_move = l_horiz;
            if (l_vert != 0) z_move = l_vert;
            CAM.Move(x_move, z_move);
            
            if (mode == Mode.PLAIN)
            {
                if (hoveredPiece != null)
                {
                    // Select piece if hovering a piece
                    bool a_button = padInput[(int) Gamepad.Button.A] > 0;
                    if (a_button) SelectPiece(hoveredPiece);
                }
                    
                if (hoveredPiece != null && __.Idx == hoveredPiece._.PlayerIdx)
                {
                    CAM.SetHandCards(hoveredPiece._);

                    // Go to HAND mode if hovering a master
                    bool x_button = padInput[(int) Gamepad.Button.X] > 0;
                    if (x_button)
                    {
                        mode = Mode.HAND;
                        pieceBeingPlayedFrom = hoveredPiece._;
                    }
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
                    mode = cardBeingPlayed.GetPlayMode();
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

    public void QueryCamera(UX_Chunk[][,] chunks, List<UX_Piece> pieces)
    {
        foreach (UX_Piece ux_piece in pieces) { ux_piece.Unhover(); }

        if (mode == Mode.TARGET_CHUNK || mode == Mode.TARGET_TILE)
        {
            // Get middle collider detected by this player's camera.
            Collider colliderDetected = CAM.GetDetectedCollider();

            // Get chunk and tile being hovered.
            UX_Chunk chunkDetected = null;
            Coord tileDetected = Coord.Null;
            foreach (UX_Chunk[,] ux_chunk_board in chunks)
            {
                foreach (UX_Chunk ux_chunk in ux_chunk_board)
                {
                    tileDetected = ux_chunk.GetTile(colliderDetected);
                    if (tileDetected != Coord.Null
                        || ux_chunk.IsCollider(colliderDetected))
                        chunkDetected = ux_chunk;
                    
                    if (chunkDetected != null)
                    {
                        chunkDetected.Hover(chunks);
                        break;
                    }
                }
                if (chunkDetected != null) break;
            }

            // Set influence range if it hasn't been set yet.
            if (influenceRange == InfluenceRange.Null)
                influenceRange = InfluenceRange._(pieceBeingPlayedFrom);
            else
            {
                // Update influence range in case the pieceBeingPlayedFrom
                // moved or its level changed.
                InfluenceRange updatedIR
                    = influenceRange.Update(pieceBeingPlayedFrom);
                if (updatedIR != InfluenceRange.Null)
                    influenceRange = updatedIR;
            }

            // Show influence range tiles.

            

            if (tileDetected != Coord.Null)
            {
                // See whether the detected tile is within the influence range
                // to determine whether it should be shown as valid or invalid.
                foreach (UX_Chunk chunk in chunks[boardIdx])
                {

                }

                // Show hovered tile.
                chunkDetected.ShowTiles(tileDetected,
                    (int) UX_Chunk.TileDispType.INVALID, chunks[boardIdx]);
            }
            
        }
        else if (mode == Mode.TARGET_PIECE || mode == Mode.PLAIN)
        {
            // Get colliders detected by this player's camera.
            List<Collider> collidersDetected = CAM.GetDetectedColliders();

            // Get piece being hovered.
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
