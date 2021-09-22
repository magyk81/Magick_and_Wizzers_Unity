using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UX_Piece : MonoBehaviour
{
    private static class InitInfo
    {
        public static int playerCount;
    }
    public static int PlayerCount {
        set { InitInfo.playerCount = value; } }
    private Piece __;
    public Piece _ { get { return __; } }
    [SerializeField]
    private GameObject art, frame, attackBar, defenseBar, lifeBar, hover,
        select, target;
    private enum Part {
        ART, FRAME, ATTACK, DEFENSE, LIFE, HOVER, SELECT, TARGET, COUNT };
    private int boardIdx, fullBoardSize, distBetweenBoards;
    private bool[] hovered, selected;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Init()
    {
        hovered = new bool[InitInfo.playerCount];
        selected = new bool[InitInfo.playerCount];

        // if (__.GetType() != typeof(Master))
        // {
        //     lifeBar.SetActive(false);
        // }
        attackBar.SetActive(false);
        defenseBar.SetActive(false);
        hover.SetActive(false);
        select.SetActive(false);
        target.SetActive(false);

        gameObject.name = "Piece - " + _.Name;
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
