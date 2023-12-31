using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchNYAF : MonoBehaviour
{
    const string nameOfNYAF = "MainBoard";

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void loadNYAFGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(nameOfNYAF);
    }

    public void loadNYAFGameNonStatic()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(nameOfNYAF);
    }
}
