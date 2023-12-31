using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchHelpOff : MonoBehaviour
{
    private SwitchHelpOnOff switchHelp;

    // Start is called before the first frame update
    void Start()
    {
        GameObject helpIcon = GameObject.FindGameObjectWithTag("HelpSwitch");
        switchHelp = helpIcon.GetComponent<SwitchHelpOnOff>();
    }

    public void switchOff()
    {
        switchHelp.anotherWasCalled();
        switchHelp.unpauseMainGame();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
