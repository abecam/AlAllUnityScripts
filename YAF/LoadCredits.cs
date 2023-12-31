using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadCredits : MonoBehaviour
{
    public void Load()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("CreditsYAF");
    }
}
