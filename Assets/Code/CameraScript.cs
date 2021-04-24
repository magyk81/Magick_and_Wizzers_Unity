using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    [SerializeField]
    private float speed;
    [SerializeField]
    private int cursorRadius;
    public int CursorRadius { get { return cursorRadius; } }
    private int x = 0, z = 0, y = 6;
    private bool[] pressing = { false, false, false, false };

    private Transform trans;
    private Camera cam;
    private Ray[] ray; private RaycastHit rayHit; private Vector3[] rayVecs;

    private bool movesOnInput = true;
    public bool MovesOnInput { set { movesOnInput = value; } }
    private int layerMask;
    public int LayerMask
    {
        set { layerMask = 0b_0011_1111 + (value * 64); }
    }

    // Start is called before the first frame update
    void Start()
    {
        trans = GetComponent<Transform>();
        cam = GetComponent<Camera>();

        // Setup rays
        ray = new Ray[5];
        rayVecs = new Vector3[ray.Length];
        float[] rayVecDirs = new float[rayVecs.Length];
        rayVecDirs[Coord.LEFT]
            = ((float) ((cam.pixelWidth / 2) - cursorRadius))
            / ((float) cam.pixelWidth);
        rayVecDirs[Coord.RIGHT]
            = ((float) ((cam.pixelWidth / 2) + cursorRadius))
            / ((float) cam.pixelWidth);
        rayVecDirs[Coord.UP]
            = ((float) ((cam.pixelHeight / 2) + cursorRadius))
            / ((float) cam.pixelHeight);
        rayVecDirs[Coord.DOWN]
            = ((float) ((cam.pixelHeight / 2) - cursorRadius))
            / ((float) cam.pixelHeight);
        rayVecs[Coord.LEFT] = new Vector3(rayVecDirs[Coord.LEFT], 0.5F, 0);
        rayVecs[Coord.RIGHT] = new Vector3(rayVecDirs[Coord.RIGHT], 0.5F, 0);
        rayVecs[Coord.UP] = new Vector3(0.5F, rayVecDirs[Coord.UP], 0);
        rayVecs[Coord.DOWN] = new Vector3(0.5F, rayVecDirs[Coord.DOWN], 0);
        rayVecs[4] = new Vector3(0.5F, 0.5F, 0);

        cam.cullingMask = layerMask;
    }

    public Piece GetHoveredPiece(List<Piece> pieces)
    {
        for (int i = 0; i < ray.Length; i++)
        {
            if (Physics.Raycast(ray[i], out rayHit))
            {
                Collider hitCollider = rayHit.collider;
                if (hitCollider != null)
                {
                    GameObject obj = hitCollider.gameObject;
                    foreach (Piece piece in pieces)
                    {
                        if (piece.IsGameObject(obj)) return piece;
                    }
                }
            }
        }
        return null;
    }
    public Chunk GetHoveredChunk(Chunk[,] chunks, int size)
    {
        if (Physics.Raycast(ray[ray.Length - 1], out rayHit))
        {
            Collider hitCollider = rayHit.collider;
            if (hitCollider != null)
            {
                GameObject obj = hitCollider.gameObject;
                for (int i = 0; i < size; i++)
                {
                    for (int j = 0; j < size; j++)
                    {
                        if (chunks[i, j].IsGameObject(obj))
                            return chunks[i, j];
                    }
                }
            }
        }
        return null;
    }
    public Tile GetHoveredTile(Chunk chunk, int chunkSize)
    {
        if (Physics.Raycast(ray[ray.Length - 1], out rayHit))
        {
            Collider hitCollider = rayHit.collider;
            if (hitCollider != null)
                return chunk.GetTileFromGrid(hitCollider.gameObject);
        }
        return null;
    }

    // Update rays depending on what state we're in
    public void UpdateRays(StateEnum state)
    {
        if (state == StateEnum.PIECE || state == StateEnum.DESTINATION)
        {
            // Update rays for selecting with circle
            for (int i = 0; i < ray.Length; i++)
            {
                ray[i] = cam.ViewportPointToRay(rayVecs[i]);
                // ray[i] = cam.ViewportPointToRay(GetRayVec(i));
            }
        }
        else if (state == StateEnum.TILE || state == StateEnum.CHUNK)
        {
            // Update middle ray for selecting with dot
            ray[ray.Length - 1] = cam.ViewportPointToRay(
                rayVecs[ray.Length - 1]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // React to input and move/relocate camera accordingly
        if (movesOnInput)
        {
            if (pressing[Coord.LEFT]) x--;
            if (pressing[Coord.RIGHT]) x++;
            if (pressing[Coord.UP]) z++;
            if (pressing[Coord.DOWN]) z--;

            float xPos = x * speed, zPos = z * speed;

            // Relocate camera if it goes into clone zone
            bool shouldRelocate = false;
            if (xPos < lowerBound)
            {
                x += (int) (boardSize / speed);
                shouldRelocate = true;
            }
            else if (xPos > upperBound)
            {
                x -= (int) (boardSize / speed);
                shouldRelocate = true;
            }
            if (zPos < lowerBound)
            {
                z += (int) (boardSize / speed);
                shouldRelocate = true;
            }
            else if (zPos > upperBound)
            {
                z -= (int) (boardSize / speed);
                shouldRelocate = true;
            }

            if (shouldRelocate)
            {
                xPos = x * speed;
                zPos = z * speed;
                trans.localPosition = new Vector3(xPos, y, zPos);
            }
            else
            {
                foreach (bool _ in pressing)
                {
                    if (_)
                    {
                        trans.localPosition = new Vector3(xPos, y, zPos);
                        break;
                    }
                }
            }
        }        
    }

    public void PressDirection(int dir, bool press)
    {
        pressing[dir] = press;
    }

    private float lowerBound, upperBound, boardSize;
    // Called once when the board is first generated
    public void SetBounds(float lowerBound, float upperBound)
    {
        this.lowerBound = lowerBound;
        this.upperBound = upperBound;
        boardSize = upperBound - lowerBound;
    }
}
