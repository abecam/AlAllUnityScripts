using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchNYAF2D : MonoBehaviour
{
    private const string rootNameOfLevel = "MainBoard";
    const string nameOfScene = "LandingPageNyaf2D";

    public void loadNYAF2D()
    {
        loadLastLevel();
    }

    private static void loadLastLevel()
    {
        if (LocalSave.HasIntKey("currentLevel"))
        {
            int lastLevel = LocalSave.GetInt("currentLevel");

            // And load the level
            if (lastLevel == 1)
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(rootNameOfLevel);
            }
            else
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(rootNameOfLevel + lastLevel);
            }
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(nameOfScene);
        }
    }

    public static void loadNYAF2DStatic()
    {
        loadLastLevel();
    }
}
