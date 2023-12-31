using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectDifficulty : MonoBehaviour
{
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

    int currentDifficulty = 2;

    // We keep the selected difficulty when a button is pressed.
    // It will be saved when the confirm button is selected (warning windows)
    int difficultyToSet = 0;

    // Start is called before the first frame update
    void Start()
    {
        // Read the current difficulty level
        {
            bool haskey = LocalSave.HasIntKey("difficulty");
            if (haskey)
            {
                currentDifficulty = LocalSave.GetInt("difficulty");
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
        {
            // Delete all level saves (not progression)
            //CreateBoard.deleteAllSaves();

            // Save the difficulty and restart Nyaf
            LocalSave.SetInt("difficulty", difficultyToSet);

            LaunchNYAF.loadNYAFGame();
        }
    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
