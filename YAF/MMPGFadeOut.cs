using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MMPGFadeOut : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (fading > 0)
        {
            fading--;
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    int fading = 0;
    const int fadingTime = 120;

    void OnEnable()
    {
        fading = fadingTime;
    }
}
