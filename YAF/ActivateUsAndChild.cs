using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateUsAndChild : MonoBehaviour
{
    int iTurn = 0;

    public MainApp mainScript;
    public GameObject child;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Only if the game is on
        if (!mainScript.gameIsInPause)
        {
            if (Application.platform != RuntimePlatform.Android)
            {
                Vector3 touchPos = Input.mousePosition;

                // Check if the user has clicked
                bool aTouch = Input.GetMouseButtonDown(0);

                if (aTouch)
                {
                    // Debug.Log( "Moused moved to point " + touchPos );

                    checkBoard(touchPos);
                }
            }
            else
            {
                if (Input.touchCount >= 1)
                {
                    Touch firstFinger = Input.GetTouch(0);

                    if (Input.touchCount == 1)
                    {
                        Vector3 touchPos = firstFinger.position;

                        if (firstFinger.phase != TouchPhase.Moved)
                        {
                            checkBoard(touchPos);
                        }
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
            child.SetActive(true);
        }
    }
}
