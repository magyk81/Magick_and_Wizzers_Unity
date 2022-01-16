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
    public readonly int ID;

    private readonly string name;
    public string Name { get { return name; } }
    private readonly int idx;
    private readonly int CLIENT_ID;
    public int ClientID { get { return CLIENT_ID; } }
    private bool isBot;
    public bool IsBot { get { return isBot; } }
    public Player(string name, int clientID, bool isBot)
    {
        ID = IdHandler.Create(GetType());

        this.name = name;
        CLIENT_ID = clientID;
        this.isBot = isBot;
    }
}