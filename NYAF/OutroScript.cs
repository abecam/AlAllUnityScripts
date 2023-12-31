using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class OutroScript : MonoBehaviour
{
    private const int DIVIDER_CAMERA_SPEED = 4; // By how much we devide the cursor deplacement to move the camera. Higher means slower camera.
    private const float REDUCTION_OF_SPEED = 0.8f; // By how much we multiply the speed at each frame. Should always be below 1 :), and smaller mean faster deceleration.
    public bool gameIsInPause = false; // Allow to not take any more selection, like when a menu is displayeds

    private bool inGameOver = false; // Game won or lost, to show the end message

    public int currentLevel;
    private String rootNameOfLevel = "MainBoard";

    public GameObject arrow;

    public GameObject backgroundImages; // The collection of images

    private GameObject backgroundImage; // The current image

    public GameObject title;

    public GameObject characters;

    public GameObject textOutro1;
    public GameObject textOutro2;

    private float minImageX;
    private float maxImageX;
    private float minImageY;
    private float maxImageY;


    private int coolDown = 0; // Do not immediately do something else after some action

    public bool isTimed = false;

    private int musicVolume = 100;
    private int soundVolume = 100;

    Vector3 targetPos;
    private const string GAMEMODEUNLOCKED = "GAMEMODEUNLOCKED";
    private const string SECRETFIGURE = "SECRETFIGURE";

    String gameModeUnlocked;
    String secretFigureFound;

    List<string> allLinesOfText = new List<string>()
        {
           "Well Done!\nThank you for playing!",
           GAMEMODEUNLOCKED,
           SECRETFIGURE,
           "NYAF\nNot Yet Another Freemium\nA Game by\nAlain Becam",
           "Graphics by\nSébastien Lesage",
           "Sounds, testing, gameplay ideas, drums\nWilliam",
           "Music by\nChris Huelsbeck\nChan Redfield\nLiQWYD\nAlexander Nakarada",
           "Font\nGlametrixBold\nSudegnakNo3 by Gluk\nhttp://www.glukfonts.pl",
           //"Development, sounds, main gameplay\nAlain Becam",
           "Sounds by\nSven",
           "Special Thanks to\nAlexander Zolotov\nhttps://www.warmplace.ru",
           "Thank you for playing!"
        };

    public LocalizedString outroLocalizedAGameBy;
    private string localizedAGameByText = "";

    public LocalizedString outroLocalizedDevGamePlay;
    private string localizedDevGamePlayText = "";

    public LocalizedString outroLocalizedGraphicsBy;
    private string localizedGraphicsByTxt = "";

    public LocalizedString outroLocalizedNobody;
    private string localizedNobodyTxt = "";

    public LocalizedString outroLocalizedAnd;
    private string localizedAndTxt = "";

    public LocalizedString outroLocalizedMonkBomb;
    private string localizedMonkBombTxt = "";

    public LocalizedString outroLocalizedTestingGamePlay;
    private string localizedTestingGamePlayTxt = "";

    public LocalizedString outroLocalizedMusicBy;
    private string localizedMusicByTxt = "";

    public LocalizedString outroLocalizedFontBy;
    private string localizedFontByTxt = "";

    public LocalizedString outroLocalizedThanksTo;
    private string localizedThanksToTxt = "";

    void OnEnable()
    {
        outroLocalizedAGameBy.StringChanged += UpdateStringAGameBy;
        outroLocalizedDevGamePlay.StringChanged += UpdateStringDevGamePlay;
        outroLocalizedGraphicsBy.StringChanged += UpdateStringGraphicsBy;
        outroLocalizedNobody.StringChanged += UpdateStringNobody;
        outroLocalizedAnd.StringChanged += UpdateStringAnd;
        outroLocalizedMonkBomb.StringChanged += UpdateStringMonkBomb;
        outroLocalizedTestingGamePlay.StringChanged += UpdateStringTestingGamePlay;
        outroLocalizedMusicBy.StringChanged += UpdateStringMusicBy;
        outroLocalizedFontBy.StringChanged += UpdateStringFontBy;
        outroLocalizedThanksTo.StringChanged += UpdateStringThanksTo;
    }

    void OnDisable()
    {
        outroLocalizedAGameBy.StringChanged -= UpdateStringAGameBy;
        outroLocalizedDevGamePlay.StringChanged -= UpdateStringDevGamePlay;
        outroLocalizedGraphicsBy.StringChanged -= UpdateStringGraphicsBy;
        outroLocalizedNobody.StringChanged -= UpdateStringNobody;
        outroLocalizedAnd.StringChanged -= UpdateStringAnd;
        outroLocalizedMonkBomb.StringChanged -= UpdateStringMonkBomb;
        outroLocalizedTestingGamePlay.StringChanged -= UpdateStringTestingGamePlay;
        outroLocalizedMusicBy.StringChanged -= UpdateStringMusicBy;
        outroLocalizedFontBy.StringChanged -= UpdateStringFontBy;
        outroLocalizedThanksTo.StringChanged -= UpdateStringThanksTo;
    }

    void UpdateStringAGameBy(string s)
    {
        localizedAGameByText = s;
    }

    void UpdateStringDevGamePlay(string s)
    {
        localizedDevGamePlayText = s;
    }

    void UpdateStringGraphicsBy(string s)
    {
        localizedGraphicsByTxt = s;
    }

    void UpdateStringNobody(string s)
    {
        localizedNobodyTxt = s;
    }

    void UpdateStringAnd(string s)
    {
        localizedAndTxt = s;
    }

    void UpdateStringMonkBomb(string s)
    {
        localizedMonkBombTxt = s;
    }

    void UpdateStringTestingGamePlay(string s)
    {
        localizedTestingGamePlayTxt = s;
    }

    void UpdateStringMusicBy(string s)
    {
        localizedMusicByTxt = s;
    }

    void UpdateStringFontBy(string s)
    {
        localizedFontByTxt = s;
    }

    void UpdateStringThanksTo(string s)
    {
        localizedThanksToTxt = s;

        SetCreditText();
    }

    private void SetCreditText()
    {
        allLinesOfText = new List<string>()
         {
           "Well Done!\nThank you for playing!",
           GAMEMODEUNLOCKED,
           SECRETFIGURE,
           "NYAF\nNot Yet Another Freemium\nA Game by\nAlain Becam",
           "Graphics by\nSébastien Lesage",
           "Sounds, testing, gameplay ideas, drums\nWilliam",
           "Music by\nChris Huelsbeck\nChan Redfield\nLiQWYD\nAlexander Nakarada",
           "Font\nGlametrixBold\nSudegnakNo3 by Gluk\nhttp://www.glukfonts.pl",
           //"Development, sounds, main gameplay\nAlain Becam",
           "Sounds by\nSven",
           "Special Thanks to\nAlexander Zolotov\nhttps://www.warmplace.ru",
           "Thank you for playing!"
        };
        completeStrings();
    }

    int currentText = 0;

    // Achievements will be defined by an interface and free numbers of achievements

    // Use this for initialization
    void Start()
    {
        if (PlayAndKeepMusic.Instance != null)
        {
            PlayAndKeepMusic.Instance.Activated = false;
            PlayAndKeepMusic.Instance.stopMusic();
        }

        Application.targetFrameRate = 60;
        /////////////////////////////////////////////////////////////////////// DEBUG REMOVE AFTER /////////////////
        //resetSmallFound();
        // See which string we need to show
        LocalizationSettings.StringDatabase.GetLocalizedStringAsync(outroLocalizedAGameBy.TableReference, outroLocalizedAGameBy.TableEntryReference);

        completeStrings();

        // Get the kind of game from the menu
        initIntroductionBox();

        GameObject newBackgroundImage = backgroundImages.transform.GetChild(0).gameObject;
        backgroundImage = (GameObject)Instantiate(newBackgroundImage.gameObject);
        // Find background boundaries
        minImageX = backgroundImage.GetComponent<SpriteRenderer>().bounds.min.x;
        maxImageX = backgroundImage.GetComponent<SpriteRenderer>().bounds.max.x;
        minImageY = backgroundImage.GetComponent<SpriteRenderer>().bounds.min.y;
        maxImageY = backgroundImage.GetComponent<SpriteRenderer>().bounds.max.y;

        bool haskey = LocalSave.HasIntKey("musicVolume");
        if (haskey)
        {
            musicVolume = LocalSave.GetInt("musicVolume"); 
        }
       
        haskey = LocalSave.HasIntKey("soundVolume");
        if (haskey)
        {
            soundVolume = LocalSave.GetInt("soundVolume");
        }
        setupNextText();

        // Position the objects to find
        setupObjectsInSphere();
    }

    private void setupNextText()
    {
        if (currentText >= allLinesOfText.Count)
        {
            currentText = 0;
            initIntroductionBox();
        }
        String lineOfText = allLinesOfText[currentText++];
        if (lineOfText.Equals(GAMEMODEUNLOCKED))
        {
            lineOfText = gameModeUnlocked;
        }
        else if (lineOfText.Equals(SECRETFIGURE))
        {
            lineOfText = secretFigureFound;
        }
        ((TextMesh)textOutro1.GetComponent("TextMesh")).text = lineOfText;
        ((TextMesh)textOutro2.GetComponent("TextMesh")).text = lineOfText;

        float newRed = ((Random.value / 4) + 0.75f);
        float newGreen = ((Random.value / 1.5f) + 0.25f);
        float newBlue = ((Random.value / 1.5f) + 0.25f);
        //float newAlpha = ((Random.value / 4) + 0.75f);

        textOutro2.GetComponent<Renderer>().material.color = new Color(newRed, newGreen, newBlue, 1);
    }

    private void completeStrings()
    {
        int lastModeFinished = 0;
        int maxGameMode = 0;

        // And which mode we are in
        bool haskey = LocalSave.HasIntKey("lastModeFinished");
        if (haskey)
        {
            lastModeFinished = LocalSave.GetInt("lastModeFinished");
        }

        LoadTimeSpent(lastModeFinished);

        haskey = LocalSave.HasIntKey("modeFinished");
        if (haskey)
        {
            maxGameMode = LocalSave.GetInt("modeFinished");
        }

        long timeSpentLongMode = (long)allTimeSpent;

        long hoursSpentMode = timeSpentLongMode / 3600;
        if (hoursSpentMode > 0)
        {
            timeSpentLongMode -= hoursSpentMode * 3600;
        }

        long minutesSpentMode = timeSpentLongMode / 60;
        if (minutesSpentMode > 0)
        {
            timeSpentLongMode -= minutesSpentMode * 60;
        }

        if (lastModeFinished == maxGameMode && lastModeFinished < 4)
        {
            gameModeUnlocked = "New mode unlocked\nCheck the start page!\n";
        }
        else
        {
            gameModeUnlocked = "";
        }
        {
            gameModeUnlocked += "You finished the mode in " + ((hoursSpentMode > 0) ? hoursSpentMode + " hours " : "") +
            ((hoursSpentMode > 0) || (minutesSpentMode > 0) ? minutesSpentMode + " minutes " : "") +
             ((timeSpentLongMode > 0) ? timeSpentLongMode + " seconds " : "0 seconds");

            if (isBestTime)
            {
                gameModeUnlocked += "\nNew best time!";
            }
            else
            {
                long bestTimeSpentLongMode = (long)bestTimeSpent;

                long hoursBestSpentMode = bestTimeSpentLongMode / 3600;
                if (hoursBestSpentMode > 0)
                {
                    bestTimeSpentLongMode -= hoursBestSpentMode * 3600;
                }

                long minutesBestSpentMode = bestTimeSpentLongMode / 60;
                if (minutesBestSpentMode > 0)
                {
                    bestTimeSpentLongMode -= minutesBestSpentMode * 60;
                }

                gameModeUnlocked += "\nBest time is " + ((hoursBestSpentMode > 0) ? hoursBestSpentMode + " hours " : "") +
                ((hoursBestSpentMode > 0) || (minutesBestSpentMode > 0) ? minutesBestSpentMode + " minutes " : "") +
                ((bestTimeSpentLongMode > 0) ? bestTimeSpentLongMode + " seconds " : "0 seconds");
            }
        }
        if (lastModeFinished == 3)
        {
            float currentTimeAllocated = 0;
            // In mode 3, also show the score
            haskey = LocalSave.HasFloatKey("currentTimeAllocated");

            if (haskey)
            {
                currentTimeAllocated = LocalSave.GetFloat("currentTimeAllocated");
            }

            float bestScoreMode3 = 100000000;

            haskey = LocalSave.HasFloatKey("bestScoreMode3");

            if (haskey)
            {
                bestScoreMode3 = LocalSave.GetFloat("bestScoreMode3");
            }

            gameModeUnlocked = "Your score is " + currentTimeAllocated + "\n" + gameModeUnlocked;

            if (currentTimeAllocated < bestScoreMode3)
            {
                bestScoreMode3 = currentTimeAllocated;

                gameModeUnlocked += "\nNew best score";

                LocalSave.SetFloat("bestScoreMode3", bestScoreMode3);
                LocalSave.Save();
            }
            else
            {
                gameModeUnlocked += "\nBest score is "+((int )bestScoreMode3);
            }
        }

        int nbSmallFound = 0;

        haskey = LocalSave.HasIntKey("smallFound");
        int levelReached = 0;
        if (haskey)
        {
            levelReached = LocalSave.GetInt("smallFound");
        }

        for (int iLevels = 0; iLevels < 12; iLevels++)
        {
            int currentOne = 2 << iLevels;

            if ((levelReached & currentOne) > 0)
            {
                nbSmallFound++;
            }
        }
        if (nbSmallFound == 11)
        {
            secretFigureFound = "Well Done!\nYou found all the\nsecret figures\nYou can find a bonus game\nIn the mode selection page.";
        }
        else
        {
            secretFigureFound = "You found " + nbSmallFound + "/11 secret figures";
        }
    }

    float allTimeSpent = 0;
    float absAllTimeSpent = 0;
    float bestTimeSpent = 100000000000;
    bool isBestTime = false;

    private void LoadTimeSpent(int currentGameMode)
    {
        bool haskey = LocalSave.HasFloatKey("allTimeSpent" + currentGameMode);

        if (haskey)
        {
            allTimeSpent = LocalSave.GetFloat("allTimeSpent" + currentGameMode);
        }

        // Best time
        haskey = LocalSave.HasFloatKey("bestTimeSpent" + currentGameMode);

        if (haskey)
        {
            bestTimeSpent = LocalSave.GetFloat("bestTimeSpent" + currentGameMode);
        }

        Debug.Log("Found time : " + allTimeSpent + "(best "+ bestTimeSpent + ") for key allTimeSpent" + currentGameMode);

        if (allTimeSpent < bestTimeSpent)
        {
            bestTimeSpent = allTimeSpent;
            isBestTime = true;
        }
        // Save the absolute total for this mode
       
        haskey = LocalSave.HasFloatKey("absAllTimeSpent" + currentGameMode);

        if (haskey)
        {
            absAllTimeSpent = LocalSave.GetFloat("absAllTimeSpent" + currentGameMode);
        }

        LocalSave.SetFloat("bestTimeSpent" + currentGameMode, bestTimeSpent);

        LocalSave.SetFloat("absAllTimeSpent" + currentGameMode, absAllTimeSpent + allTimeSpent);

        // Now reset the values!
        LocalSave.SetFloat("allTimeSpent" + currentGameMode, 0);
        LocalSave.Save();
    }

    /**
     * Push on a shape
     */
    private void setupObjectsOnSphere()
    {
        // Place all characters available, ensure they are not at the same place as others.
        int nbOfElements = characters.transform.childCount;

        float extendX = maxImageX - minImageX - 0.2f; // Not too much at the borders!
        float halfX = extendX / 2;

        float extendY = maxImageY - minImageY - 0.2f;
        float halfY = extendY / 2;

        float minOfHalf = halfX;
        if (halfY < halfX)
        {
            minOfHalf = halfY;
        }

        foreach (Transform childCharacter in characters.transform)
        {
            Debug.Log("Child: " + childCharacter);

            // TODO: - Check that we are on the picture
            // - Avoid pref icon and ad block

            float posZ = 2 * Random.value - 1;   // uniform in -1, 1
            float t = 2 * ((float )Math.PI)* Random.value;   // uniform in 0, 2*pi
            float posX = (float )(Math.Sqrt(1 - Math.Pow(posZ,2)) * Math.Cos(t));
            float posY = (float )(Math.Sqrt(1 - Math.Pow(posZ,2)) * Math.Sin(t));

            childCharacter.position = new Vector3(minOfHalf * posX, minOfHalf * posY, minOfHalf * posZ);

            // Make them a tiny bit transparent
            float newAlpha = 1;

            Color previousColor = childCharacter.GetComponent<Renderer>().material.color;
            childCharacter.GetComponent<Renderer>().material.color = new Color(previousColor.r, previousColor.g, previousColor.b, newAlpha);
        }
    }

    /**
     * Push on a shape
     */
    private void setupObjectsInSphere()
    {
        // Place all characters available, ensure they are not at the same place as others.
        int nbOfElements = characters.transform.childCount;

        float extendX = maxImageX - minImageX - 0.2f; // Not too much at the borders!
        float halfX = extendX / 2;

        float extendY = maxImageY - minImageY - 0.2f;
        float halfY = extendY / 2;

        float minOfHalf = halfX;
        if (halfY < halfX)
        {
            minOfHalf = halfY;
        }

        foreach (Transform childCharacter in characters.transform)
        {
            float phi = (Random.value)*2f*(float )Math.PI;
            float costheta = Random.value * 2 - 1;
            float u = Random.value;

            float theta = (float )Math.Acos(costheta);
            float r = minOfHalf * cubeRoot(u);

            float posX = (float )(r * Math.Sin(theta) * Math.Cos(phi));
            float posY = (float)(r * Math.Sin(theta) * Math.Sin(phi));
            float posZ = (float)(r * Math.Cos(theta));

            childCharacter.position = new Vector3(posX, posY, posZ);

            float newAlpha = 1;

            Color previousColor = childCharacter.GetComponent<Renderer>().material.color;
            childCharacter.GetComponent<Renderer>().material.color = new Color(previousColor.r, previousColor.g, previousColor.b, newAlpha);
        }
    }

    private float cubeRoot(float number)
    {
        return (float )(Math.Pow(number, (1.0 / 3.0)));
    }

    private bool dragMode = false;
    private bool zoomMode = false;
    private Vector3 totalDrag;
    private Vector3 dragOldPosition;

    private Vector3 zoomOldPosition1;
    private Vector3 zoomOldPosition2;

    // Update is called once per frame
    void Update()
    {
        if (startingGame)
        {
            // Manage the introduction box
            showIntroductionBox(true);

            return;
        }

        if (inGameOver)
        {
            // Manage the introduction box
            showIntroductionBox(false);
            return;
        }

        if (gameIsInPause)
        {
            // Nothing to do
            return;
        }

        if (coolDown != 0)
        {

            coolDown--;
        }

        // rotateSphere();

        updateText();

        zoomBackgrounds();

        {
            Vector3 touchPos = Mouse.current.position.ReadValue();

            // use the input stuff
            if (Mouse.current.leftButton.isPressed)
            { 
                moveCamera(touchPos);
            }
            else
            {
                dragMode = false;
            }
            // Check if the user has clicked
            bool aTouch = Mouse.current.leftButton.wasReleasedThisFrame;
            bool rightClick = Mouse.current.rightButton.wasReleasedThisFrame;

            if (aTouch)
            {
                // Debug.Log( "Moused moved to point " + touchPos );

                checkBoard(touchPos);
            }
            else
            {
                // No touch reset the cool down
                coolDown = 0;
            }

            
            if (rightClick)
            {
                zoomBoard(true);
            }
            else
            {
                zoomBoard(false);
            }
        }

        // Move the camera if needed
        updateCameraPos();

        // Keep absolute scale of GUI elements
        //score.transform.localScale = score.transform.localScale;
    }

    bool needToReplaceChar = true;
    int iCurrentObject = 0;
    /**
     * Zoom the background and move the characters when needed. 
     */
    private void zoomBackgrounds()
    {
        currentZoomForIntroBox += stepBetweenZoom;

        // backgroundImage.transform.localScale = new Vector3(globalZoom + currentZoomForIntroBox, globalZoom + currentZoomForIntroBox, 1);
        //Camera.main.orthographicSize = globalZoom + currentZoomForIntroBox;
        Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, globalZoom + currentZoomForIntroBox);

        if (needToReplaceChar && Camera.main.transform.position.z > 9)
        {
            setupObjectsInSphere();
            needToReplaceChar = false;
        }
        if (Camera.main.transform.position.z > 9)
        {
            // Load the next picture and reset the camera position. Will also load new text
            if (iCurrentObject >= backgroundImages.transform.childCount - 1)
            {
                iCurrentObject = 0;
            }
            Destroy(backgroundImage);
            GameObject newBackgroundImage = backgroundImages.transform.GetChild(++iCurrentObject).gameObject;
            backgroundImage = (GameObject)Instantiate(newBackgroundImage.gameObject);
            currentZoomForIntroBox = 0;
            needToReplaceChar = true;

            // Load the next text
            setupNextText();

            Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, globalZoom + currentZoomForIntroBox);
        }
    }

    private void updateText()
    {
        //
    }

    float xSphereRot = 0;
    float ySphereRot = 0;
    float zSphereRot = 0;

    Vector3 speedRotation = new Vector3(0.2f, 0.1f, 0.05f);

    float dir = 0.0f;

    private void rotateSphere()
    {
        dir += Random.value * 0.2f - 0.1f;

        //float newAccX = 4*(float )Math.Sin(dir);
        //float newAccY = 4*(float )Math.Cos(dir);
        //float newAccZ = Random.value * 0.2f - 0.1f;

        //speedRotation = new Vector3(speedRotation.x + newAccX, speedRotation.y + newAccY, 0);

        xSphereRot = xSphereRot + speedRotation.x;
        ySphereRot = ySphereRot + speedRotation.y;
        zSphereRot = zSphereRot + speedRotation.z;


        characters.transform.Rotate(Vector3.right, xSphereRot);
        characters.transform.Rotate(Vector3.up, ySphereRot);
        characters.transform.Rotate(Vector3.forward, zSphereRot);

        foreach (Transform childCharacter in characters.transform)
        {
            childCharacter.transform.rotation = new Quaternion(0, 0, 0, 1);
        }
    }

    float currentZoomCamera = normalZoom;
    public float orthoZoomSpeed = 0.1f;        // The rate of change of the orthographic size in orthographic mode.

    private void zoomCamera(Vector2 touchPos1, Vector2 touchPos2)
    {
        if (!zoomMode)
        {
            zoomOldPosition1 = touchPos1;
            zoomOldPosition2 = touchPos2;

            zoomMode = true;
        }
        else
        {
            if (Vector2.Distance(touchPos1,touchPos2) > Vector2.Distance(zoomOldPosition1,zoomOldPosition2))
            {
                // Zoom in
                if (currentZoomCamera > normalZoom)
                {
                    currentZoomCamera -= 0.3f;
                }
                else
                {
                    currentZoomCamera = normalZoom;
                }
            }
            else
            {
                // Zoom out
                if (currentZoomCamera < globalZoom)
                {
                    currentZoomCamera += 0.3f;
                }
                else
                {
                    currentZoomCamera = globalZoom;
                }
            }

            Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, currentZoomCamera);
            float newRatio = currentZoomCamera / normalZoom;
            Camera.main.transform.localScale = new Vector3(newRatio, newRatio, 1);
        }
    }

    private void zoomBoard(bool isZoom)
    {
        if (isZoom)
        {
            Camera.main.orthographicSize = globalZoom;
            Camera.main.transform.localScale = new Vector3(ratio, ratio, 1);
        }
        else
        {
            Camera.main.orthographicSize = normalZoom;
            Camera.main.transform.localScale = new Vector3(1, 1, 1);
        }
    }

    Vector3 cameraSpeed = new Vector3(0, 0, 0);

    private void moveCamera(Vector3 mousePos)
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
        Vector3 positionDelta = worldPos - Camera.main.ScreenToWorldPoint(dragOldPosition);

        if (!dragMode)
        {
            totalDrag += positionDelta;
            if (Vector3.Distance(totalDrag, Vector3.zero) > 0)
            {
                dragMode = true;
            }
        }
        else
        {
            //Vector3 cameraPos = Camera.main.transform.position;

            //cameraPos -= positionDelta;
            cameraSpeed -= positionDelta/ DIVIDER_CAMERA_SPEED;

            //Camera.main.transform.position = RestrainCamera(cameraPos);
        }

        dragOldPosition = mousePos;
    }

    private void updateCameraPos()
    {
        Vector3 cameraPos = Camera.main.transform.position;

        cameraPos += cameraSpeed;

        cameraSpeed = cameraSpeed* REDUCTION_OF_SPEED;

        if (cameraSpeed.magnitude < 0)
        {
            cameraSpeed = new Vector3(0,0,0);
        }

        Camera.main.transform.position = RestrainCamera(cameraPos);
    }

    private Vector3 RestrainCamera(Vector3 cameraPos)
    {
        // Need to check the background image boundaries.

        if (cameraPos.x > maxImageX)
        {
            cameraPos.x = maxImageX;
        }
        else if (cameraPos.x < minImageX)
        {
            cameraPos.x = minImageX;
        }
        if (cameraPos.y > maxImageY)
        {
            cameraPos.y = maxImageY;
        }
        else if (cameraPos.y < minImageY)
        {
            cameraPos.y = minImageY;
        }
        return cameraPos;
    }

    // Introduction text
    public GameObject introductionBox;

    private bool startingGame = true;

    int zoomingTime = 0;
    int currentStep = 0;
    public int totalZoomingTime = 60;
    public int dimingTime = 100; // 200 frames
    float zoomFactor;
    float currentZoomForIntroBox;

    //private float globalZoom = 0.04f;
    //private float normalZoom = 0.1f;
    //private float stepBetweenZoom = 0.1f;

    private static readonly float globalZoom = -18f;
    private static readonly float normalZoom = 4f;
    private static readonly float ratio = globalZoom / normalZoom;

    private float stepBetweenZoom = 0.1f;

    private void initIntroductionBox()
    {
        zoomingTime = 0;
        currentStep = 0;

        startingGame = true; // Show the introduction box
        zoomFactor = (0.2f / ((float)dimingTime));
        currentZoomForIntroBox = 0;
        // Z = -9
        title.SetActive(true);
        //backgroundImage.transform.localScale = new Vector3(globalZoom, globalZoom, 1);

        Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, globalZoom);

        stepBetweenZoom = (normalZoom - globalZoom) / ((float )dimingTime);
    }

    // Using same box, show the game over text
    private void initIOutroBox()
    {
        zoomingTime = 0;
        currentStep = 0;

        startingGame = true; // Show the introduction box
        zoomFactor = (0.2f / ((float)totalZoomingTime));
        currentZoomForIntroBox = 0;
        // Z = -9
        introductionBox.transform.localPosition = new Vector3(introductionBox.transform.localPosition.x, introductionBox.transform.localPosition.y, 9);
    }

    public static float GetScreenToWorldWidth()
    {
        Vector2 topRightCorner = new Vector2(1, 1);
        Vector2 edgeVector = Camera.main.ViewportToWorldPoint(topRightCorner);
        float width = edgeVector.x * 2;

        width = 1;

        return width;
    }

    private void showIntroductionBox(bool isIntro)
    {
        if (currentStep == 0)
        {
            // Zoom the box into view
            if (zoomingTime <= totalZoomingTime)
            {
                float fading = 1 - ((float)(totalZoomingTime - zoomingTime)) / ((float)totalZoomingTime);
                foreach (Transform childIntrobox in title.transform)
                {
                    Renderer renderer1 = childIntrobox.GetComponent<Renderer>();
                    if (renderer1 != null)
                    {
                        renderer1.material.color = new Color(renderer1.material.color.r, renderer1.material.color.g, renderer1.material.color.b, fading);
                    }
                }

                // backgroundImage.transform.localScale = new Vector3(0.02f + currentZoomForIntroBox, 0.02f + currentZoomForIntroBox, 1);

                //introductionBox.transform.localScale = new Vector3(0.8f+currentZoomForIntroBox, 0.8f+currentZoomForIntroBox, 0.8f+currentZoomForIntroBox);
                zoomingTime++;
            }
            else
            {
                currentStep = 2;
                zoomingTime = 0;
            }
        }
        else if (currentStep == 2)
        {
            if (zoomingTime <= dimingTime)
            {
                // Fading out of the box
                float fading = ((float)(dimingTime - zoomingTime)) / ((float)dimingTime);

                foreach (Transform childIntrobox in title.transform)
                {
                    Renderer renderer1 = childIntrobox.GetComponent<Renderer>();
                    if (renderer1 != null)
                    {
                        renderer1.material.color = new Color(childIntrobox.GetComponent<Renderer>().material.color.r, childIntrobox.GetComponent<Renderer>().material.color.g, childIntrobox.GetComponent<Renderer>().material.color.b, fading);
                    }
                }
                currentZoomForIntroBox += stepBetweenZoom;

                // backgroundImage.transform.localScale = new Vector3(globalZoom + currentZoomForIntroBox, globalZoom + currentZoomForIntroBox, 1);
                //Camera.main.orthographicSize = globalZoom + currentZoomForIntroBox;
                Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, globalZoom + currentZoomForIntroBox);

                zoomingTime++;
            }
            else
            {
                currentStep = 3;
            }
        }
        else if (currentStep == 3)
        {
            // All done
            if (isIntro)
            {
                startingGame = false;

                // Remove the title
                title.SetActive(false);
            }
            else
            {
                inGameOver = false;
                startingGame = true;
            }
        }
    }

    private const int nbOfCharsToHint = 6;

    void checkBoard(Vector3 touchPos)
    {
        Vector3 wp = Camera.main.ScreenToWorldPoint(touchPos);
        Vector2 touchPos2 = new Vector2(wp.x, wp.y);

        // Check if we found the object
        // We have 2 and a half-> the normal one, the one in the normal ads, the one in the huge ads.
        foreach (Transform childCharacter in characters.transform)
        {
            if (childCharacter.GetComponent<Collider2D>() == Physics2D.OverlapPoint(touchPos2))
            {
                //
            }
        }
    }

    void gameover()
    {
        // Do something depending on the current mode
        loadFirstLevel();
    }

    public void loadFirstLevel()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(rootNameOfLevel);
    }

    public void setInpause(bool inPause)
    {
        gameIsInPause = inPause;
    }

    public void setNextLevelReached()
    {
        bool haskey = LocalSave.HasIntKey("levelReached");
        int levelReached = 0;
        if (haskey)
        {
            levelReached = LocalSave.GetInt("levelReached");
        }

        if (currentLevel >= levelReached)
        { 
            LocalSave.SetInt("levelReached", currentLevel+1);
            LocalSave.Save();
        }
    }

    public void resetLevelReached()
    {
        LocalSave.SetInt("levelReached", 1);
        LocalSave.Save();
    }

    public void resetSmallFound()
    {
        LocalSave.SetInt("smallFound", 0);
        LocalSave.Save();
    }

    public void resetSave()
    {
        resetLevelReached();
        resetSmallFound();
    }

    // After the drum roll, start the music
    internal void playMusic()
    {
        if (GetComponent<AudioSource>() != null)
        {
            ((AudioSource)GetComponent<AudioSource>()).Play();
        }
    }
}
