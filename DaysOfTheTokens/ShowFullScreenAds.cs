using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class ShowFullScreenAds : MonoBehaviour, IUnityAdsListener
{
#if UNITY_IOS
    private string gameId = "3740580";
#elif UNITY_ANDROID
    private string gameId = "3740581";
#endif

    public string myPlacementId = "rewardedVideo";

    private string keyAdsWatched;
    private string keyAdsLastTime;

    public ManageBox ourBoxManager;
    public TextMesh ourText;
    public GetPremiumTokenInfo ourPremiumTokenManager;

    private Collider2D ourCollider;

    const float coolDownTime = 10 * 60;// 10 min between videos
    public const string ADS_WATCHED_KEYNAME = "adsWatchedAt";
    public const string ADS_TIME_KEYNAME = "adsTime";

    // Start is called before the first frame update
    void Start()
    {
        ourCollider = GetComponent<Collider2D>();

        ourText.color = new Color(0.2f, 0.2f, 0.2f);

        Advertisement.AddListener(this);

        // Second paramater force test mode!
        Advertisement.Initialize(gameId, false);
    }

    bool dragMode = false;

    // Update is called once per frame
    void Update()
    {
        if (!ourBoxManager.isInPause())
        {
            // Check if the trashcan has been clicked.
            if (Application.platform != RuntimePlatform.Android)
            {
                Vector3 touchPos = Input.mousePosition;

                // use the input stuff
                if (Input.GetMouseButton(0))
                {
                    if (!dragMode)
                    {
                        checkBoard(touchPos);
                        dragMode = true;
                    }
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
                }
                else
                {
                    // No touch reset the cool down
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

                        if (firstFinger.phase == TouchPhase.Began)
                        {
                            checkBoard(touchPos);
                            dragMode = true;
                        }
                    }
                    else if (Input.touchCount == 2)
                    {
                        Vector2 touchPos1 = firstFinger.position;
                        Touch secondFinger = Input.GetTouch(1);
                        Vector2 touchPos2 = secondFinger.position;

                        if (firstFinger.phase == TouchPhase.Moved && secondFinger.phase == TouchPhase.Moved)
                        {
                            //
                        }
                        else
                        {
                            //
                        }
                    }
                }
                else
                {
                    dragMode = false;
                }
            }
        }
    }

    void checkBoard(Vector3 touchPos)
    { 
        if (ourCollider == Physics2D.OverlapPoint(touchPos))
        {
            Advertisement.Show(myPlacementId);
        }
    }

    public void OnUnityAdsReady(string placementId)
    {
        // If the ready Placement is rewarded, activate the button: 
        if (placementId == myPlacementId)
        {
            ourText.color = new Color(0,0,0);
        }
    }

    public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
    {
        // Find a way to know which button loaded the ads, so we can filter out the other call

        {
            // Define conditional logic for each ad completion status:
            if (showResult == ShowResult.Finished)
            {
                givePremiumForXHours(12);
            }
            else if (showResult == ShowResult.Skipped)
            {
                // Do not reward the user for skipping the ad.
                givePremiumForXHours(3);
            }
            else if (showResult == ShowResult.Failed)
            {
                Debug.LogWarning("The ad did not finish due to an error.");
                givePremiumForXHours(3);
            }
        }
    }

    private void givePremiumForXHours(int nbOfHours)
    {
        long timeInTick = DateTime.Now.Ticks;

        LocalSave.SetLong(ADS_WATCHED_KEYNAME, timeInTick);
        LocalSave.SetInt(ADS_TIME_KEYNAME, nbOfHours);
        LocalSave.Save();

        Debug.Log("WAS HERE!");

        ourPremiumTokenManager.giveTemporaryToken(nbOfHours, timeInTick);
    }

    public void OnUnityAdsDidError(string message)
    {
        // Log the error.
        Debug.LogError("Ads did not work: " + message);

        givePremiumForXHours(3);
    }

    public void OnUnityAdsDidStart(string placementId)
    {
        // Optional actions to take when the end-users triggers an ad.
    }
}
