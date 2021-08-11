using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UX_Match
{
    private readonly int MAX_GAMEPADS = 6;
    Gamepad[] gamepads;

    private readonly GameObject chunkObj;

    private GameObject[][,,] chunks;

    public UX_Match(GameObject chunkObj)
    {
        gamepads = new Gamepad[MAX_GAMEPADS];
        gamepads[0] = new Gamepad(true);
        this.chunkObj = chunkObj;
    }

    public void InitBoardObjs(int[] boardSizes)
    {
        // Generate gameobjects
        chunks = new GameObject[boardSizes.Length][,,];
        for (int i = 0; i < boardSizes.Length; i++)
        {
            GameObject boardObj = new GameObject("Board " + (i + 1));
            Transform boardTra = boardObj.GetComponent<Transform>();
            chunks[i] = new GameObject[boardSizes[i], boardSizes[i], 9];

            for (int x = 0; x < boardSizes[i]; x++)
                for (int z = 0; z < boardSizes[i]; z++)
                {
                    GameObject chunkGroupObj = new GameObject(
                        "Chunk [" + x + ", " + z + "]");
                    Transform chunkTra = chunkGroupObj
                        .GetComponent<Transform>();
                    chunkTra.SetParent(boardTra);
                    for (int c = 0; c < 9; c++)
                        InstantiateChunk(chunkTra, boardSizes[i], i, x, z, c);
                }
        }
    }

    private void InstantiateChunk(Transform boardTra, int boardSize,
        int i, int x, int z, int c)
    {
        chunks[i][x, z, c] = GameObject.Instantiate(chunkObj, boardTra);
        int _c = c - 1;
        if (c == 0) chunks[i][x, z, c].name = "Real Chunk";
        else chunks[i][x, z, c].name = "Clone Chunk - "
            + Util.DirToString(_c);

        int _x = x, _z = z;
        if (_c == Util.UP || _c == Util.UP_LEFT || _c == Util.UP_RIGHT)
            _z += boardSize;
        else if (_c == Util.DOWN || _c == Util.DOWN_LEFT
            || _c == Util.DOWN_RIGHT) _z -= boardSize;
        if (_c == Util.RIGHT || _c == Util.UP_RIGHT || _c == Util.DOWN_RIGHT)
            _x += boardSize;
        else if (_c == Util.LEFT || _c == Util.UP_LEFT || _c == Util.DOWN_LEFT)
            _x -= boardSize;

        // Move the chunk far away if it's another board.
        _x += i * 20; // 20 is a magic number.

        chunks[i][x, z, c].GetComponent<Transform>()
            .localPosition = new Vector3(_x, 0, _z);
    }

    // QueryGamepad is called once per frame
    int QueryGamepad(int idx)
    {
        if (gamepads[idx] != null) return gamepads[idx].GetInput();
        return -1;
    }
}
