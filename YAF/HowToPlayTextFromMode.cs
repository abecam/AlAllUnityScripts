using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HowToPlayTextFromMode : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // Check how much level we show
        int currentMode = 0;

        bool haskey = LocalSave.HasIntKey("currentMode");
        if (haskey)
        {
            currentMode = LocalSave.GetInt("currentMode");
        }

       // Set the text depending on the mode
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
