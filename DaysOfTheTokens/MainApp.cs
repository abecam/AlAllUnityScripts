using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainApp : MonoBehaviour
{
    public bool gameIsInPause = false; // Allow to not take any more selection, like when a menu is displayeds

    public void setInpause(bool inPause)
    {
        gameIsInPause = inPause;
    }

}
