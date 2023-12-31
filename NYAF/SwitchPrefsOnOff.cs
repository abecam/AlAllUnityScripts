using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwitchPrefsOnOff : MonoBehaviour
{
    private GameObject preferences;
    private GameObject paperSave;
    public GameObject preferencesIcon;
    private MainApp mainScript;

    private SwitchLevelModeSelOnOff switchLevel;
    private SwitchHelpOnOff         switchHelp;
    public GameObject helpIcon;

    private Toggle ourToggle;

    // Start is called before the first frame update
    void Start()
    {
        GameObject mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        mainScript = mainCamera.GetComponent<MainApp>();

        GameObject preferencesParent = GameObject.FindGameObjectWithTag("PrefPanel");
        preferences = preferencesParent.transform.Find("PanelPrefs").gameObject;

        paperSave = preferencesParent.transform.Find("PanelCodes").gameObject;

        GameObject modeIcon = GameObject.FindGameObjectWithTag("ModeSwitch");
        switchLevel = modeIcon.GetComponent<SwitchLevelModeSelOnOff>();

        helpIcon = GameObject.FindGameObjectWithTag("HelpSwitch");
        switchHelp = helpIcon.GetComponent<SwitchHelpOnOff>();

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
            switchLevel.anotherWasCalled();
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
            paperSave.SetActive(false);

            ourToggle.SetIsOnWithoutNotify(false);
        }
    }
}
