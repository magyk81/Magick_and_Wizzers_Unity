using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    private readonly string name;
    public string Name { get { return name; } }
    public enum Type { LOCAL_PLAYER, REMOTE_PLAYER, BOT }
    private readonly Type PLAYER_TYPE;
    public Type PlayerType { get { return PLAYER_TYPE; } }
    private List<Master> masters = new List<Master>();
    public Player(string name, Type playerType)
    {
        this.name = name;
        PLAYER_TYPE = playerType;
    }

    public bool HasMaster(Piece piece)
    {
        return masters.Contains((Master) piece);
    }

    public void AddMaster(Master master) { masters.Add(master); }
}