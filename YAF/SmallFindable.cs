using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SmallFindable : MonoBehaviour
{
    public CreateBoard mainScript;

    public GameObject particleFoundChar;

    public GameObject particlesContainer;

    // Start is called before the first frame update
    void Start()
    {
        // Move the component around
        moveMe();
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
            GetAndAnimateCoin theCoinManager = Camera.main.gameObject.GetComponent<GetAndAnimateCoin>();

            double nbOfCoins = 0;

            bool hasKey = LocalSave.HasDoubleKey("Coins");
            if (hasKey)
            {
                nbOfCoins = LocalSave.GetDouble("Coins");
            }
            if (nbOfCoins > 5000)
            {
                double coinsToAdd = nbOfCoins + nbOfCoins / 10;

                LocalSave.SetDouble("Coins", coinsToAdd);

                LocalSave.Save();

                theCoinManager.gainSomeSCoins(500000, transform.localPosition.x, transform.localPosition.y);
            }
            else
            {
                nbOfCoins = 500;

                theCoinManager.gainSomeCoins(nbOfCoins, transform.localPosition.x, transform.localPosition.y);
                theCoinManager.gainSomeSCoins(nbOfCoins, transform.localPosition.x, transform.localPosition.y);
            }    

            showSomething();

            // Move the component around
            moveMe();
        }
    }

    private void moveMe()
    {
        float newPosX = Random.value * 10 - 5;
        float newPosY = Random.value * 10 - 5;

        transform.localPosition = new Vector2(newPosX, newPosY);
    }

    private void showSomething()
    {
        if (GetComponent<AudioSource>() != null)
        {
            ((AudioSource)GetComponent<AudioSource>()).Play();
        }
        GameObject newFoundParticle = (GameObject)Instantiate(particleFoundChar, transform.position, Quaternion.identity);

        newFoundParticle.transform.SetParent(particlesContainer.transform);
    }
}
