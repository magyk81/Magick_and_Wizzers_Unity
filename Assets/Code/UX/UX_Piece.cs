using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UX_Piece : MonoBehaviour
{
    private Piece _;
    [SerializeField]
    private GameObject real;
    private GameObject[] clones = new GameObject[8];
    private Transform realTra;
    private Transform[] clonesTra = new Transform[8];
    private int boardIdx, fullBoardSize, distBetweenBoards;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Init(Piece _,
            int fullBoardSize, int distBetweenBoards)
    {
        this._ = _;
        this.fullBoardSize = fullBoardSize;
        this.distBetweenBoards = distBetweenBoards;

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

        gameObject.SetActive(true);
    }

    public void UpdatePosition()
    {
        float _x = _.X - (Board.CHUNK_SIZE / 2),
            _z = _.Z - (Board.CHUNK_SIZE / 2),
            __x = _x, __z = _z;

        _x += _.BoardIdx * distBetweenBoards * Board.CHUNK_SIZE;
            
        for (int i = 0; i < 8; i++)
        {
            if (Board.CHUNK_SIZE % 2 == 0) { _x += 0.5F; _z += 0.5F; }
            if (i == Util.UP || i == Util.UP_LEFT
                || i == Util.UP_RIGHT) _z += fullBoardSize;
            else if (i == Util.DOWN || i == Util.DOWN_LEFT
                || i == Util.DOWN_RIGHT) _z -= fullBoardSize;
            if (i == Util.RIGHT || i == Util.UP_RIGHT
                || i == Util.DOWN_RIGHT) _x += fullBoardSize;
            else if (i == Util.LEFT || i == Util.UP_LEFT
                || i == Util.DOWN_LEFT) _x -= fullBoardSize;
                
            clonesTra[i].localPosition = new Vector3(_x, 0.1F, _z);

            _x = __x;
            _z = __z;
        }

        realTra.localPosition = new Vector3(_x, 0.1F, _z);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
