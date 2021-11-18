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
    private readonly int CLIENT_ID;
    public int ClientID { get { return CLIENT_ID; } }
    private bool isBot;
    public bool IsBot { get { return isBot; } }
    public Player(string name, int idx, int clientID, bool isBot)
    {
        this.name = name;
        this.idx = idx;
        CLIENT_ID = clientID;
        this.isBot = isBot;
    }
}