using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeLightUnderground : MonoBehaviour
{
    private Light ourLight;

    private Vector3 prevLight;
    private Vector3 nextLight;

    public int interpolationFramesCount = 20; // Number of frames to completely interpolate between the 2 positions
    int elapsedFrames = 0;

    public bool underWater = false;

    // Start is called before the first frame update
    void Start()
    {
        ourLight = GetComponent<Light>();

        setNextLight();
    }

    private void setNextLight()
    {
        prevLight = new Vector3(ourLight.color.r, ourLight.color.g, ourLight.color.b);
        nextLight = new Vector3(0.8f + Random.value * 0.2f, 0.8f + Random.value * 0.2f, 0.5f + Random.value * 0.2f);

        if (underWater)
        {
            prevLight = new Vector3(ourLight.color.r, ourLight.color.g, ourLight.color.b);
            nextLight = new Vector3(0.2f + Random.value * 0.4f, 0.2f + Random.value * 0.4f, 0.6f + Random.value * 0.2f);

        }
    }

    // Update is called once per frame
    void Update()
    {
        float interpolationRatio = (float)elapsedFrames / interpolationFramesCount;

        Vector3 currentColor = Vector3.Lerp(prevLight, nextLight, interpolationRatio);

        ourLight.color = new Color(currentColor.x, currentColor.y, currentColor.z);
        //ourLight.intensity = 0.1f + Random.value * 0.4f;

        elapsedFrames = (elapsedFrames + 1) % (interpolationFramesCount + 1);  // reset elapsedFrames to zero after it reached (interpolationFramesCount + 1)
        if (elapsedFrames == 0)
        {
            setNextLight();
        }
    }
}
