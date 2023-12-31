using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShowCredits : MonoBehaviour
{
    public GameObject thisCamera;
    private GameObject mainCamera; // When activated, we deactivate the mainCamera, then start showing the credits
    private SwitchPrefsOnOff prefGui;
    private GameObject modeLevelSelButton;

    public GameObject titleLine;
    public GameObject nameLine;

    public GameObject collectionOfSprites;
    public GameObject parentOfSprites;

    public GameObject closeCreditObject; // For mobile, provide a button to close the credits

    private GameObject sprite = null;

    //bool showCredit;

    List<string> allTitlesAndNames = new List<string>()
        {
           "NYAF","Not Yet Another Freemium",
           "A Game by","Alain Becam",
           "Graphics by","Sébastien Lesage",
           "Sounds, testing, gameplay ideas, drums", "William",
           "Music by","Chris Huelsbeck",
           "Music by","Chan Redfield",
           "Music by","LiQWYD",
           "Music by","Alexander Nakarada",
           "Font", "GlametrixBold & SudegnakNo3\nby Gluk\nhttp://www.glukfonts.pl",
           "Development, sounds, main gameplay", "Alain Becam",
           "Sounds by", "Sven",
           "Special Thanks to",
           "Alexander Zolotov\nhttps://www.warmplace.ru"
        };

    // Start is called before the first frame update
    void Start()
    {
        if (PlayAndKeepMusic.Instance != null)
        {
            PlayAndKeepMusic.Instance.Activated = false;
            PlayAndKeepMusic.Instance.stopMusic();
        }
    }

    const int nbOfFramesForText = 400;
    const int offsetForText = 100;
    const int offsetForSprite = 200;
    const int nbOfFramesForSprite = 200;
    private readonly float stepZoomTitle = 0.02f/ ((float )nbOfFramesForText);
    private readonly float stepZoomName = 0.02f / ((float)(nbOfFramesForText-50));

    int currentFrameForText = 0;
    int currentFrameForSprite = 0;

    private readonly float stepZoomSprite = 3f / ((float)nbOfFramesForSprite);

    float currentZoomTitle = 0;
    float currentZoomName = 0;
    float currentZoomSprite = 0;

    int currentLine = 0;
    int currentSprite = 0;

    private void setAllElements()
    {
        Debug.Log("Setting up elements");
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        GameObject prefImage = GameObject.FindGameObjectWithTag("PrefsSwitch");
        prefGui = prefImage.GetComponent<SwitchPrefsOnOff>();
        modeLevelSelButton = GameObject.FindGameObjectWithTag("ModeSwitch");
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
            if ( Keyboard.current.escapeKey.wasReleasedThisFrame )
            {
                hideCredits();
            }
            else
            {
                // use the input stuff
                if (Mouse.current.leftButton.wasReleasedThisFrame)
                {
                    hideCredits();
                }
                // Check if the user has clicked
                bool aTouch = Mouse.current.leftButton.wasReleasedThisFrame;

                if (aTouch)
                {
                    hideCredits();
                }

                if (Keyboard.current.escapeKey.wasReleasedThisFrame)
                {
                    hideCredits();
                }
            }
        }
    }

    public void showCredits()
    {
        setAllElements();

        mainCamera.SetActive(false);
        thisCamera.SetActive(true);
        prefGui.hideMe();
        modeLevelSelButton.SetActive(false);

        currentFrameForText = 0;
        currentFrameForSprite = 0;

        currentZoomTitle = 0;
        currentZoomName = 0;
        currentZoomSprite = 0;

        currentLine = 0;

        currentSprite = 0;

        if (Application.platform == RuntimePlatform.Android)
        {
            closeCreditObject.SetActive(true);
        }

        setUpLinesAndSprite();
    }

    public void hideCredits()
    {
        if (sprite != null)
        {
            Destroy(sprite);
        }

        if (Application.platform == RuntimePlatform.Android)
        {
            closeCreditObject.SetActive(false);
        }

        mainCamera.SetActive(true);
        thisCamera.SetActive(false);
        mainCamera.GetComponent<CreateBoard>().playMusic();

        prefGui.showMe();
        modeLevelSelButton.SetActive(true);
    }
}
