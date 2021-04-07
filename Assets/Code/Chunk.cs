using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk
{
    private readonly GameObject obj;
    private readonly GameObject[] objs_square = new GameObject[4];
    public GameObject Obj_square {
        set {
            
        }
    }
    private Chunk[] neighbors = new Chunk[4];
    public Chunk[] Neighbors {
        set {
            neighbors[Coord.LEFT] = value[Coord.LEFT];
            neighbors[Coord.RIGHT] = value[Coord.RIGHT];
            neighbors[Coord.UP] = value[Coord.UP];
            neighbors[Coord.DOWN] = value[Coord.DOWN];
        }
    }
    private Tile[,] tiles;
    public Tile[,] Tiles {
        get { return tiles; }
    }
    private readonly int SIZE;
    private readonly Coord COORD;
    private readonly Coord[] COORDS;

    public Chunk(Coord[] coords, int chunkSize)
    {
        SIZE = chunkSize;

        obj = new GameObject();
        COORD = coords[0];
        obj.name = "Chunk [" + COORD + "]";        

        tiles = new Tile[chunkSize, chunkSize];

        for (int j = 0; j < chunkSize; j++)
        {
            for (int i = 0; i < chunkSize; i++)
            {
                Coord[] tileCoords = new Coord[coords.Length];
                for (int a = 0; a < coords.Length; a++)
                {
                    tileCoords[a] = (coords[a] * chunkSize) + Coord._(i, j);
                }

                tiles[i, j] = new Tile(tileCoords, chunkSize);
            }
        }

        COORDS = new Coord[coords.Length];
        for (int a = 0; a < coords.Length; a++) { COORDS[a] = coords[a]; }
    }

    public void SetGameObject(GameObject obj_square, Transform trans_parent)
    {
        Material material = new Material((Material) Resources.Load(
            "Materials/Chunk Material", typeof(Material)));
        material.name = "Chunk Material " + COORD.ToString();

        int texNameVal = COORD.X + COORD.Y;
        Texture texture = null;

        string texName = (texNameVal < 10 ? "0" : "") + texNameVal;
        texture = (Texture) Resources.Load(
            "Textures/" + texName, typeof(Texture));
        texNameVal -= COORD.X;
            
        material.mainTexture = texture;

        Transform trans = obj.GetComponent<Transform>();
        trans.SetParent(trans_parent);

        for (int i = 0; i < COORDS.Length; i++)
        {
            objs_square[i] = Object.Instantiate(obj_square, trans);
            objs_square[i].name = "Clone [" + i + "]";

            Vector3 corner = (COORDS[i] * SIZE).ToVec3();
            float offset = (((float) SIZE) / 2) - 0.5F;
            Transform t = objs_square[i].GetComponent<Transform>();
            t.localPosition = new Vector3(
                corner.x + offset, 0, corner.z + offset);
            t.localScale = new Vector3(SIZE, SIZE, 1);

            objs_square[i].GetComponent<Renderer>().material = material;
        }
    }

    public void SetNeighbors()
    {
        for (int i = 0; i < SIZE; i++)
        {
            for (int j = 0; j < SIZE; j++)
            {
                Tile[] neighbors = { null, null, null, null };
                if (i > 0) neighbors[Coord.LEFT] = tiles[i - 1, j];
                if (i < SIZE - 1) neighbors[Coord.RIGHT] = tiles[i + 1, j];
                if (j > 0) neighbors[Coord.UP] = tiles[i, j - 1];
                if (j < SIZE - 1) neighbors[Coord.DOWN] = tiles[i, j + 1];

                tiles[i, j].Neighbors = neighbors;
            }
        }
    }
}
