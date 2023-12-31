using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectFindingNyafingsLevels : MonoBehaviour
{
    const string rootNameOfLevel = "FindingNYAFINGS/MainScene";

    public GameObject containerOfLevels;

    private int currentLevel = 0;

    private PortalScript.typeOfScene forTypeOfScene = PortalScript.typeOfScene.groundNormal;

    void Start()
    {
        if (LocalSave.HasIntKey("FindingNYAF_currentLevel"))
        {
            currentLevel = LocalSave.GetInt("FindingNYAF_currentLevel");
        }

        Debug.Log("We are at level " + currentLevel);

        Button ourCurrentLevel = containerOfLevels.transform.GetChild(currentLevel).GetComponent<Button>();
        Image ourCurrentLevelImage = containerOfLevels.transform.GetChild(currentLevel).GetComponent<Image>();
        ourCurrentLevelImage.color = new Color(0.50f, 1f, 0.58f, 1.0f);
        ourCurrentLevel.interactable = false;
    }

    public void selectAndRun(int level)
    {
        if (level == 11 || level == 12)
        {
            forTypeOfScene = PortalScript.typeOfScene.undersea;
        }
        if (level != currentLevel)
        {
            switch (forTypeOfScene)
            {
                case PortalScript.typeOfScene.groundNormal:
                    LocalSave.SetInt("FindingNYAF_currentLevel", level);
                    LocalSave.Save();
                    UnityEngine.SceneManagement.SceneManager.LoadScene("FindingNYAFINGS/MainScene");
                    break;
                case PortalScript.typeOfScene.spaceNormal:
                    LocalSave.SetInt("MovingNYAF_currentLevel", level);
                    LocalSave.Save();
                    UnityEngine.SceneManagement.SceneManager.LoadScene("FindingNYAFINGS/MovingNyafings");
                    break;
                case PortalScript.typeOfScene.asteroidField:
                    LocalSave.SetInt("MovingNYAF_currentLevel", level);
                    LocalSave.Save();
                    UnityEngine.SceneManagement.SceneManager.LoadScene("FindingNYAFINGS/AsteroidsField");
                    break;
                case PortalScript.typeOfScene.underground:
                    LocalSave.SetInt("FindingNYAF_currentLevel", level);
                    LocalSave.Save();
                    UnityEngine.SceneManagement.SceneManager.LoadScene("FindingNYAFINGS/Underground");
                    break;
                case PortalScript.typeOfScene.undersea:
                    LocalSave.SetInt("FindingNYAF_currentLevel", level);
                    LocalSave.Save();
                    UnityEngine.SceneManagement.SceneManager.LoadScene("FindingNYAFINGS/Undersea2");
                    break;
                case PortalScript.typeOfScene.oversea:
                    UnityEngine.SceneManagement.SceneManager.LoadScene("FindingNYAFINGS/Sea");
                    break;
                case PortalScript.typeOfScene.swamp:
                    LocalSave.SetInt("FindingNYAF_currentLevel", level);
                    LocalSave.Save();
                    UnityEngine.SceneManagement.SceneManager.LoadScene("FindingNYAFINGS/Swamp");
                    break;
                case PortalScript.typeOfScene.halloween:
                    LocalSave.SetInt("FindingNYAF_currentLevel", level);
                    LocalSave.Save();
                    UnityEngine.SceneManagement.SceneManager.LoadScene("FindingNYAFINGS/Halloween");
                    break;
            }
        }
    }
}
