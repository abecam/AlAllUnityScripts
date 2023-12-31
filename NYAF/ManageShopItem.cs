using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class ManageShopItem : MonoBehaviour
{
    // If for a dog, it will be linked here
    public DogScript forDog;
    public bool isUpdate = false;
    public bool isSuperUpdate = false;
    public Text ourBoughtText;
    public int price;
    Button ourButton;

    // The last update has several steps
    public Text ourUpdateText;
    public Text ourUpdatePrice;

    private GetNbOfCoins ourtextShower;

    // Start is called before the first frame update
    void Start()
    {
        // Check what was bought
        bool transactionDone = false;
        bool hasKey;

        ourButton = GetComponent<Button>();

        LocalizationSettings.StringDatabase.GetLocalizedStringAsync(localizedLevel.TableReference, localizedLevel.TableEntryReference);

        if (forDog == null)
        {
            // that's the x2, in the future maybe some units
            hasKey = LocalSave.HasBoolKey("x2Coins");
            if (hasKey)
            {
                if (LocalSave.GetBool("x2Coins"))
                {
                   transactionDone = true;
                }
            }
        }
        else
        {
            if (isSuperUpdate)
            {
                transactionDone = forDog.SuperUpdated && (forDog.levelUpdate == DogScript.maxLevelUpdate);

                if (forDog.SuperUpdated && (forDog.levelUpdate < DogScript.maxLevelUpdate))
                {
                    price = 10000 * (IntPow(2, forDog.levelUpdate));

                    Debug.Log("New price for update is " + price);

                    ourUpdateText.text = "Bill will be even faster! " + forDog.levelUpdate + "/" + DogScript.maxLevelUpdate;
                    ourUpdatePrice.text = price + "x    ";
                }
            }
            else if (isUpdate)
            {
                transactionDone = forDog.IsUpdated;
            }
            else
            {
                transactionDone = forDog.Bought;
            }
        }
        if (transactionDone)
        {
            ourBoughtText.color = new Color(1f, 1f, 1f);
            ourButton.interactable = false;
        }
    }

    private void OnEnable()
    {
        GameObject coinShower = GameObject.FindGameObjectWithTag("CoinsShower");
        ourtextShower = coinShower.GetComponent<GetNbOfCoins>();

        localizedLevel.StringChanged += UpdateStringDemo;
        localizedLeft.StringChanged += UpdateStringModeA;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void buyItem()
    {
        int nbOfCoins = 0;

        bool hasKey = LocalSave.HasIntKey("Coins");
        if (hasKey)
        {
            nbOfCoins = LocalSave.GetInt("Coins");
        }

        if (nbOfCoins > price)
        {
            bool transactionDone = false;

            if (forDog == null)
            {
                // that's the x2, in the future maybe some units
                hasKey = LocalSave.HasBoolKey("x2Coins");
                if (hasKey)
                {
                    if (!LocalSave.GetBool("x2Coins"))
                    {
                        LocalSave.SetBool("x2Coins", true);

                        LocalSave.Save();

                        transactionDone = true;
                    }
                }
                else
                {
                    LocalSave.SetBool("x2Coins", true);

                    LocalSave.Save();

                    transactionDone = true;
                }
            }
            else
            {
                if (isSuperUpdate)
                {
                    transactionDone = forDog.superUpdate();
                }
                else if(isUpdate)
                {
                    transactionDone = forDog.updateDog();
                }
                else
                {
                    transactionDone = forDog.buy();
                }
            }
            if (transactionDone)
            { 
                nbOfCoins -= price;

                ourtextShower.setNewNbOfCoin(nbOfCoins);

                LocalSave.SetInt("Coins", nbOfCoins);
                LocalSave.Save();

                if (isSuperUpdate)
                {
                    if (forDog.levelUpdate < DogScript.maxLevelUpdate)
                    {
                        price = 10000 * (IntPow(2,  forDog.levelUpdate));
                        ourUpdateText.text = "Bill will be even faster! " + forDog.levelUpdate + "/" + DogScript.maxLevelUpdate;
                        ourUpdatePrice.text = price + "x    ";
                    }
                    else
                    {
                        ourUpdateText.text = "Bill is as fast as possible!";
                        ourBoughtText.color = new Color(1f, 1f, 1f);
                        ourButton.interactable = false;
                    }
                }
                else
                {
                    ourBoughtText.color = new Color(1f, 1f, 1f);
                    ourButton.interactable = false;
                }
            }
        }
    }

    int IntPow(int x, int pow)
    {
        int ret = 1;
        while (pow != 0)
        {
            if ((pow & 1) == 1)
                ret *= x;
            x *= x;
            pow >>= 1;
        }
        return ret;
    }

    public LocalizedString localizedLevel;
    private string localizedLevelText = "Bill will be even faster!";

    public LocalizedString localizedLeft;
    private string localizedLeftText = "Bill is as fast as possible!";

    void OnDisable()
    {
        localizedLevel.StringChanged -= UpdateStringDemo;
        localizedLeft.StringChanged -= UpdateStringModeA;
    }

    void UpdateStringDemo(string s)
    {
        localizedLevelText = s;
    }

    void UpdateStringModeA(string s)
    {
        localizedLeftText = s;
    }
}
