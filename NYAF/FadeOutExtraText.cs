using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeOutExtraText : MonoBehaviour
{
    Text ourText; 
    // Start is called before the first frame update
    void Start()
    {
        ourText = gameObject.GetComponent<Text>();
    }

    int iFade = 0;
    const int timeToFade = 720;
    const float stepFade = 1 / ((float)timeToFade);

    // Update is called once per frame
    void Update()
    {
        if (iFade < timeToFade)
        {
            float lastAlpha = ourText.color.a;

            lastAlpha -= stepFade;

            if (lastAlpha < 0.2f)
            {
                lastAlpha = 0.2f;
            }
            ourText.color = new Color(ourText.color.r, ourText.color.g, ourText.color.b, lastAlpha);

            iFade++;
        }
    }
}
