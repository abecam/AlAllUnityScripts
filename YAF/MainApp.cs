using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainApp : MonoBehaviour
{
    public bool gameIsInPause = false; // Allow to not take any more selection, like when a menu is displayeds

    protected ShowResults ourShowResult; // Script to show the results at the end of a level

    public void setInpause(bool inPause)
    {
        gameIsInPause = inPause;
    }

    public void setShowResult(ShowResults aShowResult)
    {
        ourShowResult = aShowResult;

        Debug.Log("Setting ShowResults:" + aShowResult);
    }
}
