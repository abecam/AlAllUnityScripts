using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateYANYAFCodes
{
    // Code the current values in a string
    string Yanyaf_Prefs;
    string Yanyaf_Progression;
    string Yanyaf_ScoreAndTime;
    string Yanyaf_BestTime;

    CodeEncodeToString ourEncoder = new CodeEncodeToString();

    public string YanyafPrefs { get => Yanyaf_Prefs; }
    public string YanyafProgression { get => Yanyaf_Progression; }
    public string YanyafScoreAndTime { get => Yanyaf_ScoreAndTime; }
    public string YanyafBestTime { get => Yanyaf_BestTime; }

    public void getYanyafCodes()
    {
        int currentDifficulty = 0;
        int pieceTransparences = 1; // 1 -> On, 0 -> Off
        int showHints = 1;
        int showArrows = 1;
        int maxGameMode = 0;
        int allSmallFoundFlag = 0;
        int nbOfPiecesFound = 0;
        float[] lastTimeSpent = new float[5];
        float currentTimeAllocated = 0;

        int currentGameMode = 0;

        // 0 to 7
        bool haskey = LocalSave.HasIntKey("yanyaf_difficulty");
        if (haskey)
        {
            currentDifficulty = LocalSave.GetInt("yanyaf_difficulty");
        }

        haskey = LocalSave.HasIntKey("yanyaf_transparentChars");
        if (haskey)
        {
            pieceTransparences = LocalSave.GetInt("yanyaf_transparentChars");

            //Debug.Log("Transparence: " + pieceTransparences + " - " + pieceTransparencesToggle.isOn);
        }

        haskey = LocalSave.HasIntKey("yanyaf_showHints");
        if (haskey)
        {
            showHints = LocalSave.GetInt("yanyaf_showHints");
        }

        haskey = LocalSave.HasIntKey("yanyaf_showArrows");
        if (haskey)
        {
            showArrows = LocalSave.GetInt("yanyaf_showArrows");
        }

        haskey = LocalSave.HasIntKey("yanyaf_smallFound");

        if (haskey)
        {
            allSmallFoundFlag = LocalSave.GetInt("yanyaf_smallFound");
        }

        haskey = LocalSave.HasIntKey("yanyaf_nbOfPiecesFound");

        if (haskey)
        {
            nbOfPiecesFound = LocalSave.GetInt("yanyaf_nbOfPiecesFound");
        }

        for (int iGameMode = 0; iGameMode < 5; iGameMode++)
        {
            haskey = LocalSave.HasFloatKey("yanyaf_allTimeSpent" + iGameMode);

            if (haskey)
            {
                lastTimeSpent[iGameMode] = LocalSave.GetFloat("yanyaf_allTimeSpent" + iGameMode);
            }
        }
        haskey = LocalSave.HasFloatKey("yanyaf_currentTimeAllocated");

        if (haskey)
        {
            currentTimeAllocated = LocalSave.GetFloat("yanyaf_currentTimeAllocated");
        }

        // And which mode we are in
        haskey = LocalSave.HasIntKey("yanyaf_currentMode");
        if (haskey)
        {
            currentGameMode = LocalSave.GetInt("yanyaf_currentMode");
        }

        int preferences = currentDifficulty  // 0-7
            + (pieceTransparences << 4) // 0-1
            + (showHints << 5)   // 0-1
            + (showArrows << 6); // 0-1

        Debug.Log("Yanyaf PREFS: currentDifficulty " + currentDifficulty + " pieceTransparences " + pieceTransparences
            + " showHints " + showHints + " showArrows " + showArrows);

        int progression3 = nbOfPiecesFound;

        int[] timeInt = new int[5];
        string timeInEachMode = "";
        bool first = true;

        for (int iMode = 0; iMode < 5; iMode++)
        {
            timeInt[iMode] = (int)lastTimeSpent[iMode];
            timeInEachMode += (first ? "" : ".") + ourEncoder.ConvertToBase(timeInt[iMode]);
            first = false;

            Debug.Log("Yanyaf TIME: time in mode [" + iMode + "] is " + timeInt[iMode] + " (" + timeInEachMode + ")");
        }

        int currentTimeMode3 = (int)currentTimeAllocated + 10000000;

        Yanyaf_Prefs = ourEncoder.ConvertToBase(preferences);
        int returnedValue = ourEncoder.ConvertFromBase(Yanyaf_Prefs);

        Debug.Log("Yanyaf PREFS: Preference int should be " + preferences + " = " + returnedValue);
        Yanyaf_Progression = ourEncoder.ConvertToBase(allSmallFoundFlag) + "." + ourEncoder.ConvertToBase(currentGameMode) + "." + ourEncoder.ConvertToBase(progression3);
        Debug.Log("Yanyaf PROG: maxGameMode " + maxGameMode + ", currentGameMode " + currentGameMode);
        Debug.Log("Yanyaf PROG: allSmallFoundFlag int should be " + allSmallFoundFlag + ", progression2 int should be " + currentGameMode + ", progression3 int should be " + progression3);
        Yanyaf_ScoreAndTime = timeInEachMode+"."+ ourEncoder.ConvertToBase(currentTimeMode3);

        int[] bestTimeInt = new int[5];
        string bestTimeInEachMode = "";
        bool firstBest = true;
        float bestTimeForMode;

        for (int iGameMode = 0; iGameMode < 5; iGameMode++)
        {
            bestTimeForMode = 1000000000;
            haskey = LocalSave.HasFloatKey("yanyaf_bestTimeSpent" + iGameMode);

            if (haskey)
            {
                bestTimeForMode = LocalSave.GetFloat("yanyaf_bestTimeSpent" + iGameMode);
            }

            bestTimeInt[iGameMode] = (int)lastTimeSpent[iGameMode];
            bestTimeInEachMode += (firstBest ? "" : ".") + ourEncoder.ConvertToBase(bestTimeInt[iGameMode]);
            firstBest = false;

            Debug.Log("Yanyaf BEST TIME: best time in mode [" + iGameMode + "] is " + timeInt[iGameMode] + " (" + timeInEachMode + ")");
        }

        Yanyaf_BestTime = timeInEachMode;
    }
}
