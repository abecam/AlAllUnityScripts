using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class BJSMainLoop : MonoBehaviour
{
    public int currentLevel = 0;

    public GameObject title;

    private JohnVariables currentJohnVariables;
    private GameObject johnWalk;
    private GameObject johnReturn;
    private GameObject currentWalkingJohn;

    private GameObject triumphantJohn;

    // Get together the tower and the attached John
    private GameObject towerAndJohn;

    private GameObject towerOnly;

    private GameObject johnRing;

    private Rigidbody2D johnRingRB;

    private GameObject currentBell;
    private Rigidbody2D currentBellRB;

    AudioSource musicSource; // The music is attached to the parent object of the walking Johns

    public AudioSource lastJohnScream; // The very last scream, not attached to the character

    public GameObject monasterLights;
    public GameObject cityLight;

    // Intro objects
    public GameObject johnRoomLight;

   
    public GameObject monasterOpenDoor;

    public GameObject monasterFrameDoor;

    public GameObject monasterFullDoor;
    Explodable fullDoorExplodable;
    public GameObject containerOfFragment;

    public GameObject fullMonaster;

    public GameObject megaJohnGhost;

    public GameObject sun;

    public GameObject bigSun;

    public GameObject nbOfMonksEarned;
    private TextMesh nbOfMonksEarnedTM;

    private const float xSunInit = 10;
    private const float ySunInit = 0.8f;

    private float xSun = xSunInit;
    private float ySun = ySunInit;

    private GameObject[] megaJohnGhosts = new GameObject[5];

    private float[] xJohnOld = new float[50];
    private float[] yJohnOld = new float[50];

    bool isGhost = false;

    int currentPhase = -2;

    private bool startingGame = false;
    private bool inGameOver = false;
    private bool gameIsInPause = false;

    //Animation walkAnimation;
    //Animation ringAnimation;
    //Animation returnAnimation;

    private Animator walkAnimator;
    private Animator ringAnimator;
    private Animator returnAnimator;

    HingeJoint2D hingeArm;

    private int currentJohn = 1;
    private int currentBellRelative = 0; // The bell for a John
    int levelJohn = 1;
    int currentDay = 1;
    float xpJohn = 0;
    float xpNeeded = 400;
    float xpToAdd = 400;

    public GameObject curtain; // To fade the scene at the start/end
    Renderer curtainRenderer;

    public TextMesh transitionText;

    private GameObject landscape;

    private List<Renderer>  backNightRenderers; // To scroll and fade to the day background

    const float defaultStartJump = 0.99f;

    float xSteps ;

    private float xJohnMaxSpeed = 0.99f;
    private float xJohnStartJump = 0.99f;
    private float yJohnHeightJump = -3.93f;
    private float speedJumpY = 0.04f;

    private float johnStrength = 400;

    public ListOfBellsAndJohn ourListOfLevels;

    BJSSave ourSaveFacility = new BJSSave(); // Where we will save

    private float currentTime;
    private float lastTime;
    private float initialTime;

    private string lastCurrentDay;
    private float allTimeSpent = 0;
    private float lastTimeSpent = 0;

    internal void pauseGame()
    {
        gameIsInPause = true;
    }
    internal void unPauseGame()
    {
        gameIsInPause = false;
    }

    /**
    * Phases: 0-> walk to bell, 1-> ...
    */

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;

        if (PlayAndKeepMusic.Instance != null)
        {
        PlayAndKeepMusic.Instance.Activated = false;
        PlayAndKeepMusic.Instance.stopMusic();
        }
        curtain.SetActive(true);
        curtainRenderer = curtain.GetComponent<Renderer>();

        nbOfMonksEarnedTM = nbOfMonksEarned.GetComponent<TextMesh>();

        fullDoorExplodable = monasterFullDoor.GetComponent<Explodable>();
        fullDoorExplodable.newParent = containerOfFragment;

        towerAndJohn = GameObject.FindWithTag("FullTower"); // Then we will have it as it will be instanciated here

        GameObject backgroundNight = GameObject.FindWithTag("BackNight");

        backNightRenderers = new List<Renderer>();

        foreach (Transform childCharacter in backgroundNight.transform)
        {
            backNightRenderers.Add(childCharacter.gameObject.GetComponent<Renderer>());
            foreach (Transform childChildCharacter in childCharacter)
            {
                backNightRenderers.Add(childChildCharacter.gameObject.GetComponent<Renderer>());
            }
        }
        landscape = GameObject.FindWithTag("Landscape"); // To scroll together with the camera

        updateXJohn();

        Debug.Log("xInitial: " + xJohnInitial);

        xJohn = xJohnInitial;

        // Load the last time spent
        LoadTimeSpent();

        // Check if save exists
        if (ourSaveFacility.isSave())
        {
            Debug.Log("Loading last values");
            // Save all value, then get the right level/bell from the container
            ourSaveFacility.loadAllValues();

            currentJohn = ourSaveFacility.CurrentJohn;
            levelJohn = ourSaveFacility.LevelJohn;
            currentDay = ourSaveFacility.CurrentDay;
            currentBellRelative = ourSaveFacility.CurrentBellForLevel;
            xpJohn = ourSaveFacility.XpJohn;
            xpNeeded = ourSaveFacility.XpNeeded;
        }
        loadBellAndJohn();

        loadJohnBellsAndVariables();

        lastTime = Time.time; // Last "frame" time
        initialTime = lastTime; // Level start time
        currentTime = lastTime;
    }

    private void loadJohnBellsAndVariables()
    {
        // Will change with the save (?)
        johnWalk = GameObject.FindWithTag("JohnWalk");

        johnReturn = GameObject.FindWithTag("JohnReturn");

        triumphantJohn = GameObject.FindWithTag("JohnTriumphant");

        loadBells();

        currentWalkingJohn = GameObject.FindWithTag("JohnWalking");
        musicSource = (AudioSource)currentWalkingJohn.GetComponent<AudioSource>();
        currentJohnVariables = (JohnVariables)currentWalkingJohn.GetComponent<JohnVariables>();

        xSteps = (xJohnStartJump - xJohnInitial) / currentJohnVariables.stepsForCrossing;
        xJohnMaxSpeed = currentJohnVariables.speedMaxX;
        xJohnStartJump = currentJohnVariables.xJohnStartJump;
        yJohnHeightJump = currentJohnVariables.yJohnHeightJump;
        speedJumpY = currentJohnVariables.speedMaxY;
        johnStrength = currentJohnVariables.strength;
        yFloor = currentJohnVariables.yFloor;
        yJohn = yFloor;

        horForce = new Vector2(johnStrength/20,0);

        johnWalk.transform.position = new Vector3(xJohn, yJohn, -1);
       
        johnReturn.SetActive(false);
        triumphantJohn.SetActive(false);

        walkAnimator = (Animator)johnWalk.GetComponent<Animator>();
        
        returnAnimator = (Animator)johnReturn.GetComponent<Animator>();
    }

    private void loadBellAndJohn()
    {
        // Instantiate the next tower if needed
        GameObject nextLevel = ourListOfLevels.getTowerByNb(currentJohn, currentBellRelative);    

        isLastLevel = false;
        
        if (ourListOfLevels.IsLast)
        {
            isLastLevel = true;
        }
        Debug.Log("Check if last level "+isLastLevel);
        towerAndJohn = Instantiate(nextLevel);

        {
            // And get the new John
            GameObject newWalkingJohn = ourListOfLevels.getWalkingJohnByNb(currentJohn);

            Instantiate(newWalkingJohn);
        }

        isGhost = false;

        if (currentJohn == 4 && !isLastLevel)
        {
            isGhost = true;

            // Also instantiate the ghosts :)
            for (int iGhost = 0; iGhost < 5; iGhost++)
            {
                megaJohnGhosts[iGhost] = Instantiate(megaJohnGhost);
                megaJohnGhosts[iGhost].transform.position = new Vector2(xJohn, yJohn);

                // Not so correct now...
                xJohnOld[iGhost] = xJohn;
                yJohnOld[iGhost] = yJohn;
            }
        }

        bellWillBreakAt = 4 + ((currentJohn - 1) * 24) + (currentBellRelative) * 4;
        Debug.Log("Staring: Bell will break at level " + bellWillBreakAt);
    }

    private void loadBells()
    {
        Debug.Log("Loading new bells and Ringing John");

        // Will change with the save (?)
        if (GameObject.FindGameObjectsWithTag("Clocher").Length > 1)
        {
            Debug.Log("ERROR - GOT "+ GameObject.FindGameObjectsWithTag("Clocher").Length+" towers");
        }
        towerOnly = GameObject.FindWithTag("Clocher");
        johnRing = GameObject.FindWithTag("JohnRope");
        currentBell = GameObject.FindWithTag("Bell");

        Debug.Log("Loaded"+ towerOnly +" - " + johnRing+" - "+currentBell);

        // And attach ourself to the breaking joint script
        ((BellDetached)towerOnly.GetComponent<BellDetached>()).theMainScript = this;

        // John on the rope need to stay active as he is a physics object, otherwise is he detached from the rope
        johnRing.transform.position = new Vector3(johnRing.transform.position.x, johnRing.transform.position.y, 20);

        currentBellRB = currentBell.GetComponent<Rigidbody2D>();
        johnRingRB = johnRing.GetComponent<Rigidbody2D>();

        ringAnimator = (Animator)johnRing.GetComponent<Animator>();

        hingeArm = johnRing.GetComponent<HingeJoint2D>();

        if (currentJohn < 4)
        {
            monasterFullDoor.SetActive(true);
            monasterOpenDoor.SetActive(false);
        }
        else
        {
            // Mega John has another way of getting out...
            monasterOpenDoor.SetActive(false);
            monasterFrameDoor.SetActive(false);
            monasterFullDoor.SetActive(true);

            fullDoorExplodable.cleanUp();
        }

        bellSound = ((AudioSource)currentBell.GetComponent<AudioSource>());
        johnRingSound = ((AudioSource)johnRing.GetComponent<AudioSource>());
    }

    // Update is called once per frame
    void Update()
    {
        if (gameIsInPause)
        {
            return;
        }

        currentTime = Time.time;

        if (newBellLoaded && towerOnly == null)
        {
            Debug.Log("Update - Tower null!!!");
        }

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

        playMusic(); // Check if we can start playing our music

        updateXJohn();

        // Update John's position
        // Need to be here because John continue to move in outro phases... So the ghost needs his updates!
        if (johnRing != null && (currentPhase == 2 || currentPhase >= 5))
        {
            xJohn = johnRing.transform.position.x;
            yJohn = johnRing.transform.position.y;
        }

        // Also update the ghosts
        if (isGhost)
        {
            // Also instantiate the ghosts :)
            for (int iGhost = 0; iGhost < 5; iGhost++)
            {
                megaJohnGhosts[iGhost].transform.position = new Vector2(xJohnOld[iGhost * 10], yJohnOld[iGhost * 10]);

                if (iGhost > 0)
                {
                    xJohnOld[iGhost] = xJohnOld[iGhost - 1];
                    yJohnOld[iGhost] = yJohnOld[iGhost - 1] + 0.2f;
                }
            }

            // Very dumb, should be changed when more awake
            // We use 10x more position (trailing, x current = x from previous pos at previous time) to have the trail slower.
            for (int iGhost = 0; iGhost < 50; iGhost++)
            {
                if (iGhost > 0)
                {
                    xJohnOld[iGhost] = xJohnOld[iGhost - 1] + 0.02f * (Random.value - 0.5f);
                    yJohnOld[iGhost] = yJohnOld[iGhost - 1] + 0.005f + 0.01f * (Random.value - 0.5f);
                }
            }

            xJohnOld[0] = xJohn;
            yJohnOld[0] = yJohn;
        }

        if (currentPhase == -3)
        {
            manageRestart();
            return;
        }
        if (currentPhase == -2)
        {
            manageStart();
            return;
        }
        if (currentPhase == 5)
        {
            manageNextLevel();
            return;
        }
        if (currentPhase == 6)
        {
            manageOutro();
            return;
        }
        doMainLoop();
    }

    private void updateXJohn()
    {
        xJohnInitial = -2.53f + fullMonaster.transform.position.x;

        xSteps = (defaultStartJump - xJohnInitial) / nbOfJohnSteps;
    }

    private const float normalZoom = 5;
    private const float globalZoom = 15;
    private const float stepBetweenZoom = (globalZoom - normalZoom) / 120;

    private const int maxPush = 10;
    int push = 0;
    bool sunBroken = false;

    private const int timeBeforeOutro = 300;
    private int timeAfterSunBroke = 0;

    // When the sun breaks, John and the bell darken...

    private void manageOutro()
    {
        if (push < maxPush)
        {
            johnRingRB.AddForce(lastForce);
            push++;
        }

        // Check if John did not miss the sun
        if (!sunBroken && johnRing.transform.localPosition.x > 150)
        {
            // And do the special outro
            timeAfterSunBroke++;

            if (timeAfterSunBroke > timeBeforeOutro)
            {
                // First fade the curtain
                if (iFadingSteps < nbOfFadingSteps)
                {
                    float newAlpha = curtainRenderer.material.color.a + stepFading;

                    curtainRenderer.material.color = new Color(curtainRenderer.material.color.r, curtainRenderer.material.color.g, curtainRenderer.material.color.b, newAlpha);

                    transitionText.color = new Color(transitionText.color.r, transitionText.color.g, transitionText.color.b, newAlpha);

                    iFadingSteps++;
                }
                else
                {
                    SaveTimeSpent();

                    loadOutroWSun();
                }
            }
        }
        if (sunBroken)
        {
            timeAfterSunBroke++;

            if (timeAfterSunBroke > timeBeforeOutro)
            {
                // First fade the curtain
                if (iFadingSteps < nbOfFadingSteps)
                {    
                    float newAlpha = curtainRenderer.material.color.a + stepFading;

                    curtainRenderer.material.color = new Color(curtainRenderer.material.color.r, curtainRenderer.material.color.g, curtainRenderer.material.color.b, newAlpha);

                    transitionText.color = new Color(transitionText.color.r, transitionText.color.g, transitionText.color.b, newAlpha);

                    iFadingSteps++;
                }
                else
                { 
                    loadOutro();
                }
            }

            foreach (Transform childCharacter in towerAndJohn.transform)
            {
                Renderer currentRenderer = childCharacter.gameObject.GetComponent<Renderer>();

                if (currentRenderer != null)
                {
                    float r = currentRenderer.material.color.r - 0.1f;
                    float g = currentRenderer.material.color.g - 0.1f;
                    float b = currentRenderer.material.color.b - 0.1f;

                    if (r < 0)
                    {
                        r = 0;
                    }
                    if (g < 0)
                    {
                        g = 0;
                    }
                    if (b < 0)
                    {
                        b = 0;
                    }
                    currentRenderer.material.color = new Color(r, g, b, currentRenderer.material.color.a);
                }

                // And 2nd level
                foreach (Transform childChild in childCharacter.transform)
                {
                    Renderer anotherRenderer = childChild.gameObject.GetComponent<Renderer>();

                    if (anotherRenderer != null)
                    {
                        float r = anotherRenderer.material.color.r - 0.1f;
                        float g = anotherRenderer.material.color.g - 0.1f;
                        float b = anotherRenderer.material.color.b - 0.1f;

                        if (r < 0)
                        {
                            r = 0;
                        }
                        if (g < 0)
                        {
                            g = 0;
                        }
                        if (b < 0)
                        {
                            b = 0;
                        }
                        anotherRenderer.material.color = new Color(r, g, b, anotherRenderer.material.color.a);
                    }
                }
            }   
        }
        // First let John jump in this level
        transform.position = new Vector3(xJohn+2, yJohn+2, transform.position.z);

        // Back Zoom
        if (Camera.main.orthographicSize < globalZoom)
        {
            Camera.main.orthographicSize += stepBetweenZoom;
        }

        // Then load the outro screen, including thank you and credits
    }

    private void loadOutro()
    {
        InitializeSteam.unlockAchievement(InitializeSteam.achievementKey.BJS_END);

        UnityEngine.SceneManagement.SceneManager.LoadScene(nameOfBJSOutro);
    }

    private void loadOutroWSun()
    {
        InitializeSteam.unlockAchievement(InitializeSteam.achievementKey.BJS_END_SUN);

        UnityEngine.SceneManagement.SceneManager.LoadScene(nameOfBJSOutroWSun);
    }

    internal void sunIsBroken()
    {
        sunBroken = true;
    }

    bool newBellLoaded = false;
    bool inDestroyPhase = true;

    int restartPhase = -1;

    private void manageNextLevel()
    {
        // Fade
        if (restartPhase == -1)
        {
            if (iFadingSteps < nbOfFadingSteps)
            {
                transform.position = new Vector3(transform.position.x + stepFading * 6, transform.position.y, transform.position.z);

                landscape.transform.position = new Vector3(landscape.transform.position.x + stepFading * 2, landscape.transform.position.y, landscape.transform.position.z);

                sun.transform.position = new Vector3(xSun, ySun, sun.transform.position.z);
                ySun += 0.04f; 

                iFadingSteps++;
            }
            else
            {
                restartPhase = 0;
                iFadingSteps = 0;
            }
            return;
        }
        if (restartPhase == 0)
        {
            if (iFadingSteps < nbOfFadingSteps)
            {
                float newAlpha = backNightRenderers[0].material.color.a - stepFading;

                foreach (Renderer childRenderer in backNightRenderers)
                {
                    childRenderer.material.color = new Color(childRenderer.material.color.r, childRenderer.material.color.g, childRenderer.material.color.b, newAlpha);
                }
                transform.position = new Vector3(transform.position.x + stepFading*4, transform.position.y, transform.position.z);
                iFadingSteps++;
            }
            else
            {
                restartPhase = 1;
                iFadingSteps = 0;
            }
            return;
        }
        if (iFadingSteps < nbOfFadingSteps)
        {
            float newAlpha = curtainRenderer.material.color.a + stepFading;
            curtainRenderer.material.color = new Color(curtainRenderer.material.color.r, curtainRenderer.material.color.g, curtainRenderer.material.color.b, newAlpha);

            transitionText.color = new Color(transitionText.color.r, transitionText.color.g, transitionText.color.b, newAlpha);

            iFadingSteps++;
        }
        else
        {
            transform.position = new Vector3(0, transform.position.y, transform.position.z);
            landscape.transform.position = new Vector3(-0.8111087f, landscape.transform.position.y, landscape.transform.position.z);

            xSun = xSunInit;
            ySun = ySunInit;
            sun.transform.position = new Vector3(xSun, ySun, sun.transform.position.z);

            // load the next level
            loadNextBellAndOrJohn();
        }
    }

    private void loadNextBellAndOrJohn()
    {
        newBellLoaded = true;
        firstBreak = true; // The bell can break again

        if (inDestroyPhase)
        {
            // Save the time
            SaveTimeSpent();

            Debug.Log("Loading next bell/John. First phase: destroying bell and john ring");
            Destroy(towerAndJohn);

            nbOfMonksEarned.SetActive(true);
            MMPGAddNewPlayerUnits.bjsLevelEnded(currentJohn, currentBellRelative, nbOfMonksEarnedTM);

            if (ourListOfLevels.wasLastLevel())
            {
                // Also destroy the walking john, we go to the next one
                Debug.Log("Destroying john walking");
                // Remove the current John
                
                Destroy(currentWalkingJohn);

                isGhost = false;

                // Remove ghost if needed
                for (int iGhost = 0; iGhost < 5; iGhost++)
                {
                    if (megaJohnGhosts[iGhost] != null)
                    {
                        Destroy(megaJohnGhosts[iGhost]);
                    }
                }

                musicSource.Stop();

                currentJohn++;
                currentBellRelative = 0;
            }
            else
            {
                currentBellRelative++;
            }

            inDestroyPhase = false;
        }
        else
        {
            bellWillBreakAt = 4 + ((currentJohn - 1) * 24) + (currentBellRelative) * 4;

            Debug.Log("Bell will break at level " + bellWillBreakAt);

            // Check if we need to load another John   
            bool lastLevel = ourListOfLevels.wasLastLevel();

            // Instantiate the next tower
            GameObject nextLevel = ourListOfLevels.getTowerByNb(currentJohn, currentBellRelative);

            isLastLevel = false;

            if (ourListOfLevels.IsLast)
            {
                isLastLevel = true;
            }

            towerAndJohn = Instantiate(nextLevel);
            
            if (lastLevel)
            {
                // And get the new John
                GameObject newWalkingJohn = ourListOfLevels.getNextWalkingJohn();

                ourSaveFacility.CurrentJohn = currentJohn;
                ourSaveFacility.saveCurrentJohn();
                // To test the destructible door
                //currentJohn=4;
                switch (currentJohn)
                {
                    case 1:
                        InitializeSteam.unlockAchievement(InitializeSteam.achievementKey.BJS_1);
                        break;
                    case 2:
                        InitializeSteam.unlockAchievement(InitializeSteam.achievementKey.BJS_2);
                        break;
                    case 3:
                        InitializeSteam.unlockAchievement(InitializeSteam.achievementKey.BJS_3);
                        break;
                    case 4:
                        InitializeSteam.unlockAchievement(InitializeSteam.achievementKey.BJS_4);
                        break;
                }
                Instantiate(newWalkingJohn);

                if (currentJohn == 4 && !isLastLevel)
                {
                    isGhost = true;

                    // Also instantiate the ghosts :)
                    for (int iGhost = 0; iGhost < 5; iGhost++)
                    {
                        megaJohnGhosts[iGhost] = Instantiate(megaJohnGhost);
                        megaJohnGhosts[iGhost].transform.position = new Vector2(xJohn, yJohn);

                        // Not so correct now...
                        xJohnOld[iGhost] = xJohn;
                        yJohnOld[iGhost] = yJohn;
                    }
                }

                // Then reload all
                loadJohnBellsAndVariables();

                musicSource.Play();
            }
            else
            {
                // Reload only the bells.
                loadBells();
            }

            ourSaveFacility.CurrentBellForLevel = currentBellRelative;
            ourSaveFacility.saveBellNb();

            switchLightsOff();

            johnReturn.SetActive(false);

            johnRing.transform.position = new Vector3(johnRing.transform.position.x, johnRing.transform.position.y, 20);

            if (currentJohn < 4)
            {
                monasterFullDoor.SetActive(true);
                monasterOpenDoor.SetActive(false);
            }
            else
            {
                // Mega John has another way of getting out...
                monasterOpenDoor.SetActive(false);
                monasterFrameDoor.SetActive(false);
                monasterFullDoor.SetActive(true);

                fullDoorExplodable.cleanUp();
            }
            johnRoomLight.transform.localPosition = new Vector3(johnRoomLight.transform.localPosition.x, johnRoomLight.transform.localPosition.y, 4f);

            xJohn = xJohnInitial;
            yJohn = yFloor;

            johnWalk.transform.localPosition = new Vector3(xJohn, yJohn, -1);

            johnWalk.SetActive(true);

            iFadingSteps = 0;

            currentPhase = -2;
            introPhase = 0;

            //Debug.Log("loadNextBellAndOrJohn - Loaded" + johnRing + " - " + currentBell);

            inDestroyPhase = true; // Next time will be to destroy again
        }
    }

    private void switchLightsOff()
    {
        foreach (Transform childCharacter in monasterLights.transform)
        {
            childCharacter.transform.localPosition = new Vector3(childCharacter.transform.localPosition.x, childCharacter.transform.localPosition.y, 3f);
        }
        cityLight.transform.localPosition = new Vector3(cityLight.transform.localPosition.x, cityLight.transform.localPosition.y, 2f);

        foreach (Renderer childRenderer in backNightRenderers)
        {
            childRenderer.material.color = new Color(childRenderer.material.color.r, childRenderer.material.color.g, childRenderer.material.color.b, 1);
        }
    }

    private void gameWon()
    {
        // Show the earth lit

        // And John triumphant
        Debug.Log("You won ;)");
    }

    const int nbOfFadingSteps = 60;
    const float stepFading = 1f / ((float ) nbOfFadingSteps);
    int iFadingSteps = 0;

    private void manageRestart()
    {
        // Fade
        if (restartPhase == -1)
        {
            if (iFadingSteps < nbOfFadingSteps)
            {
                transform.position = new Vector3(transform.position.x + stepFading * 6, transform.position.y, transform.position.z);

                landscape.transform.position = new Vector3(landscape.transform.position.x + stepFading * 2, landscape.transform.position.y, landscape.transform.position.z);
                sun.transform.position = new Vector3(xSun, ySun, sun.transform.position.z);
                ySun += 0.04f;

                iFadingSteps++;
            }
            else
            {
                restartPhase = 0;
                iFadingSteps = 0;
            }
            return;
        }
        if (restartPhase == 0)
        {
            if (iFadingSteps < nbOfFadingSteps)
            {
                float newAlpha = backNightRenderers[0].material.color.a - stepFading;

                foreach (Renderer childRenderer in backNightRenderers)
                {
                    childRenderer.material.color = new Color(childRenderer.material.color.r, childRenderer.material.color.g, childRenderer.material.color.b, newAlpha);
                }
                transform.position = new Vector3(transform.position.x + stepFading * 4, transform.position.y, transform.position.z);

                iFadingSteps++;
            }
            else
            {
                restartPhase = 1;
                iFadingSteps = 0;
            }
            return;
        }
        if (iFadingSteps < nbOfFadingSteps)
        {   
            float newAlpha = curtainRenderer.material.color.a + stepFading;
            curtainRenderer.material.color = new Color(curtainRenderer.material.color.r, curtainRenderer.material.color.g, curtainRenderer.material.color.b, newAlpha);
            transitionText.color = new Color(transitionText.color.r, transitionText.color.g, transitionText.color.b, newAlpha);

            iFadingSteps++;
        }
        else
        {
            transform.position = new Vector3(0, transform.position.y, transform.position.z);

            landscape.transform.position = new Vector3(-0.8111087f, landscape.transform.position.y, landscape.transform.position.z);

            xSun = xSunInit;
            ySun = ySunInit;
            sun.transform.position = new Vector3(xSun, ySun, sun.transform.position.z);

            restartPhase = -1;
            // Set up everything and go to phase -2
            switchLightsOff();

            // Or fix the door
            if (currentJohn >= 4)
            {
                monasterFullDoor.SetActive(true);
                fullDoorExplodable.cleanUp();
            }

            johnWalk.transform.localPosition = new Vector3(xJohn, yJohn, -1);

            johnWalk.SetActive(true);

            iFadingSteps = 0;

            currentPhase = -2;
        }
    }

    // Do not show the transition text when we start the game
    bool notInFirstDay = false;

    private void manageStart()
    {
        // Fade
        if (iFadingSteps < nbOfFadingSteps)
        {
            float newAlpha = curtainRenderer.material.color.a - stepFading;
            curtainRenderer.material.color = new Color(curtainRenderer.material.color.r, curtainRenderer.material.color.g, curtainRenderer.material.color.b, newAlpha);
            if (notInFirstDay)
            {
                transitionText.color = new Color(transitionText.color.r, transitionText.color.g, transitionText.color.b, newAlpha);
            }
            else
            {
                transitionText.color = new Color(transitionText.color.r, transitionText.color.g, transitionText.color.b, 0);
            }
            iFadingSteps++;

            lastCurrentDay = currentDay + "";
        }
        else
        {
            // Set up everything and go to phase -1
            currentPhase = -1;

            // Next time will not be the intro
            notInFirstDay = true;
        }
    }

    float xJohnInitial = -2.76f;

    private const float defaultYFloor = -4.05f;
    private float yFloor = -4.05f;

    float xJohn;
    float yJohn = defaultYFloor;

    float xSpeed = 0;
    float ySpeed = 0;
    private float xDampling = 0.01f;
    private float gravity = 0.01f;

    const float nbOfJohnSteps = 60;

    const float xRope = 1.25f;

    private int introPhase = 0;

    private int direction = 1;

    AudioSource bellSound;
    private AudioSource johnRingSound;

    int shownTriumphant = 0;
    const int timeToShowTri = 30;

    private void doMainLoop()
    {
        bool aTouch = false;
        Vector3 touchPos = new Vector3();

        {
            touchPos = Mouse.current.position.ReadValue();

            // use the input stuff
            //if (Mouse.current.leftButton.isPressed)
            //{
            //    moveBell(touchPos);
            //}

            // Check if the user has clicked
            aTouch = Mouse.current.leftButton.wasReleasedThisFrame;
        }
        

        if (Keyboard.current.spaceKey.isPressed)
        {
            if (currentPhase == 2)
            { 
                aTouch = true;

                coolDown = 5; // Wait for a new update
            }
            else if (coolDown == 0)
            {
                aTouch = true;

                coolDown = 20; // Wait for a new update
            }
        }
        else
        {
            coolDown = 0;
        }
        if (coolDown > 0)
        {
            coolDown--;
        }
        // Actual motion of John
        if (currentPhase !=2 && currentPhase != 5)
        {
            xJohn = xJohn + xSpeed;
            yJohn = yJohn + ySpeed;
        }

        if (xSpeed > 0 && !isLastLevel)
        { 
            xSpeed -= xDampling;
        }
        else if (xSpeed < 0)
        {
            xSpeed += xDampling;
        }
        if (yJohn > yFloor && !isLastLevel)
        {
            ySpeed -= gravity;
        }

        if (direction*xSpeed < 0.0f)
        {
            xSpeed = 0;
        }

        if (yJohn < yFloor)
        {
            yJohn = yFloor;
            ySpeed = 0;

            if (currentPhase == 3)
            {
                if (shownTriumphant == 0)
                {
                    johnReturn.SetActive(false);
                    triumphantJohn.SetActive(true);
                    triumphantJohn.transform.position = new Vector3(xJohn, yJohn, -1);
                    shownTriumphant = 1;

                    xSpeed *= 0.5f;
                }
            }
        }

        if (currentPhase == -1)
        {
            if (aTouch)
            {
                if (introPhase == 0)
                {
                    johnRoomLight.transform.localPosition = new Vector3(johnRoomLight.transform.localPosition.x, johnRoomLight.transform.localPosition.y, 3f);

                    introPhase = 1;

                    return;
                }
                else
                {
                    if (currentJohn < 4)
                    {
                        monasterFullDoor.SetActive(false);
                        monasterOpenDoor.SetActive(true);
                    }
                    else
                    {
                        fullDoorExplodable.explodeBJS();
                        //monasterFullDoor.SetActive(false);
                    }

                    currentPhase = 0;

                    return;
                }
            }
        }

        if (currentPhase == 0)
        {
            //Debug.Log("Here with speed of " + xSpeed + ", " + ySpeed);
            johnWalk.transform.localPosition= new Vector3(xJohn, yJohn, -1);

            if (xJohn > xJohnStartJump)
            {
                currentPhase = 1;

                ySpeed = speedJumpY * 2.5f;
            }
            // Walking toward the bell
            else if (aTouch)
            {
                walkAnimator.SetTrigger("Start");
                // Move John   
                //if (xSpeed < xJohnMaxSpeed)
                {
                    xSpeed = xSpeed + xSteps;

                    if (currentJohn >= 4)
                    {
                        xSpeed *= 1.3f;
                    }
                    if (isLastLevel)
                    {
                        xSpeed *= 1.3f;
                    }

                    if (yJohn <= yFloor)
                    { 
                        ySpeed = speedJumpY;
                        //if (currentJohn == 4)
                        //{
                        //    ySpeed *= 0.5f;
                        //}
                    }
                }   
            }
            else
            {
                walkAnimator.SetTrigger("Stop");
            }
            return;
        }
        if (currentPhase == 1)
        {
            johnWalk.transform.position = new Vector3(xJohn, yJohn, -1);

            walkAnimator.SetTrigger("Stop");
            //foreach (AnimationState state in walkAnimation)
            //{
            //    state.speed = xSpeed;
            //}
            // Jumping
            xSpeed = xSpeed + xSteps/2;

            if (xJohn > xRope)
            {
                currentPhase = 2;

                // Now hide walking John
                johnWalk.SetActive(false);

                // And show "roping" John
                johnRing.transform.position = new Vector3(johnRing.transform.position.x, johnRing.transform.position.y, -1);

                walkAnimator.StopPlayback();

                // Super and mega John "jump" on the rope
                if (currentJohn > 2)
                {
                    int levelMult = (currentLevel - 6);
                    if (levelMult <= 0)
                    {
                        levelMult = 1;
                    }
                    johnRingRB.AddForce(levelMult * horForce);
                }
                //foreach (AnimationState state in walkAnimation)
                //{
                //    state.speed = 0f;
                //}
            }
            
            return;
        }
        if (currentPhase == 2)
        {
            if (isLastLevel)
            {
                jumpInSpace();

                return;
            }
            // Ringing the bell
            if (aTouch)
            {
                startOrContinueClimbing();

                //moveBell(touchPos);
            }
            else
            {
                updateClimbing();
            }
            // Check if the bell rings!
            checkBellState();

            // Check XP gain

            return;
        }
        if (currentPhase == 3)
        {
            direction = -1;

            if (shownTriumphant > 0)
            {
                if ( shownTriumphant < timeToShowTri )
                { 
                    triumphantJohn.transform.position = new Vector3(xJohn, yJohn, -1);

                    if (yJohn <= yFloor)
                    {
                        ySpeed = speedJumpY;
                    }
                    shownTriumphant++;
                }
                else
                {
                    johnReturn.SetActive(true);
                    triumphantJohn.SetActive(false);

                    johnReturn.transform.position = new Vector3(xJohn, yJohn, -1);

                    shownTriumphant = -1;
                }
                return;
            }
            // Returning
            johnReturn.transform.position = new Vector3(xJohn, yJohn, -1);

            // Also trigger triumphantJohn when he first touch the ground
            // Walking toward the bell
            if (aTouch)
            {
                returnAnimator.SetTrigger("Start");
                // Move John   
                //if (xSpeed < xJohnMaxSpeed)
                {
                    xSpeed = xSpeed - xSteps;

                    if (currentJohn >= 4)
                    {
                        xSpeed *= 1.3f;
                    }

                    if (yJohn <= yFloor)
                    {     
                        ySpeed = speedJumpY;

                        //if (currentJohn == 4)
                        //{
                        //    ySpeed *= 0.5f;
                        //}
                    }
                }
                if (xJohn < xJohnInitial)
                {
                    currentPhase = 4;

                    // Close the door, switch light off
                    introPhase = 0;

                    johnReturn.SetActive(false);

                    shownTriumphant = 0;
                }
            }
            else
            {
                returnAnimator.SetTrigger("Stop");
            }

            return;
        }
        if (currentPhase == 4)
        {
            if (aTouch)
            {
                if (introPhase == 1)
                {
                    johnRoomLight.transform.localPosition = new Vector3(johnRoomLight.transform.localPosition.x, johnRoomLight.transform.localPosition.y, 4f);

                    introPhase = 2;

                    return;
                }
                else if (introPhase == 0)
                {
                    if (currentJohn < 4)
                    {
                        monasterFullDoor.SetActive(true);
                        monasterOpenDoor.SetActive(false);
                    }
                   
                    introPhase = 1;

                    return;
                }
            }
            if (introPhase == 2)
            {
                // Another day
                currentDay++;

                ourSaveFacility.CurrentDay = currentDay;
                ourSaveFacility.saveCurrentDay();

                // Fade to black, start again
                restartingGame();

                // When finished if we start again here
                introPhase = 0;

                return;
            }
        }
    }

    private void jumpInSpace()
    {
        // Save the time one last time
        SaveTimeSpent();

        // Add a huge diagonal force to John
        push = 0;

        // Hide the normal sun
        sun.SetActive(false);

        johnRingRB.AddForce(lastForce);

        lastJohnScream.Play();

        currentPhase = 6;

        iFadingSteps = 0;

        curtainRenderer.material.color = new Color(curtainRenderer.material.color.r, curtainRenderer.material.color.g, curtainRenderer.material.color.b, 0);

        transitionText.gameObject.transform.localScale = new Vector3(0.1875f, 0.375f, 1);
        curtain.SetActive(true);

        if (currentDay < 500)
        { 
            currentDay = 1000;
        }
        else if (currentDay < 10000)
        {
            currentDay = 10000;
        }
        else
        {
            currentDay = 100000;
        }
        nbOfMonksEarned.SetActive(true);

        MMPGAddNewPlayerUnits.bjsGameEnded(nbOfMonksEarnedTM);
    }

    private void restartingGame()
    {
        currentPhase = -3;

        direction = 1;

        introPhase = 0;

        xJohn = xJohnInitial;
        yJohn = yFloor;

        float newAlpha = 0;
        curtainRenderer.material.color = new Color(curtainRenderer.material.color.r, curtainRenderer.material.color.g, curtainRenderer.material.color.b, newAlpha);
        transitionText.color = new Color(transitionText.color.r, transitionText.color.g, transitionText.color.b, newAlpha);

        curtain.SetActive(true);

        iFadingSteps = 0;
    }

    float lastBellSpeed = 0;
    private int nbRing = 0;

    private void checkBellState()
    {
        if (lastBellSpeed == 0)
        {
            lastBellSpeed = currentBellRB.angularVelocity;
        }
        else
        {
            if (Math.Sign(lastBellSpeed) != Math.Sign(currentBellRB.angularVelocity))
            {
                // Bell is ringing ! :)
                lightUpMonaster(Math.Abs(currentBellRB.angularVelocity));
            }

            lastBellSpeed = currentBellRB.angularVelocity;
        }
    }

    bool isClimbing = false;
    bool inPhaseOne = true;

    bool requestNewClimb = false;

    int timeInClimbing = 0;
    const int maxTimeClimbing = 5;
    private readonly string nameOfBJSOutro = "BJSOutro";
    private readonly string nameOfBJSOutroWSun = "BJSOutroWSun";

    private void startOrContinueClimbing()
    {
        if (isClimbing)
        {
            // Reset the time, he continue "climbing"
            requestNewClimb = true;

            return;
        }

        isClimbing = true;
        timeInClimbing = 0;

        ringAnimator.SetTrigger("Start");

        if (!johnRingSound.isPlaying)
        {
            johnRingSound.Play();
        }

        // If John is strong enough, he also balance himself
        if (currentJohn > 1)
        { 
            johnRingRB.AddForce(horForce);
        }

        moveJohnArmUpDown();
    }

    private void updateClimbing()
    {
        if (isClimbing)
        {
            timeInClimbing++;
            if (timeInClimbing > maxTimeClimbing && !inPhaseOne)
            {
                isClimbing = false;

                ringAnimator.SetTrigger("Stop");

                stopJohnArm();

                if (requestNewClimb)
                {
                    requestNewClimb = false;

                    startOrContinueClimbing();
                }
            }
            if (timeInClimbing > maxTimeClimbing && inPhaseOne)
            {
                moveJohnArmUpDown();

                inPhaseOne = false;
            }
        }
    }
    bool lastWasUp = false;
    public float maxAmountBell = 10;
    private Vector2 horForce = new Vector2(20,0);
    private Vector2 lastForce = new Vector2(4000, 8000);
    private int coolDown = 0;
    private int bellWillBreakAt = 4;
    private bool isLastLevel = false;

    int iTurn = 0;
    private bool shouldBreakNow = false;

    private void moveJohnArmUpDown()
    {
        Debug.Log("Moving arm up or down");

        hingeArm.useMotor = true;

        JointMotor2D motor = hingeArm.motor;

        float levelMultiplier = 1 + (((float )levelJohn) - 1) * 0.5f;

        if (lastWasUp)
        {
            Debug.Log("Moving down");
            if (shouldBreakNow && (iTurn++ % 10 == 0))
            {
                motor.motorSpeed = -5f * johnStrength * levelMultiplier;
                motor.maxMotorTorque = 400 * levelMultiplier;
                
                hingeArm.motor = motor;
                lastWasUp = false;
                shouldBreakNow = false;
            }
            else
            {
                motor.motorSpeed = -0.5f * johnStrength * levelMultiplier;
                motor.maxMotorTorque = 40 * levelMultiplier;
                hingeArm.motor = motor;
                lastWasUp = false;
            }

        }
        else
        {
            Debug.Log("Moving arm up");
            motor.motorSpeed = johnStrength * levelMultiplier;
            motor.maxMotorTorque = 20 * levelMultiplier * currentJohn;
            hingeArm.motor = motor;
            lastWasUp = true;
        }
    }


    private void stopJohnArm()
    {
        hingeArm.useMotor = false;
    }

    private void lightUpMonaster(float byAmount)
    {
        // Depending on the amount, light up one or several light.
        Debug.Log("Amount is " + byAmount);
        int nbLightOn = 0;
        int nbLightToSwitchOn = ((int)(byAmount));
        bool allAwaken = true;
        float initialAmount = byAmount;

        if (nbLightToSwitchOn > 0)
        {
            // Ring the bell (sound and count).
            bellSound.Play();

            nbRing++;

            if (byAmount > maxAmountBell)
            {
                byAmount = maxAmountBell;
            }
            // Reduce how much the amount helps us
            nbLightToSwitchOn = ((int)(0.4*byAmount));

            foreach (Transform childCharacter in monasterLights.transform)
            {
                if (childCharacter.transform.localPosition.z == 3)
                {
                    allAwaken = false;

                    childCharacter.transform.localPosition = new Vector3(childCharacter.transform.localPosition.x, childCharacter.transform.localPosition.y, 2.4f);

                    nbLightOn++;

                    if (nbLightOn >= nbLightToSwitchOn)
                    {
                        break;
                    }
                }
            }

            // And give some XP
            if (currentPhase == 2)
            {
                xpJohn += (currentBellRelative +  1)* currentJohn * byAmount * 5;
                if (xpJohn > xpNeeded)
                {
                    levelJohn++;

                    Debug.Log("John is now level " + levelJohn);

                    if (levelJohn >= bellWillBreakAt)
                    {
                        shouldBreakNow = true;
                    }

                    xpNeeded += (1 + ((float )levelJohn) / 20) * xpToAdd;

                    Debug.Log("Next level at " + xpNeeded);

                    ourSaveFacility.LevelJohn = levelJohn;
                    ourSaveFacility.XpJohn = xpJohn;
                    ourSaveFacility.XpNeeded = xpNeeded;

                    ourSaveFacility.saveXpsAndLevel();
                }
                else
                {
                    ourSaveFacility.XpJohn = xpJohn;

                    ourSaveFacility.saveXps();
                }
            }
        }
        else
        {
            allAwaken = false;
        }
        if (initialAmount* (currentBellRelative + 1) * currentJohn > 200 && currentJohn == 4)
        {
            // Also switch on the city
            cityLight.transform.localPosition = new Vector3(cityLight.transform.localPosition.x, cityLight.transform.localPosition.y, 0.5f);
        }

        // When all are on, the level is over
        if (allAwaken)
        {
            // Start again
            currentPhase = 3;

            xJohn = johnRing.transform.position.x;
            yJohn = johnRing.transform.position.y;

            // Hide ringing John
            johnRing.transform.position = new Vector3(johnRing.transform.position.x, johnRing.transform.position.y, 20);

            johnReturn.transform.position = new Vector3(johnRing.transform.position.x, johnRing.transform.position.y, johnReturn.transform.position.z);
            // And show returning John
            johnReturn.SetActive(true);
        }
    }

    private void playMusic()
    {
        if (musicSource != null && title == null)
        {
            if (!musicSource.isPlaying)
            {
                musicSource.Play();
            }
        }
    }

    private void showIntroductionBox(bool v)
    {
        ;
    }

    // Prevent next bell to call us as well.
    bool firstBreak = true;

    public void bellBroke()
    {
        if (firstBreak)
        {
            firstBreak = false;

            Debug.Log("The bell broke!");

            if (!isLastLevel)
            {
                // The bell broke, we go to the next level!
                currentPhase = 5;
                restartPhase = -1;

                float newAlpha = 0;
                curtainRenderer.material.color = new Color(curtainRenderer.material.color.r, curtainRenderer.material.color.g, curtainRenderer.material.color.b, newAlpha);
                transitionText.color = new Color(transitionText.color.r, transitionText.color.g, transitionText.color.b, newAlpha);

                curtain.SetActive(true);

                iFadingSteps = 0;

                xJohn = johnRing.transform.position.x;
                yJohn = johnRing.transform.position.y;

                currentDay++;

                ourSaveFacility.CurrentDay = currentDay;
                ourSaveFacility.saveCurrentDay();

                LocalSave.SaveCopy();

                triumphantJohn.SetActive(false);

                // Shouldn't matter, will be destroyed
                //// Hide ringing John
                //johnRing.transform.position = new Vector3(johnRing.transform.position.x, johnRing.transform.position.y, 20);

                //johnReturn.transform.position = new Vector3(johnRing.transform.position.x, johnRing.transform.position.y, johnReturn.transform.position.z);
                //// And show returning John
                //johnReturn.SetActive(true);
            }
        }
    }

    // Getters
    public int getCurrentDay()
    {
        return currentDay;
    }

    public float getNeededXP()
    {
        return xpNeeded;
    }

    public float getCurrentXP()
    {
        return xpJohn;
    }

    public int getCurrentLevel()
    {
        return levelJohn;
    }

    public void increaseLevel()
    {
        xpJohn += xpNeeded + 50;

        levelJohn++;

        Debug.Log("John is now level " + levelJohn);
        xpNeeded = xpNeeded + 0.2f * xpNeeded;

        Debug.Log("Next level at " + xpNeeded);

        ourSaveFacility.LevelJohn = levelJohn;
        ourSaveFacility.XpJohn = xpJohn;
        ourSaveFacility.XpNeeded = xpNeeded;

        ourSaveFacility.saveXpsAndLevel();
    }

    public void decreaseLevel()
    {
        xpJohn -= xpNeeded;

        levelJohn--;

        Debug.Log("John is now level " + levelJohn);
        xpNeeded -= 0.2f*xpNeeded; // Not quite exact ;)

        Debug.Log("Next level at " + xpNeeded);

        ourSaveFacility.LevelJohn = levelJohn;
        ourSaveFacility.XpJohn = xpJohn;
        ourSaveFacility.XpNeeded = xpNeeded;

        ourSaveFacility.saveXpsAndLevel();
    }

    internal string getLastCurrentDay()
    {
        return lastCurrentDay;
    }

    private void LoadTimeSpent()
    {
        bool haskey = LocalSave.HasFloatKey("BJS_allTimeSpent");

        if (haskey)
        {
            allTimeSpent = LocalSave.GetFloat("BJS_allTimeSpent");
        }

        haskey = LocalSave.HasFloatKey("BJS_currentTimeSpent");

        if (haskey)
        {
            lastTimeSpent = LocalSave.GetFloat("BJS_currentTimeSpent");
        }
    }

    private void SaveTimeSpent()
    {
        float savedTimeSpent = currentTime - initialTime + lastTimeSpent;
        float savedTimeSpentTotal = currentTime - initialTime + allTimeSpent;

        LocalSave.SetFloat("BJS_allTimeSpent", savedTimeSpentTotal);

        LocalSave.SetFloat("BJS_currentTimeSpent", savedTimeSpent);
        LocalSave.Save();
    }
}
