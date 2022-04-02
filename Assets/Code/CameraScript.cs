/* Copyright (C) All Rights Reserved
 * Unauthorized copying of this file, via any medium is prohibited.
 * Proprietary and confidential.
 * Written by Robin Campos <magyk81@gmail.com>, year 2021.
 */

using UnityEngine;

public class CameraScript : MonoBehaviour {
    private readonly static Plane TILE_DETECT_PLANE = new Plane(Vector3.up, new Vector3(0, UX_Tile.LIFT_DIST, 0));
    private readonly static Plane PIECE_DETECT_PLANE = new Plane(Vector3.up, new Vector3(0, UX_Piece.LIFT_DIST, 0));
    
    private Ray[] mRay; private Vector3[] mRayVecs;
    private int mMaskForPieces, mMaskForTiles;
    private float[][] mRayHitPoints; private bool mRayHitPointsSet;
    private Camera mCam;
    private Transform mTran;
    private CanvasScript mCanv;
    private int mLocalPlayerIdx;
    private UX_Player.Mode mMode = UX_Player.Mode.PLAIN;

    [SerializeField]
    private float speed;
    private float mPosX = 0, mPosZ = 0, mPosY = 5;
    private float[] mBoardSizeHoriz, mBoardSizeVert;
    private float[][] bounds;
    private Vector3[] boardPos;
    private int boardID = 0;

    public float[][] Bounds {
        set {
            bounds = new float[value.Length][];
            mBoardSizeHoriz = new float[value.Length];
            mBoardSizeVert = new float[value.Length];

            for (int i = 0; i < value.Length; i++) {
                bounds[i] = new float[4];
                for (int j = 0; j < 4; j++) { bounds[i][j] = value[i][j]; }

                mBoardSizeHoriz[i] = bounds[i][Util.RIGHT] - bounds[i][Util.LEFT];
                mBoardSizeVert[i] = bounds[i][Util.UP] - bounds[i][Util.DOWN];
            }
        }
    }

    public int BoardID {
        get => boardID;
        set {
            boardPos[boardID] = new Vector3(mPosX, mPosY, mPosZ);
            boardID = value;
            mPosX = boardPos[value].x;
            mPosY = boardPos[value].y;
            mPosZ = boardPos[value].z;
            Move();
            Debug.Log("Moved to board " + value);
        }
    }

    public float[][] RayHitPoints {
        get {
            if (mRayHitPoints == null) return null;
            if (!mRayHitPointsSet) CalcRayHitPoints();
            return mRayHitPoints;
        }
    }

    public UX_Player.Mode Mode { set { mMode = value; } }

    /// <summary>Called once before the match begins.</summary>
    public void Init(int localPlayerIdx, CanvasScript canv, float[][] bounds,
        UX_Collider[] quarterColls, UX_Collider[,] tileColls) {
        mLocalPlayerIdx = localPlayerIdx;
        mTran = GetComponent<Transform>();

        // Setup camera.
        mCam = GetComponent<Camera>();

        // Setup masks for camera and rays.
        int mask = 1 << (UX_Piece.LAYER + localPlayerIdx);
        mMaskForPieces = mask;
        mMaskForTiles = 1 << (UX_Tile.LAYER + localPlayerIdx);
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
        int rayCount = Util.COUNT + 1;
        mRay = new Ray[rayCount];
        mRayVecs = new Vector3[rayCount];

        // Hitpoints include 1 from every ray for the piece plane, plus 1 for the tile plane.
        mRayHitPoints = new float[rayCount + 1][];
        for (int i = 0; i < rayCount + 1; i++) { mRayHitPoints[i] = new float[2]; }
        mRayHitPointsSet = false;;

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
                bounds[i][(int) Util.LEFT] + (mBoardSizeHoriz[i] / 2),
                mPosY,
                bounds[i][(int) Util.DOWN] + (mBoardSizeVert[i] / 2));
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

    /// <summary>
    /// Relocates the camera if out of bounds.
    /// </summary>
    private void Move() {
        if (mPosX < bounds[boardID][Util.LEFT]) mPosX += mBoardSizeHoriz[boardID];
        if (mPosX >= bounds[boardID][Util.RIGHT]) mPosX -= mBoardSizeHoriz[boardID];
        if (mPosZ < bounds[boardID][Util.DOWN]) mPosZ += mBoardSizeVert[boardID];
        if (mPosZ >= bounds[boardID][Util.UP]) mPosZ -= mBoardSizeVert[boardID];

        mTran.localPosition = new Vector3(mPosX, mPosY, mPosZ);

        // Rays will be different if the camera moved.
        mRayHitPointsSet = false;
    }

    private void CalcRayHitPoints() {
        // Detect one of the planes using every ray.
        for (int i = 0; i < mRayHitPoints.Length; i++) {
            float dist = 0;
            Vector3 pointVec3 = new Vector3();
            // If on the last one, use the tiles plane.
            if (i == mRayHitPoints.Length - 1) {
                mRay[0] = mCam.ViewportPointToRay(mRayVecs[0]);
                if (TILE_DETECT_PLANE.Raycast(mRay[0], out dist)) pointVec3 = mRay[0].GetPoint(dist);
                else Debug.LogError("mRay[0] not colliding with the tiles plane.");
            } else {
                mRay[i] = mCam.ViewportPointToRay(mRayVecs[i]);
                if (PIECE_DETECT_PLANE.Raycast(mRay[i], out dist)) pointVec3 = mRay[i].GetPoint(dist);
                else Debug.LogError("mRay[" + i + "] not colliding with the pieces plane.");
            }
            mRayHitPoints[i][0] = pointVec3.x; mRayHitPoints[i][1] = pointVec3.z;

            // Keep points within board bounds.
            if (mRayHitPoints[i][0] < bounds[boardID][Util.LEFT]) mRayHitPoints[i][0] += mBoardSizeHoriz[boardID];
            if (mRayHitPoints[i][0] >= bounds[boardID][Util.RIGHT]) mRayHitPoints[i][0] -= mBoardSizeHoriz[boardID];
            if (mRayHitPoints[i][1] < bounds[boardID][Util.DOWN]) mRayHitPoints[i][1] += mBoardSizeVert[boardID];
            if (mRayHitPoints[i][1] >= bounds[boardID][Util.UP]) mRayHitPoints[i][1] -= mBoardSizeVert[boardID];
        }
        mRayHitPointsSet = true;
    }

    // Update is called once per frame
    private void Update() {
    }
}