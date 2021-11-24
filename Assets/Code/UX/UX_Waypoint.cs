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
    private Renderer rend;

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
        gameObject.SetActive(true);
    }
    public void Hide() { gameObject.SetActive(false); }
    public void SetPos(Vector3 pos) { tra.localPosition = pos; }

    // Start is called before the first frame update
    void Start()
    {
        tra = GetComponent<Transform>();
        rend = GetComponent<Renderer>();
        Hide();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
