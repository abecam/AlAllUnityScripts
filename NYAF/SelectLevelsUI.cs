using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectLevelsUI : MonoBehaviour
{
    const string rootNameOfLevel = "MainBoard";
    // We must indicate which level we are
    private CreateBoard mainScript;

    public GameObject containerOfLevels;

    // Start is called before the first frame update
    void Start()
    {
        GameObject mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        mainScript = mainCamera.GetComponent<CreateBoard>();

        Debug.Log("We are at level " + mainScript.currentLevel);

        Button ourCurrentLevel = containerOfLevels.transform.GetChild(mainScript.currentLevel - 1).GetComponent<Button>();
        Image ourCurrentLevelImage = containerOfLevels.transform.GetChild(mainScript.currentLevel - 1).GetComponent<Image>();
        ourCurrentLevelImage.color = new Color(0.50f, 1f, 0.58f, 1.0f);
        ourCurrentLevel.interactable = false;

        // Check how much level we show
        int levelReached = mainScript.currentLevel;

        bool haskey = LocalSave.HasIntKey("levelReached"+ mainScript.getCurrentModeFromSave());
        if (haskey)
        {
            levelReached = LocalSave.GetInt("levelReached" + mainScript.getCurrentModeFromSave());
        }

        //Everything above will be hidden
        for (int iLevel = levelReached; iLevel < containerOfLevels.transform.childCount; iLevel++)
        {
            containerOfLevels.transform.GetChild(iLevel).GetComponent<Image>().color = new Color(0.2f, 0.2f, 0.2f, 0.2f);
            containerOfLevels.transform.GetChild(iLevel).GetComponent<Button>().interactable = false;
        }

        // Last is BJS

    }

    public void selectAndRun(int level)
    {
        if (level != 1)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(rootNameOfLevel + level);
        }
        else
        {
            // First level has no number
            UnityEngine.SceneManagement.SceneManager.LoadScene(rootNameOfLevel);
        }
    }
}
