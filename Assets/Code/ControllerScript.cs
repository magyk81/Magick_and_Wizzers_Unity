using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerScript : MonoBehaviour
{
    private Match match;
    private UX_Match uxMatch;
    private Dictionary<Player, Gamepad> gamepadBinding
        = new Dictionary<Player, Gamepad>();
    private Dictionary<Player, CameraScript> cameraBinding
        = new Dictionary<Player, CameraScript>();

    [SerializeField]
    private CameraScript BASE_CAMERA;
    [SerializeField]
    private Canvas BASE_CANVAS;
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

        // Set up players
        Player[] players = new Player[playerCount];
        players[0] = new Player("Brooke", Player.Type.LOCAL_PLAYER);
        players[1] = new Player("Rachel", Player.Type.BOT);

        uxMatch = new UX_Match(BASE_CHUNK, BASE_PIECE, BASE_WAYPOINT,
            BASE_CAMERA, BASE_CANVAS);
        
        match = new Match(uxMatch, players);
    }

    // Update is called once per frame
    void Update()
    {
        match.Update();
    }
}
