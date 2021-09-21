using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UX_Piece : MonoBehaviour
{
    private Piece __;
    public Piece _ { get { return __; } }
    [SerializeField]
    private GameObject art, frame, attackBar, defenseBar, lifeBar, hover,
        select, target;
    private enum Part {
        ART, FRAME, ATTACK, DEFENSE, LIFE, HOVER, SELECT, TARGET, COUNT };
    [SerializeField]
    private GameObject real;
    private GameObject[] clones = new GameObject[8];
    private GameObject[][] cloneParts = new GameObject[8][];
    private Transform realTra;
    private Transform[] clonesTra = new Transform[8];
    private int boardIdx, fullBoardSize, distBetweenBoards;
    private bool hovered, selected;
    private readonly static float LIFT_DIST = 0.1F;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Init(Piece __,
            int fullBoardSize, int distBetweenBoards)
    {
        this.__ = __;
        this.fullBoardSize = fullBoardSize;
        this.distBetweenBoards = distBetweenBoards;

        if (__.GetType() != typeof(Master))
        {
            lifeBar.SetActive(false);
        }
        attackBar.SetActive(false);
        defenseBar.SetActive(false);
        hover.SetActive(false);
        select.SetActive(false);
        target.SetActive(false);

        gameObject.name = "Piece - " + _.Name;
        Transform tra = GetComponent<Transform>();

        for (int i = 0; i < 8; i++)
        {
            clones[i] = GameObject.Instantiate(real, tra);
            clones[i].name = "Clone Piece - " + Util.DirToString(i);
            clonesTra[i] = clones[i].GetComponent<Transform>();
            clones[i].SetActive(true);
        }
        realTra = real.GetComponent<Transform>();

        for (int i = 0; i < cloneParts.Length; i++)
        {
            cloneParts[i] = new GameObject[(int) Part.COUNT];
            foreach (Transform child in clonesTra[i])
            {
                for (int j = 0; j < (int) Part.COUNT; j++)
                {
                    if (child.name == art.name)
                        cloneParts[i][(int) Part.ART] = child.gameObject;
                    else if (child.name == frame.name)
                        cloneParts[i][(int) Part.FRAME] = child.gameObject;
                    else if (child.name == attackBar.name)
                        cloneParts[i][(int) Part.ATTACK] = child.gameObject;
                    else if (child.name == defenseBar.name)
                        cloneParts[i][(int) Part.DEFENSE] = child.gameObject;
                    else if (child.name == lifeBar.name)
                        cloneParts[i][(int) Part.LIFE] = child.gameObject;
                    else if (child.name == hover.name)
                        cloneParts[i][(int) Part.HOVER] = child.gameObject;
                    else if (child.name == select.name)
                        cloneParts[i][(int) Part.SELECT] = child.gameObject;
                    else if (child.name == target.name)
                        cloneParts[i][(int) Part.TARGET] = child.gameObject;
                }
            }
        }

        gameObject.SetActive(true);
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

    public bool IsCollider(Collider collider)
    {
        if (frame == collider.gameObject) return true;
        foreach (GameObject[] clonePart in cloneParts)
        {
            if (clonePart[(int) Part.FRAME] == collider.gameObject)
                return true;
        }
        return false;
    }

    public void Hover()
    {
        if (hovered) return;
        SetActive(Part.HOVER, true);
        SetActive(Part.FRAME, false);
        hovered = true;
    }

    public void Unhover()
    {
        if (!hovered) return;
        SetActive(Part.HOVER, false);
        SetActive(Part.FRAME, true);
        hovered = false;
    }

    public void Select()
    {
        if (selected) return;
        SetActive(Part.SELECT, true);
        selected = true;
    }
    public void Unselect()
    {
        if (!selected) return;
        SetActive(Part.SELECT, false);
        selected = false;
    }

    private void SetActive(Part part, bool active)
    {
        if (part == Part.FRAME)
            frame.GetComponent<MeshRenderer>().enabled = active;
        else if (part == Part.HOVER) hover.SetActive(active);
        else if (part == Part.SELECT) select.SetActive(active);
        else if (part == Part.TARGET) target.SetActive(active);
        foreach (GameObject[] clonePart in cloneParts)
        {
            clonePart[(int) part].SetActive(active);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
