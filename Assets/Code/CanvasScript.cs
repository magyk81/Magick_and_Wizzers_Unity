/* Copyright (C) All Rights Reserved
 * Unauthorized copying of this file, via any medium is prohibited.
 * Proprietary and confidential.
 * Written by Robin Campos <magyk81@gmail.com>, year 2021.
 */

using UnityEngine;

public class CanvasScript : MonoBehaviour {
    private int mCamWidth, mCamHeight;
    [SerializeField]
    private RectTransform reticle, crosshair, darkScreen;
    
    public RectTransform Reticle { get { return reticle; } }
    public RectTransform DarkScreen { get { return darkScreen; } }

    public UX_Player.Mode Mode {
        set {
            if (value == UX_Player.Mode.PLAIN
                || value == UX_Player.Mode.WAYPOINT_PIECE
                || value == UX_Player.Mode.TARGET_PIECE) {
                reticle.gameObject.SetActive(true);
                crosshair.gameObject.SetActive(false);
            } else if (value == UX_Player.Mode.WAYPOINT_TILE
                || value == UX_Player.Mode.TARGET_CHUNK
                || value == UX_Player.Mode.TARGET_TILE) {
                reticle.gameObject.SetActive(false);
                crosshair.gameObject.SetActive(true);
            } else {
                reticle.gameObject.SetActive(false);
                crosshair.gameObject.SetActive(false);
            }

            if (value == UX_Player.Mode.HAND
                || value == UX_Player.Mode.DETAIL
                || value == UX_Player.Mode.PAUSE
                || value == UX_Player.Mode.SURRENDER) {
                darkScreen.gameObject.SetActive(true);
            } else darkScreen.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Called once before the match begins.
    /// </summary>
    public void Init(int camWidth, int camHeight) {
        mCamWidth = camWidth;
        mCamHeight = camHeight;

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
}
