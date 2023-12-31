using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class ManageMovingNyaf :  MainApp
{
    private const int DIVIDER_CAMERA_SPEED = 4; // By how much we devide the cursor deplacement to move the camera. Higher means slower camera.
    private const float REDUCTION_OF_SPEED = 0.8f; // By how much we multiply the speed at each frame. Should always be below 1 :), and smaller mean faster deceleration.

    private bool inGameOver = false; // Game won or lost, to show the end message

    private int currentLevel = 0;
    private String rootNameOfLevel = "MainBoard";

    public GetAndAnimateCoin ourCoinManager;

    public GameObject arrow;

    public GameObject backgroundImage;

    public GameObject characters;

    public GameObject hints; // The dynamic hints when less than a certain numbers of characters are left
    public GameObject arrows; // The dynamic arrows pointing at the characters when less than a certain numbers of characters are left

    public Transform redBombsParent;
    public Transform blackBombsParent;
    public Transform trapsParent;

    public GameObject bombForSound;

    public AudioSource drum;

    private List<Transform> redBombs = null;
    private List<Transform> blackBombs = null;
    private List<Transform> traps = null;

    private AudioSource bombSound;

    int shakeCamera = -1; // Give a visual clue a bomb has been touched.

    public GameObject particleFoundChar;

    public GameObject particlesContainer;

    private float minImageX = -20;
    private float maxImageX = 20;
    private float minImageY = -20;
    private float maxImageY = 20;
    private float minImageZ = -20;
    private float maxImageZ = 20;

    private int nbOfElements = 0;

    private int scoreOfLevel = 0;

    private int coolDown = 0; // Do not immediately do something else after some action

    private bool pieceTransparences = true;
    private bool showHints = true;
    private bool showArrows = true;

    //public Toggle pieceTransparencesToggle;
    //public Toggle showHintsToggle;
    //public Toggle showArrowsToggle;

    Vector3 targetPos;

    float currentTime;
    float lastTime;
    float initialTime;

    float lastTimeSpent = 0; // last saved time for the mode
    float lastTimeSpentLevel = 0; // last saved time for the level

    /// GAME MODE
    int currentGameMode = 0;
    int maxGameMode = 0;
    public const int absMaxMode = 5;

    int nbOfCharFoundTotal = 0;

    // Mode 1 variables
    // Find X number of characters, no time
    int nbOfPiecesFound = 0;
    private int nbOfPiecesToFind = 1500;

    // Mode 2 variables
    // Find all character, all characters reshuffle after a while
    float currentTimeBeforeReshuffle = 0;
    const float timeBeforeReshuffle = 30; // 30 seconds

    // Mode 3 variables
    // Get the lowest score possible
    // 20 second to find a piece, -2 for each piece found.
    float currentTimeAllocated = 20;
    const float timeForAPiece = 2;

    int currentDifficulty = 2;

    public int typeOfSounds = 0; // 0-> gibberish, 1-> synth sounds
    private List<AudioSource> allSynthSounds;
    private int nbOfSynthSound;

    bool isRotation = false; // No rotation in mode D and E

    // Achievements will be defined by an interface and free numbers of achievements

    // Use this for initialization
    void Start()
    {
        Application.targetFrameRate = 60;

        if (PlayAndKeepMusic.Instance != null)
        {
            PlayAndKeepMusic.Instance.Activated = false;
            PlayAndKeepMusic.Instance.stopMusic();
        }
        /////////////////////////////////////////////////////////////////////// DEBUG REMOVE AFTER /////////////////
        //resetSmallFound();

        // Get the kind of game from the menu
        initIntroductionBox();

        if (LocalSave.HasIntKey("MovingNYAF_currentLevel"))
        {
            currentLevel = LocalSave.GetInt("MovingNYAF_currentLevel");
        }

        // Check the preferences
        bool haskey = LocalSave.HasIntKey("MovingNYAF_difficulty");
        if (haskey)
        {
            currentDifficulty = LocalSave.GetInt("MovingNYAF_difficulty");
        }

        haskey = LocalSave.HasIntKey("MovingNYAF_transparentChars");
        if (haskey)
        {
            pieceTransparences = LocalSave.GetInt("MovingNYAF_transparentChars") == 1;

            //Debug.Log("Transparence: " + pieceTransparences + " - " + pieceTransparencesToggle.isOn);
        }

        haskey = LocalSave.HasIntKey("MovingNYAF_showHints");
        if (haskey)
        {
            showHints = LocalSave.GetInt("MovingNYAF_showHints") == 1;
        }

        haskey = LocalSave.HasIntKey("MovingNYAF_showArrows");
        if (haskey)
        {
            showArrows = LocalSave.GetInt("MovingNYAF_showArrows") == 1;
        }

        // Which sounds do we play
        haskey = LocalSave.HasIntKey("MovingNYAF_typeOfSounds");
        if (haskey)
        {
            typeOfSounds = LocalSave.GetInt("MovingNYAF_typeOfSounds");
        }
        loadSoundSynths();

        // Check how far we are
        haskey = LocalSave.HasIntKey("MovingNYAF_modeFinished");
        if (haskey)
        {
            maxGameMode = LocalSave.GetInt("MovingNYAF_modeFinished");
        }

        haskey = LocalSave.HasBoolKey("MovingNYAF_isRotation");
        if (haskey)
        {
            isRotation = LocalSave.GetBool("MovingNYAF_isRotation");
        }

		haskey = LocalSave.HasIntKey("Nyaf3dallCharFound");
        if (haskey)
        {
            nbOfCharFoundTotal = LocalSave.GetInt("Nyaf3dallCharFound");
        }

        Debug.Log("#### Current Mode is " + currentGameMode);

        if (PlayAndKeepMusic.Instance != null)
        {
            PlayAndKeepMusic.Instance.Activated = false;
            PlayAndKeepMusic.Instance.stopMusic();
        }

        // Load the last time spent
        LoadTimeSpent();

        // Position the objects to find
        setupObjects();

        // Create a hidden bomb to get the sound
        GameObject hiddenBlackBomb = Instantiate(bombForSound);
        hiddenBlackBomb.transform.position = new Vector2(1000, 1000);
        bombSound = hiddenBlackBomb.GetComponent<AudioSource>();

        lastTime = Time.time; // Last "frame" time
        initialTime = lastTime; // Level start time
        currentTime = lastTime;
    }

    private void loadSoundSynths()
    {
        //allSynthSounds = new List<AudioSource>();

        //// Find the container (by tag name), then add the list of sounds,.
        //GameObject allSynthSoundsGO = GameObject.FindGameObjectWithTag("NYAFSoundsSynth");

        //foreach (Transform oneSynthSound in allSynthSoundsGO.transform)
        //{
        //    AudioSource oneSource = oneSynthSound.gameObject.GetComponent<AudioSource>();

        //    allSynthSounds.Add(oneSource);
        //}

        //nbOfSynthSound = allSynthSounds.Count;
    }

    private void setupObjects()
    {
        // Check if their is a save available, otherwise start anew
        if (false && existSave())
        {
            loadLevel();

            Debug.Log("B-  Difficulty is " + currentDifficulty + " and nb of elements is" + nbOfElements);

            // The bombs are replaced at random at each time
            placeBombsIfNeeded();
        }
        else
        {
            foreach (Transform childCharacter in characters.transform)
            {
                if (childCharacter.position.x < 900)
                {
                    nbOfElements++;
                }
            }
            placeBombsIfNeeded();
        }
        showCharsToFind();
    }

    private void placeBombsIfNeeded()
    {
        placeBombs();

        placeTraps();
    }

    private void placeTraps()
    {
        traps = new List<Transform>();
        // Create fake objects from our pool :)

        foreach (Transform oneTrap in trapsParent)
        {
            traps.Add(oneTrap);
        }
    }

    private void placeBombs()
    {
        blackBombs = new List<Transform>();
        redBombs = new List<Transform>();

        foreach (Transform oneBlackBomb in blackBombsParent)
        {
            blackBombs.Add(oneBlackBomb);
        }

        foreach (Transform oneRedBomb in redBombsParent)
        {
            redBombs.Add(oneRedBomb);
        }
    }

    private void loadLevel()
    {
        //Debug.Log("Trying to load level " + giveSavePath());

        //if (File.Exists(giveSavePath()))
        //{
        //    allPosTaken = new List<Vector2>();

        //    nbOfElements = characters.transform.childCount;
        //    scoreOfLevel = nbOfElements;

        //    // 2
        //    String json = File.ReadAllText(giveSavePath(), System.Text.UTF8Encoding.UTF8);

        //    Save save = JsonUtility.FromJson<Save>(json);

        //    // 3
        //    for (int iChar = 0; iChar < save.charX.Count; iChar++)
        //    {
        //        Transform currentObject = characters.transform.GetChild(iChar);

        //        float posX = save.charX[iChar];
        //        float posY = save.charY[iChar];

        //        Boolean wasFound = save.charFound[iChar];
        //        float posZ = 2;

        //        if (wasFound)
        //        {
        //            posX = 1000;
        //            posY = 0;

        //            nbOfElements--;
        //            scoreOfLevel = nbOfElements;
        //        }
        //        else
        //        {
        //            allPosTaken.Add(new Vector2(posX, posY));
        //        }

        //        currentObject.transform.position = new Vector3(posX, posY, posZ);

        //        Color previousColor = currentObject.GetComponent<Renderer>().material.color;
        //        float alpha = 1;
        //        if (pieceTransparences)
        //        {
        //            alpha = save.alpha[iChar];
        //        }
        //        currentObject.GetComponent<Renderer>().material.color = new Color(previousColor.r, previousColor.g, previousColor.b, alpha);
        //    }
        //    int nbLeft = save.nbLeft;

        //    if (nbLeft != nbOfElements)
        //    {
        //        Debug.Log("PROBLEM!!!! Nb of elements left (" + nbOfElements + ")not matching with Save (" + nbLeft + ") and result from save");
        //    }

        //    Debug.Log("Game Loaded");
        //}
    }

    private void saveLevel()
    {
        //Debug.Log(Application.persistentDataPath);

        //if (File.Exists(giveSavePath()))
        //{
        //    File.Delete(giveSavePath());
        //}

        //if (!File.Exists(giveSavePath()))
        //{
        //    Save save = new Save();

        //    foreach (Transform childCharacter in characters.transform)
        //    {
        //        Boolean wasFound = false;

        //        if (childCharacter.position.x > 900)
        //        {
        //            // Not found
        //            wasFound = true;
        //        }

        //        float posX = childCharacter.transform.position.x;
        //        float posY = childCharacter.transform.position.y;

        //        float alpha = childCharacter.GetComponent<Renderer>().material.color.a;

        //        save.charX.Add(posX);
        //        save.charY.Add(posY);

        //        save.alpha.Add(alpha);
        //        save.charFound.Add(wasFound);
        //    }
        //    save.nbLeft = nbOfElements;

        //    string json = JsonUtility.ToJson(save);

        //    File.WriteAllText(giveSavePath(), json, System.Text.UTF8Encoding.UTF8);

        //    Debug.Log("Game saved");
        //}
        //else
        //{
        //    Debug.Log("PROBLEM: Could not delete save file");
        //}
    }

    /**
    * When the level is finished, we delete the current save.
    */
    private void deleteSave()
    {
        if (File.Exists(giveSavePath()))
        {
            File.Delete(giveSavePath());
        }
    }

    private string giveSavePath()
    {
        return Application.persistentDataPath + "/" + getCurrentMode() + "moving_gamesave" + currentLevel + ".save";
    }

    private bool existSave()
    {
        return File.Exists(giveSavePath());
    }

    private bool dragMode = false;
    private bool rotateMode = false;
    private bool zoomMode = false;
    private Vector3 totalDrag;
    private Vector3 dragOldPosition;

    private Vector3 zoomOldPosition1;
    private Vector3 zoomOldPosition2;

    private Vector2 dummyVector = new Vector2();

    float timeBeforeSave = 0;
    const float saveEachNSeconds = 30;

    private Vector2 moveByKeyboard = new Vector2();
    private Vector2 upKeyboard = new Vector2(0, 0.1f);
    private Vector2 downKeyboard = new Vector2(0, -0.1f);
    private Vector2 leftKeyboard = new Vector2(-0.1f, 0);
    private Vector2 rightKeyboard = new Vector2(0.1f, 0);

    // Update is called once per frame
    void Update()
    {
        if (gameIsInPause)
        {
            // Nothing to do
            return;
        }

        // Play the drum then the music
        if (drum != null)
        {
            if (!drum.isPlaying)
            {
                // Only if our music is not playing yet.
                if (!((AudioSource)GetComponent<AudioSource>()).isPlaying)
                {
                    Debug.Log("Removing title and starting music");

                    playMusic();
                }
            }
        }

        currentTime = Time.time;

        timeBeforeSave += Time.deltaTime;
        if (timeBeforeSave > saveEachNSeconds)
        {
            timeBeforeSave = 0;
            LocalSave.Save();
        }

        if (shakeCamera > 0)
        {
            shakeCamera--;
            float xRand = 0.2f * Random.value - 0.1f;
            float yRand = 0.2f * Random.value - 0.1f;
            float zRand = 0.2f * Random.value - 0.1f;
            transform.position = new Vector3(xRand, yRand, zRand);
        }
        else if (shakeCamera == 0)
        {
            shakeCamera = -1;
        }

        if (currentGameMode == 2)
        {
            if (currentTimeBeforeReshuffle > timeBeforeReshuffle)
            {
                currentTimeBeforeReshuffle = 0;

                //replaceActiveCharactersAtRandom();
            }
            currentTimeBeforeReshuffle += Time.deltaTime;
        }

        if (currentGameMode == 3)
        {
            currentTimeAllocated += Time.deltaTime;
        }

        if (coolDown != 0)
        {

            coolDown--;
        }

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

            if (Mouse.current.rightButton.isPressed)
            {
                rotateCamera(touchPos);
            }
            else
            {
                rotateMode = false;
            }


            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed)
            {
                moveByKeyboard += upKeyboard;
            }
            if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed)
            {
                moveByKeyboard += downKeyboard;
            }
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
            {
                moveByKeyboard += leftKeyboard;
            }
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
            {
                moveByKeyboard += rightKeyboard;
            }

            // Check if the user has clicked
            bool aTouch = Mouse.current.leftButton.wasPressedThisFrame;
            //bool rightClick = Input.GetMouseButton(1);

            if (aTouch)
            {
                // Debug.Log( "Moused moved to point " + touchPos );

                checkBoard(touchPos, dummyVector);
            }
            else
            {
                // No touch reset the cool down
                coolDown = 0;
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

            if (Mouse.current.scroll.ReadValue().y > 0f) // forward
            {
                zoomWheel(true);
            }
            else if (Mouse.current.scroll.ReadValue().y < 0f) // backwards
            {
                zoomWheel(false);
            }
        }

        // Move the camera if needed
        updateCameraPos();

        // Remove the hints when needed
        updateHints();

        // Keep absolute scale of GUI elements
        //score.transform.localScale = score.transform.localScale;
        lastTime = currentTime; // Last "frame" time
    }

    float currentWheelValue = normalWheelValue;

    private void zoomWheel(bool zoomIn)
    {
        if (zoomIn)
        {
            currentWheelValue += 0.03f;

            if (currentWheelValue > 0.3f)
            {
                currentWheelValue = 0.3f;
            }
        }
        else
        {
            // Zoom in
            currentWheelValue -= 0.03f;

            if (currentWheelValue < -0.3f)
            {
                currentWheelValue = -0.3f;
            }
        }

        cameraSpeed = new Vector3(cameraSpeed.x, currentWheelValue, cameraSpeed.z);

        //Debug.Log("Camera speed is " + cameraSpeed);
    }

    Vector3 cameraSpeed = new Vector3(0, 0, 0);

    private void moveCamera(Vector3 mousePos)
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos + new Vector3(0, 0, 4f));
        Vector3 positionDelta = worldPos - Camera.main.ScreenToWorldPoint(dragOldPosition + new Vector3(0, 0, 4f));

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
            Vector3 deltaZ = new Vector3(positionDelta.x, positionDelta.z, positionDelta.y);

            cameraSpeed -= deltaZ / DIVIDER_CAMERA_SPEED;

            //Camera.main.transform.position = RestrainCamera(cameraPos);
        }

        dragOldPosition = mousePos;
    }

    private float _sensitivity = 0.3f;
    private Vector3 _mouseReference;
    private Vector3 _mouseOffset;
    private Vector3 _rotation = Vector3.zero;
    //private bool _isRotating;
    private void rotateCamera(Vector3 mousePos)
    {
        if (currentGameMode != 2 && currentGameMode != 3)
        {
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);

            if (!rotateMode)
            {
                rotateMode = true;

                _rotation = Vector3.zero;

                _mouseReference = mousePos;
            }
            else
            {
                //Vector3 cameraPos = Camera.main.transform.position;

                //cameraPos -= positionDelta;
                //cameraRotateSpeed -= Vector3.Angle(worldPos, transform.forward);

                //if (cameraRotateSpeed > 0.01f)
                //{
                //    cameraRotateSpeed = 0.01f;
                //}
                //else if (cameraRotateSpeed < -0.01f)
                //{
                //    cameraRotateSpeed = -0.01f;
                //}
                ////Camera.main.transform.position = RestrainCamera(cameraPos);
                //transform.Rotate(Vector3.forward, cameraRotateSpeed);
                _mouseOffset = (mousePos - _mouseReference);

                // apply rotation
                _rotation.z = -(_mouseOffset.x + _mouseOffset.y) * _sensitivity;

                // rotate
                transform.Rotate(_rotation);

                // store mouse
                _mouseReference = mousePos;
            }
        }
    }

    float cameraRotateAcc = 0;
    float cameraRotateSpeed = 0;
    float damping = 0;
    float target = 1;
    float currentAngle = 0;
    Vector3 speedFromKeyboard;

    private void updateCameraPos()
    {
        Vector3 cameraPos = Camera.main.transform.position;
        speedFromKeyboard = new Vector3(moveByKeyboard.x, 0, moveByKeyboard.y);

        cameraPos += cameraSpeed + speedFromKeyboard;

        cameraSpeed = cameraSpeed * REDUCTION_OF_SPEED;
        moveByKeyboard = moveByKeyboard * REDUCTION_OF_SPEED;
        currentWheelValue = currentWheelValue * REDUCTION_OF_SPEED;

        if (cameraSpeed.magnitude < 0)
        {
            cameraSpeed = new Vector3(0, 0, 0);
        }

        Camera.main.transform.position = RestrainCamera(cameraPos);

        if (currentGameMode == 2 && isRotation)
        {
            // Rotate the camera a bit :)
            cameraRotateAcc += (target - currentAngle) * 0.005f;

            if (cameraRotateAcc > 0.01f)
            {
                cameraRotateAcc = 0.01f;
            }
            else if (cameraRotateAcc < -0.01f)
            {
                cameraRotateAcc = -0.01f;
            }

            cameraRotateSpeed += cameraRotateAcc;

            currentAngle += cameraRotateSpeed;

            if (cameraRotateSpeed > 0.1f)
            {
                damping = (cameraRotateSpeed - 0.1f) / 4;
            }
            else if (cameraRotateSpeed < -0.1f)
            {
                damping = (cameraRotateSpeed + 0.1f) / 4;
            }

            if (Mathf.Abs(target - currentAngle) < 0.01f)
            {
                // Push the target in the other direction
                target = -(Mathf.Sign(target) * (Random.value + 1));
            }

            transform.Rotate(Vector3.forward, cameraRotateSpeed);
        }

        if (currentGameMode == 3 && isRotation)
        {
            // Rotate the camera so the line from the center is vertical

            // What is the current up?
            float newRotation = Vector2.Angle(Camera.main.transform.position, Vector2.up);
            float currentRotation = Camera.main.transform.rotation.eulerAngles.z;
            float amountToRotate = 0;

            amountToRotate = newRotation - currentRotation;

            transform.Rotate(Vector3.forward, amountToRotate);
        }
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
            currentWheelValue = 0;
        }
        if (cameraPos.y < minImageY)
        {
            cameraPos.y = minImageY;
            currentWheelValue = 0;
        }
        if (cameraPos.z > maxImageZ)
        {
            cameraPos.z = maxImageZ;
        }
        else if (cameraPos.z < minImageZ)
        {
            cameraPos.z = minImageZ;
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
    private static readonly float normalWheelValue = 1.1f;
    private static readonly float normalZoomCamera = 0.6f;
    private static readonly float ratio = globalZoom / normalWheelValue;

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

        stepBetweenZoom = (normalWheelValue - globalZoom) / ((float)dimingTime);
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

    private const int nbOfCharsToHint = 12;

    public void checkBoard(Vector3 touchPos, Vector2 absPos, bool isTouch = true)
    {
        Ray ray = Camera.main.ScreenPointToRay(touchPos);
        RaycastHit hit;

        Boolean foundSomething = false;

        // Check if we found the object
        if (Physics.Raycast(ray, out hit, 100))
        {
            //Debug.Log("We hit " + hit.transform.gameObject.name);

            foreach (Transform childCharacter in characters.transform)
            {
                if (childCharacter == hit.transform)
                {
                    updateModeVariables();

                    // Play the attached sound
                    if (typeOfSounds == 0)
                    {
                        if (childCharacter.GetComponent<AudioSource>() != null)
                        {
                            ((AudioSource)childCharacter.GetComponent<AudioSource>()).Play();
                        }
                    }
                    //else if (typeOfSounds == 1)
                    //{
                    //    int soundtoPlay = (int)(Random.value * nbOfSynthSound);

                    //    allSynthSounds[soundtoPlay].Play();
                    //}
                    GameObject newFoundParticle = (GameObject)Instantiate(particleFoundChar, childCharacter.transform.position, Quaternion.identity);
                    Renderer renderer1 = newFoundParticle.GetComponent<Renderer>();
                    if (renderer1 != null)
                    {
                        float newRed = ((Random.value / 4) + 0.75f);
                        float newGreen = ((Random.value / 1.5f) + 0.25f);
                        float newBlue = ((Random.value / 1.5f) + 0.25f);
                        float newAlpha = ((Random.value / 4) + 0.75f);

                        renderer1.material.color = new Color(newRed, newGreen, newBlue, newAlpha);
                    }
                    newFoundParticle.transform.SetParent(particlesContainer.transform);

                    // Now add some coins
                    //int nbFoundSoFar = findOfMuchFound();
                    float valueCoin = Random.value;

                    if (valueCoin > 0.99f)
                    {
                        ourCoinManager.gainSomeCoins3D(100, childCharacter.transform.position.x, childCharacter.transform.position.y, childCharacter.transform.position.z);
                    }
                    else if (valueCoin > 0.97f)
                    {
                        ourCoinManager.gainSomeCoins3D(50, childCharacter.transform.position.x, childCharacter.transform.position.y, childCharacter.transform.position.z);
                    }
                    else if (valueCoin > 0.90f)
                    {
                        ourCoinManager.gainSomeCoins3D(10, childCharacter.transform.position.x, childCharacter.transform.position.y, childCharacter.transform.position.z);
                    }
                    else if (valueCoin > 0.8f)// (nbFoundSoFar % 5 == 0)
                    {
                        ourCoinManager.gainSomeCoins3D(5, childCharacter.transform.position.x, childCharacter.transform.position.y, childCharacter.transform.position.z);
                    }
                    else if (valueCoin > 0.55f)
                    {
                        ourCoinManager.gainSomeCoins3D(1, childCharacter.transform.position.x, childCharacter.transform.position.y, childCharacter.transform.position.z);
                    }


                    //AudioSource.PlayClipAtPoint(childCharacter.GetComponent<AudioSource>(), childCharacter.transform.localPosition);

                    // Parc the object
                    childCharacter.position = new Vector3(1000, 0, -9);

                    checkForGameOver();

                    // Update the hints
                    removeAllHints();
                    showCharsToFind();

                    foundSomething = true;
                }
            }
            foreach (Transform childRedBomb in redBombs)
            {
                if (childRedBomb == hit.transform)
                {
                    doBombPenality(0);
                }
            }
            foreach (Transform childBlackBomb in blackBombs)
            {
                if (childBlackBomb == hit.transform)
                {
                    doBombPenality(1);
                }
            }
            foreach (Transform childTraps in traps)
            {
                if (childTraps == hit.transform)
                {
                    doBombPenality(2);
                }
            }
        }

        if (!foundSomething && nbOfElements <= nbOfCharsToHint && !showingHint)
        {
            showLastCharacters();
            showingHint = true;
        }
    }

    private void doBombPenality(int forType)
    {
        bombSound.Play();

        shakeCamera = 10;

        //Debug.Log("Doing the penalty for type " + forType+ " and difficulty " + currentDifficulty);
        if (currentGameMode == 1)
        {
            if (forType == 0)
            {
                // Red bomb
                if (currentDifficulty == SelectDifficulty.hard)
                {
                    decreasePiecesFound(5);
                }
                if (currentDifficulty == SelectDifficulty.veryHard)
                {
                    decreasePiecesFound(10);
                }
                if (currentDifficulty == SelectDifficulty.insane)
                {
                    decreasePiecesFound(100);
                }
                if (currentDifficulty == SelectDifficulty.purgatory)
                {
                    decreasePiecesFound(500);
                }
                if (currentDifficulty == SelectDifficulty.hell)
                {
                    decreasePiecesFound(1000);
                }

                return;
            }
            if (forType == 1)
            {
                // Black bomb
                if (currentDifficulty == SelectDifficulty.hard)
                {
                    decreasePiecesFound(3);
                }
                if (currentDifficulty == SelectDifficulty.veryHard)
                {
                    decreasePiecesFound(5);
                }
                if (currentDifficulty == SelectDifficulty.insane)
                {
                    decreasePiecesFound(50);
                }
                if (currentDifficulty == SelectDifficulty.purgatory)
                {
                    decreasePiecesFound(300);
                }
                if (currentDifficulty == SelectDifficulty.hell)
                {
                    decreasePiecesFound(800);
                }

                return;
            }
            if (forType == 2)
            {
                // Traps
                if (currentDifficulty == SelectDifficulty.purgatory)
                {
                    decreasePiecesFound(200);
                }
                if (currentDifficulty == SelectDifficulty.hell)
                {
                    decreasePiecesFound(400);
                }
                return;
            }
        }
        else
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

    private void decreasePiecesFound(int nbOfPiecesToRemove)
    {
        nbOfPiecesFound -= nbOfPiecesToRemove;
        if (nbOfPiecesFound < 0)
        {
            nbOfPiecesFound = 0;
        }

        MMPGAddNewPlayerUnits.nyafBombExploded(nbOfPiecesToRemove, ourShowResult);
    }

    private void reawakePieces(int howMany)
    {
        Debug.Log("reawakePieces " + howMany);

        int iReawaken = 0;

        //foreach (Transform childCharacter in characters.transform)
        //{
        //    if (childCharacter.position.x > 90)
        //    {
        //        childCharacter.transform.position = findNewPlace();

        //        iReawaken++;

        //        nbOfElements++;
        //        scoreOfLevel++;

        //        if (iReawaken > howMany)
        //        {
        //            break;
        //        }
        //    }
        //}

        MMPGAddNewPlayerUnits.nyafBombExploded(howMany, ourShowResult);
    }

    const int timeToShowHints = 120; // roughly 30s
    private const float xDiffBetweenHints = 0.20f;
    int timeShowingHints = timeToShowHints;
    Boolean showingHint = false;
    float alpha = 1.0f;
    float alphaSteps;

    List<Transform> charForArrows;

    public int ScoreOfLevel { get => scoreOfLevel; }
    public int NbOfPiecesFound { get => nbOfPiecesFound; }
    public int NbOfElements { get => nbOfElements; }
    public float CurrentTimeAllocated { get => currentTimeAllocated; }
    public int NbOfPiecesToFind { get => nbOfPiecesToFind; }

    private void removeAllHints()
    {
        foreach (Transform childHint in hints.transform)
        {
            Destroy(childHint.gameObject);
        }
    }
    private void showCharsToFind()
    {
        int nbToShow = 0;
        foreach (Transform childCharacter in characters.transform)
        {
            if (childCharacter.position.x < 900)
            {
                nbToShow++;
            }
        }
        float minXHint = -((float)(nbToShow)) * xDiffBetweenHints / 2;
        float xInitial = minXHint;

        foreach (Transform childCharacter in characters.transform)
        {
            if (childCharacter.position.x < 900)
            {
                //Debug.Log("Showing character " + childCharacter);
                if (showHints)
                {
                    GameObject aNewHint;

                    aNewHint = Instantiate(childCharacter.gameObject);

                    MoveAroundInScene theirMoveAround = aNewHint.GetComponent<MoveAroundInScene>();
                    Destroy(theirMoveAround);
                    Collider theirCollider = aNewHint.GetComponent<Collider>();
                    Destroy(theirCollider);

                    // Now add as child of hints
                    aNewHint.transform.SetParent(hints.transform);
                    aNewHint.transform.localPosition = new Vector3(xInitial, -0.4f, 1.0f);
                    aNewHint.transform.localRotation = new Quaternion();
                    //aNewHint.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

                    aNewHint.SetActive(true);
                    aNewHint.GetComponent<Renderer>().material.color = new Color(aNewHint.GetComponent<Renderer>().material.color.r, aNewHint.GetComponent<Renderer>().material.color.g, aNewHint.GetComponent<Renderer>().material.color.b, 1);
                }

                xInitial += xDiffBetweenHints;
            }
        }
    }
    private void showLastCharacters()
    {
        alpha = 1;
        alphaSteps = alpha / ((float)timeToShowHints);
        int nbToShow = nbOfElements;

        if (currentDifficulty < SelectDifficulty.normal)
        {
            int nbLeftToShow = 0;

            // In easy and very easy, there might be much more elements to find, despite the player having only few left to find!
            foreach (Transform childCharacter in characters.transform)
            {
                if (childCharacter.position.x < 900)
                {
                    nbLeftToShow++;

                    if (nbLeftToShow == nbOfCharsToHint)
                    {
                        break;
                    }
                }
            }
            if (nbLeftToShow > nbOfElements)
            {
                nbToShow = nbLeftToShow;
            }
        }
        float minXHint = -((float)(nbToShow)) * xDiffBetweenHints / 2;
        
        float xInitial = minXHint;

        charForArrows = new List<Transform>();

        int iHelps = 0;

        foreach (Transform childCharacter in characters.transform)
        {
            if (childCharacter.position.x < 900)
            {
                if (showArrows)
                {
                    // And add the arrow
                    GameObject aNewArrow;

                    aNewArrow = Instantiate(arrow);
                    // Now add as child of hints
                    aNewArrow.transform.SetParent(arrows.transform);
                    aNewArrow.transform.localPosition = new Vector3(xInitial, -0.8f, 0.0f);

                    //Quaternion rotation = Quaternion.LookRotation(childCharacter.transform.position - aNewArrow.transform.position, aNewArrow.transform.TransformDirection(Vector3.back));
                    //aNewArrow.transform.rotation = new Quaternion(0, 0, rotation.z, rotation.w);

                    Vector3 dir = childCharacter.position - aNewArrow.transform.position;
                    float distanceToChar = 0.25f * Vector2.Distance(childCharacter.position, aNewArrow.transform.position);
                    if (distanceToChar > 1)
                    {
                        distanceToChar = 1;
                    }

                    charForArrows.Add(childCharacter);
                    float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

                    aNewArrow.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
                    //Debug.Log("Rotating by " + aNewArrow.transform.rotation);

                    aNewArrow.SetActive(true);
                    aNewArrow.GetComponent<Renderer>().material.color = new Color(aNewArrow.GetComponent<Renderer>().material.color.r, aNewArrow.GetComponent<Renderer>().material.color.g, aNewArrow.GetComponent<Renderer>().material.color.b, alpha * distanceToChar);
                }

                xInitial += xDiffBetweenHints;

                iHelps++;

                if (iHelps >= nbOfCharsToHint)
                {
                    break;
                }
            }
            if (iHelps >= nbOfCharsToHint)
            {
                break;
            }
        }
    }

    private void updateHints()
    {
        if (showingHint)
        {
            timeShowingHints--;

            if (timeShowingHints <= 0)
            {
                foreach (Transform childCharacter in arrows.transform)
                {
                    //Debug.Log("Destroying arrows!");
                    Destroy(childCharacter.gameObject);
                }
                timeShowingHints = timeToShowHints;
                showingHint = false;
            }
            else
            {
                int iChar = 0;

                foreach (Transform childCharacter in arrows.transform)
                {
                    Transform nextChar = charForArrows[iChar++];
                    float distanceToChar = 0.25f * Vector2.Distance(childCharacter.position, nextChar.transform.position);
                    if (distanceToChar > 1)
                    {
                        distanceToChar = 1;
                    }

                    childCharacter.GetComponent<Renderer>().material.color = new Color(childCharacter.GetComponent<Renderer>().material.color.r, childCharacter.GetComponent<Renderer>().material.color.g, childCharacter.GetComponent<Renderer>().material.color.b, alpha * distanceToChar);

                    //Quaternion rotation = Quaternion.LookRotation(previousObject.transform.position - childCharacter.transform.position, childCharacter.transform.TransformDirection(Vector3.back));
                    //childCharacter.transform.rotation = new Quaternion(0, 0, rotation.z, rotation.w);
                    Vector3 dir = nextChar.position - childCharacter.position;
                    float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

                    childCharacter.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

                    //childCharacter.Rotate(new Vector3(0, 0, 1), angle);
                    //Debug.Log("Rotating by " + childCharacter.transform.rotation);
                }
                alpha -= alphaSteps;
            }
        }
    }

    internal Vector2 getOneCharNotFound()
    {
        // Build their list
        List<Transform> allAvailableChilds = new List<Transform>();

        foreach (Transform childCharacter in characters.transform)
        {
            if (childCharacter.position.x < 900)
            {
                allAvailableChilds.Add(childCharacter);
            }
        }

        if (allAvailableChilds.Count == 0)
        {
            return new Vector2(1, 1);
        }

        int childIndexToReturn = (int)(Random.value * ((float)allAvailableChilds.Count));

        return (allAvailableChilds[childIndexToReturn].position);
    }

    private void updateModeVariables()
    {
        // Do something depending on the current mode
        nbOfElements--;
        scoreOfLevel--;

        nbOfCharFoundTotal++;
        LocalSave.SetInt("Nyaf3dallCharFound", nbOfCharFoundTotal);

        // Save the time spent in the mode
        SaveTimeSpent();

        // In game mode 3, remove 20 sec
        if (currentGameMode == 3)
        {
            currentTimeAllocated -= timeForAPiece;

            // Save the time
            SaveGlobalTime();
        }
    }

    private void LoadTimeSpent()
    {
        bool haskey = LocalSave.HasFloatKey("allTimeSpent" + currentGameMode);

        if (haskey)
        {
            lastTimeSpent = LocalSave.GetFloat("allTimeSpent" + currentGameMode);
        }

        haskey = LocalSave.HasFloatKey("timeSpentLevel" + currentGameMode + "-" + currentLevel);

        if (haskey)
        {
            lastTimeSpentLevel = LocalSave.GetFloat("timeSpentLevel" + currentGameMode + "-" + currentLevel);
        }
    }

    float savedTimeSpent;
    float savedTimeSpentMode;

    private void SaveTimeSpent()
    {
        savedTimeSpent = currentTime - initialTime + lastTimeSpentLevel;
        savedTimeSpentMode = currentTime - initialTime + lastTimeSpent;

        LocalSave.SetFloat("allTimeSpent" + currentGameMode, savedTimeSpentMode);

        LocalSave.SetFloat("timeSpentLevel" + currentGameMode + "-" + currentLevel, savedTimeSpent);
        //LocalSave.Save();
    }

    void checkForGameOver()
    {
        // Do something depending on the current mode
        if (currentGameMode != 1 && nbOfElements == 0)
        {
            LocalSave.SaveCopy();

            deleteSave();
            gameover();
        }
        else if (currentGameMode == 1)
        {
            if (nbOfPiecesFound >= nbOfPiecesToFind)
            {
                deleteSave();
                // TODO!!!! Reinit the nb of pieces
                gameWonMode1();
            }
            else if (nbOfElements == 0)
            {
                // Replace the pieces in the same level.
                deleteSave();

                LocalSave.SaveCopy();

                setupObjects();
            }
        }
        else
        {
            // Otherwise save the current level
            saveLevel();
        }
    }

    void gameWonMode1()
    {
        PlayAndKeepMusic.Instance.Activated = false;
        PlayAndKeepMusic.Instance.stopMusic();

        setGameModeReached();

        LocalSave.SetInt("nbOfPiecesFound", 0);
        LocalSave.Save();

        UnityEngine.SceneManagement.SceneManager.LoadScene("OutroFindingNyaf");
    }

    void gameover()
    {
        loadNext();
    }

    private void SaveGlobalTime()
    {
        LocalSave.SetFloat("currentTimeAllocated", currentTimeAllocated);
        //LocalSave.Save();
    }

    private void SaveNbOfPiecesLeft()
    {
        LocalSave.SetInt("nbOfPiecesFound", nbOfPiecesFound);
        LocalSave.Save();
    }

    private void LoadGlobalTime()
    {
        bool haskey = LocalSave.HasFloatKey("currentTimeAllocated");

        if (haskey)
        {
            currentTimeAllocated = LocalSave.GetFloat("currentTimeAllocated");
        }
    }

    private void LoadNbOfPiecesLeft()
    {
        bool haskey = LocalSave.HasIntKey("nbOfPiecesFound");

        if (haskey)
        {
            nbOfPiecesFound = LocalSave.GetInt("nbOfPiecesFound");
        }
    }

    void loadNext()
    {
        setNextLevelReached();

        long timeSpentLong = (long)savedTimeSpent;

        long hoursSpent = timeSpentLong / 3600;
        if (hoursSpent > 0)
        {
            timeSpentLong -= hoursSpent * 3600;
        }

        long minutesSpent = timeSpentLong / 60;
        if (minutesSpent > 0)
        {
            timeSpentLong -= minutesSpent * 60;
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
        LocalSave.SetFloat("MovingNYAF_timeSpentLevel" + currentGameMode + "-" + currentLevel, 0);

        LocalSave.Save();

        // Outro or next level?
        if (currentLevel == 12 && (currentGameMode == 0 || currentGameMode >= 2))
        {
            setGameModeReached();

            //UnityEngine.SceneManagement.SceneManager.LoadScene("Outro");
            ourShowResult.showResultText(hoursSpent, minutesSpent, timeSpentLong, hoursSpentMode, minutesSpentMode, timeSpentLongMode, "OutroMovingNyaf");
        }
        else
        {
            //UnityEngine.SceneManagement.SceneManager.LoadScene(rootNameOfLevel + (currentLevel + 1));
            //ourShowResult.showResultText(hoursSpent, minutesSpent, timeSpentLong, hoursSpentMode, minutesSpentMode, timeSpentLongMode, rootNameOfLevel + (currentLevel + 1));
            //LocalSave.SetInt(LoadPanAndZoomComics.KeyComicsLevel, currentLevel);
            //LocalSave.SetInt(LoadPanAndZoomComics.KeyComicsMode, currentGameMode);
            //LocalSave.Save();

            LocalSave.SetInt("MovingNYAF_currentLevel", currentLevel++);
            LocalSave.Save();

            ourShowResult.showResultText(hoursSpent, minutesSpent, timeSpentLong, hoursSpentMode, minutesSpentMode, timeSpentLongMode, "FindingNYAFINGS/MovingNyafings");
        }
    }

    public void setUseTransparence(bool showTrans)
    {
        pieceTransparences = showTrans;
        //Debug.Log("Transparence: " + pieceTransparences + " - " + pieceTransparencesToggle.isOn+ " - " + showTrans);

        LocalSave.SetInt("MovingNYAF_transparentChars", pieceTransparences ? 1 : 0);
        LocalSave.Save();
    }

    public void setShowHints(bool newShowHints)
    {
        showHints = newShowHints;
        LocalSave.SetInt("MovingNYAF_showHints", newShowHints ? 1 : 0);
        LocalSave.Save();
    }

    public void setShowArrows(bool newShowArrows)
    {
        showArrows = newShowArrows;
        LocalSave.SetInt("MovingNYAF_showArrows", newShowArrows ? 1 : 0);
        LocalSave.Save();
    }

    public void setNextLevelReached()
    {
        bool haskey = LocalSave.HasIntKey("MovingNYAF_levelReached" + getCurrentMode());
        int levelReached = 0;
        if (haskey)
        {
            levelReached = LocalSave.GetInt("MovingNYAF_levelReached" + getCurrentMode());
        }

        if (currentLevel >= levelReached)
        {
            LocalSave.SetInt("MovingNYAF_levelReached" + getCurrentMode(), currentLevel + 1);
            LocalSave.Save();
        }
        MMPGAddNewPlayerUnits.nyafLevelEnded(currentLevel, currentGameMode, ourShowResult);
    }

    /**
     * In some game mode, the levels are all accessible.
     */
    public void setAllLevelReached()
    {
        LocalSave.SetInt("MovingNYAF_levelReached1", 11);
        LocalSave.Save();
    }

    internal string getCurrentMode()
    {
        String gameMode = "";
        if (currentGameMode > 0)
        {
            gameMode = "" + currentGameMode;
        }

        return gameMode;
    }

    /**
     * When called from another script at start, we might want to look from the save to be sure the data is the latest.
    */
    internal string getCurrentModeFromSave()
    {
        String gameMode = "";

        int currentGameModeFromSave = 0;

        bool haskey = LocalSave.HasIntKey("MovingNYAF_currentMode");
        if (haskey)
        {
            currentGameModeFromSave = LocalSave.GetInt("MovingNYAF_currentMode");
        }

        if (currentGameModeFromSave > 0)
        {
            gameMode = "" + currentGameModeFromSave;
        }

        return gameMode;
    }

    public int getCurrentModeNb()
    {
        return currentGameMode;
    }

    public void setCurrentModeNb(int newMode)
    {
        currentGameMode = newMode;

        LocalSave.SetInt("MovingNYAF_currentMode", newMode);
        LocalSave.Save();
    }

    public void resetLevelReached()
    {
        LocalSave.SetInt("MovingNYAF_levelReached", 1);
        LocalSave.Save();

        LocalSave.SetInt("MovingNYAF_levelReached1", 1);
        LocalSave.Save();

        LocalSave.SetInt("MovingNYAF_levelReached2", 1);
        LocalSave.Save();

        LocalSave.SetInt("MovingNYAF_levelReached3", 1);
        LocalSave.Save();
    }

    public void resetModeReached()
    {
        LocalSave.SetInt("MovingNYAF_modeFinished", 0);
        LocalSave.Save();

        LocalSave.SetInt("MovingNYAF_currentMode", 0);
        LocalSave.Save();
    }

    public void resetTimeSpent()
    {
        for (int iMode = 0; iMode < 4; iMode++)
        {
            LocalSave.DeleteKey("MovingNYAF_bestTimeSpent" + iMode);
            LocalSave.SetInt("MovingNYAF__allTimeSpent" + iMode, 0);
            for (int iLevel = 1; iLevel < 12; iLevel++)
            {
                LocalSave.SetFloat("MovingNYAF_timeSpentLevel" + iMode + "-" + iLevel, 0);
            }
        }
        LocalSave.DeleteKey("MovingNYAF_bestScoreMode3");

        LocalSave.Save();
    }

    public void setGameModeReached()
    {
        bool haskey = LocalSave.HasIntKey("MovingNYAF_modeFinished");
        int maxModeReached = 0;
        if (haskey)
        {
            maxModeReached = LocalSave.GetInt("MovingNYAF_modeFinished");
        }

        LocalSave.SetInt("MovingNYAF_lastModeFinished", currentGameMode);

        MMPGAddNewPlayerUnits.nyafModeFinished(ourShowResult, currentGameMode == 1);

        if (currentGameMode >= maxModeReached && currentGameMode < absMaxMode)
        {
            LocalSave.SetInt("MovingNYAF_modeFinished", currentGameMode + 1);
            LocalSave.Save();

            // And select the next level
            setCurrentModeNb(currentGameMode + 1);
        }

        //switch (currentGameMode)
        //{
        //    // Each upper mode means that the other modes have been finished too.
        //    case 4:
        //        InitializeSteam.unlockAchievement(InitializeSteam.achievementKey.NYAF_MODE_E_FINISHED);
        //        goto case 3;
        //    case 3:
        //        InitializeSteam.unlockAchievement(InitializeSteam.achievementKey.NYAF_MODE_D_FINISHED);
        //        goto case 2;
        //    case 2:
        //        InitializeSteam.unlockAchievement(InitializeSteam.achievementKey.NYAF_MODE_C_FINISHED);
        //        goto case 1;
        //    case 1:
        //        InitializeSteam.unlockAchievement(InitializeSteam.achievementKey.NYAF_MODE_B_FINISHED);
        //        goto case 0;
        //    case 0:
        //        InitializeSteam.unlockAchievement(InitializeSteam.achievementKey.NYAF_MODE_A_FINISHED);
        //        break; // You really thought I will loop to 4, no?
        //}
    }

    public void smallFound()
    {
        bool haskey = LocalSave.HasIntKey("MovingNYAF_smallFound");
        int allSmallFoundFlag = 0;
        if (haskey)
        {
            allSmallFoundFlag = LocalSave.GetInt("MovingNYAF_smallFound");
        }

        int currentOne = 2 << currentLevel;

        //switch (currentLevel)
        //{
        //    case 1:
        //        InitializeSteam.unlockAchievement(InitializeSteam.achievementKey.SMALL_SECRET_1);
        //        break;
        //    case 2:
        //        InitializeSteam.unlockAchievement(InitializeSteam.achievementKey.SMALL_SECRET_2);
        //        break;
        //    case 3:
        //        InitializeSteam.unlockAchievement(InitializeSteam.achievementKey.SMALL_SECRET_3);
        //        break;
        //    case 4:
        //        InitializeSteam.unlockAchievement(InitializeSteam.achievementKey.SMALL_SECRET_4);
        //        break;
        //    case 5:
        //        InitializeSteam.unlockAchievement(InitializeSteam.achievementKey.SMALL_SECRET_5);
        //        break;
        //    case 6:
        //        InitializeSteam.unlockAchievement(InitializeSteam.achievementKey.SMALL_SECRET_6);
        //        break;
        //    case 7:
        //        InitializeSteam.unlockAchievement(InitializeSteam.achievementKey.SMALL_SECRET_7);
        //        break;
        //    case 8:
        //        InitializeSteam.unlockAchievement(InitializeSteam.achievementKey.SMALL_SECRET_8);
        //        break;
        //    case 9:
        //        InitializeSteam.unlockAchievement(InitializeSteam.achievementKey.SMALL_SECRET_9);
        //        break;
        //    case 10:
        //        InitializeSteam.unlockAchievement(InitializeSteam.achievementKey.SMALL_SECRET_10);
        //        break;
        //    case 11:
        //        InitializeSteam.unlockAchievement(InitializeSteam.achievementKey.SMALL_SECRET_11);
        //        break;
        //}

        allSmallFoundFlag = allSmallFoundFlag | currentOne;

        LocalSave.SetInt("MovingNYAF_smallFound", allSmallFoundFlag);
        LocalSave.Save();

        // And show a * in the score.
    }

    public bool wasCurrentSmallFound()
    {
        int currentOne = 2 << currentLevel;

        bool haskey = LocalSave.HasIntKey("MovingNYAF_smallFound");
        int allSmallFoundFlag = 0;
        if (haskey)
        {
            allSmallFoundFlag = LocalSave.GetInt("MovingNYAF_smallFound");

            if ((allSmallFoundFlag & currentOne) > 0)
            {
                return true;
            }
        }
        //Debug.Log("Am here!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!"+ currentOne+" - "+ levelReached);
        return false;
    }

    public void resetSmallFound()
    {
        LocalSave.SetInt("MovingNYAF_smallFound", 0);
        LocalSave.Save();
    }

    public void resetSave()
    {
        resetLevelReached();
        resetSmallFound();
        resetModeReached();
        resetTimeSpent();
        deleteLevelSaves();

        loadFirstLevel();
    }

    private void loadFirstLevel()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(rootNameOfLevel);
    }

    // After the drum roll, start the music
    internal void playMusic()
    {
        if (currentGameMode != 1)
        {
            if (!((AudioSource)GetComponent<AudioSource>()).isPlaying)
            {
                if (GetComponent<AudioSource>() != null)
                {
                    ((AudioSource)GetComponent<AudioSource>()).Play();
                }
            }
        }
        else
        {
            PlayAndKeepMusic.Instance.Activated = true;
        }
    }

    internal static void deleteLevelSaves()
    {
        // It is not very intensive, so we plan for more modes and levels.
        for (int iMode = 0; iMode < 10; iMode++)
        {
            for (int iLevel = 0; iLevel < 30; iLevel++)
            {
                String pathToDelete = Application.persistentDataPath + "/MovingNYAF_" + iMode + "gamesave" + iLevel + ".save";

                if (iMode == 0)
                {
                    pathToDelete = Application.persistentDataPath + "/MovingNYAF_gamesave" + iLevel + ".save";
                }

                if (File.Exists(pathToDelete))
                {
                    File.Delete(pathToDelete);
                }
            }
        }
    }

    internal void restoreFromBackup()
    {
        LocalSave.restoreFromBackup();

        loadFirstLevel();
    }

    internal void SetTypeOfSound(int newTypeOfSound)
    {
        LocalSave.SetInt("MovingNYAF_typeOfSounds", newTypeOfSound);
        LocalSave.Save();

        typeOfSounds = newTypeOfSound;
    }

    internal void setIsRotation(bool newIsRotation)
    {
        LocalSave.SetBool("MovingNYAF_isRotation", newIsRotation);
        LocalSave.Save();

        isRotation = newIsRotation;
    }
}