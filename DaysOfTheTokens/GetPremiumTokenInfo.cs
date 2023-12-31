using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GetPremiumTokenInfo : MonoBehaviour
{
    public ManageToken ourManageToken;
    bool isPremium = false;
    static private int myNb = 10; // Default bonus 

    static public int MyNb { get => myNb; }

    private DateTime lastTimeAdsWatch;
    private int nbOfHours = 0;

    private bool inAdsPremium = false;

    public GameObject adsShower;

    // Start is called before the first frame update
    void Start()
    {      
        bool hasKey = LocalSave.HasStringKey("AdsBanner");

        if (hasKey)
        {
            string hideAds = LocalSave.GetString("AdsBanner");
            if (hideAds.Equals(BannerShower.keyNoAds))
            {
                isPremium = true;

                GetNbOfPurchases();

                ourManageToken.setNewNbSoFar(myNb);

                adsShower.SetActive(false);
            }
        }
        
        if (!isPremium)
        {
            // First check if the ads has been watched
            hasKey = LocalSave.HasLongKey(ShowFullScreenAds.ADS_WATCHED_KEYNAME);
            if (hasKey)
            {
                long whenLastWatched = LocalSave.GetLong(ShowFullScreenAds.ADS_WATCHED_KEYNAME);
                DateTime now = DateTime.Now;
                DateTime then = new DateTime(whenLastWatched);
                TimeSpan diff = now.Subtract(then);

                Debug.Log("Ads was watched " + diff.TotalHours + "hours ago");
                if (diff.TotalHours <= 12)
                {
                    Debug.Log("So less than 12 hours");
                    // Check for how long
                    hasKey = LocalSave.HasIntKey(ShowFullScreenAds.ADS_TIME_KEYNAME);
                    if (hasKey)
                    {
                        nbOfHours = LocalSave.GetInt(ShowFullScreenAds.ADS_TIME_KEYNAME);

                        Debug.Log("So less than "+nbOfHours +"?");

                        if (diff.TotalHours < nbOfHours)
                        {
                            Debug.Log("So less than " + nbOfHours + "!!!");

                            inAdsPremium = true;

                            lastTimeAdsWatch = then;

                            adsShower.SetActive(false);

                            GetNbOfPurchases();

                            ourManageToken.setNewNbSoFar(myNb);
                        }
                    }
                }

            }
            if (!inAdsPremium)
            { 
                myNb = 0;
            }
            // Show the Watch Ads button instead
        }
    }

    int iCheckAdsValidity = 0;

    private void Update()
    {
        if ((iCheckAdsValidity++ % 60 == 0) && inAdsPremium)
        {
            DateTime now = DateTime.Now;

            TimeSpan diff = now.Subtract(lastTimeAdsWatch);

            if (diff.TotalHours > nbOfHours)
            {
                inAdsPremium = false;

                // Remove the token and show the ads button
                adsShower.SetActive(true);

                ourManageToken.setNewNbSoFar(-1);

                myNb = 0;
            }
        }
    }

    private void GetNbOfPurchases()
    {
        myNb = 10;
    }

    public void getThePremiumToken()
    {
        GetNbOfPurchases();

        ourManageToken.setNewNbSoFar(myNb);
    }

    internal void giveTemporaryToken(int nbOfHours, long thenInTicks)
    {
        GetNbOfPurchases();

        ourManageToken.setNewNbSoFar(myNb);

        this.nbOfHours = nbOfHours;

        inAdsPremium = true;

        lastTimeAdsWatch = new DateTime(thenInTicks);

        adsShower.SetActive(false);
    }
}
