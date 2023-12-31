using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadBJSCredits : MonoBehaviour
{
    const string nameOfBJS = "BJSCredits";

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void loadBJSGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(nameOfBJS);
    }
}
