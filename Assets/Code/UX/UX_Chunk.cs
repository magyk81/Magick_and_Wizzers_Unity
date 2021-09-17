using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UX_Chunk : MonoBehaviour
{
    private int boardIdx, x, z;
    [SerializeField]
    private GameObject real, baseTile;
    private GameObject[] clones = new GameObject[8];
    
    private bool hovered = true;
    private readonly static float LIFT_DIST = 0.05F;

    private class UX_Tile
    {
        private GameObject[] objs = new GameObject[9];
        private MeshCollider[] colls = new MeshCollider[9];
        private MeshRenderer[] rends = new MeshRenderer[9];
        private Coord coord;
        public Coord Coord { get { return coord.Copy(); } }
        public UX_Tile(Coord coord, GameObject realObj)
        {
            this.coord = coord.Copy();
            SetObj(realObj, -1);
        }
        public void SetObj(GameObject obj, int cloneIdx)
        {
            objs[cloneIdx + 1] = obj;
            colls[cloneIdx + 1] = obj.GetComponent<MeshCollider>();
            rends[cloneIdx + 1] = obj.GetComponent<MeshRenderer>();
        }
        public bool IsCollider(Collider collider)
        {
            foreach (MeshCollider coll in colls)
            {
                if (coll == collider) return true;
            }
            return false;
        }
        public void EnableCollider()
        {
            foreach (MeshCollider coll in colls)
            {
                coll.enabled = true;
            }
        }
        public void DisableCollider()
        {
            foreach (MeshCollider coll in colls)
            {
                coll.enabled = false;
            }
        }
        public void Show(int dispType)
        {
            foreach (MeshRenderer rend in rends)
            {
                rend.enabled = true;
            }
        }
        public void Hide(int dispType)
        {
            foreach (MeshRenderer rend in rends)
            {
                rend.enabled = false;
            }
        }
    }
    private UX_Tile[,] tiles;
    public enum TileDispType { VALID, INVALID, AVAILABLE, INFLUENCE, COUNT }
    private List<UX_Tile>[] tilesDisp
        = new List<UX_Tile>[(int) TileDispType.COUNT];

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

    // Takes in board coord. Returns local coord.
    public Coord BoardCoordToLocal(Coord boardCoord)
    {
        return Coord._(
            boardCoord.X - (x * Board.CHUNK_SIZE),
            boardCoord.Z - (z * Board.CHUNK_SIZE));
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
        tiles = new UX_Tile[Board.CHUNK_SIZE, Board.CHUNK_SIZE];
        for (int i = 0; i < (int) TileDispType.COUNT; i++)
        {
            tilesDisp[i] = new List<UX_Tile>();
        }

        SetupChunk(real, fullBoardSize, distBetweenBoards);
        for (int i = 0; i < 8; i++)
        {
            clones[i] = GameObject.Instantiate(real, tra);
            SetupChunk(clones[i],
                fullBoardSize, distBetweenBoards, i);
        }

        Destroy(baseTile);
        hovered = false;
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
                    tiles[i, j] = new UX_Tile(Coord._(i, j), tile);
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

                tiles[num1, num2].SetObj(tile.gameObject, cloneIdx);
            }
        }
    }

    public Coord GetTile(Collider collider)
    {
        if (!hovered) return Coord.Null;
        
        foreach (UX_Tile tile in tiles)
        {
            if (tile.IsCollider(collider)) return tile.Coord;
        }
        return Coord.Null;
    }
    public bool IsCollider(Collider collider)
    {
        if (!hovered)
        {
            if (real == collider.gameObject) return true;
            foreach (GameObject clone in clones)
            {
                if (clone == collider.gameObject) return true;
            }
            return false;
        }
        else return GetTile(collider) != Coord.Null;
    }

    public void Hover(UX_Chunk[][,] allChunks)
    {
        if (hovered) return;

        foreach (UX_Chunk[,] _ in allChunks)
        {
            foreach (UX_Chunk chunk in _)
            {
                if (chunk != this) chunk.Hovered = false;
            }
        }
        foreach (UX_Tile tile in tiles)
        {
            tile.EnableCollider();
        }
        hovered = true;
    }
    public bool Hovered { set
    {
        if (value) hovered = true;
        else
        {
            foreach (UX_Tile tile in tiles)
            {
                tile.DisableCollider();
            }
            hovered = false;
        }
    } }

    // Show 1 single tile and hide every other tile.
    public void ShowTile(Coord coord, int dispType,
        UX_Chunk[,] allChunks = null)
    {
        UX_Tile tile = tiles[coord.X, coord.Z];

        // Do nothing if the tile is already being shown and it's the only one
        // being shown.
        if (tilesDisp[dispType].Count != 1 || tilesDisp[dispType][0] != tile)
        {
            HideTiles(dispType);
            tile.Show(dispType);
            tilesDisp[dispType].Add(tile);
        }

        if (allChunks != null)
        {
            // Hide every tile from other chunks.
            foreach (UX_Chunk chunk in allChunks)
            {
                if (chunk != this) chunk.HideTiles(dispType);
            }
        }
    }

    // Show tiles and hide every other tile.
    public void ShowTiles(Coord[] coords, int dispType,
        UX_Chunk[,] allChunks = null)
    {
        if (allChunks != null)
        {
            // Hide every tile from every chunk.
            foreach (UX_Chunk chunk in allChunks)
            {
                chunk.HideTiles(dispType);
            }
        }

        // Assume all tiles are hidden, so don't have to check if each tile is
        // already being shown.
        foreach (Coord coord in coords)
        {
            if (coord == Coord.Null) continue;
            
            UX_Tile tile = tiles[coord.X, coord.Z];
            tile.Show(dispType);
            tilesDisp[dispType].Add(tile);
        }
    }

    // Show tiles around origin and hide every other tile.
    // Returns the array of tile coords adjusted by the origin.
    // Can expect this function to only be called if the origin and coords are
    // different from the last call.
    public Coord[] ShowTiles(Coord origin, Coord[] coords, int dispType,
        UX_Chunk[,] allChunks)
    {
        // Hide every tile from every chunk.
        foreach (UX_Chunk chunk in allChunks)
        {
            chunk.HideTiles(dispType);
        }
        
        List<List<Coord>> chunksToUpdate = new List<List<Coord>>();
        for (int i = 0; i < coords.Length; i++)
        {
            // Calculate board coords based on origin.
            Coord boardCoord = (origin + coords[i]).ToBounds(
                Match.Boards[boardIdx].GetTileMax());
            Coord chunkWithCoord = Match.Boards[boardIdx].TileToChunk(boardCoord);
            
            bool alreadyInList = false;
            foreach (List<Coord> chunkToUpdate in chunksToUpdate)
            {
                if (chunkToUpdate[0] == chunkWithCoord)
                {
                    alreadyInList = true;

                    // Convert board coord to tile coord that's local to the
                    // chunk that it will be sent to.
                    chunkToUpdate.Add(BoardCoordToLocal(boardCoord));
                    break;
                }
            }
            if (!alreadyInList)
            {
                List<Coord> newChunkToUpdate = new List<Coord>();
                newChunkToUpdate.Add(chunkWithCoord);

                // Convert board coord to tile coord that's local to the chunk
                // that it will be sent to.
                newChunkToUpdate.Add(BoardCoordToLocal(boardCoord));
                chunksToUpdate.Add(newChunkToUpdate);
            }
        }
        foreach (List<Coord> chunkToUpdate in chunksToUpdate)
        {
            Coord chunkCoord = chunkToUpdate[0].Copy();
            chunkToUpdate[0] = Coord.Null;
            allChunks[chunkCoord.X, chunkCoord.Z].ShowTiles(
                chunkToUpdate.ToArray(), dispType);
        }

        return null;
    }

    public void HideTiles(int dispType)
    {
        foreach (UX_Tile tile in tilesDisp[dispType]) { tile.Hide(dispType); }
        tilesDisp[dispType].Clear();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
