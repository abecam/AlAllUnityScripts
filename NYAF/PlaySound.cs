using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlaySound : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
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

    void checkBoard(Vector3 touchPos)
    {
        Vector3 wp = Camera.main.ScreenToWorldPoint(touchPos);
        Vector2 touchPos2 = new Vector2(wp.x, wp.y);

        if (GetComponent<Collider2D>() == Physics2D.OverlapPoint(touchPos2))
        {
            showSomething();
        }
    }

    private void showSomething()
    {
        if (GetComponent<AudioSource>() != null)
        {
            (GetComponent<AudioSource>()).Play();
        }
    }
}
