using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManageShopPurchases : MonoBehaviour
{
    long scoinMultiplier = 0;
    float coinMultiplier = 1;

    float coinForDogsMultiplier = 1;
    float[] coinForDogMultiplier = { 0, 0, 0, 0, 0, 0, 0, 0 , 0 , 0 , 0 , 0};
    
    float speedForDogsMultiplier = 1;
    float[] speedForDogMultiplier = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

    void Start()
    {
        scoinMultiplier = 0;
        coinMultiplier = 1;

        coinForDogsMultiplier = 1;
        speedForDogsMultiplier = 1;

        for (int iDog = 0; iDog < 12; iDog++)
        {
            coinForDogMultiplier[iDog] = 1;
            speedForDogMultiplier[iDog] = 1;
        }

        getPurchases();

        LocalSave.Save();

        setActiveDogs();
    }

    public void activatePurchases()
    {
        getPurchases();

        LocalSave.Save();
    }

    private void setNbOfDogs(int newNb)
    {
        LocalSave.SetInt("NbDogs", newNb);
    }

    private void setActiveDogs()
    {
        int nbOfDogs = 0;

        bool hasKey = LocalSave.HasIntKey("NbDogs");
        if (hasKey)
        {
            nbOfDogs = LocalSave.GetInt("NbDogs");
        }

        int iDogs = 0;

        foreach (Transform childDogs in transform)
        {
            GameObject oneDog = childDogs.gameObject;

            if (iDogs++ < nbOfDogs)
            {
                oneDog.SetActive(true);
            }
            else
            {
                oneDog.SetActive(false);
            }
        }
    }

    private void setDogsIgnoreCats()
    {
        LocalSave.SetBool("DogsIgnoreCats", true);
    }

    private void setCatsIgnoreDogs()
    {
        LocalSave.SetBool("CatsIgnoreDogs", true);
    }

    private void multiplyCoinsForDogs(int forDogNb, float byMultiplier)
    {
        if (forDogNb == 0)
        {
            coinForDogsMultiplier = coinForDogsMultiplier * byMultiplier;

            if (coinForDogsMultiplier > 1E18F)
            {
                coinForDogsMultiplier = 1E18F;
            }
            Debug.Log("All dogs multiplier is " + coinForDogsMultiplier);

            LocalSave.SetFloat("AllDogMulti", coinForDogsMultiplier);
        }
        else
        {
            string key = forDogNb + "DogMulti";

            coinForDogMultiplier[forDogNb -1] = coinForDogMultiplier[forDogNb - 1] * byMultiplier;

            if (coinForDogMultiplier[forDogNb - 1] > 1E18F)
            {
                coinForDogMultiplier[forDogNb - 1] = 1E18F;
            }
            Debug.Log("Dog "+ forDogNb + " multiplier is " + coinForDogMultiplier[forDogNb - 1]);

            LocalSave.SetFloat(key, coinForDogMultiplier[forDogNb - 1]);
        }
    }

    private void setSCoinMultiplier(long newNb)
    {
        if (scoinMultiplier == 0)
        {
            scoinMultiplier += newNb;
        }
        else
        {
            scoinMultiplier *= newNb;

            Debug.Log("SCoin multiplier is " + scoinMultiplier);
        }
        LocalSave.SetDouble("SCoinsMult", scoinMultiplier);
    }

    private void setCoinMultiplier(float newCoinMultiplier)
    {
        if (coinMultiplier < 1E20)
        {
            coinMultiplier = coinMultiplier * newCoinMultiplier;
        }
        else
        {
            coinMultiplier = coinMultiplier * Mathf.Log(newCoinMultiplier + 1);
        }

        Debug.Log("Coin multiplier is " + coinMultiplier);

        LocalSave.SetDouble("CoinMultiplier", coinMultiplier);
    }

    private void setDogSpeedMultiplier(int forDogNb, float byMultiplier)
    {
        if (forDogNb == 0)
        {
            speedForDogsMultiplier *= byMultiplier;

            if (speedForDogsMultiplier > 1000000000000000)
            {
                speedForDogsMultiplier = 1000000000000000;
            }

            LocalSave.SetFloat("DogSpeedMult", speedForDogsMultiplier);
        }
        else
        {
            string key = forDogNb + "DogSpeedMult";

            speedForDogMultiplier[forDogNb - 1] = speedForDogMultiplier[forDogNb - 1] * byMultiplier;

            if (speedForDogMultiplier[forDogNb - 1] > 1000000000000000)
            {
                speedForDogMultiplier[forDogNb - 1] = 1000000000000000;
            }

            LocalSave.SetFloat(key, speedForDogMultiplier[forDogNb - 1]);
        }
    }

    private void setDogCanFindCoins(int forDogNb)
    {
        string key = forDogNb + "canFindCoin";

        LocalSave.SetBool(key, true);
    }

    private void getPurchases()
    {
        bool hasKey = false;

        hasKey = LocalSave.HasBoolKey("Dog 1");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("Dog 1");
            if (isBought)
            {
                // Your first dog, Bob!
                // Do something here
                setNbOfDogs(1);
            }
        }

        hasKey = LocalSave.HasBoolKey("Dog 2");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("Dog 2");
            if (isBought)
            {
                // Your 2nd dog, Bill!
                // Do something here
                setNbOfDogs(2);
            }
        }

        hasKey = LocalSave.HasBoolKey("Infinite1");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("Infinite1");
            if (isBought)
            {
                // The 3rd Dog, Bob v2!
                // Do something here
                setNbOfDogs(3);
            }
        }

        hasKey = LocalSave.HasBoolKey("Infinite1Rotate");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("Infinite1Rotate");
            if (isBought)
            {
                // A rotation!
                // Do something here
                setDogSpeedMultiplier(1, 1.2f);
            }
        }

        hasKey = LocalSave.HasBoolKey("Dog 1Upd1");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("Dog 1Upd1");
            if (isBought)
            {
                // Bob get faster
                // Do something here
                setDogSpeedMultiplier(1, 2);
            }
        }

        hasKey = LocalSave.HasBoolKey("Dog 1Upd2");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("Dog 1Upd2");
            if (isBought)
            {
                // Bob can get the items!
                // Do something here
                setDogCanFindCoins(1);
            }
        }

        hasKey = LocalSave.HasBoolKey("Dog 1Upd3");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("Dog 1Upd3");
            if (isBought)
            {
                // Bob is super fast!
                // Do something here
                setDogSpeedMultiplier(1, 1.5f);
            }
        }

        hasKey = LocalSave.HasBoolKey("Dog 1Upd4");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("Dog 1Upd4");
            if (isBought)
            {
                // Bob is even faster!
                // Do something here
                setDogSpeedMultiplier(1, 1.8f);
            }
        }

        hasKey = LocalSave.HasBoolKey("Dog 1Upd5");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("Dog 1Upd5");
            if (isBought)
            {
                // And faster!
                // Do something here
                setDogSpeedMultiplier(1, 2);
            }
        }

        hasKey = LocalSave.HasBoolKey("Dog 1Upd6");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("Dog 1Upd6");
            if (isBought)
            {
                // And ultrasonic!
                // Do something here
                setDogSpeedMultiplier(1, 4);
            }
        }

        hasKey = LocalSave.HasBoolKey("Dog 1Upd7");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("Dog 1Upd7");
            if (isBought)
            {
                // Can't see it now...
                // Do something here
                setDogSpeedMultiplier(1, 10);
            }
        }

        hasKey = LocalSave.HasBoolKey("Dog 1Upd8");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("Dog 1Upd8");
            if (isBought)
            {
                // Coated to be heat resistant
                // Do something here
                setDogSpeedMultiplier(1, 4);
            }
        }

        hasKey = LocalSave.HasBoolKey("Dog 1Upd9");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("Dog 1Upd9");
            if (isBought)
            {
                // Getting hot!
                // Do something here
                setDogSpeedMultiplier(1, 4);
            }
        }

        hasKey = LocalSave.HasBoolKey("Dog 1Upd10");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("Dog 1Upd10");
            if (isBought)
            {
                // Can you believe that speed?
                // Do something here
                setDogSpeedMultiplier(1, 4);
            }
        }

        hasKey = LocalSave.HasBoolKey("Dog 1Upd11");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("Dog 1Upd11");
            if (isBought)
            {
                // Stronger, faster!
                // Do something here
                setDogSpeedMultiplier(0, 2);
            }
        }

        hasKey = LocalSave.HasBoolKey("Dog 1Upd12");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("Dog 1Upd12");
            if (isBought)
            {
                // Stronger, faster!
                // Do something here
                setDogSpeedMultiplier(1, 1.6f);
            }
        }

        hasKey = LocalSave.HasBoolKey("Dog 1Upd13");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("Dog 1Upd13");
            if (isBought)
            {
                // Super-bob!
                // Do something here
                setDogSpeedMultiplier(1, 2);
            }
        }

        hasKey = LocalSave.HasBoolKey("Dog 1Upd14");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("Dog 1Upd14");
            if (isBought)
            {
                // Super-bob v2
                // Do something here
                multiplyCoinsForDogs(1, 1.5f);
            }
        }

        hasKey = LocalSave.HasBoolKey("Dog 1Upd15");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("Dog 1Upd15");
            if (isBought)
            {
                // Super-bob v3
                // Do something here
                multiplyCoinsForDogs(1, 1.5f);
            }
        }

        hasKey = LocalSave.HasBoolKey("Dog 1Upd16");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("Dog 1Upd16");
            if (isBought)
            {
                // Super-bob v3
                // Do something here
                multiplyCoinsForDogs(1, 2);
            }
        }

        hasKey = LocalSave.HasBoolKey("Dog 1Upd17");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("Dog 1Upd17");
            if (isBought)
            {
                // Hyper-bob!
                // Do something here
                multiplyCoinsForDogs(1, 4);
            }
        }

        hasKey = LocalSave.HasBoolKey("Dog 1Upd18");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("Dog 1Upd18");
            if (isBought)
            {
                // Hyper-bob 2!
                // Do something here
                multiplyCoinsForDogs(1, 4);
            }
        }

        hasKey = LocalSave.HasBoolKey("Dog 1UpdPaid1");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("Dog 1UpdPaid1");
            if (isBought)
            {
                // Bob get 2x coins
                // Do something here
                multiplyCoinsForDogs(1, 2);
            }
        }

        hasKey = LocalSave.HasBoolKey("Dog 1UpdPaid2");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("Dog 1UpdPaid2");
            if (isBought)
            {
                // Bob get x4 coins
                // Do something here
                multiplyCoinsForDogs(1, 4);
            }
        }

        hasKey = LocalSave.HasBoolKey("Dog 1Upd23");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("Dog 1Upd23");
            if (isBought)
            {
                // Bob get x10 coins
                // Do something here
                multiplyCoinsForDogs(1, 10);
            }
        }

        hasKey = LocalSave.HasBoolKey("Dog 2Upd24");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("Dog 2Upd24");
            if (isBought)
            {
                // Bill get faster
                // Do something here
                setDogSpeedMultiplier(2, 1.2f);
            }
        }

        hasKey = LocalSave.HasBoolKey("Dog 2Upd25");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("Dog 2Upd25");
            if (isBought)
            {
                // Bill: Stronger, faster!
                // Do something here
                setDogSpeedMultiplier(2, 2);
            }
        }

        hasKey = LocalSave.HasBoolKey("Dog 2Upd26");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("Dog 2Upd26");
            if (isBought)
            {
                // Super-bill!
                // Do something here
                setDogSpeedMultiplier(2, 8);
            }
        }

        hasKey = LocalSave.HasBoolKey("Dog 2Upd27");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("Dog 2Upd27");
            if (isBought)
            {
                // Super-bill v2
                // Do something here
                setDogSpeedMultiplier(2, 8);
            }
        }

        hasKey = LocalSave.HasBoolKey("Dog 2Upd28");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("Dog 2Upd28");
            if (isBought)
            {
                // Hyper-bill!
                // Do something here
                setDogSpeedMultiplier(2, 8);
            }
        }

        hasKey = LocalSave.HasBoolKey("Dog 2Upd29");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("Dog 2Upd29");
            if (isBought)
            {
                // Super fast bill
                // Do something here
                setDogSpeedMultiplier(2, 8);
            }
        }

        hasKey = LocalSave.HasBoolKey("Dog 2Upd30");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("Dog 2Upd30");
            if (isBought)
            {
                // Super-bill v3
                // Do something here
                setDogSpeedMultiplier(2, 8);
            }
        }

        hasKey = LocalSave.HasBoolKey("Dog 2Upd31");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("Dog 2Upd31");
            if (isBought)
            {
                // Hyper-bill 2!
                // Do something here
                setDogSpeedMultiplier(2, 8);
            }
        }

        hasKey = LocalSave.HasBoolKey("Dog 2Upd32");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("Dog 2Upd32");
            if (isBought)
            {
                // Bill is be heat resistant
                // Do something here
                setDogSpeedMultiplier(2, 8);
            }
        }

        hasKey = LocalSave.HasBoolKey("Dog 2Upd33");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("Dog 2Upd33");
            if (isBought)
            {
                // Bill is getting hot!
                // Do something here
                setDogSpeedMultiplier(2, 8);
            }
        }

        hasKey = LocalSave.HasBoolKey("Dog 2Upd34");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("Dog 2Upd34");
            if (isBought)
            {
                // Bill: Stronger, faster!
                // Do something here
                setDogSpeedMultiplier(2, 2);
            }
        }

        hasKey = LocalSave.HasBoolKey("Dog 2Upd35");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("Dog 2Upd35");
            if (isBought)
            {
                // Another dog!
                // Do something here
                setNbOfDogs(4);
            }
        }

        hasKey = LocalSave.HasBoolKey("Dog 2Upd36");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("Dog 2Upd36");
            if (isBought)
            {
                // All dogs get faster!
                // Do something here
                setDogSpeedMultiplier(0, 1.2f);
            }
        }

        hasKey = LocalSave.HasBoolKey("Dog 2Upd37");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("Dog 2Upd37");
            if (isBought)
            {
                // And faster!
                // Do something here
                setDogSpeedMultiplier(0, 1.2f);
            }
        }

        hasKey = LocalSave.HasBoolKey("Dog 2Upd38");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("Dog 2Upd38");
            if (isBought)
            {
                // 2x more coins
                // Do something here
                multiplyCoinsForDogs(0, 2);
            }
        }

        hasKey = LocalSave.HasBoolKey("Dog 2Upd39");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("Dog 2Upd39");
            if (isBought)
            {
                // 4x more coins
                // Do something here
                multiplyCoinsForDogs(0, 4);
            }
        }

        hasKey = LocalSave.HasBoolKey("Dog 2Upd40");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("Dog 2Upd40");
            if (isBought)
            {
                // All faster!
                // Do something here
                setDogSpeedMultiplier(0, 1.2f);
            }
        }

        hasKey = LocalSave.HasBoolKey("Dog 2Upd41");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("Dog 2Upd41");
            if (isBought)
            {
                // And faster?!
                // Do something here
                setDogSpeedMultiplier(0, 1.5f);
            }
        }

        hasKey = LocalSave.HasBoolKey("Dog 2Upd42");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("Dog 2Upd42");
            if (isBought)
            {
                // It's becoming ridiculous!
                // Do something here
                setDogSpeedMultiplier(0, 2f);
            }
        }

        hasKey = LocalSave.HasBoolKey("Dog 2Upd43");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("Dog 2Upd43");
            if (isBought)
            {
                // More dollars to find! +2
                // Do something here
                setCoinMultiplier(1.2f);
            }
        }

        hasKey = LocalSave.HasBoolKey("Dog 2Upd44");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("Dog 2Upd44");
            if (isBought)
            {
                // And even more! +2
                // Do something here
                setCoinMultiplier(1.2f);
            }
        }

        hasKey = LocalSave.HasBoolKey("Dog 2Upd45");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("Dog 2Upd45");
            if (isBought)
            {
                // +20% coins
                // Do something here
                setCoinMultiplier(1.2f);
                setSCoinMultiplier(2);
            }
        }

        hasKey = LocalSave.HasBoolKey("GeneralUpdate");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("GeneralUpdate");
            if (isBought)
            {
                // 2x more dollars
                // Do something here
                setSCoinMultiplier(2);
            }
        }

        hasKey = LocalSave.HasBoolKey("GeneralUpdate2");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("GeneralUpdate2");
            if (isBought)
            {
                // 4x more dollars
                // Do something here
                setSCoinMultiplier(4);
            }
        }

        hasKey = LocalSave.HasBoolKey("Dog 2Upd48");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("Dog 2Upd48");
            if (isBought)
            {
                // All dogs are faster!
                // Do something here
                setDogSpeedMultiplier(0, 1.2f);
            }
        }

        hasKey = LocalSave.HasBoolKey("Dog 2Upd49");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("Dog 2Upd49");
            if (isBought)
            {
                // Ridiculous Speed
                // Do something here
                setDogSpeedMultiplier(0, 4f);
            }
        }

        hasKey = LocalSave.HasBoolKey("GeneralUpdate3");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("GeneralUpdate3");
            if (isBought)
            {
                // More dollars +10
                // Do something here

            }
        }

        hasKey = LocalSave.HasBoolKey("GeneralUpdate4");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("GeneralUpdate4");
            if (isBought)
            {
                // And even more dollars! +10
                // Do something here
                
            }
        }

        hasKey = LocalSave.HasBoolKey("GeneralUpdate5");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("GeneralUpdate5");
            if (isBought)
            {
                // How many dollars can we see??? *2
                // Do something here
                setSCoinMultiplier(2);
            }
        }

        hasKey = LocalSave.HasBoolKey("GeneralUpdate6");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("GeneralUpdate6");
            if (isBought)
            {
                // Stop, there is no more room!!! *2
                // Do something here
                setSCoinMultiplier(2);
            }
        }

        hasKey = LocalSave.HasBoolKey("Dog 1Upd53");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("Dog 1Upd53");
            if (isBought)
            {
                // Ludicrous Speed
                // Do something here
                setDogSpeedMultiplier(0, 4f);
            }
        }

        hasKey = LocalSave.HasBoolKey("Dog 1Upd54");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("Dog 1Upd54");
            if (isBought)
            {
                // Plaid Speed
                // Do something here
                setDogSpeedMultiplier(0, 4f);
            }
        }

        hasKey = LocalSave.HasBoolKey("Dog 1Upd55");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("Dog 1Upd55");
            if (isBought)
            {
                // YAF Speed
                // Do something here
                setDogSpeedMultiplier(0, 10f);
            }
        }

        hasKey = LocalSave.HasBoolKey("Dog 1Upd56");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("Dog 1Upd56");
            if (isBought)
            {
                // Halfway now!
                // Do something here
                setDogSpeedMultiplier(0, 2f);
            }
        }

        hasKey = LocalSave.HasBoolKey("Dog 1Upd57");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("Dog 1Upd57");
            if (isBought)
            {
                // All dogs are 1.5% faster!
                // Do something here
                setDogSpeedMultiplier(0, 1.15f);
            }
        }

        hasKey = LocalSave.HasBoolKey("Dog 1Upd58");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("Dog 1Upd58");
            if (isBought)
            {
                // 1.4% faster
                // Do something here
                setDogSpeedMultiplier(0, 1.14f);
            }
        }

        hasKey = LocalSave.HasBoolKey("Dog 1Upd59");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("Dog 1Upd59");
            if (isBought)
            {
                // 100000% faster!
                // Do something here
                setDogSpeedMultiplier(0, 1000f);
            }
        }

        hasKey = LocalSave.HasBoolKey("Dog 1Upd60");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("Dog 1Upd60");
            if (isBought)
            {
                // 2.3% faster
                // Do something here
                setDogSpeedMultiplier(0, 1.23f);
            }
        }

        hasKey = LocalSave.HasBoolKey("Dog 1Upd61");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("Dog 1Upd61");
            if (isBought)
            {
                // 5% faster
                // Do something here
                setDogSpeedMultiplier(0, 1.5f);
            }
        }

        hasKey = LocalSave.HasBoolKey("Dog 1Upd62");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("Dog 1Upd62");
            if (isBought)
            {
                // Grind grind grind!!!
                // Do something here
                multiplyCoinsForDogs(0, 10);
            }
        }

        hasKey = LocalSave.HasBoolKey("Dog 1Upd63");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("Dog 1Upd63");
            if (isBought)
            {
                // Or pay, pay, pay!!!
                // Do something here

                setSCoinMultiplier(2);
            }
        }

        hasKey = LocalSave.HasBoolKey("Dog 1Upd64");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("Dog 1Upd64");
            if (isBought)
            {
                // 6.5% faster
                // Do something here
                setDogSpeedMultiplier(0, 1.65f);
            }
        }

        hasKey = LocalSave.HasBoolKey("Dog 1Upd65");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("Dog 1Upd65");
            if (isBought)
            {
                // 100000000000% faster
                // Do something here
                setDogSpeedMultiplier(0, 1000000000f);
            }
        }

        hasKey = LocalSave.HasBoolKey("Dog 1Upd66");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("Dog 1Upd66");
            if (isBought)
            {
                // 7.6% faster
                // Do something here
                setDogSpeedMultiplier(0, 1.76f);
            }
        }

        hasKey = LocalSave.HasBoolKey("Dog 1Upd67");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("Dog 1Upd67");
            if (isBought)
            {
                // 100000000000000% faster
                // Do something here
                setDogSpeedMultiplier(0, 1000000000000f);
            }
        }

        hasKey = LocalSave.HasBoolKey("Dog 1Upd68");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("Dog 1Upd68");
            if (isBought)
            {
                // Did you buy the last update? Seriously?
                // Do something here

                setSCoinMultiplier(20);
            }
        }

        hasKey = LocalSave.HasBoolKey("Dog 1Upd69");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("Dog 1Upd69");
            if (isBought)
            {
                // Money, money money!!!
                // Do something here

                setSCoinMultiplier(20);
            }
        }

        hasKey = LocalSave.HasBoolKey("Dog 1Upd70");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("Dog 1Upd70");
            if (isBought)
            {
                // One more dog
                // Do something here
                setNbOfDogs(5);
            }
        }

        hasKey = LocalSave.HasBoolKey("Dog 1Upd71");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("Dog 1Upd71");
            if (isBought)
            {
                // And again one dog
                // Do something here
                setNbOfDogs(6);
            }
        }

        hasKey = LocalSave.HasBoolKey("Dog 1Upd72");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("Dog 1Upd72");
            if (isBought)
            {
                // More dog!
                // Do something here
                setNbOfDogs(7);
            }
        }

        hasKey = LocalSave.HasBoolKey("Dog 1Upd73");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("Dog 1Upd73");
            if (isBought)
            {
                // And a cat!
                // Do something here
                setNbOfDogs(8);
            }
        }

        hasKey = LocalSave.HasBoolKey("Dog 1Upd74");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("Dog 1Upd74");
            if (isBought)
            {
                // Make the dogs ignore the cat!
                // Do something here
                setDogsIgnoreCats();
            }
        }

        hasKey = LocalSave.HasBoolKey("Dog 1Upd75");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("Dog 1Upd75");
            if (isBought)
            {
                // Make the cat ignores the dogs!
                // Do something here
                setCatsIgnoreDogs();
            }
        }

        hasKey = LocalSave.HasBoolKey("Dog 1Upd76");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("Dog 1Upd76");
            if (isBought)
            {
                // Another cat!
                // Do something here
                setNbOfDogs(9);
            }
        }

        hasKey = LocalSave.HasBoolKey("Dog 1Upd77");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("Dog 1Upd77");
            if (isBought)
            {
                // Another... something
                // Do something here
                setNbOfDogs(10);
            }
        }

        hasKey = LocalSave.HasBoolKey("Dog 1Upd78");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("Dog 1Upd78");
            if (isBought)
            {
                // Aren't you tired to buy updates?
                // Do something here
                multiplyCoinsForDogs(0, 20);
            }
        }

        hasKey = LocalSave.HasBoolKey("Dog 1Upd79");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("Dog 1Upd79");
            if (isBought)
            {
                // Because I am tired to sell updates!
                // Do something here
                multiplyCoinsForDogs(0, 4);
            }
        }

        hasKey = LocalSave.HasBoolKey("Dog 1Upd80");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("Dog 1Upd80");
            if (isBought)
            {
                // Are you sure there is an end to that game?
                // Do something here
                multiplyCoinsForDogs(0, 4);
            }
        }

        hasKey = LocalSave.HasBoolKey("Dog 1Upd81");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("Dog 1Upd81");
            if (isBought)
            {
                // Me neither... :(
                // Do something here

                setCoinMultiplier(20);
                setSCoinMultiplier(2);
            }
        }

        hasKey = LocalSave.HasBoolKey("Dog 1Upd82");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("Dog 1Upd82");
            if (isBought)
            {
                // I will be locked here, until the end of time...
                // Do something here
                multiplyCoinsForDogs(0, 4);
            }
        }

        hasKey = LocalSave.HasBoolKey("Dog 1Upd24-2");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("Dog 1Upd24-2");
            if (isBought)
            {
                // Or until you stop playing, probably
                // Do something here
                setCoinMultiplier(2f);
            }
        }

        hasKey = LocalSave.HasBoolKey("Dog 1Upd83");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("Dog 1Upd83");
            if (isBought)
            {
                // Another dog again
                // Do something here
                setNbOfDogs(11);
            }
        }

        hasKey = LocalSave.HasBoolKey("Dog 1Upd84");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("Dog 1Upd84");
            if (isBought)
            {
                // And again
                // Do something here
                setNbOfDogs(12);
            }
        }

        hasKey = LocalSave.HasBoolKey("Dog 1Upd85");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("Dog 1Upd85");
            if (isBought)
            {
                // You must be crazy rich now :)
                // Do something here
                setCoinMultiplier(1.2f);
            }
        }

        hasKey = LocalSave.HasBoolKey("Dog 1Upd86");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("Dog 1Upd86");
            if (isBought)
            {
                // In the game...
                // Do something here
                setCoinMultiplier(1.2f);
            }
        }

        hasKey = LocalSave.HasBoolKey("Dog 1Upd87");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("Dog 1Upd87");
            if (isBought)
            {
                // Probably much poorer in RL
                // Do something here
                setCoinMultiplier(2f);
            }
        }

        hasKey = LocalSave.HasBoolKey("Dog 1Upd88");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("Dog 1Upd88");
            if (isBought)
            {
                // Last update for all animals!
                // Do something here
                multiplyCoinsForDogs(0, 10);
            }
        }

        hasKey = LocalSave.HasBoolKey("Dog 1Upd89");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("Dog 1Upd89");
            if (isBought)
            {
                // Another rotation
                // Do something here
                multiplyCoinsForDogs(0, 1.5f);
            }
        }

        hasKey = LocalSave.HasBoolKey("Dog 1Upd90");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("Dog 1Upd90");
            if (isBought)
            {
                // Well, another update still...
                // Do something here
                multiplyCoinsForDogs(0, 4);
            }
        }

        hasKey = LocalSave.HasBoolKey("Dog 1Upd91");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("Dog 1Upd91");
            if (isBought)
            {
                // And one more, last one, promise!
                // Do something here
                multiplyCoinsForDogs(0, 10);
            }
        }

        hasKey = LocalSave.HasBoolKey("Dog 1Upd92");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("Dog 1Upd92");
            if (isBought)
            {
                // Ok, I lied...
                // Do something here
                multiplyCoinsForDogs(0, 20);
            }
        }

        hasKey = LocalSave.HasBoolKey("Dog 1Upd93");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("Dog 1Upd93");
            if (isBought)
            {
                // Any idea what you could buy with that much money IRL?
                // Do something here
                multiplyCoinsForDogs(0, 2);
            }
        }

        hasKey = LocalSave.HasBoolKey("Dog 1Upd94");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("Dog 1Upd94");
            if (isBought)
            {
                // These dogs are crazy expensive, aren't they?
                // Do something here
                setNbOfDogs(15);
            }
        }

        hasKey = LocalSave.HasBoolKey("Dog 1Upd95");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("Dog 1Upd95");
            if (isBought)
            {
                // Us freemium sure like big numbers
                // Do something here
                setCoinMultiplier(40);
                setSCoinMultiplier(10);
            }
        }

        hasKey = LocalSave.HasBoolKey("Dog 1Upd96");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("Dog 1Upd96");
            if (isBought)
            {
                // Not at all to hide a paywall... Oh no!
                // Do something here
                multiplyCoinsForDogs(0, 2);
            }
        }

        hasKey = LocalSave.HasBoolKey("Dog 1Upd97");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("Dog 1Upd97");
            if (isBought)
            {
                // Might want to pay that though.
                // Do something here
                setCoinMultiplier(80);
                setSCoinMultiplier(80);
            }
        }

        hasKey = LocalSave.HasBoolKey("Dog 1Upd98");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("Dog 1Upd98");
            if (isBought)
            {
                // Or it might become a tad harder...
                // Do something here
                setCoinMultiplier(1.5f);
            }
        }

        hasKey = LocalSave.HasBoolKey("Dog 1Upd99");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("Dog 1Upd99");
            if (isBought)
            {
                // 1.0000001% faster
                // Do something here
                setDogSpeedMultiplier(0, 1.000001f);
            }
        }

        hasKey = LocalSave.HasBoolKey("Dog 1Upd100");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("Dog 1Upd100");
            if (isBought)
            {
                // 1.0000000001% faster
                // Do something here
                setDogSpeedMultiplier(0, 1.0000001f);
            }
        }

        hasKey = LocalSave.HasBoolKey("Dog 1Upd13");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("Dog 1Upd13");
            if (isBought)
            {
                // 1.00000000001% faster
                // Do something here
                setDogSpeedMultiplier(0, 1.0000001f);
            }
        }

        hasKey = LocalSave.HasBoolKey("Dog 1Upd102");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("Dog 1Upd102");
            if (isBought)
            {
                // 1.0000001% more coins
                // Do something here
                setCoinMultiplier(1.0000001f);
            }
        }

        hasKey = LocalSave.HasBoolKey("Dog 1Upd103");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("Dog 1Upd103");
            if (isBought)
            {
                // 1.000000000001% more coins
                // Do something here
                setCoinMultiplier(1.000000000001f);
            }
        }

        hasKey = LocalSave.HasBoolKey("Dog 1Upd104");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("Dog 1Upd104");
            if (isBought)
            {
                // 100000000000% more coins
                // Do something here
                setCoinMultiplier(1000000000f);
            }
        }

        hasKey = LocalSave.HasBoolKey("Dog 1Upd105");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("Dog 1Upd105");
            if (isBought)
            {
                // 2x more coins
                // Do something here
                setCoinMultiplier(2);
            }
        }

        hasKey = LocalSave.HasBoolKey("Dog 1Upd106");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("Dog 1Upd106");
            if (isBought)
            {
                // 4x more coins
                // Do something here
                setCoinMultiplier(4);
            }
        }

        hasKey = LocalSave.HasBoolKey("Dog 1Upd107");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("Dog 1Upd107");
            if (isBought)
            {
                // Getting tired... One more dog!
                // Do something here
                setNbOfDogs(16);
            }
        }

        hasKey = LocalSave.HasBoolKey("Dog 1Upd108");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("Dog 1Upd108");
            if (isBought)
            {
                // And a cat!
                // Do something here
                setNbOfDogs(17);
            }
        }

        hasKey = LocalSave.HasBoolKey("Dog 1Upd109");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("Dog 1Upd109");
            if (isBought)
            {
                // And a bird!
                // Do something here
                setNbOfDogs(18);
            }
        }

        hasKey = LocalSave.HasBoolKey("Dog 1Upd110");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("Dog 1Upd110");
            if (isBought)
            {
                // And that!
                // Do something here
                setNbOfDogs(19);
            }
        }

        hasKey = LocalSave.HasBoolKey("Dog 1Upd111");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("Dog 1Upd111");
            if (isBought)
            {
                // Last update! Well done!
                // Do something here
                setCoinMultiplier(200);
                setSCoinMultiplier(200);
            }
        }

        hasKey = LocalSave.HasBoolKey("x10coins46");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("x10coins46");
            if (isBought)
            {
                // x10 coins!
                // Do something here
                setCoinMultiplier(10);

                setSCoinMultiplier(10);
            }
        }

        hasKey = LocalSave.HasBoolKey("x20coins88");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("x20coins88");
            if (isBought)
            {
                // x20 coins
                // Do something here
                setCoinMultiplier(20);

                setSCoinMultiplier(20);
            }
        }

        hasKey = LocalSave.HasBoolKey("x80 coins112");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("x80 coins112");
            if (isBought)
            {
                // x80 coins
                // Do something here
                setCoinMultiplier(80);

                setSCoinMultiplier(80);
            }
        }

        hasKey = LocalSave.HasBoolKey("SCoin1-24");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("SCoin1-24");
            if (isBought)
            {
                // Super coin can be found!
                // Do something here
                setSCoinMultiplier(1);
            }
        }

        hasKey = LocalSave.HasBoolKey("SCoin2-47");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("SCoin2-47");
            if (isBought)
            {
                // x2 Super Coins!
                // Do something here
                setSCoinMultiplier(2);
            }
        }

        hasKey = LocalSave.HasBoolKey("SCoin3-50");
        if (hasKey)
        {
            bool isBought = LocalSave.GetBool("SCoin3-50");
            if (isBought)
            {
                // x10 Super Coins!
                // Do something here
                setSCoinMultiplier(10);
            }
        }


    }

    // Update is called once per frame
    void Update()
    {

    }
}
