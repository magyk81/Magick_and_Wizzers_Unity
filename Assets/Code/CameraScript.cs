using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public static StateEnum state = StateEnum.NORMAL;

    [SerializeField]
    private float speed;
    private int x = 0, z = 0, y = 2;

    private readonly int LEFT = 0, RIGHT = 1, UP = 2, DOWN = 3;
    private bool[] pressing = { false, false, false, false };

    private Transform trans;

    // Start is called before the first frame update
    void Start()
    {
        trans = GetComponent<Transform>();

        Camera cam = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        if (state != StateEnum.NORMAL) return;

        if (Input.GetKeyDown(KeyCode.LeftArrow)) pressing[LEFT] = true;
        else if (Input.GetKeyUp(KeyCode.LeftArrow)) pressing[LEFT] = false;
        if (Input.GetKeyDown(KeyCode.RightArrow)) pressing[RIGHT] = true;
        else if (Input.GetKeyUp(KeyCode.RightArrow)) pressing[RIGHT] = false;
        if (Input.GetKeyDown(KeyCode.UpArrow)) pressing[UP] = true;
        else if (Input.GetKeyUp(KeyCode.UpArrow)) pressing[UP] = false;
        if (Input.GetKeyDown(KeyCode.DownArrow)) pressing[DOWN] = true;
        else if (Input.GetKeyUp(KeyCode.DownArrow)) pressing[DOWN] = false;

        if (pressing[LEFT]) x--;
        if (pressing[RIGHT]) x++;
        if (pressing[UP]) z++;
        if (pressing[DOWN]) z--;

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

    private float lowerBound, upperBound, boardSize;
    public void SetBounds(float lowerBound, float upperBound)
    {
        this.lowerBound = lowerBound;
        this.upperBound = upperBound;
        boardSize = upperBound - lowerBound;
    }
}
