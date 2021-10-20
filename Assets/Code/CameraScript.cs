using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    private Ray[] ray;private RaycastHit rayHit;private Vector3[] rayVecs;
    private int rayMask_pieces, rayMask_tiles;
    private Camera cam;
    private Transform tra;
    private CanvasScript canv;
    private int localPlayerIdx;
    private UX_Player.Mode mode = UX_Player.Mode.PLAIN;

    private UX_Collider[] quarterColls;
    private UX_Collider[,] tileColls;
    
    private UX_Collider collidedChunkPrev = null, collidedQuarterPrev;

    [SerializeField]
    private float speed;
    private float x = 0, z = 0, y = 5;

    public void Init(int localPlayerIdx, CanvasScript canv, float[][] bounds,
        UX_Collider[] quarterColls, UX_Collider[,] tileColls)
    {
        this.localPlayerIdx = localPlayerIdx;
        tra = GetComponent<Transform>();
        this.quarterColls = quarterColls;
        this.tileColls = tileColls;

        // Setup camera.
        cam = GetComponent<Camera>();

        // Setup masks for camera and rays.
        int mask = 1 << (UX_Piece.LAYER + localPlayerIdx);
        rayMask_pieces = mask;
        rayMask_tiles = 1 << (UX_Tile.LAYER + localPlayerIdx);
        for (int i = 0; i < UX_Piece.LAYER; i++)
        {
            mask |= (1 << i);
        }
        mask |= (1 << (UX_Tile.LAYER + localPlayerIdx));
        cam.cullingMask = mask;
        
        // Setup canvas.
        this.canv = canv;
        canv.Init(cam.pixelWidth, cam.pixelHeight);

        float reticleRad = canv.Reticle.sizeDelta.x / 2;
        canv.DarkScreen.sizeDelta = new Vector2(
            cam.pixelWidth, cam.pixelHeight);

        // Setup rays.
        // 1 ray for each of the 8 directions from Util, plus a middle ray.
        int rayCount = Util.DOWN_LEFT + 2;
        ray = new Ray[rayCount];
        rayVecs = new Vector3[rayCount];

        // Just the main 4 directions.
        float[] rayVecDirs = new float[4];
        rayVecDirs[Util.LEFT]
            = ((float) ((cam.pixelWidth / 2) - reticleRad))
            / ((float) cam.pixelWidth);
        rayVecDirs[Util.RIGHT]
            = ((float) ((cam.pixelWidth / 2) + reticleRad))
            / ((float) cam.pixelWidth);
        rayVecDirs[Util.UP]
            = ((float) ((cam.pixelHeight / 2) + reticleRad))
            / ((float) cam.pixelHeight);
        rayVecDirs[Util.DOWN]
            = ((float) ((cam.pixelHeight / 2) - reticleRad))
            / ((float) cam.pixelHeight);
        rayVecs[0] = new Vector3(0.5F, 0.5F, 0);
        rayVecs[Util.LEFT + 1] = new Vector3(rayVecDirs[Util.LEFT], 0.5F, 0);
        rayVecs[Util.RIGHT + 1] = new Vector3(rayVecDirs[Util.RIGHT], 0.5F, 0);
        rayVecs[Util.UP + 1] = new Vector3(0.5F, rayVecDirs[Util.UP], 0);
        rayVecs[Util.DOWN + 1] = new Vector3(0.5F, rayVecDirs[Util.DOWN], 0);
        
        float diagChange = reticleRad * (1F - 0.707F)
            / ((float) cam.pixelWidth);
        rayVecDirs[Util.LEFT] += diagChange;
        rayVecDirs[Util.RIGHT] -= diagChange;
        rayVecDirs[Util.UP] -= diagChange;
        rayVecDirs[Util.DOWN] += diagChange;

        rayVecs[Util.UP_LEFT + 1] = new Vector3(
            rayVecDirs[Util.LEFT], rayVecDirs[Util.UP], 0);
        rayVecs[Util.UP_RIGHT + 1] = new Vector3(
            rayVecDirs[Util.RIGHT], rayVecDirs[Util.UP], 0);
        rayVecs[Util.DOWN_LEFT + 1] = new Vector3(
            rayVecDirs[Util.LEFT], rayVecDirs[Util.DOWN], 0);
        rayVecs[Util.DOWN_RIGHT + 1] = new Vector3(
            rayVecDirs[Util.RIGHT], rayVecDirs[Util.DOWN], 0);

        // Setup bounds.
        Bounds = bounds;

        // Setup positions for switching boards.
        boardPos = new Vector3[bounds.Length];
        for (int i = 0; i < bounds.Length; i++)
        {
            boardPos[i] = new Vector3(
                bounds[i][(int) Util.LEFT] + (boardSize_horiz[i] / 2),
                y,
                bounds[i][(int) Util.DOWN] + (boardSize_vert[i] / 2));
        }
    }

    public void Move(int x_move, int z_move)
    {
        if (x_move == 0 && z_move == 0) return;

        x += x_move * speed;
        z += z_move * speed;

        Move();
    }
    private void MoveTo(float newX, float newZ)
    {
        x = newX; z = newZ;
        Move();
    }
    private void Move()
    {
        if (x < bounds[boardIdx][Util.LEFT])
            x += boardSize_horiz[boardIdx];
        if (x > bounds[boardIdx][Util.RIGHT])
            x -= boardSize_horiz[boardIdx];
        if (z < bounds[boardIdx][Util.DOWN])
            z += boardSize_vert[boardIdx];
        if (z > bounds[boardIdx][Util.UP])
            z -= boardSize_vert[boardIdx];

        tra.localPosition = new Vector3(x, y, z);
    }

    public void HandMove(int x_move, int y_move)
    {
        if (x_move == -1 && y_move == -1) return;
        canv.MoveCursor(x_move, y_move);
    }
    public int GetHandCard() { return canv.GetHoverIdx(); }
    public void DisplayPlayCard(int idx) { canv.DisplayPlayCard(idx); }
    public Piece GetHandPiece() { return canv.GetHandPiece(); }

    public UX_Piece GetDetectedPiece()
    {
        for (int i = 0; i < ray.Length; i++)
        {
            ray[i] = cam.ViewportPointToRay(rayVecs[i]);
            if (Physics.Raycast(ray[i], out rayHit, Mathf.Infinity,
                rayMask_pieces))
            {
                Collider hitCollider = rayHit.collider;
                if (hitCollider != null)
                {
                    return hitCollider.gameObject.GetComponent<UX_Collider>()
                        .Piece;
                }
            }
        }
        return null;
    }

    public UX_Tile GetDetectedTile()
    {
        ray[0] = cam.ViewportPointToRay(rayVecs[0]);
        if (Physics.Raycast(ray[0], out rayHit, Mathf.Infinity, rayMask_tiles))
        {
            Collider hitCollider = rayHit.collider;
            if (hitCollider != null)
            {
                UX_Collider coll
                    = hitCollider.gameObject.GetComponent<UX_Collider>();

                if (coll.IsType(UX_Collider.Type.TILE)) return coll.Tile;
                
                if (coll.IsType(UX_Collider.Type.QUARTER))
                {
                    if (collidedQuarterPrev != null
                        && collidedQuarterPrev != coll)
                        collidedQuarterPrev.Enable();
                    coll.Disable();
                    collidedQuarterPrev = coll;
                    coll.Chunk.SetTileColliders(
                        coll.Quarter, tileColls, localPlayerIdx);
                }

                if (coll.IsType(UX_Collider.Type.CHUNK))
                {
                    if (collidedChunkPrev != null && collidedChunkPrev != coll)
                        collidedChunkPrev.Enable();
                    collidedChunkPrev = coll;
                    coll.Chunk.SetQuarterColliders(
                        quarterColls, localPlayerIdx);
                }
            }
        }
        return null;
    }

    // Used for detecting chunks and tiles.
    public Collider GetDetectedCollider()
    {
        ray[0] = cam.ViewportPointToRay(rayVecs[0]);
        if (Physics.Raycast(ray[0], out rayHit,
            Mathf.Infinity, 1 << 0))
        {
            Collider hitCollider = rayHit.collider;
            return hitCollider;
        }
        return null;
    }

    // Toggle dark screen
    public void SetMode(UX_Player.Mode mode)
    {
        this.mode = mode;
        // if (mode == UX_Player.Mode.HAND)
        // {
        //     canv.DarkScreen.gameObject.SetActive(true);
        //     canv.ShowHand();
        //     canv.HideReticleCrosshair();
        // }
        // else
        // {
        //     canv.DarkScreen.gameObject.SetActive(false);
        //     canv.HideHand();
        // }
    }

    public void SetHandCards(Piece handPiece)
    {
        canv.SetHandPiece(handPiece);
    }

    // Update is called once per frame
    void Update()
    {
        // if (mode == UX_Player.Mode.HAND) canv.UpdateHandCards();
    }

    private float[] boardSize_horiz, boardSize_vert;
    private float[][] bounds;
    public float[][] Bounds
    {
        set
        {
            bounds = new float[value.Length][];
            boardSize_horiz = new float[value.Length];
            boardSize_vert = new float[value.Length];

            for (int i = 0; i < value.Length; i++)
            {
                bounds[i] = new float[4];
                for (int j = 0; j < 4; j++) { bounds[i][j] = value[i][j]; }

                boardSize_horiz[i]
                    = bounds[i][Util.RIGHT] - bounds[i][Util.LEFT];
                boardSize_vert[i]
                    = bounds[i][Util.UP] - bounds[i][Util.DOWN];
            }
        }
    }

    private Vector3[] boardPos;
    private int boardIdx = 0;
    public int BoardIdx
    {
        get { return boardIdx; }
        set
        {
            boardPos[boardIdx] = new Vector3(x, y, z);
            boardIdx = value;
            x = boardPos[value].x;
            y = boardPos[value].y;
            z = boardPos[value].z;
            Move();
            Debug.Log("Moved to board " + value);
        }
    }
}