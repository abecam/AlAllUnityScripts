using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchMovingNyaf : MonoBehaviour
{
    const string nameOfScene = "FindingNYAFINGS/MovingNyafings";

    public void loadMovingNYAF()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(nameOfScene);
    }

    public static void loadMovingNYAFStatic()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(nameOfScene);
    }
}
