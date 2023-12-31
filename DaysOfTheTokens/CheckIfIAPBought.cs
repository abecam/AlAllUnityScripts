using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheckIfIAPBought : MonoBehaviour
{
    public Text ourInfoText;

    // Start is called before the first frame update
    void Start()
    {
        checkIfBought();
    }

    public void checkIfBought()
    {
        Button ourButton = GetComponent<Button>();

        bool hasKey = LocalSave.HasStringKey("AdsBanner");

        if (hasKey)
        {
            string hideAds = LocalSave.GetString("AdsBanner");
            if (hideAds.Equals(BannerShower.keyNoAds))
            {
                ourButton.interactable = false;

                ourInfoText.text = "Thank you!";
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
