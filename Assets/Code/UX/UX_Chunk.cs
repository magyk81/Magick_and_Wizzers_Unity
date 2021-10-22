/* Copyright (C) All Rights Reserved
 * Unauthorized copying of this file, via any medium is prohibited.
 * Proprietary and confidential.
 * Written by Robin Campos <magyk81@gmail.com>, year 2021.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UX_Chunk : MonoBehaviour
{
    private int boardIdx;
    private Coord pos;

    [SerializeField]
    private UX_Collider baseCollider;
    private UX_Tile[,] tiles;

    private UX_Collider[] colliders;
    private Vector3[] quarterPos;
    private Vector3 quarterSize;
    private Coord[][] tileCollIdxOffset;

    // Start is called before the first frame update
    void Start()
    {

    }

    /// <summary>Called once before the match begins.</summary>
    public void Init(Coord pos, int boardIdx, UX_Tile[,] tiles)
    {
        this.boardIdx = boardIdx;
        this.pos = pos.Copy();
        this.tiles = tiles;

        gameObject.name = "Chunk " + pos;

        // Generate colliders.
        Transform tra = GetComponent<Transform>();
        colliders = new UX_Collider[UX_Match.localPlayerCount];
        for (int i = 0; i < UX_Match.localPlayerCount; i++)
        {
            UX_Collider coll = Instantiate(baseCollider, tra)
                .GetComponent<UX_Collider>();
            coll.GetComponent<Transform>().eulerAngles
                = new Vector3(90, 0, 0);
            coll.Chunk = this;
            coll.gameObject.layer = UX_Tile.LAYER + i;
            coll.gameObject.name = "Collider - view " + i;
            colliders[i] = coll;
        }

        // Set quarter collider postions and size.
        quarterPos = new Vector3[4];
        
        Vector3 traPos = tra.localPosition, offset = tra.localScale / 4F;
        quarterPos[Util.UP_RIGHT - 4]
            = new Vector3(traPos.x + offset.x, 0, traPos.z + offset.y);
        quarterPos[Util.DOWN_LEFT - 4]
            = new Vector3(traPos.x - offset.x, 0, traPos.z - offset.y);
        quarterPos[Util.UP_LEFT - 4]
            = new Vector3(traPos.x - offset.x, 0, traPos.z + offset.y);
        quarterPos[Util.DOWN_RIGHT - 4]
            = new Vector3(traPos.x + offset.x, 0, traPos.z - offset.y);
        quarterSize = new Vector3(
            tiles.GetLength(0) / 2, tiles.GetLength(0) / 2, 1);
        
        // Set tile collider idx offsets.
        tileCollIdxOffset = new Coord[4][];
        int halfSize = tiles.GetLength(0) / 2;
        tileCollIdxOffset[Util.UP_RIGHT - 4] = new Coord[2]
        {
            Coord._(halfSize, halfSize),
            Coord._(0, 0)
        };
        tileCollIdxOffset[Util.UP_LEFT - 4] = new Coord[2]
        {
            Coord._(0, halfSize),
            Coord._(-halfSize, 0)
        };
        tileCollIdxOffset[Util.DOWN_RIGHT - 4] = new Coord[2]
        {
            Coord._(halfSize, 0),
            Coord._(0, -halfSize)
        };
        tileCollIdxOffset[Util.DOWN_LEFT - 4] = new Coord[2]
        {
            Coord._(0, 0),
            Coord._(-halfSize, -halfSize)
        };
    }

    /// <summary>Disables the large collider and enables the 4
    /// quarter-sized colliders to take its place.</summary>
    public void SetQuarterColliders(UX_Collider[] quarterColls,
        int localPlayerIdx)
    {
        colliders[localPlayerIdx].Disable();
        for (int i = 0; i < 4; i++)
        {
            quarterColls[i].Set(quarterPos[i], quarterSize);
            quarterColls[i].Chunk = this;
            quarterColls[i].Enable();
        }
    }

    /// <summary>Called when the quarter-sized collider has been disabled. This
    /// method enables the many tile-sized colliders to take its place.
    /// </summary>
    public void SetTileColliders(int quarter, UX_Collider[,] tileColls,
        int localPlayerIdx)
    {
        colliders[localPlayerIdx].Disable();
        Coord[] idxOffset = tileCollIdxOffset[quarter - 4];
        for (int i = idxOffset[0].X, _i = 0;
            i < tiles.GetLength(0) + idxOffset[1].X; i++, _i++)
        {
            for (int j = idxOffset[0].Z, _j = 0;
                j < tiles.GetLength(1) + idxOffset[1].Z; j++, _j++)
            {
                tileColls[_i, _j].Tile = tiles[i, j];
                tileColls[_i, _j].Chunk = this;
                tileColls[_i, _j].Enable();
            }
        }
    }

    void Update()
    {
    }
}
