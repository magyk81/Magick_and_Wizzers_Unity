/* Copyright (C) All Rights Reserved
 * Unauthorized copying of this file, via any medium is prohibited.
 * Proprietary and confidential.
 * Written by Robin Campos <magyk81@gmail.com>, year 2021.
 */

using UnityEngine;

namespace Matches {
    public class CanvasScript : MonoBehaviour {
        private int mCamWidth, mCamHeight;
        [SerializeField]
        private RectTransform reticle, crosshair, darkScreen;
        
        public RectTransform Reticle { get { return reticle; } }
        public RectTransform DarkScreen { get { return darkScreen; } }

        /// <summary>
        /// Called once before the match begins.
        /// </summary>
        public void Init(int camWidth, int camHeight) {
            mCamWidth = camWidth;
            mCamHeight = camHeight;

            reticle = Instantiate(
                reticle.gameObject, GetComponent<Transform>())
                .GetComponent<RectTransform>();
            reticle.name = "Reticle";
            crosshair = Instantiate(
                crosshair.gameObject, GetComponent<Transform>())
                .GetComponent<RectTransform>();
            crosshair.name = "Crosshair";
            darkScreen = Instantiate(
                darkScreen.gameObject, GetComponent<Transform>())
                .GetComponent<RectTransform>();
            darkScreen.name = "Dark Screen";

            SetCrosshair(0);
            SetDarkScreen(false);
        }

        public void SetCrosshair(int option) {
            switch (option) {
                case 0:
                    reticle.gameObject.SetActive(true);
                    crosshair.gameObject.SetActive(false);
                    break;
                case 1:
                    reticle.gameObject.SetActive(false);
                    crosshair.gameObject.SetActive(true);
                    break;
                default:
                    reticle.gameObject.SetActive(false);
                    crosshair.gameObject.SetActive(false);
                    break;
            }
        }
        public void SetDarkScreen(bool option) { darkScreen.gameObject.SetActive(option); }
    }
}