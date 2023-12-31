using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtraForLevelAndMode : MonoBehaviour
{
    private const int widthDiv = 34;
    private const int heightDiv = 34;

    public int forLevel;
    public int forMode;

    public bool adaptToScreen = true;

    void Start()
    {
        if (adaptToScreen)
        {
            float width = getScreenWidth();
            float height = getScreenHeight();

            Debug.Log("Width is " + width + " - " + width / widthDiv);
            Debug.Log("Height is " + height + " - " + height / heightDiv);

            // Plane is size 10! (default Unity)
            if (width / widthDiv < height / heightDiv)
            {
                transform.localScale = new Vector3(width / widthDiv, width / widthDiv, width / widthDiv);
            }
            else
            {
                transform.localScale = new Vector3(height / heightDiv, height / heightDiv, height / heightDiv);
            }
        }
    }

    float getScreenWidth()
    {
        return (Camera.main.orthographicSize * 2.0f * Screen.width / Screen.height);
    }

    float getScreenHeight()
    {
        return (Camera.main.orthographicSize * 2.0f);
    }
}
