using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BellDetached : MonoBehaviour
{
    public BJSMainLoop theMainScript;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnJointBreak2D(Joint2D joint)
    {
        Debug.Log("Something broke!");

        theMainScript.bellBroke();
    }
}
