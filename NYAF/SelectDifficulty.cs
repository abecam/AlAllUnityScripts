using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectDifficulty : MonoBehaviour
{
    public Button veryEasyButton;
    public const int veryEasy = 0;
    public Button easyButton;
    public const int easy = 1;
    public Button normalButton;
    public const int normal = 2;
    public Button hardButton;
    public const int hard = 3;
    public Button veryHardButton;
    public const int veryHard = 4;
    public Button insaneButton;
    public const int insane = 5;
    public Button purgatoryButton; 
    public const int purgatory = 6;
    public Button hellButton;
    public const int hell = 7;

    public GameObject warningPage;

    public bool inYANYAF = false; // If in YANYAF, change the difficulty for this last 

    int currentDifficulty = 2;

    // We keep the selected difficulty when a button is pressed.
    // It will be saved when the confirm button is selected (warning windows)
    int difficultyToSet = 0;

    // Start is called before the first frame update
    void Start()
    {
        // Read the current difficulty level
        if (!inYANYAF)
        {
            bool haskey = LocalSave.HasIntKey("difficulty");
            if (haskey)
            {
                currentDifficulty = LocalSave.GetInt("difficulty");
            }
        }
        else
        {
            bool haskey = LocalSave.HasIntKey("yanyaf_difficulty");
            if (haskey)
            {
                currentDifficulty = LocalSave.GetInt("yanyaf_difficulty");
            }
        }
        // And select the right button
        selectCurrentDifficulty();
    }

    private void selectCurrentDifficulty()
    {
        // Selecting a difficulty will restart the game, so no need to activate the buttons here first.
        switch (currentDifficulty)
        {
            case veryEasy:
                veryEasyButton.interactable = false;
                break;
            case easy:
                easyButton.interactable = false;
                break;
            case normal:
                normalButton.interactable = false;
                break;
            case hard:
                hardButton.interactable = false;
                break;
            case veryHard:
                veryHardButton.interactable = false;
                break;
            case insane:
                insaneButton.interactable = false;
                break;
            case purgatory:
                purgatoryButton.interactable = false;
                break;
            case hell:
                hellButton.interactable = false;
                break;
        }
    }


    public void selectANewDifficulty(int newDifficulty)
    {
        Debug.Log("selecting difficulty "+ newDifficulty);

        SelectDifficultyForLater(newDifficulty);       
    }

    private void SelectDifficultyForLater(int newDifficulty)
    {
        Debug.Log("selecting difficulty for later " + newDifficulty);

        difficultyToSet = newDifficulty;

        // And show the warning page
        warningPage.SetActive(true);
    }

    public void confirmSelectedDifficulty()
    {
        if (!inYANYAF)
        {
            // Delete all level saves (not progression)
            CreateBoard.deleteLevelSaves();

            // Save the difficulty and restart Nyaf
            LocalSave.SetInt("difficulty", difficultyToSet);

            LaunchNYAF.loadNYAFGame();
        }
        else
        {
            // Delete all level saves (not progression)
            ManageYANYAF.deleteLevelSaves();

            // Save the difficulty and restart Nyaf
            LocalSave.SetInt("yanyaf_difficulty", difficultyToSet);

            LaunchYANYAF.loadGame();
        }
    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
