using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasScript : MonoBehaviour
{
    [SerializeField]
    private RectTransform reticle, darkScreen;
    public RectTransform Reticle { get { return reticle; } }
    public RectTransform DarkScreen { get { return darkScreen; } }

    private void Start()
    {
        foreach (Transform child in GetComponent<Transform>())
        {
            if (child.gameObject.name == "Reticle")
                reticle = child.gameObject.GetComponent<RectTransform>();
            else if (child.gameObject.name == "Dark Screen")
                darkScreen = child.gameObject.GetComponent<RectTransform>();
        }
    }
}
