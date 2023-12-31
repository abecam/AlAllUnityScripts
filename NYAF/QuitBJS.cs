using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class QuitBJS : MonoBehaviour
{
    private BJSMainLoop mainGameScript; // To pause gameplay

    public GameObject preferences; // To check whether we display the quit dialog or remove the preferences

    public GameObject background;
    public GameObject quitButton;
    public GameObject cancelButton;

    private bool activated = false;

    // Use this for initialization
    void Start()
    {
        GameObject mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        mainGameScript = mainCamera.GetComponent<BJSMainLoop>();
    }

    int coolDown = 0;
    const int maxCoolDown = 20;

    // Update is called once per frame
    void Update()
    {
        if (coolDown == 0 && (Keyboard.current.escapeKey.isPressed))
        {
            if (preferences != null && preferences.activeSelf)
            {
                preferences.SetActive(false);
                if (mainGameScript!= null)
                { 
                    mainGameScript.unPauseGame();
                }
            }
            else if (!activated)
            {
                background.transform.localPosition = new Vector3(0, background.transform.localPosition.y, 4);

                if (mainGameScript != null)
                {
                    mainGameScript.pauseGame();
                }

                activated = true;
            }
            else
            {
                hideMe();
            }
            coolDown = maxCoolDown;
        }
        else
        {
            if (coolDown > 0)
            {
                coolDown--;
            }
            else
            {
                coolDown = 0; // Stupidly just to be sure.
            }
        }
        if (activated)
        {
            {
                Vector3 touchPos = Mouse.current.position.ReadValue();

                // use the input stuff

                // Check if the user has clicked
                bool aTouch = Mouse.current.leftButton.isPressed;
                if (aTouch)
                {
                    // Debug.Log( "Moused moved to point " + touchPos );

                    checkBoard(touchPos);
                }

            }
        }
    }

    private void hideMe()
    {
        if (mainGameScript != null)
        {
            mainGameScript.unPauseGame();
        }
        activated = false;

        background.transform.localPosition = new Vector3(100, background.transform.localPosition.y, 100);
    }

    void checkBoard(Vector3 touchPos)
    {
        Vector3 wp = Camera.main.ScreenToWorldPoint(touchPos);
        Vector2 touchPos2 = new Vector2(wp.x, wp.y);

        // Check if we found the object
        // We have 2 and a half-> the normal one, the one in the normal ads, the one in the huge ads.

        if (quitButton.GetComponent<Collider2D>() == Physics2D.OverlapPoint(touchPos2))
        {
            Application.Quit();
        }

        if (cancelButton.GetComponent<Collider2D>() == Physics2D.OverlapPoint(touchPos2))
        {
            hideMe();
        }
    }
}
