/* Copyright (C) All Rights Reserved
 * Unauthorized copying of this file, via any medium is prohibited.
 * Proprietary and confidential.
 * Written by Robin Campos <magyk81@gmail.com>, year 2021.
 */

using UnityEngine;

public class CameraScript : MonoBehaviour {
    private RaycastHit mRayHit; private Ray[] mRay; private Vector3[] mRayVecs;
    private int mRayMaskForPieces, mRayMaskForTiles;
    private Camera mCam;
    private Transform mTran;
    private CanvasScript mCanv;
    private int mLocalPlayerIdx;
    private UX_Player.Mode mMode = UX_Player.Mode.PLAIN;
    private UX_Collider mCollidedChunkPrev = null, mCollidedQuarterPrev;
    private UX_Collider[] mQuarterColls;
    private UX_Collider[,] mTileColls;

    [SerializeField]
    private float speed;
    private float mPosX = 0, mPosZ = 0, mPosY = 5;
    private float[] boardSizeHoriz, boardSizeVert;
    private float[][] bounds;
    private Vector3[] boardPos;
    private int boardIdx = 0;

    public float[][] Bounds {
        set {
            bounds = new float[value.Length][];
            boardSizeHoriz = new float[value.Length];
            boardSizeVert = new float[value.Length];

            for (int i = 0; i < value.Length; i++) {
                bounds[i] = new float[4];
                for (int j = 0; j < 4; j++) { bounds[i][j] = value[i][j]; }

                boardSizeHoriz[i] = bounds[i][Util.RIGHT] - bounds[i][Util.LEFT];
                boardSizeVert[i] = bounds[i][Util.UP] - bounds[i][Util.DOWN];
            }
        }
    }

    public int BoardIdx {
        get { return boardIdx; }
        set {
            boardPos[boardIdx] = new Vector3(mPosX, mPosY, mPosZ);
            boardIdx = value;
            mPosX = boardPos[value].x;
            mPosY = boardPos[value].y;
            mPosZ = boardPos[value].z;
            Move();
            Debug.Log("Moved to board " + value);
        }
    }

    public UX_Player.Mode Mode { set { mMode = value; } }

    /// <summary>Called once before the match begins.</summary>
    public void Init(int localPlayerIdx, CanvasScript canv, float[][] bounds,
        UX_Collider[] quarterColls, UX_Collider[,] tileColls) {
        mLocalPlayerIdx = localPlayerIdx;
        mTran = GetComponent<Transform>();
        mQuarterColls = quarterColls;
        mTileColls = tileColls;

        // Setup camera.
        mCam = GetComponent<Camera>();

        // Setup masks for camera and rays.
        int mask = 1 << (UX_Piece.LAYER + localPlayerIdx);
        mRayMaskForPieces = mask;
        mRayMaskForTiles = 1 << (UX_Tile.LAYER + localPlayerIdx);
        for (int i = 0; i < UX_Piece.LAYER; i++) { mask |= (1 << i); }
        mask |= (1 << (UX_Tile.LAYER + localPlayerIdx));
        mCam.cullingMask = mask;
        
        // Setup canvas.
        mCanv = canv;
        canv.Init(mCam.pixelWidth, mCam.pixelHeight);

        float reticleRad = canv.Reticle.sizeDelta.x / 2;
        canv.DarkScreen.sizeDelta = new Vector2(mCam.pixelWidth, mCam.pixelHeight);

        // Setup rays.
        // 1 ray for each of the 8 directions from Util, plus a middle ray.
        int rayCount = Util.DOWN_LEFT + 2;
        mRay = new Ray[rayCount];
        mRayVecs = new Vector3[rayCount];

        // Just the main 4 directions.
        float[] rayVecDirs = new float[4];
        rayVecDirs[Util.LEFT] = ((float) ((mCam.pixelWidth / 2) - reticleRad)) / ((float) mCam.pixelWidth);
        rayVecDirs[Util.RIGHT] = ((float) ((mCam.pixelWidth / 2) + reticleRad)) / ((float) mCam.pixelWidth);
        rayVecDirs[Util.UP] = ((float) ((mCam.pixelHeight / 2) + reticleRad)) / ((float) mCam.pixelHeight);
        rayVecDirs[Util.DOWN] = ((float) ((mCam.pixelHeight / 2) - reticleRad)) / ((float) mCam.pixelHeight);
        mRayVecs[0] = new Vector3(0.5F, 0.5F, 0);
        mRayVecs[Util.LEFT + 1] = new Vector3(rayVecDirs[Util.LEFT], 0.5F, 0);
        mRayVecs[Util.RIGHT + 1] = new Vector3(rayVecDirs[Util.RIGHT], 0.5F, 0);
        mRayVecs[Util.UP + 1] = new Vector3(0.5F, rayVecDirs[Util.UP], 0);
        mRayVecs[Util.DOWN + 1] = new Vector3(0.5F, rayVecDirs[Util.DOWN], 0);
        
        float diagChange = reticleRad * (1F - 0.707F) / ((float) mCam.pixelWidth);
        rayVecDirs[Util.LEFT] += diagChange;
        rayVecDirs[Util.RIGHT] -= diagChange;
        rayVecDirs[Util.UP] -= diagChange;
        rayVecDirs[Util.DOWN] += diagChange;

        mRayVecs[Util.UP_LEFT + 1] = new Vector3(rayVecDirs[Util.LEFT], rayVecDirs[Util.UP], 0);
        mRayVecs[Util.UP_RIGHT + 1] = new Vector3(rayVecDirs[Util.RIGHT], rayVecDirs[Util.UP], 0);
        mRayVecs[Util.DOWN_LEFT + 1] = new Vector3(rayVecDirs[Util.LEFT], rayVecDirs[Util.DOWN], 0);
        mRayVecs[Util.DOWN_RIGHT + 1] = new Vector3(rayVecDirs[Util.RIGHT], rayVecDirs[Util.DOWN], 0);

        // Setup bounds.
        Bounds = bounds;

        // Setup positions for switching boards.
        boardPos = new Vector3[bounds.Length];
        for (int i = 0; i < bounds.Length; i++) {
            boardPos[i] = new Vector3(
                bounds[i][(int) Util.LEFT] + (boardSizeHoriz[i] / 2),
                mPosY,
                bounds[i][(int) Util.DOWN] + (boardSizeVert[i] / 2));
        }
    }

    /// <summary> Called every frame.</summary>
    /// <param x_move="x_move">
    /// -1 to move left, 1 to move right, or 0 not move horizontally </param>
    /// <param z_move="z_move">
    /// -1 to move down, 1 to move up, or 0 not move vertically </param>
    public void Move(int xMove, int zMove) {
        if (xMove == 0 && zMove == 0) return;

        mPosX += xMove * speed;
        mPosZ += zMove * speed;

        Move();
    }

    private void MoveTo(float newX, float newZ) {
        mPosX = newX; mPosZ = newZ;
        Move();
    }

    /// <summary>Relocates the camera if out of bounds.</summary>
    private void Move() {
        if (mPosX < bounds[boardIdx][Util.LEFT]) mPosX += boardSizeHoriz[boardIdx];
        if (mPosX > bounds[boardIdx][Util.RIGHT]) mPosX -= boardSizeHoriz[boardIdx];
        if (mPosZ < bounds[boardIdx][Util.DOWN]) mPosZ += boardSizeVert[boardIdx];
        if (mPosZ > bounds[boardIdx][Util.UP]) mPosZ -= boardSizeVert[boardIdx];

        mTran.localPosition = new Vector3(mPosX, mPosY, mPosZ);
    }

    /// <returns>
    /// Piece detected by the reticle using all 9 rays, but prioritizing the rays closest to the middle.
    /// </returns>
    public UX_Piece GetDetectedPiece() {
        for (int i = 0; i < mRay.Length; i++) {
            mRay[i] = mCam.ViewportPointToRay(mRayVecs[i]);
            if (Physics.Raycast(mRay[i], out mRayHit, Mathf.Infinity, mRayMaskForPieces)) {
                Collider hitCollider = mRayHit.collider;
                if (hitCollider != null) {
                    return hitCollider.gameObject.GetComponent<UX_Collider>().Piece;
                }
            }
        }
        return null;
    }

    /// <returns>
    /// Tile detected by the reticle using just the 1 middle ray.
    /// </returns>
    public UX_Tile GetDetectedTile() {
        mRay[0] = mCam.ViewportPointToRay(mRayVecs[0]);
        if (Physics.Raycast(mRay[0], out mRayHit, Mathf.Infinity, mRayMaskForTiles)) {
            Collider hitCollider = mRayHit.collider;
            if (hitCollider != null) {
                UX_Collider coll = hitCollider.gameObject.GetComponent<UX_Collider>();

                if (coll.IsType(UX_Collider.ColliderType.TILE)) return coll.Tile;
                
                if (coll.IsType(UX_Collider.ColliderType.QUARTER)) {
                    if (mCollidedQuarterPrev != null && mCollidedQuarterPrev != coll) mCollidedQuarterPrev.Enable();
                    coll.Disable();
                    mCollidedQuarterPrev = coll;
                    coll.Chunk.SetTileColliders(coll.Quarter, mTileColls, mLocalPlayerIdx);
                }

                if (coll.IsType(UX_Collider.ColliderType.CHUNK)) {
                    if (mCollidedChunkPrev != null && mCollidedChunkPrev != coll) mCollidedChunkPrev.Enable();
                    mCollidedChunkPrev = coll;
                    coll.Chunk.SetQuarterColliders(mQuarterColls, mLocalPlayerIdx);
                }
            }
        }
        return null;
    }

    // Update is called once per frame
    private void Update() {
    }
}