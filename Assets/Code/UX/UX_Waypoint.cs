using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UX_Waypoint : MonoBehaviour
{
    private UX_Tile tile;
    public UX_Tile Tile
    {
        set
        {
            tile = value;
            tra.localPosition = tile.UX_Pos;
        }
        get { return tile; }
    }

    private Transform tra;

    // Start is called before the first frame update
    void Start()
    {
        tra = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
