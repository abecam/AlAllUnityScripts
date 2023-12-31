using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DemoManager : MonoBehaviour
{
    public static bool isDemo = false;

    bool timeSpent = false;
    bool inDemoPage = false;

    const float maxTimeInDemo = 60 * 10; // 10 minutes
    const float tenSecondsBeforeEnds = maxTimeInDemo - 10;

    const string demoPageName = "DemoPage";

    public Text warningText;
    public Text warningTextBackground;

    // Start is called before the first frame update
    void Awake()
    {
        InitializeSteam.startSteam();

        if (InitializeSteam.steamStarted)
        {
            isDemo = InitializeSteam.inDemo;

            Debug.Log("We are in demo ? " + isDemo);

            if (isDemo)
            {
                DontDestroyOnLoad(this);
            }
            else
            {
                Destroy(this);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.realtimeSinceStartup > maxTimeInDemo && !timeSpent)
        {
            warningText.text = "";
            warningTextBackground.text = "";

            timeSpent = true;
            inDemoPage = true;

            loadDemoPage();
        }
        else if (Time.realtimeSinceStartup >= tenSecondsBeforeEnds && !inDemoPage)
        {
            int timeLeft = (int)(maxTimeInDemo - Time.realtimeSinceStartup);

            warningText.text = "End of the demo in " + timeLeft + " seconds!";
            warningTextBackground.text = "End of the demo in " + timeLeft + " seconds!";
        }
    }

    private void loadDemoPage()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(demoPageName);
    }
}
