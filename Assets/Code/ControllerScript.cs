using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerScript : MonoBehaviour
{
    private static Player[] players;

    // [SerializeField]
    // private GameObject uxPlayerParent, uxBoardParent, uxPieceParent;

    private Match match;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;

        // For debugging. This data would normally be set somewhere else.
        Player[] players = new Player[2];
        players[0] = new Player("Brooke", 0, Player.Type.LOCAL_PLAYER);
        players[1] = new Player("Rachel", 1, Player.Type.BOT);
        Match.Players = players;
        Board.Size = 1;
        Chunk.Size = 10;
        
        match = new Match();
        match.InitUX(GetComponent<UX_Match>());
    }

    // Update is called once per frame
    void Update()
    {
        match.MainLoop();
    }
}
