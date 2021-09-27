using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UX_Collider : MonoBehaviour
{
    private UX_Tile tile;
    private int quarter = -1;
    private UX_Chunk chunk;
    private UX_Piece piece;
    private MeshCollider coll;

    public UX_Piece Piece
    {
        set { if (piece == null) piece = value; }
        get { return piece; }
    }

    // Start is called before the first frame update
    void Start()
    {
        coll = GetComponent<MeshCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
