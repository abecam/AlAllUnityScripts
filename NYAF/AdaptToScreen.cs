using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdaptToScreen : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        float width = getScreenWidth();
        float height = getScreenHeight();

        Debug.Log("Width is " + width);
        Debug.Log("Height is " + height);

        // Plane is size 10! (default Unity)
        if (width / 10 < height / 14)
        {
            transform.localScale = new Vector3(width/10, width / 10, width / 10);
        }
        else
        {
            transform.localScale = new Vector3(height / 14, height / 14, height / 14);
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

    int iUpdate = 0;

    // Update is called once per frame
    void Update()
    {
        if (iUpdate++ % 30 == 0 && Camera.main != null)
        {
            float width = getScreenWidth();
            float height = getScreenHeight();

            if (width/10 < height/14)
            {
                transform.localScale = new Vector3(width / 10, width / 10, width / 10);
            }
            else
            {
                transform.localScale = new Vector3(height / 14, height / 14, height / 14);
            }
        }
    }
}
