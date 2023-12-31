using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SwitchHelpOff : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }

    public void switchOff()
    {
        GameObject helpIcon = GameObject.FindGameObjectWithTag("HelpSwitch");
        SwitchHelpOnOff switchHelp = helpIcon.GetComponent<SwitchHelpOnOff>();

        switchHelp.anotherWasCalled();
        switchHelp.unpauseMainGame();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
