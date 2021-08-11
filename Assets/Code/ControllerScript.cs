using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerScript : MonoBehaviour
{
    private Match match;
    private UX_Match uxMatch;

    [SerializeField]
    private GameObject BASE_CHUNK, BASE_PIECE;

    // Start is called before the first frame update
    void Start()
    {
        uxMatch = new UX_Match(BASE_CHUNK);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (match  == null) match = new Match(uxMatch);
        }
    }
}
