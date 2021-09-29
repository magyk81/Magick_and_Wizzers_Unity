using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UX_Chunk : MonoBehaviour
{
    private int boardIdx;
    private Coord pos;

    [SerializeField]
    UX_Collider baseCollider;

    // private class UX_Tile
    // {
    //     private GameObject[] objs = new GameObject[9];
    //     private MeshCollider[] colls = new MeshCollider[9];
    //     private MeshRenderer[] rends = new MeshRenderer[9];
    //     private Coord coord;
    //     public Coord Coord { get { return coord.Copy(); } }
    //     private bool[] disp = new bool[(int) TileDispType.COUNT];
    //     private Material[] mats;
    //     public UX_Tile(Coord coord, GameObject realObj, Material[] mats)
    //     {
    //         this.coord = coord.Copy();
    //         SetObj(realObj, -1);
    //         this.mats = mats;
    //     }
    //     public void SetObj(GameObject obj, int cloneIdx)
    //     {
    //         objs[cloneIdx + 1] = obj;
    //         colls[cloneIdx + 1] = obj.GetComponent<MeshCollider>();
    //         rends[cloneIdx + 1] = obj.GetComponent<MeshRenderer>();
    //     }
    //     public bool IsCollider(Collider collider)
    //     {
    //         foreach (MeshCollider coll in colls)
    //         {
    //             if (coll == collider) return true;
    //         }
    //         return false;
    //     }
    //     public void EnableCollider()
    //     {
    //         foreach (MeshCollider coll in colls)
    //         {
    //             coll.enabled = true;
    //         }
    //     }
    //     public void DisableCollider()
    //     {
    //         foreach (MeshCollider coll in colls)
    //         {
    //             coll.enabled = false;
    //         }
    //     }
    //     public void Show(int dispType)
    //     {
    //         disp[dispType] = true;
    //         UpdateRend();
    //     }
    //     public void Hide(int dispType)
    //     {
    //         disp[dispType] = false;
    //         UpdateRend();
    //     }
    //     private void UpdateRend()
    //     {
    //         for (int i = 0; i < disp.Length; i++)
    //         {
    //             if (disp[i])
    //             {
    //                 foreach (MeshRenderer rend in rends)
    //                 {
    //                     rend.enabled = true;
    //                     rend.material = mats[i];
    //                 }
    //                 return;
    //             }
    //         }
    //         foreach (MeshRenderer rend in rends)
    //         {
    //             rend.enabled = false;
    //         }
    //     }
    // }
    private UX_Tile[,] tiles;

    private UX_Collider[] colliders;
    private Vector3[] quarterPos;
    private Vector3 quarterSize;
    private Coord[][] tileCollIdxOffset;

    // Start is called before the first frame update
    void Start()
    {

    }

    // public float GetEdge(int dir)
    // {
    //     float chunkSizeHalf = ((float) Board.CHUNK_SIZE) / 2F;
    //     Vector3 pos = real.GetComponent<Transform>().localPosition;
    //     if (dir == Util.UP) return pos.z + chunkSizeHalf;
    //     else if (dir == Util.DOWN) return pos.z - chunkSizeHalf;
    //     else if (dir == Util.LEFT) return pos.x - chunkSizeHalf;
    //     else if (dir == Util.RIGHT) return pos.x + chunkSizeHalf;
    //     return 0;
    // }

    // Takes in board coord. Returns local coord.
    // public Coord BoardCoordToLocal(Coord boardCoord)
    // {
    //     return Coord._(
    //         boardCoord.X - (x * Board.CHUNK_SIZE),
    //         boardCoord.Z - (z * Board.CHUNK_SIZE));
    // }
    // public Coord LocalCoordToBoard(Coord localCoord)
    // {
    //     return Coord._(
    //         localCoord.X + (x * Board.CHUNK_SIZE),
    //         localCoord.Z + (z * Board.CHUNK_SIZE));
    // }

    public void Init(Coord pos, int boardIdx, UX_Tile[,] tiles)
    {
        this.boardIdx = boardIdx;
        this.pos = pos.Copy();
        this.tiles = tiles;

        gameObject.name = "Chunk " + pos;

        // Generate colliders.
        Transform tra = GetComponent<Transform>();
        colliders = new UX_Collider[UX_Match.localPlayerCount];
        for (int i = 0; i < UX_Match.localPlayerCount; i++)
        {
            UX_Collider coll = Instantiate(baseCollider, tra)
                .GetComponent<UX_Collider>();
            coll.GetComponent<Transform>().eulerAngles
                = new Vector3(90, 0, 0);
            coll.Chunk = this;
            coll.gameObject.layer = UX_Tile.LAYER + i;
            coll.gameObject.name = "Collider - view " + i;
            colliders[i] = coll;
        }

        // Set quarter collider postions and size.
        quarterPos = new Vector3[4];
        
        Vector3 traPos = tra.localPosition, offset = tra.localScale / 4F;
        quarterPos[Util.UP_RIGHT - 4]
            = new Vector3(traPos.x + offset.x, 0, traPos.z + offset.y);
        quarterPos[Util.DOWN_LEFT - 4]
            = new Vector3(traPos.x - offset.x, 0, traPos.z - offset.y);
        quarterPos[Util.UP_LEFT - 4]
            = new Vector3(traPos.x - offset.x, 0, traPos.z + offset.y);
        quarterPos[Util.DOWN_RIGHT - 4]
            = new Vector3(traPos.x + offset.x, 0, traPos.z - offset.y);
        quarterSize = new Vector3(
            tiles.GetLength(0) / 2, tiles.GetLength(0) / 2, 1);
        
        // Set tile collider idx offsets.
        tileCollIdxOffset = new Coord[4][];
        int halfSize = tiles.GetLength(0) / 2;
        tileCollIdxOffset[Util.UP_RIGHT - 4] = new Coord[2]
        {
            Coord._(halfSize, halfSize),
            Coord._(0, 0)
        };
        tileCollIdxOffset[Util.UP_LEFT - 4] = new Coord[2]
        {
            Coord._(0, halfSize),
            Coord._(-halfSize, 0)
        };
        tileCollIdxOffset[Util.DOWN_RIGHT - 4] = new Coord[2]
        {
            Coord._(halfSize, 0),
            Coord._(0, -halfSize)
        };
        tileCollIdxOffset[Util.DOWN_LEFT - 4] = new Coord[2]
        {
            Coord._(0, 0),
            Coord._(-halfSize, -halfSize)
        };
    }

    public void SetQuarterColliders(UX_Collider[] quarterColls,
        int localPlayerIdx)
    {
        colliders[localPlayerIdx].Disable();
        for (int i = 0; i < 4; i++)
        {
            quarterColls[i].Set(quarterPos[i], quarterSize);
            quarterColls[i].Chunk = this;
            quarterColls[i].Enable();
        }
    }

    public void SetTileColliders(int quarter, UX_Collider[,] tileColls,
        int localPlayerIdx)
    {
        colliders[localPlayerIdx].Disable();
        Coord[] idxOffset = tileCollIdxOffset[quarter - 4];
        for (int i = idxOffset[0].X, _i = 0;
            i < tiles.GetLength(0) + idxOffset[1].X; i++, _i++)
        {
            for (int j = idxOffset[0].Z, _j = 0;
                j < tiles.GetLength(1) + idxOffset[1].Z; j++, _j++)
            {
                tileColls[_i, _j].Tile = tiles[i, j];
                tileColls[_i, _j].Chunk = this;
                tileColls[_i, _j].Enable();
            }
        }
    }

    private void SetupChunk(GameObject obj,
        int fullBoardSize, int distBetweenBoards, int cloneIdx = -1)
    {
        // obj.name = (cloneIdx >= 0)
        //     ? ("Chunk Clone - " + Util.DirToString(cloneIdx))
        //     : "Real Chunk";
        // Transform childTra = obj.GetComponent<Transform>();

        // childTra.localScale = new Vector3(
        //     Board.CHUNK_SIZE, Board.CHUNK_SIZE, 1);
            
        // int _x = x * Board.CHUNK_SIZE, _z = z * Board.CHUNK_SIZE;

        // if (cloneIdx == Util.UP || cloneIdx == Util.UP_LEFT
        //     || cloneIdx == Util.UP_RIGHT) _z += fullBoardSize;
        // else if (cloneIdx == Util.DOWN || cloneIdx == Util.DOWN_LEFT
        //     || cloneIdx == Util.DOWN_RIGHT) _z -= fullBoardSize;
        // if (cloneIdx == Util.RIGHT || cloneIdx == Util.UP_RIGHT
        //     || cloneIdx == Util.DOWN_RIGHT) _x += fullBoardSize;
        // else if (cloneIdx == Util.LEFT || cloneIdx == Util.UP_LEFT
        //     || cloneIdx == Util.DOWN_LEFT) _x -= fullBoardSize;

        // // Move the chunk far away if it's another board.
        // _x += boardIdx * distBetweenBoards * Board.CHUNK_SIZE;

        // childTra.localPosition = new Vector3(_x, 0, _z);

        // // Set up tiles
        // if (cloneIdx == -1)
        // {
        //     for (int i = 0; i < Board.CHUNK_SIZE; i++)
        //     {
        //         for (int j = 0; j < Board.CHUNK_SIZE; j++)
        //         {
        //             GameObject tile = Instantiate(baseTile, childTra);
        //             tile.name = "[" + i + ", " + j + "]";
        //             Transform tileTra = tile.GetComponent<Transform>();
        //             float tileSize = 1F / Board.CHUNK_SIZE;
        //             tileTra.localPosition = new Vector3(
        //                 (i + 0.5F) * tileSize - 0.5F,
        //                 (j + 0.5F) * tileSize - 0.5F, -LIFT_DIST);
        //             tileTra.localScale = new Vector3(tileSize, tileSize, 1);
        //             tiles[i, j] = new UX_Tile(Coord._(i, j), tile, tileMats);
        //         }
        //     }
        // }
        // else
        // {
        //     foreach (Transform tile in childTra)
        //     {
        //         int num1 = 0, num2 = 0;

        //         int strIdx = 0;
        //         string str = tile.gameObject.name;
        //         bool reachedComma = false;

        //         char curr = str[strIdx];
        //         while (strIdx < str.Length - 1)
        //         {
        //             if (curr == ',') reachedComma = true;

        //             if (Util.IsNum(curr))
        //             {
        //                 if (reachedComma)
        //                 {
        //                     num2 *= 10;
        //                     num2 += Util.CharToNum(curr);
        //                 }
        //                 else
        //                 {
        //                     num1 *= 10;
        //                     num1 += Util.CharToNum(curr);
        //                 }
        //             }

        //             strIdx++;
        //             curr = str[strIdx];
        //         }

        //         tiles[num1, num2].SetObj(tile.gameObject, cloneIdx);
        //     }
        // }
    }

    // public Coord GetTile(Collider collider)
    // {
    //     if (!hovered) return Coord.Null;
        
    //     foreach (UX_Tile tile in tiles)
    //     {
    //         if (tile.IsCollider(collider)) return tile.Coord;
    //     }
    //     return Coord.Null;
    // }
    // public bool IsCollider(Collider collider)
    // {
    //     if (!hovered)
    //     {
    //         if (real == collider.gameObject) return true;
    //         foreach (GameObject clone in clones)
    //         {
    //             if (clone == collider.gameObject) return true;
    //         }
    //         return false;
    //     }
    //     else return GetTile(collider) != Coord.Null;
    // }

    // public void Hover(UX_Chunk[][,] allChunks)
    // {
    //     if (hovered) return;

    //     foreach (UX_Chunk[,] _ in allChunks)
    //     {
    //         foreach (UX_Chunk chunk in _)
    //         {
    //             if (chunk != this) chunk.Hovered = false;
    //         }
    //     }
    //     foreach (UX_Tile tile in tiles)
    //     {
    //         tile.EnableCollider();
    //     }
    //     hovered = true;
    // }
    // public bool Hovered { set
    // {
    //     if (value) hovered = true;
    //     else
    //     {
    //         foreach (UX_Tile tile in tiles)
    //         {
    //             tile.DisableCollider();
    //         }
    //         hovered = false;
    //     }
    // } }

    // // Show 1 single tile and hide every other tile.
    // public void ShowTile(Coord coord, int dispType,
    //     UX_Chunk[,] allChunks = null)
    // {
    //     UX_Tile tile = tiles[coord.X, coord.Z];

    //     // Do nothing if the tile is already being shown and it's the only one
    //     // being shown.
    //     if (tilesDisp[dispType].Count != 1 || tilesDisp[dispType][0] != tile)
    //     {
    //         HideTiles(dispType);
    //         tile.Show(dispType);
    //         tilesDisp[dispType].Add(tile);
    //     }

    //     if (allChunks != null)
    //     {
    //         // Hide every tile from other chunks.
    //         foreach (UX_Chunk chunk in allChunks)
    //         {
    //             if (chunk != this) chunk.HideTiles(dispType);
    //         }
    //     }
    // }

    // // Show tiles and hide every other tile.
    // public void ShowTiles(Coord[] coords, int dispType,
    //     UX_Chunk[,] allChunks = null)
    // {
    //     if (allChunks != null)
    //     {
    //         // Hide every tile from every chunk.
    //         foreach (UX_Chunk chunk in allChunks)
    //         {
    //             chunk.HideTiles(dispType);
    //         }
    //     }

    //     // Assume all tiles are hidden, so don't have to check if each tile is
    //     // already being shown.
    //     foreach (Coord coord in coords)
    //     {
    //         if (coord == Coord.Null) continue;
            
    //         UX_Tile tile = tiles[coord.X, coord.Z];
    //         tile.Show(dispType);
    //         tilesDisp[dispType].Add(tile);
    //     }
    // }

    // Show tiles around origin and hide every other tile.
    // Returns the array of tile coords adjusted by the origin.
    // Can expect this function to only be called if the origin and coords are
    // different from the last call.
    // public Coord[] ShowTiles(Coord origin, Coord[] coords, int dispType,
    //     UX_Chunk[,] allChunks)
    // {
        // // Hide every tile from every chunk.
        // foreach (UX_Chunk chunk in allChunks)
        // {
        //     chunk.HideTiles(dispType);
        // }
        
        // List<List<Coord>> chunksToUpdate = new List<List<Coord>>();
        // List<Coord> allBoardCoords = new List<Coord>();
        // for (int i = 0; i < coords.Length; i++)
        // {
        //     // Calculate board coords based on origin.
        //     Coord boardCoord = (origin + coords[i]).ToBounds(
        //         Match.Boards[boardIdx].GetTileMax());
        //     allBoardCoords.Add(boardCoord);
        //     Coord chunkWithCoord = Match.Boards[boardIdx].TileToChunk(boardCoord);
            
        //     bool alreadyInList = false;
        //     foreach (List<Coord> chunkToUpdate in chunksToUpdate)
        //     {
        //         if (chunkToUpdate[0] == chunkWithCoord)
        //         {
        //             alreadyInList = true;

        //             // Convert board coord to tile coord that's local to the
        //             // chunk that it will be sent to.
        //             chunkToUpdate.Add(
        //                 allChunks[chunkWithCoord.X, chunkWithCoord.Z]
        //                 .BoardCoordToLocal(boardCoord));
        //             break;
        //         }
        //     }
        //     if (!alreadyInList)
        //     {
        //         List<Coord> newChunkToUpdate = new List<Coord>();
        //         newChunkToUpdate.Add(chunkWithCoord);

        //         // Convert board coord to tile coord that's local to the chunk
        //         // that it will be sent to.
        //         newChunkToUpdate.Add(
        //             allChunks[chunkWithCoord.X, chunkWithCoord.Z]
        //             .BoardCoordToLocal(boardCoord));
        //         chunksToUpdate.Add(newChunkToUpdate);
        //     }
        // }

        // foreach (List<Coord> chunkToUpdate in chunksToUpdate)
        // {
        //     Coord chunkCoord = chunkToUpdate[0].Copy();
        //     chunkToUpdate[0] = Coord.Null;
        //     allChunks[chunkCoord.X, chunkCoord.Z].ShowTiles(
        //         chunkToUpdate.ToArray(), dispType);
        // }

        // return allBoardCoords.ToArray();
    // }

    // public void HideTiles(int dispType)
    // {
    //     foreach (UX_Tile tile in tilesDisp[dispType]) { tile.Hide(dispType); }
    //     tilesDisp[dispType].Clear();
    // }

    void Update()
    {
    }
}
