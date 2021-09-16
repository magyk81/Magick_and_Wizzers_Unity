using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UX_Chunk : MonoBehaviour
{
    private int boardIdx, x, z;
    [SerializeField]
    private GameObject real, baseTile;
    private GameObject[] clones = new GameObject[8];
    private Dictionary<GameObject, Coord> tileCoords
        = new Dictionary<GameObject, Coord>();
    private MeshCollider[] tileColls;
    private MeshRenderer[] tileRends;
    
    private bool hovered = true;
    private readonly static float LIFT_DIST = 0.05F;

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
        tileColls = new MeshCollider[Board.CHUNK_SIZE * Board.CHUNK_SIZE * 9];
        tileRends = new MeshRenderer[Board.CHUNK_SIZE * Board.CHUNK_SIZE * 9];

        SetupChunk(real, fullBoardSize, distBetweenBoards);
        for (int i = 0; i < 8; i++)
        {
            clones[i] = GameObject.Instantiate(real, tra);
            SetupChunk(clones[i],
                fullBoardSize, distBetweenBoards, i);
        }

        Destroy(baseTile);
        Unhover();
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

        // Set up tiles
        if (cloneIdx == -1)
        {
            for (int i = 0; i < Board.CHUNK_SIZE; i++)
            {
                for (int j = 0; j < Board.CHUNK_SIZE; j++)
                {
                    GameObject tile = Instantiate(baseTile, childTra);
                    tile.name = "[" + i + ", " + j + "]";
                    Transform tileTra = tile.GetComponent<Transform>();
                    float tileSize = 1F / Board.CHUNK_SIZE;
                    tileTra.localPosition = new Vector3(
                        (i + 0.5F) * tileSize - 0.5F,
                        (j + 0.5F) * tileSize - 0.5F, -LIFT_DIST);
                    tileTra.localScale = new Vector3(tileSize, tileSize, 1);
                    tileCoords.Add(tile, Coord._(i, j));
                    tileColls[j + i * Board.CHUNK_SIZE]
                        = tile.GetComponent<MeshCollider>();
                    tileRends[j + i * Board.CHUNK_SIZE]
                        = tile.GetComponent<MeshRenderer>();
                }
            }
        }
        else
        {
            foreach (Transform tile in childTra)
            {
                int num1 = 0, num2 = 0;

                int strIdx = 0;
                string str = tile.gameObject.name;
                bool reachedComma = false;

                char curr = str[strIdx];
                while (strIdx < str.Length - 1)
                {
                    if (curr == ',') reachedComma = true;

                    if (Util.IsNum(curr))
                    {
                        if (reachedComma)
                        {
                            num2 *= 10;
                            num2 += Util.CharToNum(curr);
                        }
                        else
                        {
                            num1 *= 10;
                            num1 += Util.CharToNum(curr);
                        }
                    }

                    strIdx++;
                    curr = str[strIdx];
                }

                tileCoords.Add(tile.gameObject, Coord._(num1, num2));
                tileColls[(num1 + (num2 * Board.CHUNK_SIZE))
                    + ((cloneIdx + 1) * Board.CHUNK_SIZE * Board.CHUNK_SIZE)]
                    = tile.GetComponent<MeshCollider>();
                tileRends[(num1 + (num2 * Board.CHUNK_SIZE))
                    + ((cloneIdx + 1) * Board.CHUNK_SIZE * Board.CHUNK_SIZE)]
                    = tile.GetComponent<MeshRenderer>();
            }
        }
    }

    public bool IsCollider(Collider collider)
    {
        if (hovered) return (tileCoords.ContainsKey(collider.gameObject));

        if (real == collider.gameObject) return true;
        foreach (GameObject clone in clones)
        {
            if (clone == collider.gameObject) return true;
        }
        return false;
    }
    public Coord GetTileCoord(Collider collider)
    {
        if (tileCoords.ContainsKey(collider.gameObject))
            return tileCoords[collider.gameObject].Copy();
        return Coord.Null;
    }

    public void Hover(UX_Chunk[][,] allChunks)
    {
        if (hovered) return;

        real.GetComponent<MeshCollider>().enabled = false;
        foreach (GameObject clone in clones)
        {
            clone.GetComponent<MeshCollider>().enabled = false;
        }
        foreach (MeshCollider tileColl in tileColls)
        {
            tileColl.enabled = true;
        }
        foreach (UX_Chunk[,] _ in allChunks)
        {
            foreach (UX_Chunk chunk in _)
            {
                if (chunk != this) chunk.Unhover();
            }
        }
        hovered = true;
    }

    public void Unhover()
    {
        if (!hovered) return;

        real.GetComponent<MeshCollider>().enabled = true;
        foreach (GameObject clone in clones)
        {
            clone.GetComponent<MeshCollider>().enabled = true;
        }
        foreach (MeshCollider tileColl in tileColls)
        {
            tileColl.enabled = false;
        }
        hovered = false;
    }

    public void ShowTiles(Coord coord)
    {
        int[] tileIndices = GetTileIndices(coord);
        foreach (int idx in tileIndices)
        {
            tileRends[idx].enabled = true;
        }
    }
    public void HideTiles()
    {
        foreach (MeshRenderer tileRend in tileRends)
        {
            tileRend.enabled = false;
        }
    }

    private int[] GetTileIndices(Coord coord)
    {
        int[] indices = new int[9];
        for (int i = 0; i < 9; i++)
        {
            indices[i] = (coord.Z + (coord.X * Board.CHUNK_SIZE))
                + (i * Board.CHUNK_SIZE * Board.CHUNK_SIZE);
        }
        return indices;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
