/* Copyright (C) All Rights Reserved
 * Unauthorized copying of this file, via any medium is prohibited.
 * Proprietary and confidential.
 * Written by Robin Campos <magyk81@gmail.com>, year 2021.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UX_Waypoint : MonoBehaviour
{
    [SerializeField]
    private Material opaqueRedMat, semitransRedMat, opaqueYellowMat,
        semitransYellowMat;
    private Material rendMaterial {
        set {
            if (rend.material != value) rend.material = value;
        }
    }

    [SerializeField]
    private Mesh meshForTile, meshForPiece;
    private Mesh rendMesh {
        set {
            if (filter.mesh != value) filter.mesh = value;
        }
    }

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

    private Transform tra, pieceTra;
    private Renderer rend;
    private MeshFilter filter;

    public void Show(bool opaque, bool hovered)
    {
        if (opaque)
        {
            if (hovered) rendMaterial = opaqueYellowMat;
            else rendMaterial = opaqueRedMat;
        }
        else
        {
            if (hovered) rendMaterial = semitransYellowMat;
            else rendMaterial = semitransRedMat;
        }
        UpdatePosToPiece();
        gameObject.SetActive(true);
    }
    public void Hide() { gameObject.SetActive(false); }
    public void SetPos(Vector3 pos) { tra.localPosition = pos; }
    public void SetPiece(UX_Piece piece)
    {
        if (piece != null)
        {
            this.pieceTra = piece.GetComponent<Transform>();
            rendMesh = meshForPiece;
            UpdatePosToPiece();
        }
        else
        {
            pieceTra = null;
            rendMesh = meshForTile;
        }
    }

    // Start is called before the first frame update
    private void Start()
    {
        tra = GetComponent<Transform>();
        rend = GetComponent<Renderer>();
        filter = GetComponent<MeshFilter>();
        Hide();
    }

    // Update is called once per frame
    private void Update()
    {
        UpdatePosToPiece();
    }

    private void UpdatePosToPiece()
    {
        if (pieceTra != null) SetPos(pieceTra.localPosition);
    }
}
