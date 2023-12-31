using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwitchHelpOnOff : MonoBehaviour
{
    public GameObject modeLevelSelection;
    private GameObject instanceOfSelection;
    public GameObject modeSelIcon;
    private MainApp mainScript;

    public SwitchPrefsOnOff preferences;

    private bool isShown = false;

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

    public bool isOn()
    {
        return isShown;
    }

    public void hideShowPrefs(bool isOn)
    {
        if (isOn)
        {
            preferences.anotherWasCalled();

            instanceOfSelection = Instantiate(modeLevelSelection);

            isShown = true;
        }
        else
        {
            Destroy(instanceOfSelection);

            isShown = false;
        }

        mainScript.gameIsInPause = isOn;

    }

    public void anotherWasCalled()
    {
        // Another GUI was called (on screen), see what to do.
        if (isShown)
        {
            Destroy(instanceOfSelection);

            isShown = false;

            ourToggle.SetIsOnWithoutNotify(false);
        }
    }

    public void unpauseMainGame()
    {
        mainScript.gameIsInPause = false;
    }

    // Hide the preference but keep it active (the game is still paused then)
    public void hideMe()
    {
        modeSelIcon.SetActive(false);
    }

    // Show the preference but keep it active (the game is still paused then)
    public void showMe()
    {
        modeSelIcon.SetActive(true);
    }
}
