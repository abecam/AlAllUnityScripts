using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShiftMonasterIfNeeded : MonoBehaviour
{
    // If the aspect ratio is on the wider size, shift the monster left

    // Start is called before the first frame update
    void Start()
    {
        float width = getScreenWidth();
        float height = getScreenHeight();

        if (width > height)
        {
            float ratio = width / height;

            transform.position = new Vector3(-0.23f - 2.5f * ratio, transform.position.y, transform.position.z);
        }
    }

    // Update is called once per frame
    void Update()
    {
        float width = getScreenWidth();
        float height = getScreenHeight();

        if (width > height)
        {
            float ratio = width / height;

            transform.position = new Vector3(-0.23f - 2.5f * ratio, transform.position.y, transform.position.z);
        }
        else
        {
            transform.position = new Vector3(-0.23f, transform.position.y, transform.position.z);
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
