/* Copyright (C) All Rights Reserved
 * Unauthorized copying of this file, via any medium is prohibited.
 * Proprietary and confidential.
 * Written by Robin Campos <magyk81@gmail.com>, year 2021.
 */

public class Player
{
    public readonly int ID;

    public readonly string NAME;
    public readonly int CLIENT_ID;
    public readonly bool IS_BOT;
    public Player(string name, int clientID, bool isBot)
    {
        ID = IdHandler.Create(GetType());

        NAME = name;
        CLIENT_ID = clientID;
        IS_BOT = isBot;
    }
}