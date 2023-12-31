using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BFSSelectLevel : MonoBehaviour
{
    public GameObject unfitJohn;
    public GameObject fitJohn;
    public GameObject SJohn;
    public GameObject MJohn;

    public GameObject unfitJohnArrow;
    public GameObject fitJohnArrow;
    public GameObject SJohnArrow;
    public GameObject MJohnArrow;

    public GameObject level1;
    public GameObject level2;
    public GameObject level3;
    public GameObject level4;
    public GameObject level5;
    public GameObject level6;

    private int maxJohn = 0;
    private int maxBellForMaxJohn = 0;

    private int selectedJohn = 1;

    BJSSave ourSaveFacility = new BJSSave();

    private LoadBJS ourLoader;


    // Start is called before the first frame update
    void Start()
    {
        ourLoader = GetComponent<LoadBJS>();

        LoadMaxValues();

        prepareJohns();

        selectOneJohn(1);
    }

    private void LoadMaxValues()
    {
        ourSaveFacility.loadAllValues();

        maxJohn = ourSaveFacility.MaxJohn;

        int maxBellForJohn = ourSaveFacility.MaxBellForLevel;

        int maxJohnFromMBFJ = maxBellForJohn / 100;

        if (maxJohn != maxJohnFromMBFJ)
        {
            Debug.Log("ERROR: Max John mismatch :" + maxJohn + " - From BFJ: " + maxJohnFromMBFJ);
        }
        maxBellForMaxJohn = maxBellForJohn - maxJohnFromMBFJ * 100;

        Debug.Log("Max bell was " + maxBellForMaxJohn + " for John #" + maxJohn);
    }

    private void prepareJohns()
    {
        MJohn.SetActive(true);
        SJohn.SetActive(true);
        fitJohn.SetActive(true);

        if (maxJohn <= 3)
        {
            MJohn.SetActive(false);
        }
        if (maxJohn <= 2)
        {
            SJohn.SetActive(false);
        }
        if (maxJohn <= 1)
        {
            fitJohn.SetActive(false);
        }
    }

    private void prepareLvlForJohn(int levelJohn)
    {
        level6.SetActive(true);
        level5.SetActive(true);
        level4.SetActive(true);
        level3.SetActive(true);
        level2.SetActive(true);

        if (levelJohn > maxJohn)
        {
            // Shouldn't be here :)
            Debug.Log("ERROR: levelJohn selected more than maxJohn saved, shouldn't happen");
        }
        if (levelJohn == maxJohn)
        {
            if (maxBellForMaxJohn <= 4)
            {
                level6.SetActive(false);
            }
            if (maxBellForMaxJohn <= 3)
            {
                level5.SetActive(false);
            }
            if (maxBellForMaxJohn <= 2)
            {
                level4.SetActive(false);
            }
            if (maxBellForMaxJohn <= 1)
            {
                level3.SetActive(false);
            }
            if (maxBellForMaxJohn <= 0)
            {
                level2.SetActive(false);
            }
        }
    }

    public void selectOneJohn(int whichOne)
    {
        selectedJohn = whichOne;

        prepareLvlForJohn(whichOne);

        Debug.Log("Selecting John " + whichOne);

        // Show the corresponding arrow as well
        if (whichOne == 1)
        {
            unfitJohnArrow.SetActive(true);
            fitJohnArrow.SetActive(false);
            SJohnArrow.SetActive(false);
            MJohnArrow.SetActive(false);

            return;
        }
        if (whichOne == 2)
        {
            unfitJohnArrow.SetActive(false);
            fitJohnArrow.SetActive(true);
            SJohnArrow.SetActive(false);
            MJohnArrow.SetActive(false);

            return;
        }
        if (whichOne == 3)
        {
            unfitJohnArrow.SetActive(false);
            fitJohnArrow.SetActive(false);
            SJohnArrow.SetActive(true);
            MJohnArrow.SetActive(false);

            return;
        }
        if (whichOne == 4)
        {
            unfitJohnArrow.SetActive(false);
            fitJohnArrow.SetActive(false);
            SJohnArrow.SetActive(false);
            MJohnArrow.SetActive(true);

            return;
        }
    }

    public void selectOneLevel(int whichOne)
    {
        Debug.Log("Selecting level " + whichOne);

        ourSaveFacility.CurrentJohn = selectedJohn;

        ourSaveFacility.CurrentBellForLevel = whichOne;

        ourSaveFacility.saveCurrentJohn();

        ourSaveFacility.saveBellNb();

        // No title restart
        ourSaveFacility.saveNoTitle();

        ourLoader.loadBJSGame();
    }

    void OnEnable()
    {
        Debug.Log("PrintOnEnable: script was enabled");

        LoadMaxValues();

        prepareJohns();

        selectOneJohn(1);
    }

    // Update is called once per frame
    void Update()
    {
       
    }
}
