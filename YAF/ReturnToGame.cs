using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnToGame : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnMouseDown()
    {
        loadMainGame();
    }

    public void loadMainGame()
    {
        bool hasKey = LocalSave.HasBoolKey("IsHeroShop");

        if (hasKey)
        {
            LocalSave.DeleteKey("IsHeroShop");
            LocalSave.Save();

            ManageShopPurchasesHero manageShopPurchasesHero = GetComponent<ManageShopPurchasesHero>();
            if (manageShopPurchasesHero != null)
            { 
                manageShopPurchasesHero.loadPurchases();
            }

            UnityEngine.SceneManagement.SceneManager.LoadScene("NYPB");
        }
        else
        {
            ManageShopPurchases manageShopPurchases = GetComponent<ManageShopPurchases>();
            if (manageShopPurchases != null)
            {
                manageShopPurchases.activatePurchases();
            }

            UnityEngine.SceneManagement.SceneManager.LoadScene("MainBoard");
        }
    }
}
