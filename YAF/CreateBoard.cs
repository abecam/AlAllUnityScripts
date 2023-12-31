using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class CreateBoard : MainApp
{
    private const int DIVIDER_CAMERA_SPEED = 4; // By how much we devide the cursor deplacement to move the camera. Higher means slower camera.
    private const float REDUCTION_OF_SPEED = 0.8f; // By how much we multiply the speed at each frame. Should always be below 1 :), and smaller mean faster deceleration.

    private bool inGameOver = false; // Game won or lost, to show the end message

    internal int currentLevel = 1;

    private String rootNameOfLevel = "MainBoard";

    public GetAndAnimateCoin ourCoinManager;

    public GameObject arrow;

    public GameObject backgroundImage;

    public GameObject title;

    public GameObject characters;

    public GameObject redBomb;
    public GameObject blackBomb;

    private List<GameObject> redBombs = null;
    private List<GameObject> blackBombs = null;
    private List<GameObject> traps = null;

    private AudioSource bombSound;

    int shakeCamera = 0; // Give a visual clue a bomb has been touched.

    public Color red; //= new Color(0.8f, 0.1f,0.0f);
    public Color green; //= new Color(0.1f, 0.9f,0.1f);
    public Color blue; //= new Color(0.1f, 0.2f,1.0f);
    public Color velvet; //= new Color(0.1f, 0.2f,1.0f);

    public Color redLight; //= new Color(1.0f, 0.3f,0.0f);
    public Color greenLight; // = new Color(0.3f, 1.0f,0.2f);
    public Color blueLight; //= new Color(0.3f, 0.3f,1.0f);

    public Color yellow = new Color(1.0f, 0.8f, 0.1f);

    public GameObject particleFoundChar;

    public GameObject particlesContainer;

    private float minImageX;
    private float maxImageX;
    private float minImageY;
    private float maxImageY;

    private int nbOfElements = 0;

    private int scoreOfLevel = 0;

    private bool pieceTransparences = true;
    private bool showHints = true;
    private bool showArrows = true;

    public Toggle pieceTransparencesToggle;
    public Toggle showHintsToggle;
    public Toggle showArrowsToggle;

    Vector3 targetPos;

    float currentTime;
    float lastTime;
    float initialTime;

    float lastTimeSpent = 0; // last saved time for the mode
    float lastTimeSpentLevel = 0; // last saved time for the level

    int currentDifficulty = 2;

    float extendX ; // Not too much at the borders!
    float extendY ;

    float halfX ;
    float halfY ;


    Dictionary<Transform, Collider2D> allChildColliders = new Dictionary<Transform, Collider2D>();

    // Achievements will be defined by an interface and free numbers of achievements

    // Use this for initialization
    void Start()
    {
        Application.targetFrameRate = 60;

        // Get the kind of game from the menu
        initIntroductionBox();

        // Find background boundaries
        minImageX = backgroundImage.GetComponent<SpriteRenderer>().bounds.min.x;
        maxImageX = backgroundImage.GetComponent<SpriteRenderer>().bounds.max.x;
        minImageY = backgroundImage.GetComponent<SpriteRenderer>().bounds.min.y;
        maxImageY = backgroundImage.GetComponent<SpriteRenderer>().bounds.max.y;

        extendX = maxImageX - minImageX - 0.2f; // Not too much at the borders!
        extendY = maxImageY - minImageY - 0.2f;

        halfX = extendX / 2;
        halfY = extendY / 2;

        // Check the preferences
        bool haskey = LocalSave.HasIntKey("difficulty");
        if (haskey)
        {
            currentDifficulty = LocalSave.GetInt("difficulty");
        }

        haskey = LocalSave.HasIntKey("transparentChars");
        if (haskey)
        {
            pieceTransparences = LocalSave.GetInt("transparentChars") == 1;
            pieceTransparencesToggle.isOn = pieceTransparences;

            //Debug.Log("Transparence: " + pieceTransparences + " - " + pieceTransparencesToggle.isOn);
        }

        // The last level
        haskey = LocalSave.HasIntKey("currentLevel");
        if (haskey)
        {
            currentLevel = LocalSave.GetInt("currentLevel");
        }

        // Load the last time spent
        LoadTimeSpent();

        setupObjects();

        foreach (Transform childCharacter in characters.transform)
        {
            allChildColliders.Add(childCharacter, childCharacter.GetComponent<Collider2D>());
        }

        // Create a hidden bomb to get the sound
        GameObject hiddenBlackBomb = Instantiate(blackBomb);
        hiddenBlackBomb.transform.position = new Vector2(1000, 1000);
        bombSound = hiddenBlackBomb.GetComponent<AudioSource>();

        giveIdleMoney();

        lastTime = Time.time; // Last "frame" time
        initialTime = lastTime; // Level start time
        currentTime = lastTime;
    }

    private static void giveIdleMoney()
    {
        // Check how much money we give
        // Its one time the current amount of money (regular) for 1 day, so 1/24th for one hour!
        if (LocalSave.HasLongKey(KeyLastPlayTime))
        {
            long lastPlayTick = LocalSave.GetLong(KeyLastPlayTime);

            DateTime then = new DateTime(lastPlayTick);
            DateTime now = DateTime.Now;

            TimeSpan diff = now.Subtract(then);

            string keyRegularMoney = ManageOneShopLine.moneyInSave[ManageOneShopLine.Money.Regular];

            if (diff.TotalHours > 0)
            {
                double nbOfHours = diff.TotalHours;

                double actualCoins = 1000;

                
                if (LocalSave.HasDoubleKey(keyRegularMoney))
                {
                    actualCoins = LocalSave.GetDouble(keyRegularMoney);
                }

                double nbOfCoinsToAdd = actualCoins * (nbOfHours / 24f);

                LocalSave.SetDouble(keyRegularMoney, actualCoins + nbOfCoinsToAdd);
            }
            else // Same with seconds
            {
                double nbOfHours = diff.TotalSeconds;

                double actualCoins = 1000;

                if (LocalSave.HasDoubleKey(keyRegularMoney))
                {
                    actualCoins = LocalSave.GetDouble(keyRegularMoney);
                }

                double nbOfCoinsToAdd = actualCoins * (nbOfHours / (24f * 3600f));

                LocalSave.SetDouble(ManageOneShopLine.moneyInSave[ManageOneShopLine.Money.Regular], actualCoins + nbOfCoinsToAdd);
            }

            LocalSave.Save();
        }
    }

    private void setupObjects()
    {
        // Place all characters available, ensure they are not at the same place as others.
        nbOfElements = characters.transform.childCount;

        Debug.Log("Difficulty is " + currentDifficulty + " and nb of elements is" + nbOfElements);

        Debug.Log("B-  Difficulty is " + currentDifficulty + " and nb of elements is" + nbOfElements);

        scoreOfLevel = nbOfElements;

        placeCharactersAtRandom();

        placeBombsIfNeeded();
    }

    private void placeBombsIfNeeded()
    {
        if (currentDifficulty < 3)
        {
            return;
        }
        if (currentDifficulty == SelectDifficulty.hard)
        {
            placeBombs(1);
        }
        if (currentDifficulty == SelectDifficulty.veryHard)
        {
            // 4 of them, random size
            placeBombs(2);
        }
        if (currentDifficulty == SelectDifficulty.insane)
        {
            // 8 of them
            placeBombs(4);
        }
        if (currentDifficulty == SelectDifficulty.purgatory)
        {
            // 16 of them, some trapped pieces
            placeBombs(8);

            placeTraps(8);
        }
        if (currentDifficulty == SelectDifficulty.hell)
        {
            // 16 of them, lot of trapped pieces
            placeBombs(8);

            placeTraps(16);
        }
    }

    private void placeTraps(int nbOfTraps)
    {
        traps = new List<GameObject>();
        // Create fake objects from our pool :)

        for (int iBomb = 0; iBomb < nbOfTraps; iBomb++)
        {
            // Find the object
            int posInCharacters = (int )(((float )nbOfElements) * Random.value);
            GameObject ourTrap = characters.transform.GetChild(posInCharacters).gameObject;

            GameObject aNewTrap = Instantiate(ourTrap);
            // Push it somewhere
            Vector2 newPos = findNewPlace();

            aNewTrap.transform.position = new Vector3(newPos.x, newPos.y, 2);

            // And change the color :)
            Color previousColor = aNewTrap.GetComponent<Renderer>().material.color;
            aNewTrap.GetComponent<Renderer>().material.color = new Color(1, 0.6f, 0.6f, 1);

            traps.Add(aNewTrap);
        }
    }

    private void placeBombs(int nbOfPairs)
    {
        blackBombs = new List<GameObject>();
        redBombs = new List<GameObject>();

        for (int iBomb = 0; iBomb < nbOfPairs; iBomb++)
        {
            // Just the 2
            GameObject aNewBlackBomb = Instantiate(blackBomb);
            // Push it somewhere
            Vector2 newPos = findNewPlace();

            aNewBlackBomb.transform.position = new Vector3(newPos.x, newPos.y, 2);
            // Size between 0.2 and 0.5
            float newSize = Random.value * 0.3f + 0.2f;
            Vector2 newBombSize = new Vector2(newSize, newSize);
            aNewBlackBomb.transform.localScale = newBombSize;

            blackBombs.Add(aNewBlackBomb);

            GameObject aNewRedBomb = Instantiate(redBomb);
            // Push it somewhere
            newPos = findNewPlace();

            aNewRedBomb.transform.position = new Vector3(newPos.x, newPos.y, 2);

            newSize = Random.value * 0.3f + 0.2f;
            Vector2 newRedBombSize = new Vector2(newSize, newSize);
            aNewBlackBomb.transform.localScale = newBombSize;

            redBombs.Add(aNewRedBomb);
        }
    }

    private Vector2 findNewPlace()
    {
        float posX = (extendX * Random.value) - halfX;
        float posY = (extendY * Random.value) - halfY;

        return new Vector2(posX, posY);
    }

    private void placeCharactersAtRandom()
    {
        foreach (Transform childCharacter in characters.transform)
        {
            //Debug.Log("Child: " + childCharacter);

            // TODO: - Check that we are on the picture
            // - Avoid pref icon and ad block
            float posX = (extendX * Random.value) - halfX;
            float posY = (extendY * Random.value) - halfY;

            childCharacter.position = new Vector3(posX, posY, 2);

            // Make them a tiny bit transparent
            float newAlpha = ((Random.value / 4) + 0.75f);
            if (!pieceTransparences)
            {
                newAlpha = 1;
            }

            Color previousColor = childCharacter.GetComponent<Renderer>().material.color;
            childCharacter.GetComponent<Renderer>().material.color = new Color(previousColor.r, previousColor.g, previousColor.b, newAlpha);
        }
    }

    /**
     * Same thing but only with active characters, do not alter the score
     */
    private void replaceActiveCharactersAtRandom()
    {
        float extendX = maxImageX - minImageX - 0.2f; // Not too much at the borders!
        float halfX = extendX / 2;

        float extendY = maxImageY - minImageY - 0.2f;
        float halfY = extendY / 2;

        int iCharacter = 0;

        foreach (Transform childCharacter in characters.transform)
        {
            if (childCharacter.position.x < 90)
            {
                float posX = (extendX * Random.value) - halfX;
                float posY = (extendY * Random.value) - halfY;

                childCharacter.position = new Vector3(posX, posY, 2);

                // Make them a tiny bit transparent
                float newAlpha = ((Random.value / 4) + 0.75f);
                if (!pieceTransparences)
                {
                    newAlpha = 1;
                }

                Color previousColor = childCharacter.GetComponent<Renderer>().material.color;
                childCharacter.GetComponent<Renderer>().material.color = new Color(previousColor.r, previousColor.g, previousColor.b, newAlpha);
            }
        }
    }

    private bool dragMode = false;
    private bool zoomMode = false;
    private Vector3 totalDrag;
    private Vector3 dragOldPosition;

    private Vector3 zoomOldPosition1;
    private Vector3 zoomOldPosition2;

    private Vector2 dummyVector = new Vector2();

    float timeBeforeSave = 0;
    const float saveEachNSeconds = 30;
    private const string KeyLastPlayTime = "LastPlayTime";

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

        // Wait for the title to stop playing before removing it
        if (title.activeInHierarchy)
        {
            if (title.GetComponent<AudioSource>() != null)
            {
                if (!((AudioSource)GetComponent<AudioSource>()).isPlaying)
                {
                    // Only if our music is not playing yet.
                    if (!((AudioSource)title.GetComponent<AudioSource>()).isPlaying)
                    {
                        Debug.Log("Removing title and starting music");
                        // Remove the title
                        title.SetActive(false);
                        playMusic();
                    }
                }
            }
        }

        currentTime = Time.time;

        timeBeforeSave += Time.deltaTime;
        if (timeBeforeSave > saveEachNSeconds)
        {
            timeBeforeSave = 0;

            long nowInTicks = DateTime.Now.Ticks;
            LocalSave.SetLong(KeyLastPlayTime, nowInTicks);

            LocalSave.Save();
        }

        if (shakeCamera > 0)
        {
            shakeCamera--;
            float xRand = 0.2f*Random.value - 0.1f;
            float yRand = 0.2f * Random.value - 0.1f;
            transform.position = new Vector3(xRand, yRand, -7);
        }
        else if (shakeCamera == 0)
        {
            transform.position = new Vector3(0, 0, -7);
            shakeCamera = -1;
        }

        if (Application.platform != RuntimePlatform.Android)
        {
            Vector3 touchPos = Input.mousePosition;

            // use the input stuff
            if (Input.GetMouseButton(0))
            { 
                moveCamera(touchPos);
            }
            else
            {
                dragMode = false;
            }
            // Check if the user has clicked
            bool aTouch = Input.GetMouseButtonDown(0);
            bool rightClick = Input.GetMouseButton(1);

            if (aTouch)
            {
                // Debug.Log( "Moused moved to point " + touchPos );

                checkBoard(touchPos, dummyVector);
            }

            /*
            if (rightClick)
            {
                zoomBoard(true);
            }
            else
            {
                zoomBoard(false);
            }
            */

            if (Input.GetAxis("Mouse ScrollWheel") > 0f) // forward
            {
                zoomWheel(true);
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0f) // backwards
            {
                zoomWheel(false);
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

                    if (firstFinger.phase == TouchPhase.Moved)
                    {
                        moveCamera(touchPos);
                    }
                    else
                    {
                        dragMode = false;
                        checkBoard(touchPos, dummyVector);
                    }
                }
                else if (Input.touchCount == 2)
                {
                    Vector2 touchPos1 = firstFinger.position;
                    Touch secondFinger = Input.GetTouch(1);
                    Vector2 touchPos2 = secondFinger.position;

                    if (firstFinger.phase == TouchPhase.Moved && secondFinger.phase == TouchPhase.Moved)
                    {
                        //zoomCamera(firstFinger, secondFinger);
                        zoomCamera(touchPos1, touchPos2);
                    }
                    else
                    {
                        zoomMode = false;
                    }
                }
            }
            else
            {
                dragMode = false;
            }
        }

        // Move the camera if needed
        updateCameraPos();

        // Remove the hints when needed
        //updateHints();

        // Keep absolute scale of GUI elements
        //score.transform.localScale = score.transform.localScale;
        lastTime = currentTime; // Last "frame" time
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
                //else
                //{
                //    currentZoomCamera = normalZoom;
                //}
            }
            else
            {
                // Zoom out
                if (currentZoomCamera < globalZoom)
                {
                    currentZoomCamera += 0.3f;
                }
                //else
                //{
                //    currentZoomCamera = globalZoom;
                //}
            }

            Camera.main.orthographicSize = currentZoomCamera;
            float newRatio = currentZoomCamera / normalZoom;
            Camera.main.transform.localScale = new Vector3(newRatio, newRatio, 1);
        }
    }

    private void zoomWheel(bool zoomIn)
    {
        if (zoomIn)
        {
            // Zoom in
            if (currentZoomCamera > normalZoom)
            {
                currentZoomCamera -= 0.3f;
            }
            //else
            //{
            //    currentZoomCamera = normalZoom;
            //}
        }
        else
        {
            // Zoom out
            if (currentZoomCamera < globalZoom)
            {
                currentZoomCamera += 0.3f;
            }
            //else
            //{
            //    currentZoomCamera = globalZoom;
            //}
        }

        Camera.main.orthographicSize = currentZoomCamera;
        float newRatio = currentZoomCamera / normalZoom;
        Camera.main.transform.localScale = new Vector3(newRatio, newRatio, 1);
    }

    Vector3 cameraSpeed = new Vector3(0, 0, 0);

    private void moveCamera(Vector3 mousePos)
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
        Vector3 positionDelta = worldPos - Camera.main.ScreenToWorldPoint(dragOldPosition);


        Physics2D.gravity = positionDelta/5;

        Debug.Log("New gravity is " + Physics2D.gravity);

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

        if (cameraPos.x > maxImageX + 4)
        {
            cameraPos.x = maxImageX + 4;
        }
        else if (cameraPos.x < minImageX - 4)
        {
            cameraPos.x = minImageX - 4;
        }
        if (cameraPos.y > maxImageY + 4)
        {
            cameraPos.y = maxImageY + 4;
        }
        else if (cameraPos.y < minImageY - 4)
        {
            cameraPos.y = minImageY - 4;
        }
        return cameraPos;
    }

    // Introduction text
    public GameObject introductionBox;

    private bool startingGame = true;

    int zoomingTime = 0;
    int currentStep = 0;
    public int totalZoomingTime = 60;
    public int dimingTime = 2000; // 200 frames
    float zoomFactor;
    float currentZoomForIntroBox;

    //private float globalZoom = 0.04f;
    //private float normalZoom = 0.1f;
    //private float stepBetweenZoom = 0.1f;

    private static readonly float globalZoom = 8f; // Was 4.
    private static readonly float normalZoom = 1.6f;
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
        
        //backgroundImage.transform.localScale = new Vector3(globalZoom, globalZoom, 1);
        Camera.main.orthographicSize = globalZoom;

        float newRatio = globalZoom / normalZoom;
        Camera.main.transform.localScale = new Vector3(newRatio, newRatio, 1);

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
        return width;
    }

    bool initialTitleSetup = true;
    bool initialHowToPlaySetup = true;
    private bool inTitle = false;

    private void showIntroductionBox(bool isIntro)
    {
        if (currentStep == 0)
        {
            // Zoom the box into view
            if (zoomingTime <= totalZoomingTime)
            {
                if (initialTitleSetup)
                {
                    float width = GetScreenToWorldWidth();

                    if (width > 7.6f)
                    {
                        width = 7.6f;
                    }
                    title.transform.localScale = Vector2.one * width * 0.5f;

                    // And the scale on z must stay 1, or we can't layer.
                    title.transform.localScale = new Vector3(title.transform.localScale.x, title.transform.localScale.y, 1);

                    // Can show how to box
                    initialHowToPlaySetup = true;

                    // And the title (and its selections) is active
                    inTitle = true;
                }
                float fading = 1 - ((float)(totalZoomingTime - zoomingTime)) / ((float)totalZoomingTime);
                foreach (Transform childIntrobox in title.transform)
                {
                    //Debug.Log("Child: " + childIntrobox);
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
                currentStep = 1;
                zoomingTime = 0;
            }
        }
        else if (currentStep == 1)
        {
            // Wait for user input
            if (Application.platform != RuntimePlatform.Android)
            {
                // use the input stuff

                bool aTouch = Input.GetMouseButton(0);
                if (aTouch)
                {
                    currentStep = 2;
                }
            }
            else
            {
                if (Input.touchCount >= 1)
                {
                    currentStep = 2;
                }
            }
        }
        else if (currentStep == 2)
        {
            inTitle = false;

            if (zoomingTime <= dimingTime)
            {
                // Fading out of the box
                float fading = ((float)(dimingTime - zoomingTime)) / ((float)dimingTime);

                foreach (Transform childIntrobox in title.transform)
                {
                    //Debug.Log("Child: " + childIntrobox);
                    Renderer renderer1 = childIntrobox.GetComponent<Renderer>();
                    if (renderer1 != null)
                    {
                        renderer1.material.color = new Color(childIntrobox.GetComponent<Renderer>().material.color.r, childIntrobox.GetComponent<Renderer>().material.color.g, childIntrobox.GetComponent<Renderer>().material.color.b, fading);
                    }
                }
                currentZoomForIntroBox += stepBetweenZoom;

                // backgroundImage.transform.localScale = new Vector3(globalZoom + currentZoomForIntroBox, globalZoom + currentZoomForIntroBox, 1);
                Camera.main.orthographicSize = globalZoom + currentZoomForIntroBox;

                float newRatio = (globalZoom + currentZoomForIntroBox) / normalZoom;
                Camera.main.transform.localScale = new Vector3(newRatio, newRatio, 1);

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

                // Move the menu away
                // We can't remove it (deactivate it) as we need to know if its sound (drum) is still playing
                // We also need to push it aside, as otherwise we could still select the level (even if they are behind us).
                title.transform.localPosition = new Vector3(600, title.transform.localPosition.y, 20);
            }
            else
            {
                inGameOver = false;
                startingGame = true;
                initialTitleSetup = true;
            }
        }
    }

    private const int nbOfCharsToHint = 6;

    public void checkBoard(Vector3 touchPos, Vector2 absPos, bool isTouch = true)
    {
        Vector2 touchPos2 = absPos;

        if (isTouch)
        {
            Vector3 wp = Camera.main.ScreenToWorldPoint(touchPos);
            touchPos2 = new Vector2(wp.x, wp.y);
        }

        Boolean foundSomething = false;

        // Check if we found the object
        // We have 2 and a half-> the normal one, the one in the normal ads, the one in the huge ads.
        foreach (Transform childCharacter in characters.transform)
        {
            if (allChildColliders[childCharacter] == Physics2D.OverlapPoint(touchPos2))
            {
                updateModeVariables();

                // Play the attached sound
                //if (childCharacter.GetComponent<AudioSource>() != null)
                //{
                //    ((AudioSource)childCharacter.GetComponent<AudioSource>()).Play();
                //}
                //GameObject newFoundParticle = (GameObject)Instantiate(particleFoundChar, childCharacter.transform.position, Quaternion.identity);
                //Renderer renderer1 = newFoundParticle.GetComponent<Renderer>();
                //if (renderer1 != null)
                //{
                //    float newRed = ((Random.value / 4) + 0.75f);
                //    float newGreen = ((Random.value / 1.5f) + 0.25f);
                //    float newBlue = ((Random.value / 1.5f) + 0.25f);
                //    float newAlpha = ((Random.value / 4) + 0.75f);

                //    renderer1.material.color = new Color(newRed, newGreen, newBlue, newAlpha);
                //}
                //newFoundParticle.transform.SetParent(particlesContainer.transform);

                // Now add some coins
                //int nbFoundSoFar = findOfMuchFound();
                float valueCoin = Random.value;

                if (valueCoin > 0.99f)
                {
                    ourCoinManager.gainSomeCoins(100, childCharacter.transform.position.x, childCharacter.transform.position.y);
                }
                else if (valueCoin > 0.85f)
                {
                    ourCoinManager.gainSomeCoins(50, childCharacter.transform.position.x, childCharacter.transform.position.y);
                }
                else if (valueCoin > 0.70f)
                {
                    ourCoinManager.gainSomeCoins(10, childCharacter.transform.position.x, childCharacter.transform.position.y);
                }
                else if (valueCoin > 0.55f)// (nbFoundSoFar % 5 == 0)
                {
                    ourCoinManager.gainSomeCoins(5, childCharacter.transform.position.x, childCharacter.transform.position.y);
                }
                else
                {
                    ourCoinManager.gainSomeCoins(1, childCharacter.transform.position.x, childCharacter.transform.position.y);
                }


                //AudioSource.PlayClipAtPoint(childCharacter.GetComponent<AudioSource>(), childCharacter.transform.localPosition);

                // Parc the object
                childCharacter.position = new Vector3(100, 0, -9);

                checkForGameOver();

                foundSomething = true;
            }
        }

        if (redBombs != null)
        {
            foreach (GameObject childRedBomb in redBombs)
            {
                if (childRedBomb.GetComponent<Collider2D>() == Physics2D.OverlapPoint(touchPos2))
                {
                    // Add a penality
                    doBombPenality(0);
                }
            }
        }
        if (redBombs != null)
        {
            foreach (GameObject childBlackBomb in blackBombs)
            {
                if (childBlackBomb.GetComponent<Collider2D>() == Physics2D.OverlapPoint(touchPos2))
                {
                    // Add a penality
                    doBombPenality(1);
                }
            }
        }
        if (traps != null)
        {
            foreach (GameObject childTraps in traps)
            {
                if (childTraps.GetComponent<Collider2D>() == Physics2D.OverlapPoint(touchPos2))
                {
                    // Add a penality
                    doBombPenality(2);
                }
            }
        }

        //if (!foundSomething && nbOfElements <= nbOfCharsToHint && !showingHint)
        //{
        //    showLastCharacters();
        //    showingHint = true;
        //}
    }

    private int findOfMuchFound()
    {
        return characters.transform.childCount - nbOfElements;
    }

    private void doBombPenality(int forType)
    {
        bombSound.Play();

        shakeCamera = 10;

        {
            if (forType == 0)
            {
                // Red bomb
                if (currentDifficulty == SelectDifficulty.hard)
                {
                    reawakePieces(5);
                }
                if (currentDifficulty == SelectDifficulty.veryHard)
                {
                    reawakePieces(10);
                }
                if (currentDifficulty == SelectDifficulty.insane)
                {
                    reawakePieces(20);
                }
                if (currentDifficulty == SelectDifficulty.purgatory)
                {
                    reawakePieces(1000);
                }
                if (currentDifficulty == SelectDifficulty.hell)
                {
                    reawakePieces(1000);
                }

                return;
            }
            if (forType == 1)
            {
                // Black bomb
                if (currentDifficulty == SelectDifficulty.hard)
                {
                    reawakePieces(2);
                }
                if (currentDifficulty == SelectDifficulty.veryHard)
                {
                    reawakePieces(5);
                }
                if (currentDifficulty == SelectDifficulty.insane)
                {
                    reawakePieces(10);
                }
                if (currentDifficulty == SelectDifficulty.purgatory)
                {
                    reawakePieces(1000);
                }
                if (currentDifficulty == SelectDifficulty.hell)
                {
                    reawakePieces(1000);
                }

                return;
            }
            if (forType == 2)
            {
                // Traps
                if (currentDifficulty == SelectDifficulty.purgatory)
                {
                    reawakePieces(20);
                }
                if (currentDifficulty == SelectDifficulty.hell)
                {
                    reawakePieces(40);
                }
                return;
            }
        }
    }

    private void reawakePieces(int howMany)
    {
        Debug.Log("reawakePieces " + howMany);

        int iReawaken = 0;

        foreach (Transform childCharacter in characters.transform)
        {
            if (childCharacter.position.x > 90)
            {
                childCharacter.transform.position = findNewPlace();

                iReawaken++;

                nbOfElements++;
                scoreOfLevel++;

                if (iReawaken > howMany)
                {
                    break;
                }
            }
        }
    }

    const int timeToShowHints = 120; // roughly 30s
    private const float xDiffBetweenHints = 0.30f;
    int timeShowingHints = timeToShowHints;
    Boolean showingHint = false;
    float alpha = 1.0f;
    float alphaSteps;

    List<Transform> charForArrows;

    // Was our small found?
    private bool ourSmallFound = false;

    public bool InTitle { get => inTitle; }
    public int ScoreOfLevel { get => scoreOfLevel; }
    public int NbOfElements { get => nbOfElements; }


    internal Vector2 getOneCharNotFound()
    {
        // Build their list

        foreach (Transform childCharacter in characters.transform)
        {
            if (childCharacter.position.x < 90)
            {
               return childCharacter.position;
            }
        }

        return new Vector2(1, 1);
    }

    private void updateModeVariables()
    {
        // Do something depending on the current mode
        nbOfElements--;
        scoreOfLevel--;

        // Save the time spent in the mode
        SaveTimeSpent();
    }

    private void LoadTimeSpent()
    {
        bool haskey = LocalSave.HasFloatKey("timeSpentLevel");

        if (haskey)
        {
            lastTimeSpentLevel = LocalSave.GetFloat("timeSpentLevel");
        }

        haskey = LocalSave.HasFloatKey("lastTimeSpent");

        if (haskey)
        {
            lastTimeSpent = LocalSave.GetFloat("lastTimeSpent");
        }
    }

    float savedTimeSpent;
    float savedTimeSpentMode;

    private void SaveTimeSpent()
    {
        savedTimeSpent = currentTime - initialTime + lastTimeSpentLevel;
        savedTimeSpentMode = currentTime - initialTime + lastTimeSpent;

        LocalSave.SetFloat("timeSpentLevel", savedTimeSpent);

        LocalSave.SetFloat("lastTimeSpent", savedTimeSpentMode);
    }

    void checkForGameOver()
    {
        // Do something depending on the current mode
        if (nbOfElements == 0)
        {
            // Remove all bombs if 
            if (traps != null)
            {
            foreach (GameObject oneTrap in traps)
            {
                Destroy(oneTrap);
            }
            }
            if (redBombs != null)
            {
            foreach (GameObject oneTrap in redBombs)
            {
                Destroy(oneTrap);
            }
            }
            if (blackBombs != null)
            {
            foreach (GameObject oneTrap in blackBombs)
            {
                Destroy(oneTrap);
            } 
            }

            // And start again
            setupObjects();
        }
    }

    void gameover()
    {
        loadNext();
    }

    void loadNext()
    {
        setNextLevelReached();

        long timeSpentLong = (long)savedTimeSpent;

        long hoursSpent = timeSpentLong / 3600;
        if (hoursSpent > 0)
        {
            timeSpentLong -= hoursSpent*3600;
        }

        long minutesSpent = timeSpentLong / 60;
        if (minutesSpent > 0)
        {
            timeSpentLong -= minutesSpent*60;
        }

        long timeSpentLongMode = (long)savedTimeSpentMode;

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
        // Reset the time for this level
        LocalSave.SetFloat("timeSpentLevel", 0);

        LocalSave.Save();

        // Outro or next level?
       
        ourShowResult.showResultText(hoursSpent, minutesSpent, timeSpentLong, hoursSpentMode, minutesSpentMode, timeSpentLongMode, rootNameOfLevel);
    }

    public void setUseTransparence(bool showTrans)
    {
        pieceTransparences = showTrans;
        //Debug.Log("Transparence: " + pieceTransparences + " - " + pieceTransparencesToggle.isOn+ " - " + showTrans);

        LocalSave.SetInt("transparentChars", pieceTransparences? 1 : 0);
        LocalSave.Save();
    }

    public void setShowHints(bool newShowHints)
    {
        showHints = newShowHints;
        LocalSave.SetInt("showHints", newShowHints ? 1 : 0);
        LocalSave.Save();
    }

    public void setShowArrows(bool newShowArrows)
    {
        showArrows = newShowArrows;
        LocalSave.SetInt("showArrows", newShowArrows ? 1 : 0);
        LocalSave.Save();
    }

    public void setNextLevelReached()
    {
        currentLevel++;

        LocalSave.SetInt("currentLevel", currentLevel);
        LocalSave.Save();
    }

    public void resetTimeSpent()
    {
        LocalSave.DeleteKey("bestTimeSpent");

        LocalSave.SetInt("timeSpentLevel", 0);

        LocalSave.DeleteKey("lastTimeSpent");

        LocalSave.Save();
    }
    
    public void resetSmallFound()
    {
        LocalSave.SetInt("smallFound", 0);
        LocalSave.Save();
    }

    public void resetSave()
    {
        resetTimeSpent();
        deleteAllSaves();

        loadFirstLevel();
    }

    private void loadFirstLevel()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(rootNameOfLevel);
    }

    // After the drum roll, start the music
    internal void playMusic()
    {
        if (!((AudioSource)GetComponent<AudioSource>()).isPlaying)
        {
            if (GetComponent<AudioSource>() != null)
            {
                ((AudioSource)GetComponent<AudioSource>()).Play();
            }
        }
    }

    internal static void deleteAllSaves()
    {
        LocalSave.deleteSave();

        LocalSave.deleteMemorySave();
    }
}
