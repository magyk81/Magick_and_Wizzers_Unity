using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    private Ray[] ray;private RaycastHit rayHit;private Vector3[] rayVecs;
    private Camera cam;
    private Transform tra;
    private CanvasScript canv;
    private UX_Player.Mode mode = UX_Player.Mode.PLAIN;

    [SerializeField]
    private float speed;
    private float x = 0, z = 0, y = 5;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void InitCamObjs(CanvasScript canv)
    {
        cam = GetComponent<Camera>();
        tra = GetComponent<Transform>();
        this.canv = canv;
        canv.InitCanvObjs(cam.pixelWidth, cam.pixelHeight);

        float reticleRad = canv.Reticle.sizeDelta.x / 2;
        canv.DarkScreen.sizeDelta = new Vector2(
            cam.pixelWidth, cam.pixelHeight);

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

    public void HandMove(int x_move, int y_move)
    {
        if (x_move == -1 && y_move == -1) return;
        canv.MoveCursor(x_move, y_move);
    }

    public List<Collider> GetDetectedColliders()
    {
        List<Collider> collidersDetected = new List<Collider>();
        // The last element of ray is the middle of the reticle, which is what
        // we want to check first
        for (int i = ray.Length - 1; i >= 0; i--)
        {
            ray[i] = cam.ViewportPointToRay(rayVecs[i]);
            if (Physics.Raycast(ray[i], out rayHit))
            {
                Collider hitCollider = rayHit.collider;
                if (hitCollider != null)
                {
                    if (!collidersDetected.Contains(hitCollider))
                        collidersDetected.Add(hitCollider);                        
                }
            }
        }
        return collidersDetected;
    }

    // Toggle dark screen
    public void SetMode(UX_Player.Mode mode)
    {
        this.mode = mode;
        if (mode == UX_Player.Mode.HAND)
        {
            canv.DarkScreen.gameObject.SetActive(true);
            canv.ShowHand();
        }
        else
        {
            canv.DarkScreen.gameObject.SetActive(false);
            canv.HideHand();
        }
    }

    public void SetHandCards(Card[] cards)
    {
        canv.SetHandCards(cards);
    }

    // Update is called once per frame
    void Update()
    {
        if (mode == UX_Player.Mode.HAND) canv.UpdateHandCards();
    }

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
