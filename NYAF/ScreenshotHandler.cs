using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ScreenshotHandler : MonoBehaviour
{
    public Text infoText;
    public Text infoText2;

    // Start is called before the first frame update
    void Start()
    {
        infoText.color = new Color(infoText.color.r, infoText.color.g, infoText.color.b, 0);
        infoText2.color = new Color(infoText2.color.r, infoText2.color.g, infoText2.color.b, 0);
    }

    int nbShowInfoText = -1;

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.f2Key.wasPressedThisFrame) // forward
        {
            string currentTime = System.DateTime.Now.ToString("yyyyMMdd_HH_mm_ss");
            ScreenCapture.CaptureScreenshot("Nyaf-"+ currentTime);

            showInfoText();
        }

        if (nbShowInfoText > 0)
        {
            nbShowInfoText--;

            float alphaTarget = ((float )nbShowInfoText) / 100;
            infoText.color = new Color(infoText.color.r, infoText.color.g, infoText.color.b, alphaTarget);
            infoText2.color = new Color(infoText2.color.r, infoText2.color.g, infoText2.color.b, alphaTarget / 1.5f);
        }
    }

    private void showInfoText()
    {
        infoText.color = new Color(infoText.color.r, infoText.color.g, infoText.color.b, 0.6f);
        infoText2.color = new Color(infoText2.color.r, infoText2.color.g, infoText2.color.b, 0.6f);

        nbShowInfoText = 60;
    }
}
