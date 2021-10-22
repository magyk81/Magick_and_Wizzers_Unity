/* Copyright (C) All Rights Reserved
 * Unauthorized copying of this file, via any medium is prohibited.
 * Proprietary and confidential.
 * Written by Robin Campos <magyk81@gmail.com>, year 2021.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
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