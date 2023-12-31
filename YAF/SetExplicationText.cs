using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetExplicationText : MonoBehaviour
{
    private Text ourText;
    private int nbOfPiecesToFind = 1500;
    private int currentDifficulty = SelectDifficulty.normal; // Default difficulty
    private int nbOfElements = 79;

    // Start is called before the first frame update
    void Start()
    {
        ourText = GetComponent<Text>();

        int currentGameMode = 0;

        bool haskey = LocalSave.HasIntKey("currentMode");
        if (haskey)
        {
            currentGameMode = LocalSave.GetInt("currentMode");
        }

        findNbsToFind();

        setExplanationText(currentGameMode);
    }

    private void findNbsToFind()
    {
        bool haskey = LocalSave.HasIntKey("difficulty");
        if (haskey)
        {
            currentDifficulty = LocalSave.GetInt("difficulty");
        }
    }

    private void setExplanationText(int currentGameMode)
    {
        String text = "Find the dollars to gain some coins and super-coins (to upgrade the hero). When you can, let the dogs find the coins for you!";

        ourText.text = text;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
