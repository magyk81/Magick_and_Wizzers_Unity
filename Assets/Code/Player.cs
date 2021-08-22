using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    public enum Interface { PLAIN, WAYPOINT, HAND, DETAIL, SURRENDER, PAUSE }
    private Interface currInterface = Interface.PLAIN;
    private Piece pieceHovered;
    private List<Piece> pieceSelected = new List<Piece>();
    public enum Type { LOCAL_PLAYER, REMOTE_PLAYER, BOT }
    private readonly Type playerType;
    public Player(Type playerType) { this.playerType = playerType; }

    public void SendInput(int[] padInput, CameraScript cam)
    {
        if (currInterface == Interface.PLAIN)
        {
            int x_move = 0, z_move = 0,
                l_horiz = padInput[(int) Gamepad.Button.L_HORIZ],
                l_vert = padInput[(int) Gamepad.Button.L_VERT];
            if (l_horiz != 0) x_move = l_horiz;
            if (l_vert != 0) z_move = l_vert;
            cam.Move(x_move, z_move);
        }
    }

    public void HoverPiece(GameObject obj)
    {
        
    }
}
