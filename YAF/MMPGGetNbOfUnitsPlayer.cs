﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MMPGGetNbOfUnitsPlayer : MonoBehaviour
{
    private Text level;
    private ManageNYPB mainScript;

    // Start is called before the first frame update
    void Start()
    {
        GameObject mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        mainScript = mainCamera.GetComponent<ManageNYPB>();
        level = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        level.text = "Player\n\n"+"Hero + clones " + mainScript.CurrentNbOfClosed;
    }
}
