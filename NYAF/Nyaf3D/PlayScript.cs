using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayScript : MonoBehaviour
{

    protected int shakeCamera = -1; // Shake the camera if asked to, it's up to the children to do whatever they deem adapted

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void shakeTheCamera(int shakeForTick)
    {
        shakeCamera = shakeForTick;
    }
}
