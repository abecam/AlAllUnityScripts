using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchBJSTutorial : MonoBehaviour
{
    public void loadTutorial()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("TutorialBJS");
    }
}
