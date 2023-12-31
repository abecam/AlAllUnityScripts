using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class BannerShower : MonoBehaviour
{
    public string gameId = "3740581";
    public string placementId = "bannerPlacement";
    public bool testMode = false;
    public const string keyNoAds = "ZGIUZ3fziu8uzfuz";

    void Start()
    {
        bool noAds = false;
        bool hasKey = LocalSave.HasStringKey("AdsBanner");

        if (hasKey)
        {
            string hideAds = LocalSave.GetString("AdsBanner");
            Debug.Log("Go HideAds code " + hideAds + " - ZGIUZ3fziu8uzfuz");
            if (hideAds.Equals(keyNoAds))
            {
                noAds = true;

                Debug.Log("We will not show ads");
            }
        }
        if (!noAds)
        {
            Advertisement.Initialize(gameId, testMode);
            StartCoroutine(ShowBannerWhenInitialized());
        }
    }

    IEnumerator ShowBannerWhenInitialized()
    {
        while (!Advertisement.isInitialized)
        {
            yield return new WaitForSeconds(0.5f);
        }
        Advertisement.Banner.SetPosition(BannerPosition.TOP_CENTER);

        Advertisement.Banner.Show(placementId);
    }

    public void hideAds()
    {
        Advertisement.Banner.Hide(true);

        LocalSave.SetString("AdsBanner", keyNoAds);

        LocalSave.Save();
    }
}
