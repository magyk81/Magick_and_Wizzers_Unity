using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class Piece
{
    private readonly GameObject obj;
    private readonly GameObject[] objs_piece = new GameObject[4];
    private readonly Transform[] trans_piece = new Transform[4];
    private readonly Transform[][] trans_dest;
    private Tile tilePrev, tileCurr, tileNext;
    private Tile[] destinations;
    private readonly int PERC_NEXT_MAX = 300;
    private int speed = 1, percNext = 0, moveDir = -1;
    private int boardSize = 0;
    public int BoardSize { set { if (boardSize == 0) boardSize = value; } }

    public Piece(Tile tile, Transform trans_board, GameObject obj_piece,
        Transform[][] trans_dest)
    {
        obj = new GameObject();
        obj.name = "Piece";

        Transform trans = obj.GetComponent<Transform>();
        trans.SetParent(trans_board);

        for (int i = 0; i < 4; i++)
        {
            objs_piece[i] = Object.Instantiate(obj_piece, trans);
            objs_piece[i].SetActive(true);
            objs_piece[i].name = "[" + i + "]";
            trans_piece[i] = objs_piece[i].GetComponent<Transform>();
        }
        trans_piece[0].localPosition = tile.Coord.ToVec3();

        this.trans_dest = trans_dest;

        tileCurr = tile; tilePrev = tile; tileNext = tile;

        destinations = new Tile[BoardScript.MAX_DESTINATIONS];
    }

    public void SetDestinationObjects(bool show)
    {
        for (int i = 0; i < BoardScript.MAX_DESTINATIONS; i++)
        {
            if (!show || destinations[i] == null)
            {
                for (int j = 0; j < 4; j++)
                    trans_dest[i][j].gameObject.SetActive(false);
            }
            else
            {
                for (int j = 0; j < 4; j++)
                    trans_dest[i][j].gameObject.SetActive(true);

                trans_dest[i][0].localPosition = new Vector3(
                    destinations[i].X,
                    trans_piece[0].localPosition.y,
                    destinations[i].Y);
                BoardScript.SetClonePositions(trans_dest[i], boardSize);
            }
        }
    }

    private void PrintDestinations()
    {
        StringBuilder str = new StringBuilder();
        for (int i = 0; i < destinations.Length; i++)
        {
            if (i > 0) str.Append(",  ");
            str.Append("[" + i + "]: ");
            if (destinations[i] == null)
                str.Append("null");
            else str.Append(destinations[i].Coord);
        }
        Debug.Log(str);
    }

    public void AddDestination(Tile destination)
    {
        for (int j = 0; j < BoardScript.MAX_DESTINATIONS; j++)
        {
            if (destinations[j] == null)
            {
                destinations[j] = destination;
                //SetDestinationObjects(j);
                break;
            }
        }
        PrintDestinations();
    }

    /// <summary>If the piece is at the front destination: Removes the front
    /// destination and moves the remaining destinations to the front.
    /// </summary>
    /// <returns><c>True</c> if the list of destinations is empty.</returns>
    private bool CheckIfAtDestination()
    {
        if (destinations[0] == null) return true;
        if (tileCurr.Equals(destinations[0]))
        {
            for (int j = 1; j < BoardScript.MAX_DESTINATIONS; j++)
            {
                destinations[j - 1] = null;
                destinations[j - 1] = destinations[j];
                //SetDestinationObjects(j - 1);
            }
            destinations[BoardScript.MAX_DESTINATIONS - 1] = null;
        }
        PrintDestinations();
        return destinations[0] == null;
    }

    /// <remarks>Prioritizes diagonal paths.</remarks>
    /// <returns>The direction towards the front destination.</returns>
    private int GetNextDirection()
    {
        int[] distances = tileCurr.Coord.GetDistancesTo(
            destinations[0].Coord, boardSize);
        int shorterHoriz = (distances[Coord.LEFT] < distances[Coord.RIGHT])
            ? Coord.LEFT : Coord.RIGHT;
        int shorterVert = (distances[Coord.UP] < distances[Coord.DOWN])
            ? Coord.UP : Coord.DOWN;
        if (distances[shorterHoriz] > distances[shorterVert])
            return shorterHoriz;
        return shorterVert;
    }

    public bool HasSameDestinationsAs(Piece piece)
    {
        for (int i = 0; i < destinations.Length; i++)
        {
            if (destinations[i] != piece.destinations[i]) return false;
        }
        return true;
    }

    public Vector3[] GetPositions()
    {
        Vector3[] positions = new Vector3[trans_piece.Length];
        for (int i = 0; i < trans_piece.Length; i++)
        {
            positions[i] = trans_piece[i].localPosition;
        }
        return positions;
    }

    public bool IsGameObject(GameObject obj)
    {
        foreach (GameObject o in objs_piece) { if (obj == o) return true; }
        return false;
    }

    public void FixedUpdate()
    {
        if (!tilePrev.Equals(tileNext))
        {
            percNext += speed;
            if (percNext >= PERC_NEXT_MAX / 2) tileCurr = tileNext;
            if (percNext >= PERC_NEXT_MAX)
            {
                tilePrev = tileNext;
                percNext -= PERC_NEXT_MAX; // leftover for next travel
            }
        }
        if (tilePrev.Equals(tileNext))
        {
            moveDir = -1;
            if (CheckIfAtDestination()) percNext = 0;
            else
            {
                moveDir = GetNextDirection();
                tileNext += moveDir;
                Debug.Log(Coord.ToString(GetNextDirection())
                    + ", " + tileNext.Coord);
                if (percNext >= PERC_NEXT_MAX / 2)
                    tileCurr = tileNext;
                else tileCurr = tilePrev;
            }
        }
    }

    public void Update()
    {
        float xPosF = tileCurr.X;
        float yPosF = tileCurr.Y;
        float altitude = trans_piece[0].localPosition.y;

        if (moveDir != -1)
        {
            float _percNext = (float) percNext / (float) PERC_NEXT_MAX;
            if (tileCurr == tileNext)
            {
                if (moveDir == Coord.LEFT) xPosF += (1 - _percNext);
                else if (moveDir == Coord.RIGHT) xPosF -= (1 - _percNext);
                else if (moveDir == Coord.UP) yPosF -= (1 - _percNext);
                else yPosF += (1 - _percNext);
            }
            else if (tileCurr == tilePrev)
            {
                if (moveDir == Coord.LEFT) xPosF -= _percNext;
                else if (moveDir == Coord.RIGHT) xPosF += _percNext;
                else if (moveDir == Coord.UP) yPosF += _percNext;
                else yPosF -= _percNext;
            }
        }

        trans_piece[0].localPosition = new Vector3(xPosF, altitude, yPosF);
        BoardScript.SetClonePositions(trans_piece, boardSize);
    }
}