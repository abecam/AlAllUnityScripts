using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class DogScript : MonoBehaviour
{
    public const string DogBoughtBaseKey = "dogBought";
    public const string DogUpdatedBaseKey = "dogUpdated";
    public const string DogSuperUpdatedBaseKey = "dogSuperUpdated";
    public const string DogLevelUpdatedBaseKey = "dogLevelUpdated";

    private CreateBoard mainScript;
    // Manage the different dogs.
    // Most of the time wander around and from time to time go fetch/find a character
    internal bool bought = false;
    private bool notModeE = true;
    private bool isUpdated = false;
    private bool superUpdated = false;
    internal int levelUpdate = 0; // The superUpdate has several steps.
    public const int maxLevelUpdate = 10;

    public bool doFind = false;
    private float timeBeforeNextSearch = 120; // 2 min initially

    dogState currentState = dogState.returning; // Will start to come to the player
    public string nameDog = "Bob";

    float currentTime;
    float lastTime;

    public GameObject myButton; // Activate the button to deactivate this dog ;)
    public GameObject myButtonsPanel;

    public bool Bought { get => bought; }
    public bool IsUpdated { get => isUpdated; }
    public bool SuperUpdated { get => superUpdated; }

    private enum dogState 
    {
        walkingAround,
        searching,
        found,
        returning,
        away // If bought but switched off
    };

    // Start is called before the first frame update
    void Start()
    {
        string keyUpdate = DogUpdatedBaseKey + nameDog;
        string keySuperUpdate = DogSuperUpdatedBaseKey + nameDog;
        string keyLevelUpdate = DogLevelUpdatedBaseKey + nameDog;
        string keyBought = DogBoughtBaseKey + nameDog;

        GameObject mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        mainScript = mainCamera.GetComponent<CreateBoard>();

        bool hasKey = LocalSave.HasBoolKey(keyBought);
        if (hasKey)
        {
            bought = LocalSave.GetBool(keyBought);
        }
        if (bought)
        {
            // Get it closer
            transform.position = new Vector3(15, 0, 0);

            myButton.SetActive(true);
            myButtonsPanel.SetActive(true); // Might already be active but ok.
        }
        hasKey = LocalSave.HasBoolKey(keyUpdate);
        if (hasKey)
        {
            isUpdated = LocalSave.GetBool(keyUpdate);
        }

        hasKey = LocalSave.HasBoolKey(keySuperUpdate);
        if (hasKey)
        {
            superUpdated = LocalSave.GetBool(keySuperUpdate);
        }

        hasKey = LocalSave.HasIntKey(keyLevelUpdate);
        if (hasKey)
        {
            levelUpdate = LocalSave.GetInt(keyLevelUpdate);
        }
        Debug.Log("Dog level is "+levelUpdate);

        // Randomize a bit
        timeBeforeNextSearch += (20*Random.value - 10);
        maxSpeed = 0.1f+ (0.1f * Random.value - 0.05f);
        standardSpeed = maxSpeed/2;

        useAvailableUpdates();

        currentTime = Time.time; // Last "frame" time
        lastTime = currentTime;

        // Check how much level we show
        int currentMode = 0;

        bool haskey = LocalSave.HasIntKey("currentMode");
        if (haskey)
        {
            currentMode = LocalSave.GetInt("currentMode");
        }

        if (currentMode == 4)
        {
            notModeE = false;
        }

        // And check if enabled or not
        checkIfOff();
    }

    void useAvailableUpdates()
    {
        if (superUpdated)
        {
            timeBeforeNextSearch = (2+ (1 * Random.value))/(levelUpdate + 1); // ~2 seconds!
        }
        else if (isUpdated)
        {
            timeBeforeNextSearch = 60+(20 * Random.value - 10); // ~1 min
        }
    }

    Vector3 lastPos = new Vector3(0, 0, 0);
    Vector2 dogCurrPos;
    Vector2 dogSpeed = new Vector2(0, 0);
    Vector2 dogAcc = new Vector2(0, 0);

    Vector2 targetPos = new Vector2(0, 0);
    Vector2 farAway = new Vector2(-100, 0);


    float forTime = 0;
    // Update is called once per frame
    void Update()
    {
        if (notModeE && bought && !mainScript.gameIsInPause)
        {
            currentTime += Time.deltaTime;

            if ((currentState == dogState.returning || currentState == dogState.walkingAround) && (currentTime - lastTime > timeBeforeNextSearch))
            {
                // Ask for a position
                targetPos = mainScript.getOneCharNotFound();
                currentState = dogState.searching;

                lastTime = currentTime;
            }
            dogCurrPos = transform.localPosition;

            Vector3 touchPos = lastPos;

            touchPos = Mouse.current.position.ReadValue();

            Vector3 wp = Camera.main.ScreenToWorldPoint(touchPos);
            Vector2 touchPos2 = new Vector2(wp.x, wp.y);

            switch (currentState)
            {
                case dogState.returning:
                    // Try to go where the mouse cursor is
                    if (seekTarget(touchPos2))
                    {
                        currentState = dogState.walkingAround;
                    }
                    break;
                case dogState.searching:
                    // Go to a predefined character. If found by the player, go to another one
                    if (seekTarget(targetPos))
                    {
                        currentState = dogState.found;

                        forTime = 0;
                    }
                    forTime += Time.deltaTime;

                    // After 30 seconds, it's probably stuck, go back to walking
                    if (forTime > 30)
                    {
                        currentState = dogState.walkingAround;

                        forTime = 0;
                    }
                    break;
                case dogState.found:
                    forTime += Time.deltaTime;

                    // Show that it found the piece. If doFind is true, he will then go back by himself
                    if (!doFind)
                    {
                        if (turnAroundTarget(targetPos, forTime, false))
                        {
                            currentState = dogState.returning;
                        }
                    }
                    else
                    {
                        //Debug.Log("Here, seeking");
                        if (turnAroundTarget(targetPos, forTime, true))
                        {
                            currentState = dogState.returning;

                            mainScript.checkBoard(targetPos, targetPos, false);
                        }
                    }
                    break;
                case dogState.away:
                    // Go away until switch on again
                    seekTarget(farAway);
                    break;
                default:
                    // Walking around
                    goAround(touchPos2);
                    break;
            }

            dogCurrPos.x += dogSpeed.x;
            dogCurrPos.y += dogSpeed.y;

            transform.localPosition = dogCurrPos;
        }
    }

    
    //Vector3 lastPos = new Vector3(0, 0, 0);
    //Vector2 dogCurrPos;
    //Vector2 dogSpeed = new Vector2(0, 0);
    //Vector2 dogAcc = new Vector2(0, 0);
    Vector2 dogTmpSpeed = new Vector2(0, 0);
    Vector2 dogWantedSpeed = new Vector2(0, 0);
    float tmpSpeedN;
    float currentSpeed = 0;
    float maxSpeed = 0.1f;
    float standardSpeed = 0.05f;
    float stopTargetingRadiusSq = 1;
    float orientation = 0;

    private void goAround(Vector2 targetPos)
    {
        float distToTarget = distSq(dogCurrPos.x, dogCurrPos.y, targetPos.x, targetPos.y);
        // If too far, go back to the "owner"
        if (distToTarget > stopTargetingRadiusSq * 20)
        {
            currentState = dogState.returning;
        }

        this.dogWantedSpeed.x = maxSpeed * Random.value - standardSpeed;
        this.dogWantedSpeed.y = maxSpeed * Random.value - standardSpeed;

        //checkAndNormaliseSpeed();

        this.accX(dogWantedSpeed.x);
        this.accY(dogWantedSpeed.y);

        checkAndNormaliseSpeed();
    }

    private bool turnAroundTarget(Vector2 targetPos, float forTime, bool toGetIt)
    {
        this.dogTmpSpeed.x = (targetPos.x - dogCurrPos.x);
        this.dogTmpSpeed.y = (targetPos.y - dogCurrPos.y);
        this.tmpSpeedN = Mathf.Sqrt(dogTmpSpeed.x * dogTmpSpeed.x + dogTmpSpeed.y * dogTmpSpeed.y);
        this.dogWantedSpeed.x = this.dogTmpSpeed.x / tmpSpeedN;
        this.dogWantedSpeed.y = this.dogTmpSpeed.y / tmpSpeedN;
        this.dogWantedSpeed.x *= this.standardSpeed / 8;
        this.dogWantedSpeed.y *= this.standardSpeed / 8;

        float speedAcc = 0.02f;
        float timeAround = 20;

        if (toGetIt)
        {
            speedAcc = 0.001f;
            timeAround = 5 / (levelUpdate + 1);
        }
        float distToTarget = distSq(dogCurrPos.x, dogCurrPos.y, targetPos.x, targetPos.y);
        // Try to accelerate
        if ((distToTarget < stopTargetingRadiusSq * 2) && (this.currentSpeed > this.standardSpeed))
            this.currentSpeed -= speedAcc;
        else if (this.currentSpeed < this.maxSpeed)
            this.currentSpeed += speedAcc;

        // After timeAround seconds, back
        if (forTime > timeAround)
        {
            return true;
        }

        //checkAndNormaliseSpeed();

        this.accX(dogWantedSpeed.x);
        this.accY(dogWantedSpeed.y);

        checkAndNormaliseSpeed();

        return false;
    }

    private bool seekTarget(Vector2 targetPos)
    {
        this.dogTmpSpeed.x = (targetPos.x - dogCurrPos.x);
        this.dogTmpSpeed.y = (targetPos.y - dogCurrPos.y);
        this.tmpSpeedN = Mathf.Sqrt(dogTmpSpeed.x  * dogTmpSpeed.x  + dogTmpSpeed.y * dogTmpSpeed.y);
        this.dogWantedSpeed.x = this.dogTmpSpeed.x  / tmpSpeedN;
        this.dogWantedSpeed.y = this.dogTmpSpeed.y / tmpSpeedN;
        this.dogWantedSpeed.x *= this.standardSpeed / 8;
        this.dogWantedSpeed.y *= this.standardSpeed / 8;

        float distToTarget = distSq(dogCurrPos.x, dogCurrPos.y, targetPos.x, targetPos.y);
        // Try to accelerate
        if ((distToTarget < stopTargetingRadiusSq * 2) && (this.currentSpeed > this.standardSpeed))
            this.currentSpeed -= 0.025f;
        else if (this.currentSpeed < this.maxSpeed)
            this.currentSpeed += 0.025f;

        if (distToTarget < stopTargetingRadiusSq)
        {
            return true;
        }
        //checkAndNormaliseSpeed();

        this.accX(dogWantedSpeed.x);
        this.accY(dogWantedSpeed.y);

        checkAndNormaliseSpeed();

        return false;
    }

    public void accX(float xAcc)
    {
        dogSpeed.x += xAcc;
        //checkAndNormaliseSpeed();
    }

    public void accY(float yAcc)
    {
        dogSpeed.y += yAcc;
        //checkAndNormaliseSpeed();
    }

    public void checkAndNormaliseSpeed()
    {
        float actualSpeed = Mathf.Sqrt(dogSpeed.x * dogSpeed.x + dogSpeed.y * dogSpeed.y);


        if (dogSpeed.x != 0)
        {
            this.orientation = Mathf.Acos(dogSpeed.x / actualSpeed);
        }
        else
        {
            this.orientation = Mathf.PI / 2;
        }
        if (dogSpeed.y < 0)
        {
            this.orientation = -this.orientation + 2 * Mathf.PI;
        }

        if (actualSpeed > this.maxSpeed)
        {
            this.dogSpeed.x = maxSpeed * (this.dogSpeed.x / actualSpeed);
            this.dogSpeed.y = maxSpeed * (this.dogSpeed.y / actualSpeed);

            //this.currentSpeed = this.maxSpeed;	
        }
        //System.out.println("Current speed "+currentSpeed);
    }

    public static float distSq(float x, float y, float x1, float y1)
    {
        return (Mathf.Pow(x - x1, 2) + Mathf.Pow(y - y1, 2));
    }

    internal bool updateDog()
    {
        if (!isUpdated)
        {
            isUpdated = true;

            string keyUpdate = DogUpdatedBaseKey + nameDog;

            LocalSave.SetBool(keyUpdate, true);

            LocalSave.Save();

            useAvailableUpdates();

            return true;
        }
        return false;
    }

    internal bool superUpdate()
    {
        Debug.Log("Super update : " + superUpdated + " - " + levelUpdate + " < " + maxLevelUpdate);
        if (!superUpdated || (levelUpdate < maxLevelUpdate))
        {
            superUpdated = true;

            string keySuperUpdate = DogSuperUpdatedBaseKey + nameDog;
            string keyLevelUpdate = DogLevelUpdatedBaseKey + nameDog;

            LocalSave.SetBool(keySuperUpdate, true);

            levelUpdate++;

            LocalSave.SetInt(keyLevelUpdate, levelUpdate);

            LocalSave.Save();

            useAvailableUpdates();

            return true;
        }
        return false;
    }

    internal bool buy()
    {
        if (!bought)
        {
            bought = true;

            string keyBought = DogBoughtBaseKey + nameDog;

            LocalSave.SetBool(keyBought, true);

            LocalSave.Save();

            myButton.SetActive(true);

            return true;
        }
        return false;
    }

    public void switchDogOff(bool isOff)
    {
        if (isOff)
        {
            currentState = dogState.away;
        }
        else
        {
            currentState = dogState.returning;
        }
        LocalSave.SetBool("DogOff" + nameDog, isOff);
        LocalSave.Save();
    }

    public void checkIfOff()
    {
        string keyOff = "DogOff" + nameDog;

        if (LocalSave.HasBoolKey(keyOff))
        {
            bool isOff = LocalSave.GetBool(keyOff);

            if (isOff)
            {
                currentState = dogState.away;
            }
            else
            {
                currentState = dogState.returning;
            }
        }
    }
}
