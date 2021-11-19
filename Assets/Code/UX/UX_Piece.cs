/* Copyright (C) All Rights Reserved
 * Unauthorized copying of this file, via any medium is prohibited.
 * Proprietary and confidential.
 * Written by Robin Campos <magyk81@gmail.com>, year 2021.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UX_Piece : MonoBehaviour
{
    [SerializeField]
    private GameObject art_base, frame_base, attackBar_base, defenseBar_base,
        lifeBar_base, hover_base, select_base, target_base;
    private GameObject[] art, frame, attackBar, defenseBar, lifeBar, hover,
        select, target;
    private enum Part {
        ART, FRAME, ATTACK, DEFENSE, LIFE, HOVER, SELECT, TARGET, COUNT };
    private Transform tra;
    private Material artMat;
    private UX_Tile[] waypoints = new UX_Tile[Piece.MAX_WAYPOINTS];
    public UX_Tile[] Waypoints { get { return waypoints; } }
    private readonly static float LIFT_DIST = 0.1F;
    public readonly static int LAYER = 6;

    private int playerID; public int PlayerID { get { return playerID; } }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Cleans memory if/when application is stopped.
    private void OnDestroy()
    {
        if (artMat != null) Destroy(artMat);
    }

    /// <summary>Called once before the match begins.</summary>
    public void Init(SignalFromHost signal, string pieceName, int layerCount)
    {
        tra = GetComponent<Transform>();

        if (signal.PieceType == (int) Piece.Type.MASTER)
        {
            lifeBar = new GameObject[layerCount];
            for (int i = 0; i < layerCount; i++)
            {
                lifeBar[i] = Instantiate(lifeBar_base, tra);
                lifeBar[i].layer = LAYER + i;
                lifeBar[i].name = "Life Bar - view " + i;
            }
            SetActive(lifeBar, true);
        }
        else if (signal.PieceType == (int) Piece.Type.CREATURE)
        {
            attackBar = new GameObject[layerCount];
            for (int i = 0; i < layerCount; i++)
            {
                attackBar[i] = Instantiate(attackBar_base, tra);
                attackBar[i].layer = LAYER + i;
                attackBar[i].name = "Attack Bar - view " + i;
            }
            SetActive(attackBar, true);
            defenseBar = new GameObject[layerCount];
            for (int i = 0; i < layerCount; i++)
            {
                defenseBar[i] = Instantiate(defenseBar_base, tra);
                defenseBar[i].layer = LAYER + i;
                defenseBar[i].name = "Defense Bar - view " + i;
            }
            SetActive(defenseBar, true);
        }

        art = new GameObject[layerCount];
        artMat = new Material(
            art_base.GetComponent<MeshRenderer>().sharedMaterial);
        artMat.name = "Piece Art Material - " + pieceName;
        // artMat.mainTexture = piece.Art;
        for (int i = 0; i < layerCount; i++)
        {
            art[i] = Instantiate(art_base, tra);
            art[i].GetComponent<MeshRenderer>().material = artMat;
            art[i].layer = LAYER + i;
            art[i].name = "Art - view " + i;
        }
        SetActive(art, true);
        frame = new GameObject[layerCount];
        for (int i = 0; i < layerCount; i++)
        {
            frame[i] = Instantiate(frame_base, tra);
            frame[i].layer = LAYER + i;

            // Set collider script info.
            frame[i].GetComponent<UX_Collider>().Piece = this;

            frame[i].name = "Frame - view " + i;
        }
        SetActive(frame, true);

        hover = new GameObject[layerCount];
        for (int i = 0; i < layerCount; i++)
        {
            hover[i] = Instantiate(hover_base, tra);
            hover[i].layer = LAYER + i;
            hover[i].name = "Hover Crown - view " + i;
        }
        SetActive(hover, false);
        select = new GameObject[layerCount];
        for (int i = 0; i < layerCount; i++)
        {
            select[i] = Instantiate(select_base, tra);
            select[i].layer = LAYER + i;
            select[i].name = "Select Crown - view " + i;
        }
        SetActive(select, false);
        target = new GameObject[layerCount];
        for (int i = 0; i < layerCount; i++)
        {
            target[i] = Instantiate(target_base, tra);
            target[i].layer = LAYER + i;
            target[i].name = "Target Crown - view " + i;
        }
        SetActive(target, false);

        if (signal.PieceType == (int) Piece.Type.MASTER)
            gameObject.name = "Master - " + pieceName;
        else gameObject.name = "Piece - " + pieceName;
    }

    private void SetActive(GameObject[] obj, bool active)
    {
        if (obj != null)
        {
            for (int i = 0; i < obj.Length; i++) { obj[i].SetActive(active); }
        }
    }

    public void SetPos(UX_Tile a, UX_Tile b, float lerp)
    {
        if (a == null || b == null) gameObject.SetActive(false);
        else
        {
            Vector3 pos = Vector3.Lerp(a.UX_Pos, b.UX_Pos, lerp);
            tra.localPosition = new Vector3(pos.x, LIFT_DIST, pos.z);
            gameObject.SetActive(true);
        }
    }

    public void UpdateWaypoints(UX_Tile[] tiles)
    {
        for (int i = 0; i < waypoints.Length; i++)
        {
            waypoints[i] = tiles[i];
        }
    }

    public void Hover(int localPlayerIdx)
    {
        hover[localPlayerIdx].SetActive(true);

        // Not using SetActive(false) because the collider needs to work.
        frame[localPlayerIdx].GetComponent<MeshRenderer>().enabled = false;
    }

    public void Unhover(int localPlayerIdx)
    {
        hover[localPlayerIdx].SetActive(false);
        frame[localPlayerIdx].GetComponent<MeshRenderer>().enabled = true;
    }

    public void Select(int localPlayerCount)
    {
        select[localPlayerCount].SetActive(true);
    }

    public void Unselect(int localPlayerCount)
    {
        select[localPlayerCount].SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
