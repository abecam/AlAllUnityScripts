using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowCredits : MonoBehaviour
{
    public GameObject thisCamera;

    public GameObject titleLine;
    public GameObject nameLine;

    public GameObject collectionOfSprites;
    public GameObject parentOfSprites;

    public GameObject closeCreditObject; // For mobile, provide a button to close the credits

    private GameObject sprite = null;

    //bool showCredit;

    List<string> allTitlesAndNames = new List<string>()
        {
           "YAF","Yet Another Freemium",
           "A Game by","Alain Becam",
           "Graphics by","Sébastien Lesage",
           "Drums", "William",
           "Music by","Chan Redfield",
           "Font", "GlametrixBold & SudegnakNo3\nby Gluk\nhttp://www.glukfonts.pl",
           "Special Thanks to",
           "Your purchases!"
        };


    const int nbOfFramesForText = 400;
    const int offsetForText = 100;
    const int offsetForSprite = 200;
    const int nbOfFramesForSprite = 200;
    private readonly float stepZoomTitle = 0.02f / ((float)nbOfFramesForText);
    private readonly float stepZoomName = 0.02f / ((float)(nbOfFramesForText - 50));

    int currentFrameForText = 0;
    int currentFrameForSprite = 0;

    private readonly float stepZoomSprite = 3f / ((float)nbOfFramesForSprite);

    float currentZoomTitle = 0;
    float currentZoomName = 0;
    float currentZoomSprite = 0;

    int currentLine = 0;
    int currentSprite = 0;


    // Start is called before the first frame update
    void Start()
    {
        currentFrameForText = 0;
        currentFrameForSprite = 0;

        currentZoomTitle = 0;
        currentZoomName = 0;
        currentZoomSprite = 0;

        currentLine = 0;

        currentSprite = 0;

        setUpLinesAndSprite();
    }

    // Update is called once per frame
    void Update()
    {
        checkForExit();

        // For each 2 line:
        // Zoom the first one, then the 2nd one
        if (currentFrameForText < nbOfFramesForText)
        {
            currentZoomTitle += stepZoomTitle;
            titleLine.transform.localScale = new Vector3(currentZoomTitle, currentZoomTitle);

            if (currentFrameForText > offsetForText)
            {
                currentZoomName += stepZoomName;
                nameLine.transform.localScale = new Vector3(currentZoomName, currentZoomName);
            }

            currentFrameForText++;
        }
        if (currentFrameForText > offsetForSprite && currentFrameForSprite < nbOfFramesForSprite)
        {
            currentZoomSprite += stepZoomSprite;
            sprite.transform.localScale = new Vector3(currentZoomSprite, currentZoomSprite);

            currentFrameForSprite++;
        }
        if (currentFrameForSprite >= nbOfFramesForSprite)
        {
            Destroy(sprite);

            // Start again
            setUpLinesAndSprite();
        }
        // Then zoom the next sprite until it covers everything
    }

    private void setUpLinesAndSprite()
    {
        currentFrameForText = 0;
        currentFrameForSprite = 0;

        currentZoomTitle = 0;
        currentZoomName = 0;
        currentZoomSprite = 0;

        if (allTitlesAndNames.Count <= currentLine)
        {
            // Back to start
            currentLine = 0;
        }
        String title = allTitlesAndNames[currentLine++];
        String name = allTitlesAndNames[currentLine++];

        ((TextMesh)titleLine.GetComponent("TextMesh")).text = title;
        ((TextMesh)nameLine.GetComponent("TextMesh")).text = name;
        titleLine.transform.localScale = new Vector3(0.0f, 0.0f);
        nameLine.transform.localScale = new Vector3(0.0f, 0.0f);

        if (currentSprite >= collectionOfSprites.transform.childCount)
        {
            currentSprite = 0;
        }
        Transform spriteToInstanciate = collectionOfSprites.transform.GetChild(currentSprite++);

        sprite = (GameObject)Instantiate(spriteToInstanciate.gameObject);
        sprite.transform.SetParent(parentOfSprites.transform);

        sprite.transform.localPosition = new Vector3(0.0f, 0.0f, -1);
        sprite.transform.localScale = new Vector3(0.0f, 0.0f);    
    }

    private void checkForExit()
    {
        if (thisCamera.activeSelf)
        {
            if (Input.GetKeyDown("escape") || Input.GetKey(KeyCode.Escape))
            {
                hideCredits();
            }
            else if (Application.platform != RuntimePlatform.Android)
            {
                // use the input stuff
                if (Input.GetMouseButton(0))
                {
                    hideCredits();
                }
                // Check if the user has clicked
                bool aTouch = Input.GetMouseButtonDown(0);
                bool rightClick = Input.GetMouseButton(1);

                if (aTouch)
                {
                    hideCredits();
                }

                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    hideCredits();
                }
            }
            else
            {
                if (Input.touchCount >= 1)
                {
                    Touch firstFinger = Input.GetTouch(0);

                    //if (Input.touchCount == 1)
                    //{
                    //    Vector3 touchPos = firstFinger.position;

                    //    if (firstFinger.phase != TouchPhase.Moved)
                    //    {
                    //        hideCredits();
                    //    }
                    //} else
                    if (Input.touchCount == 2)
                    {
                        hideCredits();
                    }
                }
            }
        }
    }

    public void hideCredits()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainBoard");
    }
}
