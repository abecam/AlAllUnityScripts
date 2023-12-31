using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetAndAnimateCoin : MonoBehaviour
{
    public GameObject coin;
    public AudioSource soundForCoin;
    public AudioSource soundForCoins;
    public GetNbOfCoins ourtextShower;

    int nbOfCoins = 0;

    const int maxNbOfCoins = int.MaxValue/10;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void gainSomeCoins(int nbOfNewCoins, float atPosX, float atPosY)
    {
        if (nbOfCoins < maxNbOfCoins)
        {
            bool hasX2 = false;

            // Add some coins
            bool hasKey = LocalSave.HasIntKey("Coins");
            if (hasKey)
            {
                nbOfCoins = LocalSave.GetInt("Coins");
            }
            hasKey = LocalSave.HasBoolKey("x2Coins");
            if (hasKey)
            {
                hasX2 = LocalSave.GetBool("x2Coins");
            }

            int newNbOfCoins = nbOfCoins + nbOfNewCoins * (hasX2 ? 2 : 1);

            if (newNbOfCoins >= maxNbOfCoins)
            {
                // Add an achievement!
                InitializeSteam.unlockAchievement(InitializeSteam.achievementKey.NYAF_SHOP_ALL_COINS);
            }
            LocalSave.SetInt("Coins", newNbOfCoins);

            int allCoins = 0;

            hasKey = LocalSave.HasIntKey("TotalCoinsWon");
            if (hasKey)
            {
                allCoins = LocalSave.GetInt("TotalCoinsWon");
            }

            if (allCoins < newNbOfCoins)
            {
                allCoins = newNbOfCoins;
            }
            allCoins+= nbOfNewCoins * (hasX2 ? 2 : 1);

            LocalSave.SetInt("TotalCoinsWon", allCoins);

            ourtextShower.setNewNbOfCoin(newNbOfCoins);
            //LocalSave.Save();

            if (nbOfNewCoins == 1)
            {
                GameObject aNewCoin = Instantiate(coin);

                MoveAndDisappear moveAndDisScript = aNewCoin.GetComponent<MoveAndDisappear>();

                moveAndDisScript.setPos(atPosX, atPosY);

                // And the corresponding sound
                soundForCoin.Play();
            }
            else
            {
                float speed = 0.001f;

                for (int iCoin = 0; iCoin < nbOfNewCoins; ++iCoin)
                {
                    GameObject aNewCoin = Instantiate(coin);

                    MoveAndDisappear moveAndDisScript = aNewCoin.GetComponent<MoveAndDisappear>();

                    moveAndDisScript.setPosWithSpeed(atPosX, atPosY, speed);

                    speed += 0.001f;
                }
                // And the corresponding sound
                soundForCoins.Play();
            }
        }
    }

    public void gainSomeCoins3D(int nbOfNewCoins, float atPosX, float atPosY, float atPosZ)
    {
        if (nbOfCoins < maxNbOfCoins)
        {
            bool hasX2 = false;

            // Add some coins
            bool hasKey = LocalSave.HasIntKey("Coins");
            if (hasKey)
            {
                nbOfCoins = LocalSave.GetInt("Coins");
            }
            hasKey = LocalSave.HasBoolKey("x2Coins");
            if (hasKey)
            {
                hasX2 = LocalSave.GetBool("x2Coins");
            }

            int newNbOfCoins = nbOfCoins + nbOfNewCoins * (hasX2 ? 2 : 1);

            if (newNbOfCoins >= maxNbOfCoins)
            {
                // Add an achievement!
                InitializeSteam.unlockAchievement(InitializeSteam.achievementKey.NYAF_SHOP_ALL_COINS);
            }
            LocalSave.SetInt("Coins", newNbOfCoins);

            int allCoins = 0;

            hasKey = LocalSave.HasIntKey("TotalCoinsWon");
            if (hasKey)
            {
                allCoins = LocalSave.GetInt("TotalCoinsWon");
            }

            if (allCoins < newNbOfCoins)
            {
                allCoins = newNbOfCoins;
            }
            allCoins += nbOfNewCoins * (hasX2 ? 2 : 1);

            LocalSave.SetInt("TotalCoinsWon", allCoins);

            ourtextShower.setNewNbOfCoin(newNbOfCoins);
            //LocalSave.Save();

            if (nbOfNewCoins == 1)
            {
                GameObject aNewCoin = Instantiate(coin);

                MoveAndDisappear moveAndDisScript = aNewCoin.GetComponent<MoveAndDisappear>();

                moveAndDisScript.setPos3D(atPosX, atPosY, atPosZ);

                // And the corresponding sound
                soundForCoin.Play();
            }
            else
            {
                float speed = 0.001f;

                for (int iCoin = 0; iCoin < nbOfNewCoins; ++iCoin)
                {
                    GameObject aNewCoin = Instantiate(coin);

                    MoveAndDisappear moveAndDisScript = aNewCoin.GetComponent<MoveAndDisappear>();

                    moveAndDisScript.setPosWithSpeed3D(atPosX, atPosY, atPosZ, speed);

                    speed += 0.001f;
                }
                // And the corresponding sound
                soundForCoins.Play();
            }
        }
    }
}
