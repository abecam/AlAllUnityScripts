using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchNYAFOrLaunchPage : MonoBehaviour
{
    private string rootNameOfLevel = "MainBoard";

    public void loadGame()
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
            UnityEngine.SceneManagement.SceneManager.LoadScene("LandingPage");
        }
    }
}
