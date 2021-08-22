using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    [SerializeField]
    private RectTransform reticle;
    private Ray[] ray;private RaycastHit raycastHit;private Vector3[] rayVecs;
    private Camera cam;
    private Transform tra;

    [SerializeField]
    private float speed;
    private float x = 0, z = 0, y = 5;

    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>();
        tra = GetComponent<Transform>();
        int reticleRad = (int) reticle.sizeDelta.x / 2;

        // Setup rays
        ray = new Ray[5];
        rayVecs = new Vector3[ray.Length];
        float[] rayVecDirs = new float[rayVecs.Length];
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
        rayVecs[Util.LEFT] = new Vector3(rayVecDirs[Util.LEFT], 0.5F, 0);
        rayVecs[Util.RIGHT] = new Vector3(rayVecDirs[Util.RIGHT], 0.5F, 0);
        rayVecs[Util.UP] = new Vector3(0.5F, rayVecDirs[Util.UP], 0);
        rayVecs[Util.DOWN] = new Vector3(0.5F, rayVecDirs[Util.DOWN], 0);
        rayVecs[4] = new Vector3(0.5F, 0.5F, 0);
    }

    public void Move(int x_move, int z_move)
    {
        if (x_move == 0 && z_move == 0) return;

        x += x_move * speed;
        z += z_move * speed;

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

    // Update is called once per frame
    void Update()
    {
        // React to input and move/relocate camera accordingly
        // if (moveOnInput)
        // {
        //     if (pressing[Util.LEFT]) x -= speed;
        //     if (pressing[Util.RIGHT]) x += speed;
        //     if (pressing[Util.UP]) z += speed;
        //     if (pressing[Util.DOWN]) z -= speed;

        //     // Relocate camera if it goes into clone zone
        //     bool shouldRelocate = false;
        //     if (x < bounds[boardIdx][Util.LEFT])
        //     {
        //         x += speed;
        //         shouldRelocate = true;
        //     }
        //     else if (x > bounds[boardIdx][Util.RIGHT])
        //     {
        //         x -= speed;
        //         shouldRelocate = true;
        //     }
        //     if (z < bounds[boardIdx][Util.DOWN])
        //     {
        //         z += speed;
        //         shouldRelocate = true;
        //     }
        //     else if (z > bounds[boardIdx][Util.UP])
        //     {
        //         z -= speed;
        //         shouldRelocate = true;
        //     }

        //     if (shouldRelocate)
        //     {
        //         tra.localPosition = new Vector3(x, y, z);
        //     }
        //     else
        //     {
        //         foreach (bool _ in pressing)
        //         {
        //             if (_)
        //             {
        //                 tra.localPosition = new Vector3(x, y, z);
        //                 break;
        //             }
        //         }
        //     }
        // }
    }

    // public void PressDirection(int dir, bool press)
    // {
    //     pressing[dir] = press;
    // }

    private float[] boardSize_horiz, boardSize_vert;
    private float[][] bounds;
    public float[][] Bounds
    {
        get { return bounds; }
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

    private int boardIdx = 0;
    public int BoardIdx { set { boardIdx = value; } }
}
