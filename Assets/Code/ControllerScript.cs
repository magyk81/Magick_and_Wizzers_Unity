using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerScript : MonoBehaviour
{
    private Match match;
    private UX_Match uxMatch;

    [SerializeField]
    private GameObject BASE_WAYPOINT, BASE_CAMERA;
    [SerializeField]
    private UX_Chunk BASE_CHUNK;
    [SerializeField]
    private UX_Piece BASE_PIECE;
    [SerializeField]
    private int playerCount;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;

        uxMatch = new UX_Match(
            BASE_CHUNK, BASE_PIECE, BASE_WAYPOINT, BASE_CAMERA);
    }

    // Update is called once per frame
    void Update()
    {
        // For debugging
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (match  == null) match = new Match(uxMatch, playerCount);
        }
    }
}
