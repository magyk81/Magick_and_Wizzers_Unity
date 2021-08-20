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

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A)) tra.localPosition = new Vector3(
            tra.localPosition.x - 0.2F, tra.localPosition.y, tra.localPosition.z);
    }

    private float[][] bounds;
    public float[][] Bounds
    {
        get { return bounds; }
        set
        {
            bounds = new float[value.Length][];
            for (int i = 0; i < value.Length; i++)
            {
                bounds[i] = new float[4];
                for (int j = 0; j < 4; j++) { bounds[i][j] = value[i][j]; }
            }
        }
    }

    private int boardIdx = 0;
    public int BoardIdx { get { return boardIdx; } set { boardIdx = value; } }
}
