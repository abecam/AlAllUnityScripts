using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoomAndPanForeground : MonoBehaviour
{
    float timeBeforeMove = 5;
    float timeToMove = 10;
    float currentTime = 0;

    float zoomFactor = 0.15f;
    float panFactor = 0.2f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        currentTime += Time.deltaTime;

        if (currentTime > timeBeforeMove)
        {
            if (currentTime < timeToMove)
            {
                float newZoom = Time.deltaTime * zoomFactor;
                float newPan = Time.deltaTime * panFactor;

                transform.localPosition = new Vector3(transform.localPosition.x + newPan, transform.localPosition.y - newPan, transform.localPosition.z);
                transform.localScale = new Vector3(transform.localScale.x + newZoom, transform.localScale.y + newZoom, transform.localScale.z + newZoom);
            }
        }
    }
}
