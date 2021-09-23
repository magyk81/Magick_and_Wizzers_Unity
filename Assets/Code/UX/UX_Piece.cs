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
    private bool[] hovered, selected;
    private Transform tra;
    private Material artMat;
    private readonly static float PIECE_LIFT_DIST = 0.1F;
    public readonly static int pieceLayer = 7;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnDestroy()
    {
        if (artMat != null) Destroy(artMat);
    }

    public void Init(Piece piece)
    {
        hovered = new bool[UX_Match.localPlayerCount];
        selected = new bool[UX_Match.localPlayerCount];

        tra = GetComponent<Transform>();

        if (piece.pieceType == Piece.Type.MASTER)
        {
            lifeBar = new GameObject[UX_Match.localPlayerCount];
            for (int i = 0; i < UX_Match.localPlayerCount; i++)
            {
                lifeBar[i] = Instantiate(lifeBar_base, tra);
                lifeBar[i].layer = pieceLayer + i;
                lifeBar[i].name = "Life Bar - view " + i;
            }
            SetActive(lifeBar, true);
        }
        else if (piece.pieceType == Piece.Type.CREATURE)
        {
            attackBar = new GameObject[UX_Match.localPlayerCount];
            for (int i = 0; i < UX_Match.localPlayerCount; i++)
            {
                attackBar[i] = Instantiate(attackBar_base, tra);
                attackBar[i].layer = pieceLayer + i;
                attackBar[i].name = "Attack Bar - view " + i;
            }
            SetActive(attackBar, true);
            defenseBar = new GameObject[UX_Match.localPlayerCount];
            for (int i = 0; i < UX_Match.localPlayerCount; i++)
            {
                defenseBar[i] = Instantiate(defenseBar_base, tra);
                defenseBar[i].layer = pieceLayer + i;
                defenseBar[i].name = "Defense Bar - view " + i;
            }
            SetActive(defenseBar, true);
        }

        art = new GameObject[UX_Match.localPlayerCount];
        artMat = new Material(art_base.GetComponent<MeshRenderer>().material);
        artMat.name = "Piece Art Material - " + piece.Name;
        artMat.mainTexture = piece.Art;
        for (int i = 0; i < UX_Match.localPlayerCount; i++)
        {
            art[i] = Instantiate(art_base, tra);
            art[i].GetComponent<MeshRenderer>().material = artMat;
            art[i].layer = pieceLayer + i;
            art[i].name = "Art - view " + i;
        }
        SetActive(art, true);
        frame = new GameObject[UX_Match.localPlayerCount];
        for (int i = 0; i < UX_Match.localPlayerCount; i++)
        {
            frame[i] = Instantiate(frame_base, tra);
            frame[i].layer = pieceLayer + i;
            frame[i].name = "Frame - view " + i;
        }
        SetActive(frame, true);

        hover = new GameObject[UX_Match.localPlayerCount];
        for (int i = 0; i < UX_Match.localPlayerCount; i++)
        {
            hover[i] = Instantiate(hover_base, tra);
            hover[i].layer = pieceLayer + i;
            hover[i].name = "Hover Crown - view " + i;
        }
        SetActive(hover, false);
        select = new GameObject[UX_Match.localPlayerCount];
        for (int i = 0; i < UX_Match.localPlayerCount; i++)
        {
            select[i] = Instantiate(select_base, tra);
            select[i].layer = pieceLayer + i;
            select[i].name = "Select Crown - view " + i;
        }
        SetActive(select, false);
        target = new GameObject[UX_Match.localPlayerCount];
        for (int i = 0; i < UX_Match.localPlayerCount; i++)
        {
            target[i] = Instantiate(target_base, tra);
            target[i].layer = pieceLayer + i;
            target[i].name = "Target Crown - view " + i;
        }
        SetActive(target, false);

        if (piece.pieceType == Piece.Type.MASTER)
            gameObject.name = "Master - " + piece.Name;
        else gameObject.name = "Piece - " + piece.Name;
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
            tra.localPosition = new Vector3(pos.x, PIECE_LIFT_DIST, pos.z);
            gameObject.SetActive(true);
        }
    }

    public void UpdatePosition()
    {
        // float _x = _.X - (Board.CHUNK_SIZE / 2),
        //     _z = _.Z - (Board.CHUNK_SIZE / 2),
        //     __x = _x, __z = _z;

        // _x += _.BoardIdx * distBetweenBoards * Board.CHUNK_SIZE;
            
        // for (int i = 0; i < 8; i++)
        // {
        //     if (Board.CHUNK_SIZE % 2 == 0) { _x += 0.5F; _z += 0.5F; }
        //     if (i == Util.UP || i == Util.UP_LEFT
        //         || i == Util.UP_RIGHT) _z += fullBoardSize;
        //     else if (i == Util.DOWN || i == Util.DOWN_LEFT
        //         || i == Util.DOWN_RIGHT) _z -= fullBoardSize;
        //     if (i == Util.RIGHT || i == Util.UP_RIGHT
        //         || i == Util.DOWN_RIGHT) _x += fullBoardSize;
        //     else if (i == Util.LEFT || i == Util.UP_LEFT
        //         || i == Util.DOWN_LEFT) _x -= fullBoardSize;
                
        //     clonesTra[i].localPosition = new Vector3(_x, LIFT_DIST, _z);

        //     _x = __x;
        //     _z = __z;
        // }
        // if (Board.CHUNK_SIZE % 2 == 0) { _x += 0.5F; _z += 0.5F; }

        // realTra.localPosition = new Vector3(_x, LIFT_DIST, _z);
    }

    // public bool IsCollider(Collider collider)
    // {
    //     if (frame == collider.gameObject) return true;
    //     foreach (GameObject[] clonePart in cloneParts)
    //     {
    //         if (clonePart[(int) Part.FRAME] == collider.gameObject)
    //             return true;
    //     }
    //     return false;
    // }

    // public void Hover()
    // {
    //     if (hovered) return;
    //     SetActive(Part.HOVER, true);
    //     SetActive(Part.FRAME, false);
    //     hovered = true;
    // }

    // public void Unhover()
    // {
    //     if (!hovered) return;
    //     SetActive(Part.HOVER, false);
    //     SetActive(Part.FRAME, true);
    //     hovered = false;
    // }

    // public void Select()
    // {
    //     if (selected) return;
    //     SetActive(Part.SELECT, true);
    //     selected = true;
    // }
    // public void Unselect()
    // {
    //     if (!selected) return;
    //     SetActive(Part.SELECT, false);
    //     selected = false;
    // }

    // private void SetActive(Part part, bool active)
    // {
    //     if (part == Part.FRAME)
    //         frame.GetComponent<MeshRenderer>().enabled = active;
    //     else if (part == Part.HOVER) hover.SetActive(active);
    //     else if (part == Part.SELECT) select.SetActive(active);
    //     else if (part == Part.TARGET) target.SetActive(active);
    //     foreach (GameObject[] clonePart in cloneParts)
    //     {
    //         clonePart[(int) part].SetActive(active);
    //     }
    // }

    // Update is called once per frame
    void Update()
    {
        
    }
}
