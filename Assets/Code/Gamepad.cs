/* Copyright (C) All Rights Reserved
 * Unauthorized copying of this file, via any medium is prohibited.
 * Proprietary and confidential.
 * Written by Robin Campos <magyk81@gmail.com>, year 2021.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gamepad
{
    private readonly int STICK_INT = 100;
    private bool isKeyboard;

    private int[] padInput;
    public int[] PadInput
    {
        // Called once per frame.
        get
        {
            if (isKeyboard)
            {
                GetInput(KeyCode.Escape, Button.START);
                GetInput(KeyCode.Backspace, Button.SELECT);
                GetInput(KeyCode.Space, Button.A);
                GetInput(KeyCode.L, Button.B);
                GetInput(KeyCode.J, Button.X);
                GetInput(KeyCode.K, Button.Y);
                GetInput(KeyCode.LeftShift, Button.L_TRIG);
                GetInput(KeyCode.RightShift, Button.R_TRIG);
                GetInput(KeyCode.Q, Button.L_BUMP);
                GetInput(KeyCode.E, Button.R_BUMP);
                GetInput(KeyCode.UpArrow, Button.UP);
                GetInput(KeyCode.DownArrow, Button.DOWN);
                GetInput(KeyCode.LeftArrow, Button.LEFT);
                GetInput(KeyCode.RightArrow, Button.RIGHT);

                bool l_horiz = GetInput(KeyCode.A, Button.L_HORIZ, -STICK_INT);
                if (!l_horiz)
                    l_horiz = GetInput(KeyCode.D, Button.L_HORIZ, STICK_INT);
                bool l_vert = GetInput(KeyCode.W, Button.L_VERT, STICK_INT);
                if (!l_vert)
                    l_vert = GetInput(KeyCode.S, Button.L_VERT, -STICK_INT);
                // If two directions are being held down at the same time,
                // the diagonal speed needs to be nerfed to match a gamepad's
                // stick.
                if (l_horiz && l_vert)
                {
                    int l_horiz_sign = padInput[(int) Button.L_HORIZ] > 0
                        ? 1 : -1;
                    padInput[(int) Button.L_HORIZ]
                        = (int) ((Mathf.Sin(Mathf.PI / 4)
                        * STICK_INT) * l_horiz_sign);
                    int l_vert_sign = padInput[(int) Button.L_VERT] > 0
                        ? 1 : -1;
                    padInput[(int) Button.L_VERT]
                        = (int) ((Mathf.Sin(Mathf.PI / 4)
                        * STICK_INT) * l_vert_sign);
                }
            }
            return padInput;
        }
    }
    public Gamepad(bool isKeyboard)
    {
        this.isKeyboard = isKeyboard;
        padInput = new int[(int) Button.COUNT];
    }

    /// <summary>Updates the array padInput with the info of whether the key is
    /// pressed, being held down, or depressed.</summary>
    /// <returns>True if the key was pressed or depressed this frame.
    /// </returns>
    private bool GetInput(KeyCode keyCode, Button button, int mult = 1)
    {
        // mult != 1 when it's the stick
        if ((mult != 1 && Input.GetKey(keyCode))
            || Input.GetKeyDown(keyCode))
        {
            padInput[(int) button] = mult;
            return true;
        }
        else if (Input.GetKeyUp(keyCode) && mult == 1)
        {
            padInput[(int) button] = -mult;
            return true;
        }
        
        padInput[(int) button] = 0;
        return false;
    }

    public enum Button { DEBUG, START, SELECT, A, B, X, Y, UP, DOWN, LEFT,
        RIGHT, L_BUMP, R_BUMP, L_TRIG, R_TRIG, L_HORIZ, L_VERT, R_HORIZ,
        R_VERT, COUNT }
}
