using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetAndAnimateCoin : MonoBehaviour
{
    public GameObject coin;
    public AudioSource soundForCoin;
    public AudioSource soundForCoins;
    public GetNbOfCoins ourtextShower;
    public GetNbOfCoins ourtextShowerSCoin;

    double nbOfCoins = 0;

    int nbOfCoinsSpawned = 0;
    const int maxNbCoinsSpawned = 20;

    int nbOfCoinsSound = 0;
    const int maxNbCoinsSound = 10;

    float currentTime = 0;
    float currentTimeSnd = 0;

    // Start is called before the first frame update
    void Start()
    {
        // Check how long we have been offline and calculate how much money has been won.
    }

    // Update is called once per frame
    void Update()
    {
        currentTime += Time.deltaTime;
        currentTimeSnd += Time.deltaTime;

        if ((currentTime > 5) && (nbOfCoinsSpawned > 0))
        {
            nbOfCoinsSpawned--;

            if (nbOfCoinsSound > 0)
            {
                nbOfCoinsSound--;
            }

            currentTime = 0;
        }

        if ((currentTimeSnd > 5) && (nbOfCoinsSpawned > 0))
        {
            nbOfCoinsSound--;

            currentTimeSnd = 0;
        }
    }

    public void gainSomeSCoins(double nbOfNewCoins, float atPosX, float atPosY)
    {
        double multiplier = 1;
        double currentMultiplier = 1;

        // Add some coins
        bool hasKey = LocalSave.HasDoubleKey("Hero");
        if (hasKey)
        {
            nbOfCoins = LocalSave.GetDouble("Hero");
        }
        hasKey = LocalSave.HasDoubleKey("SCoinsMult");
        if (hasKey)
        {
            multiplier = LocalSave.GetDouble("SCoinsMult");
        }

        hasKey = LocalSave.HasDoubleKey(WatchAds.ADS_MULTIPLIER_KEYNAME);
        if (hasKey)
        {
            currentMultiplier = LocalSave.GetDouble(WatchAds.ADS_MULTIPLIER_KEYNAME);
        }

        double newNbOfCoins = nbOfCoins + nbOfNewCoins* multiplier * currentMultiplier;

        LocalSave.SetDouble("Hero", newNbOfCoins);
        ourtextShowerSCoin.setNewNbOfCoin(newNbOfCoins);
        //LocalSave.Save();

        if (nbOfCoinsSpawned < maxNbCoinsSpawned)
        {
            if (((int)nbOfNewCoins) == 1)
            {
                GameObject aNewCoin = Instantiate(coin);

                MoveAndDisappear moveAndDisScript = aNewCoin.GetComponent<MoveAndDisappear>();

                moveAndDisScript.setPos(atPosX, atPosY);

                // And the corresponding sound
                if (nbOfCoinsSound < maxNbCoinsSound)
                {
                    soundForCoin.Play();

                    nbOfCoinsSound++;
                }

                nbOfCoinsSpawned++;
            }
            else
            {
                float speed = 0.001f;

                if (nbOfNewCoins > 10)
                {
                    nbOfNewCoins = 10;
                }


                for (int iCoin = 0; iCoin < nbOfNewCoins; ++iCoin)
                {
                    GameObject aNewCoin = Instantiate(coin);

                    MoveAndDisappear moveAndDisScript = aNewCoin.GetComponent<MoveAndDisappear>();

                    moveAndDisScript.setPosWithSpeed(atPosX, atPosY, speed);

                    speed += 0.001f;
                }

                nbOfCoinsSpawned += (int)nbOfNewCoins;

                // And the corresponding sound
                if (nbOfCoinsSound < maxNbCoinsSound)
                {
                    soundForCoins.Play();

                    nbOfCoinsSound++;
                }
            }
        }
    }

    public void gainSomeCoins(double nbOfNewCoins, float atPosX, float atPosY)
    {
        double multiplier = 1;
        double currentMultiplier = 1;

        // Add some coins
        bool hasKey = LocalSave.HasDoubleKey("Coins");
        if (hasKey)
        {
            nbOfCoins = LocalSave.GetDouble("Coins");
        }
        hasKey = LocalSave.HasDoubleKey("CoinMultiplier");
        if (hasKey)
        {
            multiplier = LocalSave.GetDouble("CoinMultiplier");
        }

        hasKey = LocalSave.HasDoubleKey(WatchAds.ADS_MULTIPLIER_KEYNAME);
        if (hasKey)
        {
            currentMultiplier = LocalSave.GetDouble(WatchAds.ADS_MULTIPLIER_KEYNAME);
        }

        //Debug.Log("Adding " + nbOfNewCoins + " with multipliers " + multiplier + " and " + currentMultiplier);

        double newNbOfCoins = nbOfCoins + nbOfNewCoins* multiplier * currentMultiplier;

        LocalSave.SetDouble("Coins", newNbOfCoins);
        ourtextShower.setNewNbOfCoin(newNbOfCoins);
        //LocalSave.Save();

        if (nbOfCoinsSpawned < maxNbCoinsSpawned)
        {
            if (((int)nbOfNewCoins) == 1)
            {
                GameObject aNewCoin = Instantiate(coin);

                MoveAndDisappear moveAndDisScript = aNewCoin.GetComponent<MoveAndDisappear>();

                moveAndDisScript.setPos(atPosX, atPosY);

                // And the corresponding sound
                if (nbOfCoinsSound < maxNbCoinsSound)
                {
                    soundForCoin.Play();

                    nbOfCoinsSound++;
                }
            }
            else
            {
                float speed = 0.001f;

                if (nbOfNewCoins > 10)
                {
                    nbOfNewCoins = 10;
                }

                for (int iCoin = 0; iCoin < nbOfNewCoins; ++iCoin)
                {
                    GameObject aNewCoin = Instantiate(coin);

                    MoveAndDisappear moveAndDisScript = aNewCoin.GetComponent<MoveAndDisappear>();

                    moveAndDisScript.setPosWithSpeed(atPosX, atPosY, speed);

                    speed += 0.001f;
                }
                nbOfCoinsSpawned += (int)nbOfNewCoins;

                // And the corresponding sound
                if (nbOfCoinsSound < maxNbCoinsSound)
                {
                    soundForCoins.Play();

                    nbOfCoinsSound++;
                }
            }
        }
    }
}
