using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SmallFindable : MonoBehaviour
{
    public GameObject me;

    public CreateBoard mainScript;

    public GameObject particleFoundChar;

    public GameObject particlesContainer;

    // Start is called before the first frame update
    void Start()
    {
        // Check if we have been found before, and if so removed ourself.
        if (mainScript.wasCurrentSmallFound())
        {
            Destroy(me);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Only if the game is on
        if (!mainScript.gameIsInPause)
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
            }
        }
    }

    void checkBoard(Vector3 touchPos)
    {
        Vector3 wp = Camera.main.ScreenToWorldPoint(touchPos);
        Vector2 touchPos2 = new Vector2(wp.x, wp.y);

        if (me.GetComponent<Collider2D>() == Physics2D.OverlapPoint(touchPos2))
        {
            mainScript.smallFound();

            showSomething();

            Destroy(me);
        }
    }

    private void showSomething()
    {
        if (me.GetComponent<AudioSource>() != null)
        {
            ((AudioSource)me.GetComponent<AudioSource>()).Play();
        }
        GameObject newFoundParticle = (GameObject)Instantiate(particleFoundChar, me.transform.position, Quaternion.identity);

        newFoundParticle.transform.SetParent(particlesContainer.transform);
    }
}
