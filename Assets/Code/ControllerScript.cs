using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerScript : MonoBehaviour
{
    private Match match;
    private UX_Match uxMatch;
    private Dictionary<int, Player> gamepadBinding
        = new Dictionary<int, Player>();
    private Dictionary<Player, CameraScript> cameraBinding
        = new Dictionary<Player, CameraScript>();

    [SerializeField]
    private CameraScript BASE_CAMERA;
    [SerializeField]
    private GameObject BASE_WAYPOINT;
    [SerializeField]
    private UX_Chunk BASE_CHUNK;
    [SerializeField]
    private UX_Piece BASE_PIECE;
    [SerializeField]
    private int playerCount;
    public static readonly int MAX_GAMEPADS = 6;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;

        // The contents in this region should have been set while the user(s)
        // was in the lobby prior to the match starting.
        #region lobby setup

        // Set up players
        Player[] players = new Player[playerCount];
        players[0] = new Player("Haylee", Player.Type.LOCAL_PLAYER);
        players[1] = new Player("Brooke", Player.Type.BOT);

        // Set up gamepads
        Gamepad[] gamepads = new Gamepad[MAX_GAMEPADS];
        gamepads[0] = new Gamepad(true);

        // Set up gamepad-to-player binding
        gamepadBinding.Add(0, players[0]);

        // Set up player-to-camera binding
        cameraBinding.Add(players[0], BASE_CAMERA);

        #endregion

        uxMatch = new UX_Match(gamepads,
            BASE_CHUNK, BASE_PIECE, BASE_WAYPOINT, BASE_CAMERA);
        
        match = new Match(uxMatch, gamepadBinding, cameraBinding, players);
    }

    // Update is called once per frame
    void Update()
    {
        match.Update();
    }
}
