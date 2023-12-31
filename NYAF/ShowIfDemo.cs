using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowIfDemo : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (!InitializeSteam.inDemo)
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
