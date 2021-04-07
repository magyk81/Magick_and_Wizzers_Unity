using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece
{
    private readonly GameObject obj;
    private readonly GameObject[] objs_piece = new GameObject[4];
    private readonly int MAX_DESTINATIONS = 3;
    private readonly GameObject[][] objs_dest;
    private readonly Transform[] trans_piece = new Transform[4];
    private readonly Transform[][] trans_dest;
    private Tile tilePrev, tileCurr, tileNext;
    private Tile[] destinations;
    private readonly int PERC_NEXT_MAX = 300;
    private int speed = 1, percNext = 0, moveDir = -1;
    private int boardSize = 0;
    public int BoardSize { set { if (boardSize == 0) boardSize = value; } }

    public Piece(Tile tile, Transform trans_board, GameObject obj_piece, GameObject obj_dest)
    {
        obj = new GameObject();
        obj.name = "Piece";

        Transform trans = obj.GetComponent<Transform>();
        trans.SetParent(trans_board);

        objs_dest = new GameObject[MAX_DESTINATIONS][];
        trans_dest = new Transform[MAX_DESTINATIONS][];
        for (int j = 0; j < MAX_DESTINATIONS; j++)
        {
            objs_dest[j] = new GameObject[4];
            trans_dest[j] = new Transform[4];
        }

        for (int i = 0; i < 4; i++)
        {
            objs_piece[i] = Object.Instantiate(obj_piece, trans);
            objs_piece[i].SetActive(true);
            objs_piece[i].name = "Piece [" + i + "]" + (i == 0 ? " _" : "");
            trans_piece[i] = objs_piece[i].GetComponent<Transform>();
            //objs[i].GetComponent<Renderer>().material = material;

            for (int j = 0; j < MAX_DESTINATIONS; j++)
            {
                objs_dest[j][i] = Object.Instantiate(obj_dest, trans);
                objs_dest[j][i].name = "Destination_ " + j + " [" + i + "]"
                    + (i == 0 ? " _" : "");
                trans_dest[j][i] = objs_dest[j][i].GetComponent<Transform>();
                objs_dest[j][i].SetActive(false);
            }
        }

        tileCurr = tile; tilePrev = tile; tileNext = tile;
        objs_piece[0].GetComponent<Transform>().localPosition
            = tile.Coord.ToVec3();

        destinations = new Tile[MAX_DESTINATIONS];
    }

    private void SetDestinationObjects(int j)
    {
        if (destinations[j] == null)
            for (int i = 0; i < 4; i++) { objs_dest[j][i].SetActive(false); }
        else
        {
            for (int i = 0; i < 4; i++) { objs_dest[j][i].SetActive(true); }
            trans_dest[j][0].localPosition = new Vector3(
                destinations[j].X,
                trans_piece[0].localPosition.y,
                destinations[j].Y);
            SetClonePositions(trans_dest[j]);
        }
    }

    public void AddDestination(Tile destination)
    {
        for (int j = 0; j < MAX_DESTINATIONS; j++)
        {
            if (destinations[j] == null)
            {
                destinations[j] = destination;
                SetDestinationObjects(j);
                break;
            }
        }
    }

    /// <summary>If the piece is at the front destination: Removes the front
    /// destination and moves the remaining destinations to the front.
    /// </summary>
    /// <returns><c>True</c> if the list of destinations is empty.</returns>
    private bool CheckIfAtDestination()
    {
        if (tileCurr.Equals(destinations[0]))
        {
            for (int j = 1; j < MAX_DESTINATIONS; j++)
            {
                destinations[j - 1] = null;
                destinations[j - 1] = destinations[j];
                SetDestinationObjects(j - 1);
            }
            destinations[MAX_DESTINATIONS - 1] = null;
        }
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

    private void SetClonePositions(Transform[] trans)
    {
        float xPosF = trans[0].localPosition.x;
        float altitude = trans[0].localPosition.y;
        float yPosF = trans[0].localPosition.z;

        int xMult = (xPosF < boardSize / 2 - 0.5F) ? 1 : -1;
        int yMult = (yPosF < boardSize / 2 - 0.5F) ? 1 : -1;

        trans[1].localPosition = new Vector3(
            xPosF + (boardSize * xMult),
            altitude,
            yPosF);
        trans[2].localPosition = new Vector3(
            xPosF,
            altitude,
            yPosF + (boardSize * yMult));
        trans[3].localPosition = new Vector3(
            xPosF + (boardSize * xMult),
            altitude,
            yPosF + (boardSize * yMult));
        
        //if (trans == trans_dest[0]) Debug.Log(trans_dest[0][3].localPosition);
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
        SetClonePositions(trans_piece);
    }
}