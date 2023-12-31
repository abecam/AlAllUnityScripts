using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SelectLevels : MonoBehaviour
{
    const string rootNameOfLevel = "MainBoard";
    // We must indicate which level we are
    private CreateBoard mainScript;

    public int initialCooldown = 20; // Don't change level right away, in case some event were still there.

    public GameObject containerOfLevels;

    // Start is called before the first frame update
    void Start()
    {
        GameObject mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        mainScript = mainCamera.GetComponent<CreateBoard>();

        GameObject ourCurrentLevel = containerOfLevels.transform.GetChild(mainScript.currentLevel - 1).gameObject;
        ourCurrentLevel.GetComponent<Renderer>().material.color = new Color(0.50f, 1f, 0.58f, 1.0f);

        // Check how much level we show
        int levelReached = mainScript.currentLevel;

        bool haskey = LocalSave.HasIntKey("levelReached"+ mainScript.getCurrentModeFromSave());
        if (haskey)
        {
            levelReached = LocalSave.GetInt("levelReached" + mainScript.getCurrentModeFromSave());
        }

        //Everything above will be hidden
        for (int iLevel = levelReached; iLevel < containerOfLevels.transform.childCount; iLevel++)
        {
            containerOfLevels.transform.GetChild(iLevel).transform.localPosition = new Vector3(500, 0);
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

        int iLevel = 1;

        foreach (Transform childCharacter in containerOfLevels.transform)
        {
            if (childCharacter.GetComponent<Collider2D>() == Physics2D.OverlapPoint(touchPos2))
            {
                if (iLevel != mainScript.currentLevel)
                {
                    if (iLevel != 1)
                    {
                        UnityEngine.SceneManagement.SceneManager.LoadScene(rootNameOfLevel + iLevel);
                    }
                    else
                    {
                        // First level has no number
                        UnityEngine.SceneManagement.SceneManager.LoadScene(rootNameOfLevel);
                    }
                }
            }
            iLevel++;
        }
    }

    void checkHover(Vector3 touchPos)
    {
        Vector3 wp = Camera.main.ScreenToWorldPoint(touchPos);
        Vector2 touchPos2 = new Vector2(wp.x, wp.y);

        int iLevel = 1;

        foreach (Transform childCharacter in containerOfLevels.transform)
        {
            if (iLevel != mainScript.currentLevel)
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
            iLevel++;
        }
    }
}
