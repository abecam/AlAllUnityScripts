using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SelectModes : MonoBehaviour
{
    const string rootNameOfLevel = "MainBoard";
    // We must indicate which level we are
    private CreateBoard mainScript;

    public int initialCooldown = 20; // Don't change level right away, in case some event were still there.

    public GameObject containerOfModes;

    // Start is called before the first frame update
    void Start()
    {
        GameObject camera = GameObject.FindGameObjectWithTag("MainCamera");
        mainScript = camera.GetComponent<CreateBoard>();

        GameObject ourCurrentMode = containerOfModes.transform.GetChild(mainScript.getCurrentModeNb()).gameObject;
        ourCurrentMode.GetComponent<Renderer>().material.color = new Color(0.50f, 1f, 0.58f, 1.0f);

        // Check how much level we show
        int modeReached = 0;

        bool haskey = LocalSave.HasIntKey("modeFinished");
        if (haskey)
        {
            modeReached = LocalSave.GetInt("modeFinished");
        }
        Debug.Log("Mode reached:" + modeReached);
        if (modeReached == 0)
        {
            // Hide all
            for (int iLevel = 0; iLevel < containerOfModes.transform.childCount; iLevel++)
            {
                containerOfModes.transform.GetChild(iLevel).gameObject.SetActive(false);
            }
        }
        else
        {
            //Everything above will be hidden
            for (int iLevel = modeReached + 1; iLevel < containerOfModes.transform.childCount; iLevel++)
            {
                containerOfModes.transform.GetChild(iLevel).gameObject.SetActive(false);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (mainScript.InTitle)
        {
            if (initialCooldown > 0)
            {
                initialCooldown--;
            }
            else
            {
                {
                    Vector3 touchPos = Mouse.current.position.ReadValue();

                    // Check if the user has clicked
                    bool aTouch = Mouse.current.leftButton.isPressed;

                    if (aTouch)
                    {
                        // Debug.Log( "Moused moved to point " + touchPos );

                        checkBoard(touchPos);
                    }
                    else
                    {
                        // Check if we hover on an element.
                        checkHover(touchPos);
                    }
                }
            }
        }
    }

    void checkBoard(Vector3 touchPos)
    {
        Vector3 wp = Camera.main.ScreenToWorldPoint(touchPos);
        Vector2 touchPos2 = new Vector2(wp.x, wp.y);

        int iMode = 0;

        foreach (Transform childCharacter in containerOfModes.transform)
        {
            if (childCharacter.GetComponent<Collider2D>() == Physics2D.OverlapPoint(touchPos2))
            {
                if (iMode != mainScript.getCurrentModeNb())
                {
                    // Change the mode and reload the current scene
                    mainScript.setCurrentModeNb(iMode);

                    int currentLevel = mainScript.currentLevel;

                    if (currentLevel != 1)
                    {
                        Debug.Log("Loading level "+currentLevel+" with mode " + iMode);
                        UnityEngine.SceneManagement.SceneManager.LoadScene(rootNameOfLevel + currentLevel);
                    }
                    else
                    {
                        Debug.Log("Loading first level with mode " + iMode);
                        // First level has no number
                        UnityEngine.SceneManagement.SceneManager.LoadScene(rootNameOfLevel);
                    }
                }
            }
            iMode++;
        }
    }

    void checkHover(Vector3 touchPos)
    {
        Vector3 wp = Camera.main.ScreenToWorldPoint(touchPos);
        Vector2 touchPos2 = new Vector2(wp.x, wp.y);

        int iMode = 0;

        foreach (Transform childCharacter in containerOfModes.transform)
        {
            if (iMode != mainScript.getCurrentModeNb())
            {
                if (childCharacter.GetComponent<Collider2D>() == Physics2D.OverlapPoint(touchPos2))
                {
                    childCharacter.GetComponent<Renderer>().material.color = new Color(0.50f, 0.5f, 1f, 1.0f);
                }
                else
                {
                    childCharacter.GetComponent<Renderer>().material.color = new Color(1f, 1f, 1f, 1.0f);
                }
            }
            iMode++;
        }
    }
}
