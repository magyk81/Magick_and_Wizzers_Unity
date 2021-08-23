using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    public enum Mode { PLAIN, WAYPOINT, HAND, DETAIL, SURRENDER, PAUSE }
    private Mode currMode = Mode.PLAIN;
    private Piece pieceHovered;
    private List<Piece> pieceSelected = new List<Piece>();
    public enum Type { LOCAL_PLAYER, REMOTE_PLAYER, BOT }
    private readonly string name;
    public string Name { get { return name; } }
    private readonly Type playerType;
    public Type PlayerType { get { return playerType; } }
    private List<Master> masters = new List<Master>();
    public Player(string name, Type playerType)
    {
        this.name = name;
        this.playerType = playerType;
    }

    public void SendInput(int[] padInput, CameraScript cam)
    {
        Mode oldMode = currMode;

        if (currMode == Mode.PLAIN)
        {
            // Move camera
            int x_move = 0, z_move = 0,
                l_horiz = padInput[(int) Gamepad.Button.L_HORIZ],
                l_vert = padInput[(int) Gamepad.Button.L_VERT];
            if (l_horiz != 0) x_move = l_horiz;
            if (l_vert != 0) z_move = l_vert;
            cam.Move(x_move, z_move);

            // Go to HAND mode if hovering a master
            if (pieceHovered != null
                && pieceHovered.GetType() == typeof(Master))
            {
                bool x_button = padInput[(int) Gamepad.Button.X] > 0;
                if (x_button) currMode = Mode.HAND;
            }
        }

        if (currMode == Mode.HAND)
        {
            // Go to PLAIN mode
            bool b_button = padInput[(int) Gamepad.Button.B] > 0;
            if (b_button) currMode = Mode.PLAIN;
        }

        if (oldMode != currMode) cam.SetMode(currMode);
    }

    public void HoverPiece(Piece piece)
    {
        pieceHovered = piece;
    }

    public void AddMaster(Master master) { masters.Add(master); }
}
