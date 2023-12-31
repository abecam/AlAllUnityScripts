using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class DogScript : MonoBehaviour
{
    //private const string DogBoughtBaseKey = "dogBought";
    //private const string DogUpdatedBaseKey = "dogUpdated";
    //private const string DogSuperUpdatedBaseKey = "dogSuperUpdated";
    //private const string DogLevelUpdatedBaseKey = "dogLevelUpdated";

    private CreateBoard mainScript;
    // Manage the different dogs.
    // Most of the time wander around and from time to time go fetch/find a character
    private bool bought = false;
    private bool isUpdated = false;
    private bool superUpdated = false;
    internal int levelUpdate = 0; // The superUpdate has several steps.
    public const int maxLevelUpdate = 100;

    public int dogNumber = 0;

    public String dogDescription = "The first dog";

    public bool doFind = false;
    private float timeBeforeNextSearch = 120; // 2 min initially

    int level = 0;

    dogState currentState = dogState.returning; // Will start to come to the player
    public string nameDog = "Bob";

    float currentTime;
    float lastTime;

    //public bool Bought { get => bought; }
    //public bool IsUpdated { get => isUpdated; }
    //public bool SuperUpdated { get => superUpdated; }

    private enum dogState 
    {
        walkingAround,
        searching,
        found,
        returning
    };

    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(5, 0, 0);

        //string keyUpdate = DogUpdatedBaseKey + nameDog;
        //string keySuperUpdate = DogSuperUpdatedBaseKey + nameDog;
        //string keyLevelUpdate = DogLevelUpdatedBaseKey + nameDog;
        //string keyBought = DogBoughtBaseKey + nameDog;

        GameObject mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        mainScript = mainCamera.GetComponent<CreateBoard>();

        //bool hasKey = LocalSave.HasBoolKey(keyBought);
        //if (hasKey)
        //{
        //    bought = LocalSave.GetBool(keyBought);
        //}
        //if (bought)
        //{
        //    // Get it closer
        //    transform.position = new Vector3(5, 0, 0);
        //}
        //hasKey = LocalSave.HasBoolKey(keyUpdate);
        //if (hasKey)
        //{
        //    isUpdated = LocalSave.GetBool(keyUpdate);
        //}

        //hasKey = LocalSave.HasBoolKey(keySuperUpdate);
        //if (hasKey)
        //{
        //    superUpdated = LocalSave.GetBool(keySuperUpdate);
        //}

        //hasKey = LocalSave.HasIntKey(keyLevelUpdate);
        //if (hasKey)
        //{
        //    levelUpdate = LocalSave.GetInt(keyLevelUpdate);
        //}

        // Randomize a bit
        timeBeforeNextSearch += (20*Random.value - 10);
        maxSpeed = 0.1f+ (0.1f * Random.value - 0.05f);
        standardSpeed = maxSpeed/2;

        useAvailableUpdates();

        currentTime = Time.time; // Last "frame" time
        lastTime = currentTime;
    }

    float multForAllDogs = 1;
    float multForThisDog = 1;

    float globalCoinMultiplier = 1;
    double finalMultiplier = 0;

    double globalSCoinMultiplier = 0;
    double finalSMultiplier = 0;

    GetAndAnimateCoin theCoinManager;

    void useAvailableUpdates()
    {
        string key = dogNumber + "canFindCoin";

        bool hasKey = LocalSave.HasBoolKey(key);
        if (hasKey)
        {
            doFind = LocalSave.GetBool(key);
        }

        hasKey = LocalSave.HasFloatKey("AllDogMulti");
        if (hasKey)
        {
            multForAllDogs = LocalSave.GetFloat("AllDogMulti");
        }


        string keyMulti = dogNumber + "DogMulti";
        hasKey = LocalSave.HasFloatKey(keyMulti);
        if (hasKey)
        {
            multForThisDog = LocalSave.GetFloat(keyMulti);
        }

        hasKey = LocalSave.HasFloatKey("CoinMultiplier");
        if (hasKey)
        {
            globalCoinMultiplier = LocalSave.GetFloat("CoinMultiplier");
        }

        finalMultiplier = multForAllDogs * multForThisDog * globalCoinMultiplier;
        //Debug.Log("Dog Coins :" + finalMultiplier);

        hasKey = LocalSave.HasDoubleKey("SCoinsMult");
        if (hasKey)
        {
            globalSCoinMultiplier = LocalSave.GetDouble("SCoinsMult");
        }

        if (globalSCoinMultiplier > 0)
        {
            finalSMultiplier = multForAllDogs * multForThisDog * globalSCoinMultiplier;
        }
        //Debug.Log("Dog SCoins :" + finalMultiplier);

        theCoinManager = Camera.main.gameObject.GetComponent<GetAndAnimateCoin>();

        //if (superUpdated)
        //{
        //    timeBeforeNextSearch = (2+ (1 * Random.value))/(levelUpdate + 1); // ~2 seconds!
        //}
        //else if (isUpdated)
        //{
        //    timeBeforeNextSearch = 60+(20 * Random.value - 10); // ~1 min
        //}
        setSpeed();
    }

    Vector3 lastPos = new Vector3(0, 0, 0);
    Vector2 dogCurrPos;
    Vector2 dogSpeed = new Vector2(0, 0);
    Vector2 dogAcc = new Vector2(0, 0);

    Vector2 targetPos = new Vector2(0, 0);

    float forTime = 0;
    // Update is called once per frame
    void Update()
    {
        //if (bought)
        if (!mainScript.gameIsInPause)
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

            switch (currentState)
            {
                case dogState.returning:
                    // Try to go where the mouse cursor is
                    if (seekTarget(Vector3.zero))
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

                            if (maxSpeed > 20)
                            {
                                // Add some coins whatever happen!
                                addCoins();
                                addSCoins();
                            }
                        }
                    }
                    else
                    {
                        //Debug.Log("Here, seeking");
                        if (turnAroundTarget(targetPos, forTime, true))
                        {
                            currentState = dogState.returning;

                            mainScript.checkBoard(targetPos, targetPos, false);

                            if (maxSpeed > 20)
                            {
                                // Add some coins whatever happen!
                                addCoins();
                                addSCoins();
                            }
                        }
                    }
                    break;
                default:
                    // Walking around
                    goAround(Vector3.zero);
                    break;
            }

            dogCurrPos.x += dogSpeed.x;
            dogCurrPos.y += dogSpeed.y;

            if (float.IsNaN(dogCurrPos.x))
            {
                dogCurrPos.x = 100;
            }
            if (float.IsNaN(dogCurrPos.y))
            {
                dogCurrPos.y = 100;
            }

            transform.localPosition = dogCurrPos;
        }
    }

    private void addCoins()
    {
        theCoinManager.gainSomeCoins(finalMultiplier, dogCurrPos.x, dogCurrPos.y);
    }

    private void addSCoins()
    {
        theCoinManager.gainSomeSCoins(finalSMultiplier, dogCurrPos.x, dogCurrPos.y);
    }

    private void setSpeed()
    {

        float multForAllDogs = 1;

        bool hasKey = LocalSave.HasFloatKey("DogSpeedMult");
        if (hasKey)
        {
            multForAllDogs = LocalSave.GetFloat("DogSpeedMult");
        }

        float multForThisDog = 1;

        string key = dogNumber + "DogSpeedMult";
        hasKey = LocalSave.HasFloatKey(key);
        if (hasKey)
        {
            multForThisDog = LocalSave.GetFloat(key);
        }

        if (multForAllDogs > 10000)
        {
            multForAllDogs = 10000;
        }
        if (multForThisDog > 10000)
        {
            multForThisDog = 10000;
        }
        maxSpeed = 1f * multForAllDogs * multForThisDog;

        Debug.Log("Max speed is " + maxSpeed);

        timeBeforeNextSearch = 120 / maxSpeed;

        if (maxSpeed > 1000)
        {
            maxSpeed = 1000;
        }
        levelUpdate = (int)multForThisDog;
    }

    //Vector3 lastPos = new Vector3(0, 0, 0);
    //Vector2 dogCurrPos;
    //Vector2 dogSpeed = new Vector2(0, 0);
    //Vector2 dogAcc = new Vector2(0, 0);
    Vector2 dogTmpSpeed = new Vector2(0, 0);
    Vector2 dogWantedSpeed = new Vector2(0, 0);
    float tmpSpeedDist;
    float currentSpeed = 0;
    float maxSpeed = 0.1f;
    float standardSpeed = 0.05f;
    float stopTargetingRadiusSq = 0.2f;
    float orientation = 0;

    private void goAround(Vector2 targetPos)
    {
        float distToTarget = Vector2.Distance(dogCurrPos, targetPos);
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
        this.tmpSpeedDist = dogTmpSpeed.magnitude;
        this.dogWantedSpeed.x = this.dogTmpSpeed.x / tmpSpeedDist;
        this.dogWantedSpeed.y = this.dogTmpSpeed.y / tmpSpeedDist;
        this.dogWantedSpeed.x *= this.standardSpeed / 8;
        this.dogWantedSpeed.y *= this.standardSpeed / 8;

        float speedAcc = 0.02f;
        float timeAround = 20;

        if (toGetIt)
        {
            speedAcc = 0.001f;
            timeAround = 5 / (levelUpdate + 1);
        }
        float distToTarget = Vector2.Distance(dogCurrPos, targetPos);
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
        this.tmpSpeedDist = dogTmpSpeed.magnitude;
        this.dogWantedSpeed.x = this.dogTmpSpeed.x; // / tmpSpeedDist;
        this.dogWantedSpeed.y = this.dogTmpSpeed.y; // / tmpSpeedDist;
        //this.dogWantedSpeed.x *= this.standardSpeed / 8;
        //this.dogWantedSpeed.y *= this.standardSpeed / 8;

        float distToTarget = Vector2.Distance(dogCurrPos, targetPos);
        // Try to accelerate
        //if ((distToTarget < stopTargetingRadiusSq * 2) && (this.currentSpeed > this.standardSpeed))
        //    this.currentSpeed -= 0.025f;
        //else if (this.currentSpeed < this.maxSpeed)
        //    this.currentSpeed += 0.025f;

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
        dogSpeed.x = maxSpeed * xAcc;
        //checkAndNormaliseSpeed();
    }

    public void accY(float yAcc)
    {
        dogSpeed.y = maxSpeed * yAcc;
        //checkAndNormaliseSpeed();
    }

    public void checkAndNormaliseSpeed()
    {
        float actualSpeed = dogSpeed.magnitude;


        //if (dogSpeed.x != 0)
        //{
        //    this.orientation = Mathf.Acos(dogSpeed.x / actualSpeed);
        //}
        //else
        //{
        //    this.orientation = Mathf.PI / 2;
        //}
        //if (dogSpeed.y < 0)
        //{
        //    this.orientation = -this.orientation + 2 * Mathf.PI;
        //}

        if (actualSpeed > this.maxSpeed)
        {
            this.dogSpeed.x = maxSpeed * (this.dogSpeed.x / actualSpeed);
            this.dogSpeed.y = maxSpeed * (this.dogSpeed.y / actualSpeed);

            actualSpeed = this.maxSpeed;	
        }

        if (actualSpeed > this.tmpSpeedDist)
        {
            this.dogSpeed.x = tmpSpeedDist * (this.dogSpeed.x / actualSpeed);
            this.dogSpeed.y = tmpSpeedDist * (this.dogSpeed.y / actualSpeed);

            //this.currentSpeed = this.maxSpeed;	
        }
        //System.out.println("Current speed "+currentSpeed);
    }

    public static float distSq(float x, float y, float x1, float y1)
    {
        return (Mathf.Pow(x - x1, 2) + Mathf.Pow(y - y1, 2));
    }

    //internal bool updateDog()
    //{
    //    if (!isUpdated)
    //    {
    //        isUpdated = true;

    //        string keyUpdate = DogUpdatedBaseKey + nameDog;

    //        LocalSave.SetBool(keyUpdate, true);

    //        LocalSave.Save();

    //        useAvailableUpdates();

    //        return true;
    //    }
    //    return false;
    //}

    //internal bool superUpdate()
    //{
    //    if (!superUpdated || (levelUpdate < maxLevelUpdate))
    //    {
    //        superUpdated = true;

    //        string keySuperUpdate = DogSuperUpdatedBaseKey + nameDog;
    //        string keyLevelUpdate = DogLevelUpdatedBaseKey + nameDog;

    //        LocalSave.SetBool(keySuperUpdate, true);

    //        levelUpdate++;

    //        LocalSave.SetInt(keyLevelUpdate, levelUpdate);

    //        LocalSave.Save();

    //        useAvailableUpdates();

    //        return true;
    //    }
    //    return false;
    //}

    //internal bool buy()
    //{
    //    if (!bought)
    //    {
    //        bought = true;

    //        string keyBought = DogBoughtBaseKey + nameDog;

    //        LocalSave.SetBool(keyBought, true);

    //        LocalSave.Save();

    //        return true;
    //    }
    //    return false;
    //}
}
