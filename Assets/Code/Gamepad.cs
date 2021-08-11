using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gamepad
{
    private bool isKeyboard;
    public Gamepad(bool isKeyboard)
    {
        this.isKeyboard = isKeyboard;
    }

    // GetInput is called once per frame
    public int GetInput()
    {
        if (isKeyboard)
        {
            // for debugging
            if (Input.GetKeyDown(KeyCode.Space)) return 0;
        }

        return -1;
    }
}
