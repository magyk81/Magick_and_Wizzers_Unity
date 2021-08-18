using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UX_Match
{
    private readonly int MAX_GAMEPADS = 6;
    private int[] boardSizes;
    private int[] fullSizes;
    private readonly int DIST_BETWEEN_BOARDS = 20;
    private readonly int MAX_WAYPOINTS = 5;

    Gamepad[] gamepads;
    private readonly GameObject chunkObj, pieceObj, waypointObj, cameraObj;
    private Transform boardTra;
    
    // [boardIdx][x, z]
    private ChunkObj[][,] chunks;
    private GameObject[] cameras;
    private List<PieceObj> pieces = new List<PieceObj>();
    private GameObject[,] waypoints;

    private Material debugPieceMat, debugPieceHoverMat, debugPieceSelectMat;

    public UX_Match(GameObject chunkObj,
        GameObject pieceObj, GameObject waypointObj, GameObject cameraObj)
    {
        gamepads = new Gamepad[MAX_GAMEPADS];
        gamepads[0] = new Gamepad(true);
        this.chunkObj = chunkObj;
        this.pieceObj = pieceObj;
        this.waypointObj = waypointObj;
        this.cameraObj = cameraObj;

        // Load debugging materials
        debugPieceMat = Resources.Load<Material>("Materials/Debug Piece");
        debugPieceHoverMat = Resources.Load<Material>(
            "Materials/Debug Piece Hover");
        debugPieceSelectMat = Resources.Load<Material>(
            "Materials/Debug Piece Select");
    }

    public void InitBoardObjs(int[] boardSizes, int playerCount)
    {
        // Generate chunks
        this.boardSizes = boardSizes;
        fullSizes = new int[boardSizes.Length];

        chunks = new ChunkObj[boardSizes.Length][,];

        for (int i = 0; i < boardSizes.Length; i++)
        {
            GameObject boardObj = new GameObject("Board " + (i + 1));
            boardTra = boardObj.GetComponent<Transform>();
            chunks[i] = new ChunkObj[boardSizes[i], boardSizes[i]];

            fullSizes[i] = boardSizes[i] * Board.CHUNK_SIZE;
            for (int x = 0; x < boardSizes[i]; x++)
                for (int z = 0; z < boardSizes[i]; z++)
                {
                    chunks[i][x, z] = new ChunkObj(chunkObj, boardTra,
                        fullSizes[i], DIST_BETWEEN_BOARDS, i, x, z);
                }
        }

        // Generate waypoints
        waypoints = new GameObject[playerCount, MAX_WAYPOINTS];
        GameObject waypointGroupObj = new GameObject("Waypoints");
        Transform waypointGroupTra = waypointGroupObj
            .GetComponent<Transform>();
        for (int i = 0; i < playerCount; i++)
        {
            GameObject waypointGroupPlayerObj = new GameObject(
                "Waypoints - Player " + i);
            Transform waypointGroupPlayerTra = waypointGroupPlayerObj
                .GetComponent<Transform>();
            waypointGroupPlayerTra.SetParent(waypointGroupTra);
            for (int j = 0; j < MAX_WAYPOINTS; j++)
            {
                waypoints[i, j] = GameObject.Instantiate(
                    waypointObj, waypointGroupPlayerTra);
                waypoints[i, j].name = "Waypoint " + (j + 1);
                waypoints[i, j].SetActive(false);
            }
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
            cameras[i].GetComponent<CameraScript>().Setup(i);
        }

        chunkObj.SetActive(false);
        pieceObj.SetActive(false);
        waypointObj.SetActive(false);
    }

    public void AddPiece(Piece piece)
    {
        PieceObj newPieceObj = new PieceObj(pieceObj, boardTra, piece,
            fullSizes[piece.BoardIdx], DIST_BETWEEN_BOARDS);
        pieces.Add(newPieceObj);
        newPieceObj.UpdatePosition();
    }

    private class ChunkObj
    {
        private int boardIdx, x, z;
        private GameObject real;
        private GameObject[] clones;
        public ChunkObj(GameObject chunkObj, Transform boardTra,
            int fullBoardSize, int distBetweenBoards,
            int boardIdx, int x, int z)
        {
            this.boardIdx = boardIdx;
            this.x = x;
            this.z = z;

            clones = new GameObject[8];
            GameObject groupObj = new GameObject(
                "Chunk [" + x + ", " + z + "]");
            Transform groupTra = groupObj.GetComponent<Transform>();
            groupTra.SetParent(boardTra);

            real = GameObject.Instantiate(chunkObj, groupTra);
            SetupChunk(real, groupTra, fullBoardSize, distBetweenBoards);
            for (int i = 0; i < 8; i++)
            {
                clones[i] = GameObject.Instantiate(chunkObj, boardTra);
                SetupChunk(clones[i], groupTra,
                    fullBoardSize, distBetweenBoards, i);                
            }
        }

        private void SetupChunk(GameObject obj, Transform groupParent,
            int fullBoardSize, int distBetweenBoards, int cloneIdx = -1)
        {
            obj.name = (cloneIdx >= 0)
                ? ("Chunk Clone - " + Util.DirToString(cloneIdx))
                : "Real Chunk";
            Transform tra = obj.GetComponent<Transform>();
            tra.SetParent(groupParent);

            tra.localScale = new Vector3(
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

            tra.localPosition = new Vector3(_x, 0, _z);
        }
    }

    private class PieceObj
    {
        private class PieceTag : MonoBehaviour
        {
            private PieceObj __;
            public PieceObj _
            {
                get { return __; }
                set { if (__ == null) __ = value; }
            }
        }
        private int boardIdx;
        private Piece piece;
        private GameObject real;
        private GameObject[] clones;
        private Transform realTra;
        private Transform[] clonesTra;
        private int fullBoardSize, distBetweenBoards;

        public void UpdatePosition()
        {
            float _x = piece.X - (Board.CHUNK_SIZE / 2),
                _z = piece.Z - (Board.CHUNK_SIZE / 2),
                __x = _x, __z = _z;

            _x += piece.BoardIdx * distBetweenBoards * Board.CHUNK_SIZE;
            
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

            realTra.localPosition = new Vector3(_x, 0.1F, _z);
        }

        public PieceObj(GameObject pieceObj, Transform boardTra, Piece piece,
            int fullBoardSize, int distBetweenBoards)
        {
            clones = new GameObject[8];
            clonesTra = new Transform[8];
            GameObject groupObj = new GameObject(
                "Piece - " + piece.Name);
            Transform groupTra = groupObj.GetComponent<Transform>();
            groupTra.SetParent(boardTra);

            real = GameObject.Instantiate(pieceObj, groupTra);
            real.name = "Real Piece";
            realTra = real.GetComponent<Transform>();
            realTra.SetParent(groupTra);
            real.SetActive(true);
            for (int i = 0; i < 8; i++)
            {
                clones[i] = GameObject.Instantiate(pieceObj, groupTra);
                clones[i].name = "Clone Piece - " + Util.DirToString(i);
                clonesTra[i] = clones[i].GetComponent<Transform>();
                clonesTra[i].SetParent(groupTra);
                clones[i].SetActive(true);
            }

            this.piece = piece;
            this.fullBoardSize = fullBoardSize;
            this.distBetweenBoards = distBetweenBoards;

            groupObj.AddComponent<PieceTag>();
            groupObj.GetComponent<PieceTag>()._ = this;
        }
    }

    // QueryGamepad is called once per frame
    int QueryGamepad(int idx)
    {
        if (gamepads[idx] != null) return gamepads[idx].GetInput();
        return -1;
    }
}
