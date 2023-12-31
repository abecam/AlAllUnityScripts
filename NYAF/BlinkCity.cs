using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkCity : MonoBehaviour
{
    // Start is called before the first frame update
    private SpriteRenderer ourRenderer;

    void Start()
    {
        ourRenderer = GetComponent<SpriteRenderer>();
    }

    float currentTime = 0;
    const float timeBetweenBlink = 5;
    bool isOn = false;
    // Update is called once per frame
    void Update()
    {
        currentTime += Time.deltaTime;

        if (currentTime > timeBetweenBlink)
        {
            currentTime = 0;

            isOn = !isOn;

            ourRenderer.enabled = isOn;
        }
    }
}
