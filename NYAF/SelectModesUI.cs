using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectModesUI : MonoBehaviour
{
    const string rootNameOfLevel = "MainBoard";
    // We must indicate which level we are
    private CreateBoard mainScript;

    public GameObject containerOfModes;

    public bool isYANYAF = false;
    private ManageYANYAF mainScriptYANYAF;
    const string rootNameOfLevelYANYAF = "YANYAF";

    int currentLevel;

    // Start is called before the first frame update
    void Start()
    {
        int currentMode = 0;

        if (!isYANYAF)
        {
            GameObject mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            mainScript = mainCamera.GetComponent<CreateBoard>();
            currentLevel = mainScript.currentLevel;

            currentMode = mainScript.getCurrentModeNb();

        }
        else
        {
            GameObject mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            mainScriptYANYAF = mainCamera.GetComponent<ManageYANYAF>();
            currentLevel = mainScriptYANYAF.currentLevel;

            currentMode = mainScriptYANYAF.getCurrentModeNb();

        }

        // Check how much level we show
        {

            Button ourCurrentLevel = containerOfModes.transform.GetChild(currentMode).GetChild(0).GetComponent<Button>();
            Image ourCurrentLevelImage = containerOfModes.transform.GetChild(currentMode).GetChild(0).GetComponent<Image>();

            ourCurrentLevel.interactable = false;
            //ourCurrentLevelImage.color = new Color(0.50f, 1f, 0.58f, 1.0f);
        }
        //if (!isYANYAF)
        //{
        //    // Check how much level we show
        //    int modeReached = 0;

        //    bool haskey = LocalSave.HasIntKey("modeFinished");
        //    if (haskey)
        //    {
        //        modeReached = LocalSave.GetInt("modeFinished");
        //    }
        //    Debug.Log("Mode reached:" + modeReached);
        //    if (modeReached == 0)
        //    {
        //        // Hide all
        //        for (int iLevel = 0; iLevel < containerOfModes.transform.childCount; iLevel++)
        //        {
        //            Button ourCurrentLevel = containerOfModes.transform.GetChild(iLevel).GetComponent<Button>();
        //            Image ourCurrentLevelImage = containerOfModes.transform.GetChild(iLevel).GetComponent<Image>();

        //            ourCurrentLevel.interactable = false;
        //            ourCurrentLevelImage.color = new Color(0.2f, 0.2f, 0.2f, 0.2f);
        //        }
        //    }
        //    else
        //    {
        //        //Everything above will be hidden
        //        for (int iLevel = modeReached + 1; iLevel < containerOfModes.transform.childCount; iLevel++)
        //        {
        //            Button ourCurrentLevel = containerOfModes.transform.GetChild(iLevel).GetComponent<Button>();
        //            Image ourCurrentLevelImage = containerOfModes.transform.GetChild(iLevel).GetComponent<Image>();

        //            ourCurrentLevel.interactable = false;
        //            ourCurrentLevelImage.color = new Color(0.2f, 0.2f, 0.2f, 0.2f);
        //        }
        //    }
        //}
    }

    public void selectAndRun(int mode)
    {
        if (!isYANYAF)
        {
            mainScript.setCurrentModeNb(mode);
        
            if (currentLevel != 1)
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(rootNameOfLevel + currentLevel);
            }
            else
            {
                // First level has no number
                UnityEngine.SceneManagement.SceneManager.LoadScene(rootNameOfLevel);
            }
        }
        else
        {
            // -1 is for regenerating the background image.
            if (mode != -1)
            {
                mainScriptYANYAF.setCurrentModeNb(mode);
            }
            // Just reload the same level
            UnityEngine.SceneManagement.SceneManager.LoadScene(rootNameOfLevelYANYAF);
        }
    }
}
