using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadYANYAF : MonoBehaviour
{
    const string nameOfYANYAF = "YANYAF";

    public void loadYANYAF()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(nameOfYANYAF);
    }
}
