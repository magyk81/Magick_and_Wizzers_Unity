using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerScript : MonoBehaviour
{
    private Match match;
    private UX_Match uxMatch;

    [SerializeField]
    private GameObject BASE_CHUNK, BASE_PIECE, BASE_CAMERA;

    // Start is called before the first frame update
    void Start()
    {
        uxMatch = new UX_Match(BASE_CHUNK, BASE_CAMERA);
    }

    // Update is called once per frame
    void Update()
    {
        // For debugging
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (match  == null) match = new Match(uxMatch, 2);
        }
    }
}
