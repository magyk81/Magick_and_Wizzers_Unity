/* Copyright (C) All Rights Reserved
 * Unauthorized copying of this file, via any medium is prohibited.
 * Proprietary and confidential.
 * Written by Robin Campos <magyk81@gmail.com>, year 2021.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UX_Collider : MonoBehaviour
{
    private Transform tra;
    private UX_Tile tile;
    private int quarter = -1;
    private UX_Chunk chunk;
    private UX_Piece piece;
    private BoxCollider coll;

    public UX_Tile Tile
    {
        set
        {
            tile = value;
            tra.localPosition = tile.UX_Pos;
        }
        get { return tile; }
    }

    public int Quarter
    {
        set { if (quarter == -1) { quarter = value; SetType(Type.QUARTER); } }
        get { return quarter; }
    }

    public UX_Chunk Chunk
    {
        set { SetType(Type.CHUNK); chunk = value; }
        get { return chunk; }
    }

    public UX_Piece Piece
    {
        set { if (piece == null) piece = value; SetType(Type.PIECE); }
        get { return piece; }
    }

    public enum Type { NONE, TILE, QUARTER, CHUNK, PIECE }

    private Type type = Type.NONE;
    private void SetType(Type type)
    {
        if (type == Type.PIECE) this.type = type;
        else if (type == Type.QUARTER)
        {
            if (this.type != Type.PIECE && this.type != Type.TILE)
                this.type = type;
        }
        else if (type == Type.CHUNK)
        {
            if (this.type != Type.PIECE && this.type != Type.TILE
                && this.type != Type.QUARTER)
                this.type = type;
        }
    }
    public void SetTypeTile() { type = Type.TILE; }
    public bool IsType(Type type) { return this.type == type; }
    public void Enable()
    {
        coll.enabled = true;
    }
    public void Disable()
    {
        coll.enabled = false;
    }

    public void Set(Vector3 position, Vector3 scale)
    {
        tra.localPosition = position;
        tra.localScale = scale;
    }

    // Start is called before the first frame update
    void Start()
    {
        tra = GetComponent<Transform>();
        coll = GetComponent<BoxCollider>();
        if (IsType(Type.QUARTER) || IsType(Type.TILE)) Disable();
        else
        {
            Disable();
            Enable();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
