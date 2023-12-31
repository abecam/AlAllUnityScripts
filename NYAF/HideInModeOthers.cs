using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideInModeOthers : MonoBehaviour
{
    public GameObject me;

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

        if (currentMode != 2)
        {
            me.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
