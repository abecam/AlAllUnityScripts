using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListOfBellsAndJohn : MonoBehaviour
{
    public GameObject towersForUnfitJohn;
    public GameObject towersForFitJohn;
    public GameObject towersForSuperJohn;
    public GameObject towersForMegaJohn;
    public GameObject lastTower;

    int iInTowers = 0; // In the current list of towers

    private GameObject currentTowers = null;
    private GameObject lastNonNullTower;

    public GameObject UnfitJohn;
    public GameObject fitJohn;
    public GameObject superJohn;
    public GameObject megaJohn;

    int iJohn = 0;
    private bool isLast = false;

    public bool IsLast { get => isLast; }

    // Start is called before the first frame update
    void Start()
    {
        lastNonNullTower = towersForUnfitJohn.transform.GetChild(0).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool wasLastLevel()
    {
        if (currentTowers != null)
        {
            return iInTowers >= currentTowers.transform.childCount - 1;
        }
        else
        {
            Debug.Log("wasLastLevel: current level null, shouldn't happen!");
        }
        return false;
    }

    public GameObject getNonNullTower()
    {
        return lastNonNullTower;
    }

    public GameObject getNextWalkingJohn()
    {
        switch (iJohn)
        {
            case 0:
                iJohn++;
                return UnfitJohn;

            case 1:
                iJohn++;
                return fitJohn;

            case 2:
                iJohn++;
                return superJohn;
        }

        return megaJohn;
    }

    internal GameObject getTowerByNb(int currentJohn, int currentBellNb)
    {
        GameObject nextTower = null;

        if (currentJohn > 4)
        {
            isLast = true;
            return lastTower;
        }
        isLast = false;

        iInTowers = currentBellNb;

        if (nextTower == null)
        {

            switch (currentJohn)
            {
                case 1:
                    currentTowers = towersForUnfitJohn;
                    break;

                case 2:
                    currentTowers = towersForFitJohn;
                    break;

                case 3:
                    currentTowers = towersForSuperJohn;
                    break;

                default:
                    currentTowers = towersForMegaJohn;
                    break;
            }

            if (currentTowers.transform.childCount > currentBellNb)
            {
                nextTower = currentTowers.transform.GetChild(currentBellNb).gameObject;
            }
            if (nextTower == null)
            {
                Debug.Log("No tower available there! " + currentJohn + " - " + currentBellNb);
                nextTower = currentTowers.transform.GetChild(0).gameObject;
            }
            // If null, nothing is left.
        }
        // Always keep a backup tower
        if (nextTower != null)
        {
            lastNonNullTower = nextTower;
        }
        else
        {
            nextTower = lastNonNullTower;
            Debug.Log("No tower available there! " + currentJohn+ " - " + currentBellNb);
        }

        return nextTower;
    }

    internal GameObject getWalkingJohnByNb(int currentJohn)
    {
        Debug.Log("John " + currentJohn + " requested");
        GameObject foundJohn = null;

        // Simply iterate through the towers
        for (int iJohn = 0; iJohn < currentJohn; iJohn++)
        {
            foundJohn = getNextWalkingJohn();
        }

        return foundJohn;
    }
}
