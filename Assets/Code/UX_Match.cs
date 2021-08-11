using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UX_Match
{
    private readonly int MAX_GAMEPADS = 6;
    private int[] boardSizes;

    Gamepad[] gamepads;
    private readonly GameObject chunkObj, cameraObj;
    private GameObject[][,,] chunks;
    private GameObject[] cameras;

    public UX_Match(GameObject chunkObj, GameObject cameraObj)
    {
        gamepads = new Gamepad[MAX_GAMEPADS];
        gamepads[0] = new Gamepad(true);
        this.chunkObj = chunkObj;
        this.cameraObj = cameraObj;
    }

    public void InitBoardObjs(int[] boardSizes, int playerCount)
    {
        // Generate chunks
        this.boardSizes = boardSizes;
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
                        InstantiateChunk(chunkTra, i, x, z, c);
                }

            chunkObj.SetActive(false);
        }

        // Generate cameras
        GameObject cameraGroupObj = new GameObject("Cameras");
        Transform cameraGroupTra = cameraGroupObj.GetComponent<Transform>();
        cameras = new GameObject[playerCount];
        for (int i = 0; i < playerCount; i++)
        {
            cameraObj.SetActive(false);
            cameras[i] = GameObject.Instantiate(cameraObj, cameraGroupTra);
            cameras[i].name = "Camera " + (i + 1);
            if (i > 0)
                cameras[i].GetComponent<AudioListener>().enabled = false;
            cameras[i].GetComponent<Camera>().rect = new Rect(
                ((float) i) / playerCount, 0, 1F / playerCount, 1);
            cameras[i].SetActive(true);
        }
    }

    private void InstantiateChunk(Transform boardTra,
        int i, int x, int z, int c)
    {
        chunks[i][x, z, c] = GameObject.Instantiate(chunkObj, boardTra);
        chunks[i][x, z, c].GetComponent<Transform>()
            .localScale = new Vector3(Board.CHUNK_SIZE, Board.CHUNK_SIZE, 1);
        int _c = c - 1;
        if (c == 0) chunks[i][x, z, c].name = "Real Chunk";
        else chunks[i][x, z, c].name = "Clone Chunk - "
            + Util.DirToString(_c);

        int _x = x * Board.CHUNK_SIZE, _z = z * Board.CHUNK_SIZE,
            fullSize = boardSizes[i] * Board.CHUNK_SIZE;
        if (_c == Util.UP || _c == Util.UP_LEFT || _c == Util.UP_RIGHT)
            _z += fullSize;
        else if (_c == Util.DOWN || _c == Util.DOWN_LEFT || _c == Util.DOWN_RIGHT)
            _z -= fullSize;
        if (_c == Util.RIGHT || _c == Util.UP_RIGHT || _c == Util.DOWN_RIGHT)
            _x += fullSize;
        else if (_c == Util.LEFT || _c == Util.UP_LEFT || _c == Util.DOWN_LEFT)
            _x -= fullSize;

        // Move the chunk far away if it's another board.
        _x += i * 20 * Board.CHUNK_SIZE; // 20 is a magic number.

        chunks[i][x, z, c].GetComponent<Transform>()
            .localPosition = new Vector3(_x, 0, _z);
    }

    public int[] GetBoardCenter(int boardIdx)
    {
        if (boardIdx < boardSizes.Length && boardIdx >= 0)
        {
            int size = boardSizes[boardIdx];
            Transform tra = chunks[boardIdx][size, size, 0]
                .GetComponent<Transform>();
            int x = (int) tra.localPosition.x;
            int z = (int) tra.localPosition.z;
            return new int[] { x, z };
        }
        else return null;
    }

    // QueryGamepad is called once per frame
    int QueryGamepad(int idx)
    {
        if (gamepads[idx] != null) return gamepads[idx].GetInput();
        return -1;
    }
}
