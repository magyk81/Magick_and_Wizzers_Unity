/* Copyright (C) All Rights Reserved
 * Unauthorized copying of this file, via any medium is prohibited.
 * Proprietary and confidential.
 * Written by Robin Campos <magyk81@gmail.com>, year 2021.
 */

using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class CanvasScript : MonoBehaviour
{
    private int camWidth, camHeight;
    [SerializeField]
    private RectTransform reticle, crosshair, darkScreen;
    public RectTransform Reticle { get { return reticle; } }
    public RectTransform DarkScreen { get { return darkScreen; } }

    /// <summary>Called once before the match begins.</summary>
    public void Init(int camWidth, int camHeight)
    {
        this.camWidth = camWidth;
        this.camHeight = camHeight;

        reticle = Instantiate(
            reticle.gameObject, GetComponent<Transform>())
            .GetComponent<RectTransform>();
        crosshair = Instantiate(
            crosshair.gameObject, GetComponent<Transform>())
            .GetComponent<RectTransform>();
        darkScreen = Instantiate(
            darkScreen.gameObject, GetComponent<Transform>())
            .GetComponent<RectTransform>();

        darkScreen.gameObject.SetActive(false);
    }

    public void SetMode(UX_Player.Mode mode)
    {
        if (mode == UX_Player.Mode.PLAIN
            || mode == UX_Player.Mode.WAYPOINT_PIECE
            || mode == UX_Player.Mode.TARGET_PIECE)
        {
            reticle.gameObject.SetActive(true);
            crosshair.gameObject.SetActive(false);
        }
        else if (mode == UX_Player.Mode.WAYPOINT_TILE
            || mode == UX_Player.Mode.TARGET_CHUNK
            || mode == UX_Player.Mode.TARGET_TILE)
        {
            reticle.gameObject.SetActive(false);
            crosshair.gameObject.SetActive(true);
        }
        else
        {
            reticle.gameObject.SetActive(false);
            crosshair.gameObject.SetActive(false);
        }

        if (mode == UX_Player.Mode.HAND
            || mode == UX_Player.Mode.DETAIL
            || mode == UX_Player.Mode.PAUSE
            || mode == UX_Player.Mode.SURRENDER)
        {
            darkScreen.gameObject.SetActive(true);
        }
        else darkScreen.gameObject.SetActive(false);
    }
}
