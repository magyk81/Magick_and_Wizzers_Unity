/* Copyright (C) All Rights Reserved
 * Unauthorized copying of this file, via any medium is prohibited.
 * Proprietary and confidential.
 * Written by Robin Campos <magyk81@gmail.com>, year 2021.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerScript : MonoBehaviour
{
    private static Player[] players;

    private Match match;
    private UX_Match uxMatch;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;

        // For debugging. This data would normally be set from somewhere else.
        Player[] players = new Player[2];
        players[0] = new Player("Brooke", 0, Player.Type.LOCAL_PLAYER);
        players[1] = new Player("Rachel", 1, Player.Type.BOT);
        Match.Players = players;
        Board.Size = 2;
        Chunk.Size = 10;
        
        match = new Match();
        uxMatch = GetComponent<UX_Match>();
        match.InitUX(uxMatch);
    }

    // Update is called once per frame
    void Update()
    {
        match.SkinTickets = uxMatch.SkinTickets;
        match.MainLoop();
        uxMatch.SkinTickets = match.SkinTickets;
    }
}
