﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.UI;

public class WatchAdsHero : MonoBehaviour, IUnityAdsListener
{
#if UNITY_IOS
    private string gameId = "3800184";
#elif UNITY_ANDROID
    private string gameId = "3800185";
#endif

    public int nbAds;
    public float multiplier;

    public Text infoText;

    // Too short to use templating...
    const String baseText1 = "Watch an Ads\nx";
    const String baseText2 = "damage\nfor ";
    const String baseText3 = " minutes!";

    bool isPaidApp = false;

    bool noAds = false;

    public string myPlacementId = "rewardedVideo";

    private string keyAdsWatched;
    private string keyAdsLastTime;

    const float coolDownTime = 10 * 60;// 10 min between videos
    public const string ADS_MULTIPLIER_KEYNAME = "adsHeroMultiplier";
    public const string ADS_TIME_KEYNAME = "adsHeroMultiplier";

    float currentTime = coolDownTime;

    float lastPressed = 0;
    const float TEN_MIN = 600;
    const int TEN_MIN_INT = 600;

    bool isMultiplier = true;
    double currentMultiplier = 1;


    void Start()
    {
        Button myButton = GetComponent<Button>();

        // Set interactivity to be dependent on the Placement’s status:
        isPaidApp = Advertisement.IsReady(myPlacementId);

        bool haskey = LocalSave.HasIntKey("noAds_bought");

        if (haskey)
        {
            noAds = LocalSave.GetBool("noAds_bought");
        }
        // Map the ShowRewardedVideo function to the button’s click listener:
        if (myButton) myButton.onClick.AddListener(ShowRewardedVideo);

        // Initialize the Ads listener and service:
        Advertisement.AddListener(this);

        // 2nd parameter is test mode!!!
        Advertisement.Initialize(gameId, false);

        keyAdsWatched = ADS_MULTIPLIER_KEYNAME;
        keyAdsLastTime = ADS_TIME_KEYNAME + nbAds;

        haskey = LocalSave.HasDoubleKey(keyAdsWatched);
        if (haskey)
        {
            currentMultiplier = LocalSave.GetDouble(keyAdsWatched);
        }

        float now = Time.realtimeSinceStartup;

        haskey = LocalSave.HasFloatKey(keyAdsLastTime);
        if (haskey)
        {
            lastPressed = LocalSave.GetFloat(keyAdsLastTime);
        }

        if (isMultiplier)
        {
            // Check if more than 1 hour in the past, if so, remove the multiplier
            if (lastPressed < 0)
            {
                LocalSave.SetDouble(keyAdsWatched, 0);

                LocalSave.Save();

                isMultiplier = false;
            }
            else
            {
                isMultiplier = true;
            }
        }
    }

    int iUpdate = 0;

    void Update()
    {
        currentTime += Time.deltaTime;

        float now = Time.realtimeSinceStartup;

        if (isMultiplier)
        {
            lastPressed -= Time.deltaTime;

            //Debug.Log("Ads " + nbAds + " for " + (lastPressed));

            // Check if more than 1 hour in the past, if so, remove the multiplier
            if (lastPressed < 0)
            {
                LocalSave.SetDouble(keyAdsWatched, 1);

                LocalSave.SetFloat(keyAdsLastTime, 0);

                LocalSave.Save();

                isMultiplier = false;

                currentMultiplier = 1;

                infoText.text = "Watch Ads\n(x1.2 damage for 10 min)";
            }
            else if (iUpdate++ % 60 == 0)
            {
                LocalSave.SetFloat(keyAdsLastTime, lastPressed);
                LocalSave.Save();

                isMultiplier = true;

                int timeLeft = (int)lastPressed;
                int timeLeftHour = (int)(timeLeft / TEN_MIN);
                if (timeLeftHour > 0)
                {
                    timeLeft = timeLeft - (TEN_MIN_INT * timeLeftHour);
                }
                int timeLeftMinute = (int)(timeLeft / 60);
                if (timeLeftMinute > 0)
                {
                    timeLeft = timeLeft - (60 * timeLeftMinute);
                }

                infoText.text = "Watch Ads\n(x" + ((int)currentMultiplier) + "\n" + "for " +
                    ((timeLeftHour > 0) ? timeLeftHour + " : " : "") +
                    ((timeLeftMinute > 0) ? timeLeftMinute + " ' " : "0 ' ") +
                    ((timeLeft > 0) ? timeLeft + " s " : "")+ ") ";
            }
        }
    }
    // Implement a function for showing a rewarded video ad:
    void ShowRewardedVideo()
    {
        if (!noAds && isPaidApp && currentTime > coolDownTime)
        {
            // Find a way to know which button loaded the ads, so we can filter out the other call
            LocalSave.SetInt("AdsNb", nbAds);

            LocalSave.Save();

            Advertisement.Show(myPlacementId);

            currentTime = 0;
        }
        else
        {
            showOurAd();
        }
    }

    private void showOurAd()
    {
        ApplyMultiplier();

        LocalSave.SetBool("IsHeroAds", true); // The value is ignored, the watch checks only the key presence.

        LocalSave.Save();

        UnityEngine.SceneManagement.SceneManager.LoadScene("OurAds");
    }

    private void ApplyMultiplier()
    {
        bool haskey = LocalSave.HasDoubleKey(keyAdsWatched);
        if (haskey)
        {
            currentMultiplier = LocalSave.GetDouble(keyAdsWatched);
        }
        Debug.Log("A - Multiplying by " + multiplier + " : " + currentMultiplier);
        currentMultiplier += multiplier;
        Debug.Log("B - Multiplying by " + multiplier + " : " + currentMultiplier);

        LocalSave.SetDouble(keyAdsWatched, currentMultiplier);

        isMultiplier = true;

        float newPressed = TEN_MIN;

        // And add an hour if not in the timer already
        if (lastPressed < 0)
        {
            LocalSave.SetFloat(keyAdsLastTime, newPressed);

            lastPressed = newPressed;
        }

        LocalSave.Save();
    }

    private void changeMultiplier(double newMultiplier)
    {
        currentMultiplier = newMultiplier;
    }

    // Implement IUnityAdsListener interface methods:
    public void OnUnityAdsReady(string placementId)
    {
        // If the ready Placement is rewarded, activate the button: 
        if (placementId == myPlacementId)
        {
            isPaidApp = true;
        }
    }

    public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
    {
        // Find a way to know which button loaded the ads, so we can filter out the other call
        int forWhichAds = LocalSave.GetInt("AdsNb");

        if (forWhichAds == nbAds)
        {
            // Define conditional logic for each ad completion status:
            if (showResult == ShowResult.Finished)
            {
                // Reward the user for watching the ad to completion.
                ApplyMultiplier();
            }
            else if (showResult == ShowResult.Skipped)
            {
                // Do not reward the user for skipping the ad.
            }
            else if (showResult == ShowResult.Failed)
            {
                Debug.LogWarning("The ad did not finish due to an error.");
            }
        }
    }

    public void OnUnityAdsDidError(string message)
    {
        // Log the error.
        Debug.LogError("Ads did not work: " + message);
    }

    public void OnUnityAdsDidStart(string placementId)
    {
        // Optional actions to take when the end-users triggers an ad.
    }
}
