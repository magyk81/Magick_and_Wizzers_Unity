using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UX_Chunk : MonoBehaviour
{
    private int boardIdx, x, z;
    [SerializeField]
    private GameObject real;
    private GameObject[] clones = new GameObject[8];

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public float GetEdge(int dir)
    {
        float chunkSizeHalf = ((float) Board.CHUNK_SIZE) / 2F;
        Vector3 pos = real.GetComponent<Transform>().localPosition;
        if (dir == Util.UP) return pos.z + chunkSizeHalf;
        else if (dir == Util.DOWN) return pos.z - chunkSizeHalf;
        else if (dir == Util.LEFT) return pos.x - chunkSizeHalf;
        else if (dir == Util.RIGHT) return pos.x + chunkSizeHalf;
        return 0;
    }

    public void Init(int fullBoardSize, int distBetweenBoards,
            int boardIdx, int x, int z)
    {
        this.boardIdx = boardIdx;
        this.x = x;
        this.z = z;

        clones = new GameObject[8];
        gameObject.name = "Chunk [" + x + ", " + z + "]";
        Transform tra = GetComponent<Transform>();

        SetupChunk(real, fullBoardSize, distBetweenBoards);
        for (int i = 0; i < 8; i++)
        {
            clones[i] = GameObject.Instantiate(real, tra);
            SetupChunk(clones[i],
                fullBoardSize, distBetweenBoards, i);
        }
    }

    private void SetupChunk(GameObject obj,
        int fullBoardSize, int distBetweenBoards, int cloneIdx = -1)
    {
        obj.name = (cloneIdx >= 0)
            ? ("Chunk Clone - " + Util.DirToString(cloneIdx))
            : "Real Chunk";
        Transform childTra = obj.GetComponent<Transform>();

        childTra.localScale = new Vector3(
            Board.CHUNK_SIZE, Board.CHUNK_SIZE, 1);
            
        int _x = x * Board.CHUNK_SIZE, _z = z * Board.CHUNK_SIZE;

        if (cloneIdx == Util.UP || cloneIdx == Util.UP_LEFT
            || cloneIdx == Util.UP_RIGHT) _z += fullBoardSize;
        else if (cloneIdx == Util.DOWN || cloneIdx == Util.DOWN_LEFT
            || cloneIdx == Util.DOWN_RIGHT) _z -= fullBoardSize;
        if (cloneIdx == Util.RIGHT || cloneIdx == Util.UP_RIGHT
            || cloneIdx == Util.DOWN_RIGHT) _x += fullBoardSize;
        else if (cloneIdx == Util.LEFT || cloneIdx == Util.UP_LEFT
            || cloneIdx == Util.DOWN_LEFT) _x -= fullBoardSize;

        // Move the chunk far away if it's another board.
        _x += boardIdx * distBetweenBoards * Board.CHUNK_SIZE;

        childTra.localPosition = new Vector3(_x, 0, _z);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
