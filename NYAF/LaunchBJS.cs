using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LaunchBJS : MonoBehaviour
{
    public int initialCooldown = 60; // Don't change level right away, in case some event were still there.

    const string nameOfBJS = "BJS";

    private CreateBoard mainScript;

    // Start is called before the first frame update
    void Start()
    {
        GameObject camera = GameObject.FindGameObjectWithTag("MainCamera");
        mainScript = camera.GetComponent<CreateBoard>();
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



        if (GetComponent<Collider2D>() == Physics2D.OverlapPoint(touchPos2))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(nameOfBJS);
        }
    }

    void checkHover(Vector3 touchPos)
    {
        Vector3 wp = Camera.main.ScreenToWorldPoint(touchPos);
        Vector2 touchPos2 = new Vector2(wp.x, wp.y);

        if (GetComponent<Collider2D>() == Physics2D.OverlapPoint(touchPos2))
        {
            GetComponent<Renderer>().material.color = new Color(0.50f, 0.5f, 1f, 1.0f);
        }
        else
        {
            GetComponent<Renderer>().material.color = new Color(1f, 1f, 1f, 1.0f);
        }

    }
}
