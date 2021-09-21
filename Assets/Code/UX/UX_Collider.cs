using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UX_Collider : MonoBehaviour
{
    private UX_Tile tile;
    private int quarter = -1;
    private UX_Chunk chunk;
    private MeshCollider coll;

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
