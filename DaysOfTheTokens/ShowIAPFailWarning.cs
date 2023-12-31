using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;

public class ShowIAPFailWarning : MonoBehaviour
{
    public Text explanationText;
    public BannerShower ourBannerShower;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void giveReason(Product forProduct, PurchaseFailureReason reason)
    {
        if (reason == PurchaseFailureReason.DuplicateTransaction)
        {
            explanationText.text = "You already bought the NoAds/Premium\nThank you!";

            ourBannerShower.hideAds();
        }
        else
        { 
            explanationText.text = "Sorry, the purchased failed. Please try again or contact our support.\nReason given: " + reason.ToString();
        }
    }
}
