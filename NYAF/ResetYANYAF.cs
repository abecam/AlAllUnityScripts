using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetYANYAF : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ResetAllYANYAF()
    {
        // Remove all saves
        deleteAllSaves();

        // And restart YANYAF
        LaunchYANYAF.loadGame();
    }

    private void deleteAllSaves()
    {
        throw new NotImplementedException();
    }
}
