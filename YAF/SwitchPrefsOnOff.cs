using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwitchPrefsOnOff : MonoBehaviour
{
    public GameObject preferences;
    public GameObject preferencesIcon;
    private MainApp mainScript;

    public SwitchHelpOnOff switchHelp;
    public GameObject helpIcon;

    private Toggle ourToggle;

    // Start is called before the first frame update
    void Start()
    {
        GameObject mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        mainScript = mainCamera.GetComponent<MainApp>();

        ourToggle = GetComponent<Toggle>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void hideShowPrefs(bool isOn)
    {
        if (isOn)
        {
            switchHelp.anotherWasCalled();
        }

        preferences.SetActive(isOn);

        mainScript.gameIsInPause = isOn;
    }

    public void hideShowPrefsFromOthers(bool isOn)
    {
        hideShowPrefs(!isOn);
    }

    // Hide the preference but keep it active (the game is still paused then)
    public void hideMe()
    {
        preferences.SetActive(false);
        preferencesIcon.SetActive(false);
        helpIcon.SetActive(false);
    }

    // Show the preference but keep it active (the game is still paused then)
    public void showMe()
    {
        preferences.SetActive(true);
        preferencesIcon.SetActive(true);
        helpIcon.SetActive(true);
    }

    public void anotherWasCalled()
    {
        // Another GUI was called (on screen), see what to do.
        if (preferences.activeInHierarchy)
        {
            preferences.SetActive(false);

            ourToggle.SetIsOnWithoutNotify(false);
        }
    }
}
