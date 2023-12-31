using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyQuitBox : MonoBehaviour
{
    public GameObject ourBox;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void destroyOurBox()
    {
        Destroy(ourBox);
    }
}
