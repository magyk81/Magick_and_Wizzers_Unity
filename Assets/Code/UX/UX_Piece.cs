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

    private UX_Tile[] waypointTiles = new UX_Tile[Piece.MAX_WAYPOINTS];
    private UX_Piece[] waypointPieces = new UX_Piece[Piece.MAX_WAYPOINTS];
    public UX_Tile[] WaypointTiles { get { return waypointTiles; } }
    public UX_Piece[] WaypointPieces { get { return waypointPieces; } }
    private readonly static float LIFT_DIST = 0.1F;
    public readonly static int LAYER = 6;

    private UX_Piece real = null;
    private UX_Piece[] uxAll = null;
    public UX_Piece[] UX_All {
        get {
                if (real != null) return real.uxAll;
                return uxAll;
            }
    }

    private int pieceID; public int PieceID { get { return pieceID; } }
    private int boardID; public int BoardID { get { return boardID; } }
    private int playerID; public int PlayerID { get { return playerID; } }

    private List<int> hand = new List<int>();
    public Card[] Hand
    {
        get
        {
            Card[] hand = new Card[this.hand.Count];
            for (int i = 0; i < hand.Length; i++)
            { hand[i] = Card.friend_cards[this.hand[i]]; }
            return hand;
        }
    }

    // Cleans memory if/when application is stopped.
    private void OnDestroy()
    {
        if (artMat != null) Destroy(artMat);
    }

    /// <summary>Called once before the match begins.</summary>
    public void Init(SignalFromHost signal, string pieceName, int layerCount)
    {
        pieceID = signal.PieceID;
        boardID = signal.BoardID;
        uxAll = new UX_Piece[9];

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
        Texture cardArt = signal.CardID == -1 ? null
            : Card.friend_cards[signal.CardID].Art;
        if (cardArt != null) artMat.mainTexture = cardArt;
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

    public void SetReal()
    {
        real = this;
        uxAll[0] = this;
    }
    public void AddClone(UX_Piece piece, int cloneIdx)
    {
        piece.real = this;
        uxAll[cloneIdx] = piece;
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

    public void UpdateWaypoints(UX_Tile[] tiles, UX_Piece[] pieces)
    {
        for (int i = 0; i < waypointTiles.Length; i++)
        {
            waypointTiles[i] = tiles[i];
        }
        for (int i = 0; i < waypointPieces.Length; i++)
        {
            waypointPieces[i] = pieces[i];
        }
    }

    public bool HasSameWaypoints(UX_Piece piece)
    {
        for (int i = 0; i < waypointTiles.Length; i++)
        {
            if (waypointTiles[i] == null)
            {
                if (piece.waypointTiles[i] != null) return false;
            }
            else if (piece.waypointTiles[i] == null)
            {
                if (waypointTiles[i] != null) return false;
            }
            else if (waypointTiles[i].Pos != piece.waypointTiles[i].Pos)
                return false;
        }
        for (int i = 0; i < waypointPieces.Length; i++)
        {
            if (waypointPieces[i] == null)
            {
                if (piece.waypointPieces[i] == null) return false;
            }
            else if (piece.waypointPieces[i] == null)
            {
                if (waypointPieces[i] != null) return false;
            }
            else if (waypointPieces[i].PieceID
                != piece.waypointPieces[i].PieceID) return false;
        }
        return true;
    }

    // Called from UX_Player, so apply to every clone.
    public void Hover(int localPlayerID)
    {
        foreach (UX_Piece piece in UX_All)
        {
            piece.hover[localPlayerID].SetActive(true);

            // Not using SetActive(false) because the collider needs to work.
            piece.frame[localPlayerID].GetComponent<MeshRenderer>()
                .enabled = false;
        }
        
    }

    // Called from UX_Player, so apply to every clone.
    public void Unhover(int localPlayerID)
    {
        foreach (UX_Piece piece in UX_All)
        {
            piece.hover[localPlayerID].SetActive(false);
            piece.frame[localPlayerID].GetComponent<MeshRenderer>()
                .enabled = true;
        }
        
    }

    // Called from UX_Player, so apply to every clone.
    public void Select(int localPlayerCount)
    {
        foreach (UX_Piece piece in UX_All)
        { piece.select[localPlayerCount].SetActive(true); }
    }

    // Called from UX_Player, so apply to every clone.
    public void Unselect(int localPlayerCount)
    {
        foreach (UX_Piece piece in UX_All)
        { piece.select[localPlayerCount].SetActive(false); }
    }

    public void AddCard(int cardID)
    {
        hand.Add(cardID);
    }

    public void RemoveCard(int cardID)
    {
        hand.Remove(cardID);
    }
}
