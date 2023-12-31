using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoomTitle : MonoBehaviour
{
    const float timeBeforeMove = 0;
    const float timeToMove = 10;
    float currentTime = 0;

    const float zoomFactor = 0.3f;
    const float panFactor = 0.0f;

    const float timeBeforeFade = 5;
    const float fadeFactor = 1 / (timeToMove / 1.5f);
    float ourAlpha = 0;
    Material ourMaterial;

    // Start is called before the first frame update
    void Start()
    {
        transform.localScale = new Vector3(4, 4, 4);
        Renderer ourRenderer = GetComponent<Renderer>();

        ourMaterial = ourRenderer.material;

        ourMaterial.color = new Color(1, 1, 1, 0);
    }

    // Update is called once per frame
    void Update()
    {
        currentTime += Time.deltaTime;

        if (currentTime > timeBeforeFade)
        {
            if (ourAlpha < 1)
            {
                ourAlpha = ourAlpha + fadeFactor * Time.deltaTime;

                if (ourAlpha >= 1)
                {
                    ourAlpha = 1;
                }

                ourMaterial.color = new Color(1, 1, 1, ourAlpha);
            }
        }
        if (currentTime > timeBeforeMove)
        { 
            if (currentTime < timeToMove)
            {
                float newZoom = Time.deltaTime * zoomFactor;
                float newPan = Time.deltaTime * panFactor;

                transform.localPosition = new Vector3(transform.localPosition.x + newPan, transform.localPosition.y - newPan, transform.localPosition.z);
                transform.localScale = new Vector3(transform.localScale.x - newZoom, transform.localScale.y - newZoom, transform.localScale.z - newZoom);
            }
        }
    }
}
