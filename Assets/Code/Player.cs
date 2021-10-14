using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    // private UX_Player ux;
    // public UX_Player UX { set { ux = value; } }
    private readonly string name;
    public string Name { get { return name; } }
    private readonly int idx;
    public int Idx { get { return idx; } }
    public enum Type { LOCAL_PLAYER, REMOTE_PLAYER, BOT }
    private readonly Type PLAYER_TYPE;
    public Type PlayerType { get { return PLAYER_TYPE; } }
    public Player(string name, int idx, Type playerType)
    {
        this.name = name;
        this.idx = idx;
        PLAYER_TYPE = playerType;
    }
}