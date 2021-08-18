using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    [SerializeField]
    private Canvas BASE_CANVAS;
    [SerializeField]
    private RectTransform reticle;
    private Ray[] ray;private RaycastHit raycastHit;private Vector3[] rayVecs;
    private Canvas canv; private Camera cam;

    // Start is called before the first frame update
    void Start()
    {
    }

    public void Setup(int idx)
    {
        cam = GetComponent<Camera>();
        GameObject canvObj = Instantiate(
            BASE_CANVAS.gameObject, GetComponent<Transform>().parent);
        canvObj.name = "Canvas " + idx;
        canv = canvObj.GetComponent<Canvas>();
        canv.worldCamera = GetComponent<Camera>();
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
        
    }
}
