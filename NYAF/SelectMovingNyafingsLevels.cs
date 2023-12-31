using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectMovingNyafingsLevels : MonoBehaviour
{
    const string rootNameOfLevel = "FindingNYAFINGS/MovingNyafings";
    const string rootNameOfLevelAsteroid = "FindingNYAFINGS/AsteroidsField";

    public GameObject containerOfLevels;

    private int currentLevel = 0;

    void Start()
    {
        if (LocalSave.HasIntKey("MovingNYAF_currentLevel"))
        {
            currentLevel = LocalSave.GetInt("MovingNYAF_currentLevel");
        }

        Debug.Log("We are at level " + currentLevel);

        Button ourCurrentLevel = containerOfLevels.transform.GetChild(currentLevel).GetComponent<Button>();
        Image ourCurrentLevelImage = containerOfLevels.transform.GetChild(currentLevel).GetComponent<Image>();
        ourCurrentLevelImage.color = new Color(0.50f, 1f, 0.58f, 1.0f);
        ourCurrentLevel.interactable = false;
    }

    public void selectAndRun(int level)
    {
        if (level != currentLevel)
        {
            LocalSave.SetInt("MovingNYAF_currentLevel", level);
            LocalSave.Save();

            if (level != 3)
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(rootNameOfLevel);
            }
            else
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(rootNameOfLevelAsteroid);
            }
        }
    }
}
