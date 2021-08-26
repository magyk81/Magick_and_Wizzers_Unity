using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UX_Piece : MonoBehaviour
{
    private Piece __;
    public Piece _ { get { return __; } }
    [SerializeField]
    private GameObject[] realParts;
    private enum Part {
        ART, FRAME, ATTACK, DEFENSE, LIFE, HOVER, SELECT, TARGET, COUNT };
    [SerializeField]
    private GameObject real;
    private GameObject[] clones = new GameObject[8];
    private GameObject[][] cloneParts = new GameObject[8][];
    private Transform realTra;
    private Transform[] clonesTra = new Transform[8];
    private int boardIdx, fullBoardSize, distBetweenBoards;

    private void OnValidate()
    {
        int partCount = (int) Part.COUNT;
        if (realParts.Length != partCount)
        {
            Debug.LogWarning("Real Parts array size must be " + partCount);
            System.Array.Resize(ref realParts, partCount);
        }
    }

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
            cloneParts[i] = new GameObject[realParts.Length];
            foreach (Transform child in clonesTra[i])
            {
                for (int j = 0; j < realParts.Length; j++)
                {
                    if (realParts[j].name == child.name)
                        cloneParts[i][j] = child.gameObject;
                }
            }
        }

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
        if (Board.CHUNK_SIZE % 2 == 0) { _x += 0.5F; _z += 0.5F; }

        realTra.localPosition = new Vector3(_x, 0.1F, _z);
    }

    public bool IsCollider(Collider collider)
    {
        foreach (GameObject clone in clones)
        {
            if (clone == collider.gameObject) return true;
        }
        if (real == collider.gameObject) return true;
        return false;
    }

    public void Hover()
    {

    }

    public void Unhover()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
