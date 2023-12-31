using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchMMPG : MonoBehaviour
{
    const string nameOfBJS = "NYPB";

    public void loadMMPG()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(nameOfBJS);
    }
}
