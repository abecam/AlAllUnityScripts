using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class EnterCode : MonoBehaviour
{
    public InputField fromField;
    public GameObject warningWindows;
    public Text warningText;

    public GameObject confirmWindows;
    public Text confirmText;

    //public Text fromFieldTxt;

    CodeEncodeToString ourDecoder = new CodeEncodeToString();

    private readonly string password1 = "XHHZGGFHV67";
    private readonly string password2 = "ZHFTFH2HZ6Q";
    private readonly string password3 = "HJUGTZ52";
    private readonly string passwordbis3 = "HUGTZ52";
    private readonly string passwordbisbis3 = "JUGTZ52";
    private readonly string password4 = "TFG67HZ";
    private readonly string password5 = "HUZTFGREVCF";
    private readonly string password6 = "IGZTDUJ";

    private enum typeOfAction
    {   NYAF_Prefs, NYAF_Progs, NYAF_TimeScore , NYAF_DogsAndCoins,
        YANYAF_Prefs, YANYAF_Progs, YANYAF_TimeScore,
        MMPG_Prefs, MMPG_Progs, MMPG_TimeScore,
        BJS_Prefs, BJS_Progs, BJS_TimeScore,
        NYAF_BestTimeScore,
        YANYAF_BestTimeScore,
        BJS_BestTimeScore,
        MMPG_BestTime,
    };

    public void getStringFromField()
    {
        String newString = fromField.text;

        addOneOrSeveralCodes(newString);
    }

    int nbOfPasswordAdded;

    void addOneOrSeveralCodes(string newCodes)
    {
        needToRestartNyaf = false;
        nbOfPasswordAdded = 0;

        Debug.Log("Full text is" + newCodes);
        // First cut by lines
        string[] allLines = newCodes.Split('\n');

        foreach (string oneLine in allLines)
        {
            Debug.Log("One line is" + oneLine);
            // Then check if they should be save code (start with * then an ID letter)

            if (oneLine.Contains("*"))
            {
                string oneSaveCode = oneLine.Substring(oneLine.IndexOf("*")+1).Trim();

                Debug.Log("One code is" + oneSaveCode);

                if (oneSaveCode.StartsWith(GetCurrentCodes.nyafPrefsID))
                {
                    Debug.Log("Preferences for nyaf");
                    decodeNyafPreferences(oneSaveCode);
                }
                else if (oneSaveCode.StartsWith(GetCurrentCodes.nyafProgressionID))
                {
                    Debug.Log("Progression for nyaf");
                    decodeNyafProgression(oneSaveCode);
                }
                else if (oneSaveCode.StartsWith(GetCurrentCodes.nyafScoreAndTimeID))
                {
                    Debug.Log("Score and time for nyaf");
                    decodeNyafTimeAndScore(oneSaveCode);
                }
                else if (oneSaveCode.StartsWith(GetCurrentCodes.nyafDogsAndCoinsID))
                {
                    Debug.Log("Dogs and coins for nyaf");
                    decodeNyafDogsAndCoins(oneSaveCode);
                }
                else if (oneSaveCode.StartsWith(GetCurrentCodes.nyafBestScoreAndTimeID))
                {
                    Debug.Log("Best Scores and time for nyaf");
                    decodeNyafBestTimeAndScore(oneSaveCode);
                }
                else if (oneSaveCode.StartsWith(GetCurrentCodes.yanyafPrefsID))
                {
                    Debug.Log("Preferences for yanyaf");
                    decodeYanyafPreferences(oneSaveCode);
                }
                else if (oneSaveCode.StartsWith(GetCurrentCodes.yanyafProgressionID))
                {
                    Debug.Log("Progression for yanyaf");
                    decodeYanyafProgression(oneSaveCode);
                }
                else if (oneSaveCode.StartsWith(GetCurrentCodes.yanyafScoreAndTimeID))
                {
                    Debug.Log("Score and time for yanyaf");
                    decodeYanyafTimeAndScore(oneSaveCode);
                }
                else if (oneSaveCode.StartsWith(GetCurrentCodes.bjsPrefsID))
                {
                    Debug.Log("Preferences for bjs");
                    decodeBJSPrefs(oneSaveCode);
                }
                else if (oneSaveCode.StartsWith(GetCurrentCodes.bjsProgressionID))
                {
                    Debug.Log("Progression for bjs");
                    decodeBJSProgression(oneSaveCode);
                }
                else if (oneSaveCode.StartsWith(GetCurrentCodes.bjsScoreAndTimeID))
                {
                    Debug.Log("Score and time for bjs");
                    decodeBJSTime(oneSaveCode);
                }
                else if (oneSaveCode.StartsWith(GetCurrentCodes.bjsBestScoreAndTimeID))
                {
                    Debug.Log("Best time for bjs");
                    decodeBJSBestTime(oneSaveCode);
                }
                else if (oneSaveCode.StartsWith(GetCurrentCodes.mmpgPrefsID))
                {
                    Debug.Log("Preferences for mmpg");
                    decodeMMPGPrefs(oneSaveCode);
                }
                else if (oneSaveCode.StartsWith(GetCurrentCodes.mmpgProgressionID))
                {
                    Debug.Log("Progression for mmpg");
                    decodeMMPGProgression(oneSaveCode);
                }
                else if (oneSaveCode.StartsWith(GetCurrentCodes.mmpgScoreAndTimeID))
                {
                    Debug.Log("Score and time for mmpg");
                    decodeMMPGTime(oneSaveCode);
                }
                else if (oneSaveCode.StartsWith(GetCurrentCodes.mmpgBestScoreAndTimeID))
                {
                    Debug.Log("Best time for mmpg");
                    decodeMMPGBestTime(oneSaveCode);
                }
            }
            // otherwise check if these are secret code
            String oneLineUpper = oneLine.ToUpper();

            if (oneLineUpper.Equals(password1))
            {
                Debug.Log("Hero 1 unlocked");

                unlockHero(1);
            }
            else if (oneLineUpper.Equals(password2))
            {
                Debug.Log("Hero 2 unlocked");

                unlockHero(2);
            }
            else if (oneLineUpper.Equals(password3) || oneLineUpper.Equals(passwordbis3) || oneLineUpper.Equals(passwordbisbis3))
            {
                Debug.Log("Hero 3 unlocked");

                unlockHero(3);
            }
            else if (oneLineUpper.Equals(password4))
            {
                Debug.Log("Hero 4 unlocked");

                unlockHero(4);
            }
            else if (oneLineUpper.Equals(password5))
            {
                Debug.Log("Hero 5 unlocked");

                unlockHero(5);
            }
            else if (oneLineUpper.Equals(password6))
            {
                Debug.Log("Hero 6 unlocked");

                unlockHero(6);
            }
        }
    }

    /*
     * Save the money won maybe?
     */
    private void decodeNyafDogsAndCoins(string oneSaveCode)
    {
        oneSaveCode = oneSaveCode.Substring(1);

        int allCoins ;

        nyafStoreCoins = new NyafStoreCoins();


        allCoins = ourDecoder.ConvertFromBase(oneSaveCode);
        nyafStoreCoins.allCoins = allCoins;

        Debug.Log("NYAF Shop: allCoins is " + allCoins + " (" + oneSaveCode + ")");

        askForNextAction(typeOfAction.NYAF_DogsAndCoins);
    }

    private class NyafStoreCoins
    {
        public int allCoins;
    }

    NyafStoreCoins nyafStoreCoins;

    private void applyNyafStoreCoins()
    {
        LocalSave.SetInt("TotalCoinsWon", nyafStoreCoins.allCoins);

        // Now to restore in the shop
        LocalSave.SetInt("Coins", nyafStoreCoins.allCoins);

        // And reset the dogs!
        LocalSave.SetBool("x2Coins", false);

        LocalSave.SetBool("dogBoughtBill", false); //"dogBoughtBill","dogUpdatedBill","dogSuperUpdatedBill","x2Coins","dogBoughtBob","dogUpdatedBob"

        LocalSave.SetBool("dogUpdatedBill", false);

        LocalSave.SetBool("dogSuperUpdatedBill", false);

        LocalSave.SetBool("dogBoughtBob", false);

        LocalSave.SetBool("dogUpdatedBob", false);

        LocalSave.SetInt("dogLevelUpdatedBill", 0);

        LocalSave.SetBool("x2Coins", false);

        LocalSave.Save();
    }

    private void unlockHero(int heroNb)
    {
        int currentHero = nbOfPasswordAdded++;

        bool isHero = false;

        // First check if the hero has not been already unlocked
        bool haskey = LocalSave.HasBoolKey("nypb_unitHero"+heroNb);
        if (haskey)
        {
            isHero = LocalSave.GetBool("nypb_unitHero"+ heroNb);
        }

        if (isHero)
        {
            Debug.Log("This hero has already been unlocked");
            if (currentHero > 0)
            {
                confirmText.text = confirmText.text+ "\nBut the hero " + heroNb + " was already unlocked. I got you a small bonus to compensate...";
            }
            else
            {
                confirmText.text = "But the hero " + heroNb + " was already unlocked. I got you a small bonus to compensate...";
            }

            confirmWindows.SetActive(true);

            int currentNbOfSpy = 0;

            haskey = LocalSave.HasIntKey("nypb_unitSpyPlayer");
            if (haskey)
            {
                currentNbOfSpy = LocalSave.GetInt("nypb_unitSpyPlayer");
            }
            currentNbOfSpy += 50;
            LocalSave.SetInt("nypb_unitSpyPlayer", currentNbOfSpy);

            LocalSave.Save();

            return;
        }
        LocalSave.SetBool("nypb_unitHero" + heroNb, true);
        LocalSave.Save();

        Debug.Log("Hero "+heroNb+" has been unlocked!!!");

        confirmText.text = "You unlocked the hero " + heroNb + "! Go in MMPG to discover it";
        if (currentHero > 0)
        {
            confirmText.text = confirmText.text + "\nYou unlocked the hero " + heroNb + "! Go in MMPG to discover it";
        }
        else
        {
            confirmText.text = "You unlocked the hero " + heroNb + "! Go in MMPG to discover it";
        }
        confirmWindows.SetActive(true);
    }

    private class NyafPrefs
    {
        public int currentDifficulty = 0;
        public int pieceTransparences = 1; // 1 -> On, 0 -> Off
        public int showHints = 1;
        public int showArrows = 1;
    }

    NyafPrefs nyafPrefs;

    private typeOfAction nextActionAsked;

    private void decodeNyafPreferences(string oneSaveCode)
    {
        oneSaveCode = oneSaveCode.Substring(1);

        int prefsInt = ourDecoder.ConvertFromBase(oneSaveCode);

        Debug.Log("OOOOO - NYAF PREFS: Preference int is " + prefsInt+ " ("+ oneSaveCode + ")");

        //int preferences = currentDifficulty  // 0-7
        //    + (pieceTransparences << 4)
        //    + (showHints << 5)
        //    + (showArrows << 6);

        nyafPrefs = new NyafPrefs();

        nyafPrefs.showArrows = prefsInt >> 6;
        nyafPrefs.showHints = (prefsInt >> 5) & 1; //  >> 5;
        nyafPrefs.pieceTransparences = (prefsInt >> 4) & 1;
        nyafPrefs.currentDifficulty = (prefsInt & 0b111);

        Debug.Log("OOOOO - NYAF PREFS: showArrows " + nyafPrefs.showArrows + " showHints " + nyafPrefs.showHints 
            + ", pieceTransparences " + nyafPrefs.pieceTransparences + ", currentDifficulty " + nyafPrefs.currentDifficulty);

        askForNextAction(typeOfAction.NYAF_Prefs);
    }

    private void applyNyafPreferences()
    {
        LocalSave.SetInt("transparentChars", nyafPrefs.pieceTransparences);

        LocalSave.SetInt("showHints", nyafPrefs.showHints);

        LocalSave.SetInt("showArrows", nyafPrefs.showArrows);

        LocalSave.SetInt("difficulty", nyafPrefs.currentDifficulty);

        LocalSave.Save();
    }
    
    private class NyafProgression
    {
        public int smallFoundFlag = 0;
        public int levelReachedInLastMode = 0;
        public int maxGameMode = 0;
        public int currentGameMode = 0;
        public int nbOfPiecesFound = 0;
    }

    NyafProgression nyafProgression;

    private void decodeNyafProgression(string oneSaveCode)
    {
        nyafProgression = new NyafProgression();

        oneSaveCode = oneSaveCode.Substring(1);

        String[] subStrings = oneSaveCode.Split('.');

        if (subStrings.Length != 3)
        {
            Debug.Log("NYAF PROG: Not enough parts " + subStrings.Length + " (3 expected)");
        }

        // nyafProgression = ourEncoder.ConvertToBase(allSmallFoundFlag) + "." +ourEncoder.ConvertToBase(progression2) + "." + ourEncoder.ConvertToBase(progression3);

        nyafProgression.smallFoundFlag = ourDecoder.ConvertFromBase(subStrings[0]);

        Debug.Log("NYAF PROG: smallFoundFlag is " + nyafProgression.smallFoundFlag + " (" + subStrings[0] + ")");

        //int progression2 = levelReachedInLastMode + (maxGameMode << 8) + (currentGameMode << 16); // 0-11 now, reserved for more
        //int progression3 = nbOfPiecesFound;

        int progression2 = ourDecoder.ConvertFromBase(subStrings[1]);

        nyafProgression.levelReachedInLastMode = progression2 & 0xF;
        nyafProgression.maxGameMode = (progression2 >> 8) & 0xf ;
        nyafProgression.currentGameMode = (progression2 >> 16) & 0xf;

        Debug.Log("NYAF PROG: levelReachedInLastMode " + nyafProgression.levelReachedInLastMode + " maxGameMode " + nyafProgression.maxGameMode 
            + "currentGameMode " + nyafProgression.currentGameMode);

        nyafProgression.nbOfPiecesFound = ourDecoder.ConvertFromBase(subStrings[2]);

        Debug.Log("NYAF PROG: nbOfPiecesFound " + nyafProgression.nbOfPiecesFound);

        askForNextAction(typeOfAction.NYAF_Progs);
    }

    private void applyNyafProgression()
    {
        LocalSave.SetInt("currentMode", nyafProgression.currentGameMode);

        LocalSave.SetInt("modeFinished", nyafProgression.maxGameMode);

        LocalSave.SetInt("levelReached"+ nyafProgression.maxGameMode, nyafProgression.levelReachedInLastMode);

        LocalSave.SetInt("smallFound", nyafProgression.smallFoundFlag);

        LocalSave.SetInt("nbOfPiecesFound", nyafProgression.nbOfPiecesFound);
        
        LocalSave.Save();
    }

    private void decodeNyafTimeAndScore(string oneSaveCode)
    {
        oneSaveCode = oneSaveCode.Substring(1);

        String[] subStrings = oneSaveCode.Split('.');

        if (subStrings.Length != 6)
        {
            Debug.Log("NYAF TIME: Not enough parts " + subStrings.Length + " (2 expected)");
        }

        //int[] timeInt = new int[5];
        //string timeInEachMode = "";

        //for (int iMode = 0; iMode < 5; iMode++)
        //{
        //    timeInt[iMode] = (int)lastTimeSpent[iMode];
        //    timeInEachMode += ourEncoder.ConvertToBase(timeInt[iMode]) + ".";
        //}

        int[] timeInt = new int[5];

        nyafTimeScore = new NyafTimeScore();

        for (int iMode = 0; iMode < 5; iMode++)
        {
            timeInt[iMode] = ourDecoder.ConvertFromBase(subStrings[iMode]);
            nyafTimeScore.timeSpent[iMode] = (float)timeInt[iMode];

            Debug.Log("NYAF TIME: time in mode ["+iMode+"] is " + timeInt[iMode] + " (" + subStrings[iMode] + ")");
        }

        long currentTimeMode3 = ourDecoder.ConvertFromBase(subStrings[5]) - 10000000;
        Debug.Log("NYAF TIME: currentTimeMode3 is " + currentTimeMode3 + " (" + subStrings[5] + ")");

        nyafTimeScore.currentTimeAllocated = (float)currentTimeMode3;

        askForNextAction(typeOfAction.NYAF_TimeScore);
    }

    NyafTimeScore bestNyafTimeScore;

    private void decodeNyafBestTimeAndScore(string oneSaveCode)
    {
        oneSaveCode = oneSaveCode.Substring(1);

        String[] subStrings = oneSaveCode.Split('.');

        if (subStrings.Length != 2)
        {
            Debug.Log("NYAF TIME: Not enough parts " + subStrings.Length + " (2 expected)");
        }

        int[] timeInt = new int[5];

        bestNyafTimeScore = new NyafTimeScore();

        for (int iMode = 0; iMode < 5; iMode++)
        {
            timeInt[iMode] = ourDecoder.ConvertFromBase(subStrings[iMode]);
            bestNyafTimeScore.timeSpent[iMode] = (float)timeInt[iMode];

            Debug.Log("NYAF TIME: time in mode [" + iMode + "] is " + timeInt[iMode] + " (" + subStrings[iMode] + ")");
        }

        long currentTimeMode3 = ourDecoder.ConvertFromBase(subStrings[5]) - 10000000;
        Debug.Log("NYAF TIME: currentTimeMode3 is " + currentTimeMode3 + " (" + subStrings[5] + ")");

        bestNyafTimeScore.currentTimeAllocated = (float)currentTimeMode3;

        askForNextAction(typeOfAction.NYAF_BestTimeScore);
    }

    private class NyafTimeScore
    {
        public float[] timeSpent = new float[5];
        public float currentTimeAllocated;
    }

    NyafTimeScore nyafTimeScore;

    private void applyNyafTimeScore()
    {
        for (int iGameMode = 0; iGameMode < 5; iGameMode++)
        {
            LocalSave.SetFloat("allTimeSpent" + iGameMode, nyafTimeScore.timeSpent[iGameMode]);
        }

        LocalSave.SetFloat("currentTimeAllocated", nyafTimeScore.currentTimeAllocated);

        LocalSave.Save();
    }

    private void applyBestNyafTimeScore()
    {
        for (int iGameMode = 0; iGameMode < 5; iGameMode++)
        {
            LocalSave.SetFloat("bestTimeSpent" + iGameMode, bestNyafTimeScore.timeSpent[iGameMode]);
        }

        LocalSave.SetFloat("bestScoreMode3", bestNyafTimeScore.currentTimeAllocated);

        LocalSave.Save();
    }

    NyafPrefs yanyafPrefs;

    private void decodeYanyafPreferences(string oneSaveCode)
    {
        oneSaveCode = oneSaveCode.Substring(1);

        int prefsInt = ourDecoder.ConvertFromBase(oneSaveCode);

        Debug.Log("YANYAF PREFS: Preference int is " + prefsInt + " (" + oneSaveCode + ")");

        yanyafPrefs = new NyafPrefs();
        //int preferences = currentDifficulty  // 0-7
        //    + (pieceTransparences << 4)
        //    + (showHints << 5)
        //    + (showArrows << 6);

        yanyafPrefs.showArrows = prefsInt >> 6;
        yanyafPrefs.showHints = (prefsInt >> 5) & 1; //  >> 5;
        yanyafPrefs.pieceTransparences = (prefsInt >> 4) & 1;
        yanyafPrefs.currentDifficulty = (prefsInt & 0b111);

        Debug.Log("YANYAF: showArrows " + yanyafPrefs.showArrows + " showHints " + yanyafPrefs.showHints + ", pieceTransparences " 
            + yanyafPrefs.pieceTransparences + ", currentDifficulty" + yanyafPrefs.currentDifficulty);

        askForNextAction(typeOfAction.YANYAF_Prefs);
    }

    private void applyYanyafPreferences()
    {
        LocalSave.SetInt("yanyaf_transparentChars", yanyafPrefs.pieceTransparences);

        LocalSave.SetInt("yanyaf_showHints", yanyafPrefs.showHints);

        LocalSave.SetInt("yanyaf_showArrows", yanyafPrefs.showArrows);

        LocalSave.SetInt("yanyaf_difficulty", yanyafPrefs.currentDifficulty);

        LocalSave.Save();
    }

    NyafProgression yanyafProgression;

    private void decodeYanyafProgression(string oneSaveCode)
    {
        yanyafProgression = new NyafProgression();

        oneSaveCode = oneSaveCode.Substring(1);

        String[] subStrings = oneSaveCode.Split('.');

        if (subStrings.Length != 3)
        {
            Debug.Log("YANYAF PROG: Not enough parts " + subStrings.Length + " (3 expected)");
        }

        // nyafProgression = ourEncoder.ConvertToBase(allSmallFoundFlag) + "." +ourEncoder.ConvertToBase(progression2) + "." + ourEncoder.ConvertToBase(progression3);

        yanyafProgression.smallFoundFlag = ourDecoder.ConvertFromBase(subStrings[0]);

        Debug.Log("YANYAF PROG: smallFoundFlag is " + yanyafProgression.smallFoundFlag + " (" + subStrings[0] + ")");

        //int progression2 = levelReachedInLastMode + (maxGameMode << 8) + (currentGameMode << 16); // 0-11 now, reserved for more
        //int progression3 = nbOfPiecesFound;

        yanyafProgression.currentGameMode = ourDecoder.ConvertFromBase(subStrings[1]);

        Debug.Log("YANYAF PROG: currentGameMode " + yanyafProgression.currentGameMode);

        yanyafProgression.nbOfPiecesFound = ourDecoder.ConvertFromBase(subStrings[2]);

        Debug.Log("YANYAF PROG: nbOfPiecesFound " + yanyafProgression.nbOfPiecesFound);

        askForNextAction(typeOfAction.YANYAF_Progs);
    }

    private void applyYayafProgression()
    {
        LocalSave.SetInt("yanyaf_currentMode", yanyafProgression.currentGameMode);

        LocalSave.SetInt("yanyaf_modeFinished", yanyafProgression.maxGameMode);

        LocalSave.SetInt("yanyaf_levelReached" + yanyafProgression.maxGameMode, yanyafProgression.levelReachedInLastMode);

        LocalSave.SetInt("yanyaf_smallFound", yanyafProgression.smallFoundFlag);

        LocalSave.SetInt("yanyaf_nbOfPiecesFound", yanyafProgression.nbOfPiecesFound);

        LocalSave.Save();
    }

    NyafTimeScore yanyafTimeScore;

    private void decodeYanyafTimeAndScore(string oneSaveCode)
    {
        yanyafTimeScore = new NyafTimeScore();

        oneSaveCode = oneSaveCode.Substring(1);

        String[] subStrings = oneSaveCode.Split('.');

        if (subStrings.Length != 6)
        {
            Debug.Log("YANYAF TIME: Not enough parts " + subStrings.Length + " (6 expected)");
        }

        int[] timeInt = new int[5];

        for (int iMode = 0; iMode < 5; iMode++)
        {
            timeInt[iMode] = ourDecoder.ConvertFromBase(subStrings[iMode]);
            yanyafTimeScore.timeSpent[iMode] = (float)timeInt[iMode];

            Debug.Log("YANYAF TIME: time in mode [" + iMode + "] is " + timeInt[iMode] + " (" + subStrings[iMode] + ")");
        }

        int currentTimeMode3 = ourDecoder.ConvertFromBase(subStrings[5]) - 10000000;
        yanyafTimeScore.currentTimeAllocated = (float)currentTimeMode3;

        Debug.Log("YANYAF TIME: currentTimeMode3 is " + currentTimeMode3 + " (" + subStrings[5] + ")");

        askForNextAction(typeOfAction.YANYAF_TimeScore);
    }

    private void applyYanyafTimeScore()
    {
        for (int iGameMode = 0; iGameMode < 5; iGameMode++)
        {
            LocalSave.SetFloat("yanyaf_allTimeSpent" + iGameMode, yanyafTimeScore.timeSpent[iGameMode]);
        }

        LocalSave.SetFloat("yanyaf_currentTimeAllocated", yanyafTimeScore.currentTimeAllocated);

        LocalSave.Save();
    }

    NyafTimeScore yanyafBestTimeScore;

    private void decodeYanyafBestTimeAndScore(string oneSaveCode)
    {
        yanyafBestTimeScore = new NyafTimeScore();

        oneSaveCode = oneSaveCode.Substring(1);

        String[] subStrings = oneSaveCode.Split('.');

        if (subStrings.Length != 6)
        {
            Debug.Log("YANYAF BEST TIME: Not enough parts " + subStrings.Length + " (6 expected)");
        }

        int[] timeInt = new int[5];

        for (int iMode = 0; iMode < 5; iMode++)
        {
            timeInt[iMode] = ourDecoder.ConvertFromBase(subStrings[iMode]);
            yanyafTimeScore.timeSpent[iMode] = (float)timeInt[iMode];

            Debug.Log("YANYAF BEST TIME: best time in mode [" + iMode + "] is " + timeInt[iMode] + " (" + subStrings[iMode] + ")");
        }

        askForNextAction(typeOfAction.YANYAF_BestTimeScore);
    }

    private void applyYanyafBestTimeScore()
    {
        for (int iGameMode = 0; iGameMode < 5; iGameMode++)
        {
            LocalSave.SetFloat("yanyaf_bestTimeSpent" + iGameMode, nyafTimeScore.timeSpent[iGameMode]);
        }

        LocalSave.Save();
    }

    private class MmpgPrefs
    {
        public int hasNature = 1;
        public int hasMonster = 1; // 1 -> On, 0 -> Off
        public int isRestricted = 1;
    }

    MmpgPrefs mmpgPrefs;

    private void decodeMMPGPrefs(string oneSaveCode)
    {
        mmpgPrefs = new MmpgPrefs();

        oneSaveCode = oneSaveCode.Substring(1);

        String[] subStrings = oneSaveCode.Split('.');

        if (subStrings.Length != 1)
        {
            Debug.Log("MMPG PREFS: Not enough parts " + subStrings.Length + " (1 expected)");
        }

        // nyafProgression = ourEncoder.ConvertToBase(allSmallFoundFlag) + "." +ourEncoder.ConvertToBase(progression2) + "." + ourEncoder.ConvertToBase(progression3);

        int prefs = ourDecoder.ConvertFromBase(subStrings[0]);

        Debug.Log("MMPG PREFS: prefs is " + prefs + " (" + subStrings[0] + ")");

        mmpgPrefs.hasNature = prefs & 1;
        mmpgPrefs.hasMonster = (prefs >> 1) & 1;
        mmpgPrefs.isRestricted = (prefs >> 2) & 1;

        Debug.Log("MMPG PREFS: hasNature " + mmpgPrefs.hasNature + " hasMonster " + mmpgPrefs.hasMonster + " isRestricted " + mmpgPrefs.isRestricted);

        askForNextAction(typeOfAction.MMPG_Prefs);
    }

    private void applyMmpgPreferences()
    {
        LocalSave.SetInt("nypb_hasNature", mmpgPrefs.hasNature);

        LocalSave.SetInt("nypb_hasMonster", mmpgPrefs.hasMonster);

        LocalSave.SetInt("nypb_hasRestrict", mmpgPrefs.isRestricted);

        LocalSave.Save();
    }

    private class MmpgProgression
    {
        public int nbOfClosed = 0;
        public int nbOfRanged = 0;
        public int nbOfMachines = 0;
        public int nbOfMonks = 0;
        public int nbOfSpies = 0;
        public int nbOfBombs = 0;

        public int isHero1 = 0;
        public int isHero2 = 0;
        public int isHero3 = 0;
        public int isHero4 = 0;
        public int isHero5 = 0;
        public int isHero6 = 0;

        public int maxLevel = 1;
    }

    MmpgProgression mmpgProgression;

    private void decodeMMPGProgression(string oneSaveCode)
    {
        mmpgProgression = new MmpgProgression();

        oneSaveCode = oneSaveCode.Substring(1);

        String[] subStrings = oneSaveCode.Split('.');

        if (subStrings.Length != 8)
        {
            Debug.Log("MMPG PROG: Not enough parts " + subStrings.Length + " (8 expected)");
        }

        //int heroes = (isHero1 ? 1 : 0) + ((isHero2 ? 1 : 0) << 2) + ((isHero3 ? 1 : 0) << 3)
        //   + ((isHero4 ? 1 : 0) << 4) + ((isHero5 ? 1 : 0) << 5) + ((isHero6 ? 1 : 0) << 6);

        //MMPG_Progression = ourEncoder.ConvertToBase(nbOfClosed) + "." + ourEncoder.ConvertToBase(nbOfRanged)
        //    + "." + ourEncoder.ConvertToBase(nbOfMachines) + "." + ourEncoder.ConvertToBase(nbOfMonks)
        //    + "." + ourEncoder.ConvertToBase(nbOfSpies) + "." + ourEncoder.ConvertToBase(nbOfBombs) + "." + ourEncoder.ConvertToBase(heroes);

        mmpgProgression.nbOfClosed = ourDecoder.ConvertFromBase(subStrings[0]);

        Debug.Log("MMPG PROG: nbOfClosed is " + mmpgProgression.nbOfClosed + " (" + subStrings[0] + ")");

        mmpgProgression.nbOfRanged = ourDecoder.ConvertFromBase(subStrings[1]);

        Debug.Log("MMPG PROG: nbOfRanged is " + mmpgProgression.nbOfRanged + " (" + subStrings[1] + ")");

        mmpgProgression.nbOfMachines = ourDecoder.ConvertFromBase(subStrings[2]);

        Debug.Log("MMPG PROG: nbOfMachines is " + mmpgProgression.nbOfMachines + " (" + subStrings[2] + ")");

        mmpgProgression.nbOfMonks = ourDecoder.ConvertFromBase(subStrings[3]);

        Debug.Log("MMPG PROG: nbOfMonks is " + mmpgProgression.nbOfMonks + " (" + subStrings[3] + ")");

        mmpgProgression.nbOfSpies = ourDecoder.ConvertFromBase(subStrings[4]);

        Debug.Log("MMPG PROG: nbOfSpies is " + mmpgProgression.nbOfSpies + " (" + subStrings[4] + ")");

        mmpgProgression.nbOfBombs = ourDecoder.ConvertFromBase(subStrings[5]);

        Debug.Log("MMPG PROG: nbOfBombs is " + mmpgProgression.nbOfBombs + " (" + subStrings[5] + ")");


        //int progression2 = levelReachedInLastMode + (maxGameMode << 8) + (currentGameMode << 16); // 0-11 now, reserved for more
        //int progression3 = nbOfPiecesFound;

        int heroes = ourDecoder.ConvertFromBase(subStrings[6]);

        mmpgProgression.isHero1 = heroes & 1;
        mmpgProgression.isHero2 = (heroes >> 1) & 1;
        mmpgProgression.isHero3 = (heroes >> 2) & 1;
        mmpgProgression.isHero4 = (heroes >> 3) & 1;
        mmpgProgression.isHero5 = (heroes >> 4) & 1;
        mmpgProgression.isHero6 = (heroes >> 5) & 1;

        mmpgProgression.maxLevel = ourDecoder.ConvertFromBase(subStrings[7]);

        Debug.Log("MMPG PROG: isHero1 " + mmpgProgression.isHero1 + " isHero2 " + mmpgProgression.isHero2 + " isHero3 " + mmpgProgression.isHero3 +
            " isHero4 " + mmpgProgression.isHero4 + " isHero5 " + mmpgProgression.isHero5 + " isHero6 " + mmpgProgression.isHero6+" maxLevel "+ mmpgProgression.maxLevel);

        askForNextAction(typeOfAction.MMPG_Progs);
    }

    private void applyMmpgProgression()
    {
        LocalSave.SetInt("nypb_unitClosePlayer", mmpgProgression.nbOfClosed);

        LocalSave.SetInt("nypb_unitRangedPlayer", mmpgProgression.nbOfRanged);

        LocalSave.SetInt("nypb_unitTanksPlayer", mmpgProgression.nbOfMachines);

        LocalSave.SetInt("nypb_bombsPlayer", mmpgProgression.nbOfBombs);

        LocalSave.SetInt("nypb_unitMonkPlayer", mmpgProgression.nbOfMonks);

        LocalSave.SetInt("nypb_unitSpyPlayer", mmpgProgression.nbOfSpies);

        LocalSave.SetBool("nypb_unitHero1", mmpgProgression.isHero1 == 1);
        LocalSave.SetBool("nypb_unitHero2", mmpgProgression.isHero2 == 1);
        LocalSave.SetBool("nypb_unitHero3", mmpgProgression.isHero3 == 1);
        LocalSave.SetBool("nypb_unitHero4", mmpgProgression.isHero4 == 1);
        LocalSave.SetBool("nypb_unitHero5", mmpgProgression.isHero5 == 1);
        LocalSave.SetBool("nypb_unitHero6", mmpgProgression.isHero6 == 1);

        LocalSave.SetInt("nypb_levelReached", mmpgProgression.maxLevel);    

        LocalSave.Save();
    }

    float mmpgTime;

    private void decodeMMPGTime(string oneSaveCode)
    {
        oneSaveCode = oneSaveCode.Substring(1);

        String[] subStrings = oneSaveCode.Split('.');

        if (subStrings.Length != 1)
        {
            Debug.Log("MMPG TIME: Not enough parts " + subStrings.Length + " (1 expected)");
        }

        int allTimeSpent = ourDecoder.ConvertFromBase(subStrings[0]);

        mmpgTime = (float)allTimeSpent;

        Debug.Log("MMPG TIME: allTimeSpent is " + allTimeSpent + " (" + subStrings[0] + ")");

        askForNextAction(typeOfAction.MMPG_TimeScore);
    }

    private void applyMmpgTime()
    {
        LocalSave.SetFloat("nypb_allTimeSpent", mmpgTime);

        LocalSave.Save();
    }

    float mmpgBestTime;

    private void decodeMMPGBestTime(string oneSaveCode)
    {
        oneSaveCode = oneSaveCode.Substring(1);

        String[] subStrings = oneSaveCode.Split('.');

        if (subStrings.Length != 1)
        {
            Debug.Log("MMPG TIME: Not enough parts " + subStrings.Length + " (1 expected)");
        }

        int bestTime = ourDecoder.ConvertFromBase(subStrings[0]);

        mmpgBestTime = (float)bestTime;

        Debug.Log("MMPG TIME: allTimeSpent is " + bestTime + " (" + subStrings[0] + ")");

        askForNextAction(typeOfAction.MMPG_BestTime);
    }

    private void applyMmpgBestTime()
    {
        LocalSave.SetFloat("nypb_bestTime", mmpgBestTime);

        LocalSave.Save();
    }

    private class BJSPrefs
    {
        public int CurrentJohn = 0;
        public int CurrentBellForLevel = 0;
    }

    BJSPrefs bJSPrefs;

    private void decodeBJSPrefs(string oneSaveCode)
    {
        bJSPrefs = new BJSPrefs();

        oneSaveCode = oneSaveCode.Substring(1);

        String[] subStrings = oneSaveCode.Split('.');

        if (subStrings.Length != 2)
        {
            Debug.Log("BJS PREFS: Not enough parts " + subStrings.Length + " (1 expected)");
        }

        // Debug.Log("BJS PREFS: CurrentJohn " + ourBJSSave.CurrentJohn + " CurrentBellForLevel " + ourBJSSave.CurrentBellForLevel );

        // BJS_Prefs = ourEncoder.ConvertToBase(ourBJSSave.CurrentJohn) + "." + ourEncoder.ConvertToBase(ourBJSSave.CurrentBellForLevel);

        bJSPrefs.CurrentJohn = ourDecoder.ConvertFromBase(subStrings[0]);

        bJSPrefs.CurrentBellForLevel = ourDecoder.ConvertFromBase(subStrings[1]);

        Debug.Log("CurrentJohn is " + bJSPrefs.CurrentJohn + "(" + subStrings[0] + ") CurrentBellForLevel is " + bJSPrefs.CurrentBellForLevel + " (" + subStrings[1] + ")");

        askForNextAction(typeOfAction.BJS_Prefs);
    }

    private void applyBJSPrefs()
    {
        LocalSave.SetInt("BJS_CurrentJohn", bJSPrefs.CurrentJohn);

        LocalSave.SetInt("BJS_CurrentBellForLevel", bJSPrefs.CurrentBellForLevel);

        LocalSave.Save();
    }

    private class BJSProgression
    {
        public int MaxJohn;
        internal int CurrentDay;
        internal int XpJohn;
        internal int LevelJohn;
        internal int XpNeeded;
        internal int MaxBellForLevel;
    }

    BJSProgression bJSProg;

    private void decodeBJSProgression(string oneSaveCode)
    {
        bJSProg = new BJSProgression();

        oneSaveCode = oneSaveCode.Substring(1);

        String[] subStrings = oneSaveCode.Split('.');

        if (subStrings.Length != 7)
        {
            Debug.Log("BJS PROG: Not enough parts " + subStrings.Length + " (6 expected)");
        }

        //Debug.Log("BJS PROG: CurrentDay " + ourBJSSave.CurrentDay + " xpJohn " + ourBJSSave.XpJohn
        //    + " levelJohn " + ourBJSSave.LevelJohn + " xpNeeded " + ourBJSSave.XpNeeded
        //    + " maxBellForLevel " + ourBJSSave.MaxBellForLevel + " maxJohn " + ourBJSSave.MaxJohn);

        //BJS_Progression = ourEncoder.ConvertToBase(ourBJSSave.CurrentDay) + "." + ourEncoder.ConvertToBase((int)ourBJSSave.XpJohn)
        //    + "." + ourEncoder.ConvertToBase(ourBJSSave.LevelJohn) + "." + ourEncoder.ConvertToBase((int)ourBJSSave.XpNeeded)
        //    + "." + ourEncoder.ConvertToBase(ourBJSSave.MaxBellForLevel) + "." + ourEncoder.ConvertToBase(ourBJSSave.MaxJohn);

        bJSProg.CurrentDay = ourDecoder.ConvertFromBase(subStrings[0]);

        Debug.Log("BJS PROG: CurrentDay is " + bJSProg.CurrentDay + " (" + subStrings[0] + ")");

        bJSProg.XpJohn = ourDecoder.ConvertFromBase(subStrings[1]);

        Debug.Log("BJS PROG: XpJohn is " + bJSProg.XpJohn + " (" + subStrings[1] + ")");

        bJSProg.LevelJohn = ourDecoder.ConvertFromBase(subStrings[2]);

        Debug.Log("BJS PROG: LevelJohn is " + bJSProg.LevelJohn + " (" + subStrings[2] + ")");

        bJSProg.XpNeeded = ourDecoder.ConvertFromBase(subStrings[3]);

        Debug.Log("BJS PROG: XpNeeded is " + bJSProg.XpNeeded + " (" + subStrings[3] + ")");

        bJSProg.MaxBellForLevel = ourDecoder.ConvertFromBase(subStrings[4]);

        Debug.Log("BJS PROG: MaxBellForLevel is " + bJSProg.MaxBellForLevel + " (" + subStrings[4] + ")");

        bJSProg.MaxJohn = ourDecoder.ConvertFromBase(subStrings[5]);

        Debug.Log("BJS PROG: MaxJohn is " + bJSProg.MaxJohn + " (" + subStrings[5] + ")");

        askForNextAction(typeOfAction.BJS_Progs);
    }

    private void applyBJSProgression()
    {
        LocalSave.SetInt("BJS_JohnLevel", bJSProg.LevelJohn);

        LocalSave.SetInt("BJS_XPJohn", bJSProg.XpJohn);

        LocalSave.SetInt("BJS_XPNeeded", bJSProg.XpNeeded);

        LocalSave.SetInt("BJS_MaxJohn", bJSProg.MaxJohn);

        LocalSave.SetInt("BJS_CurrentDay", bJSProg.CurrentDay);

        LocalSave.SetInt("BJS_MaxBellForLevel", bJSProg.MaxBellForLevel);

        LocalSave.Save();
    }

    private class BJSTime
    {
        public float allTimeSpent;
        public float lastTimeSpent;
    }

    BJSTime bjsTime;

    private void decodeBJSTime(string oneSaveCode)
    {
        bjsTime = new BJSTime();

        oneSaveCode = oneSaveCode.Substring(1);

        String[] subStrings = oneSaveCode.Split('.');

        if (subStrings.Length != 2)
        {
            Debug.Log("BJS TIME: Not enough parts " + subStrings.Length + " (2 expected)");
        }

        bjsTime.allTimeSpent = ourDecoder.ConvertFromBase(subStrings[0]);

        bjsTime.lastTimeSpent = ourDecoder.ConvertFromBase(subStrings[1]);

        Debug.Log("BJS TIME: allTimeSpent is " + bjsTime.allTimeSpent + " (" + subStrings[0] + ") lastTimeSpent is " + bjsTime.lastTimeSpent + " (" + subStrings[1] + ")");

        askForNextAction(typeOfAction.BJS_TimeScore);
    }

    private void applyBJSTime()
    {
        LocalSave.SetFloat("BJS_allTimeSpent", bjsTime.allTimeSpent);

        LocalSave.SetFloat("BJS_currentTimeSpent", bjsTime.lastTimeSpent);

        LocalSave.Save();
    }

    float bjsBestTime;

    private void decodeBJSBestTime(string oneSaveCode)
    {
        bjsBestTime = 0;

        oneSaveCode = oneSaveCode.Substring(1);

        String[] subStrings = oneSaveCode.Split('.');

        if (subStrings.Length != 1)
        {
            Debug.Log("BJS TIME: Not enough parts " + subStrings.Length + " (1 expected)");
        }

        bjsBestTime = ourDecoder.ConvertFromBase(subStrings[0]);

        Debug.Log("BJS BEST TIME: bestTimeSpent is " + bjsBestTime + " (" + subStrings[0] + ")");

        askForNextAction(typeOfAction.BJS_BestTimeScore);
    }

    private void applyBJSBestTime()
    {
        LocalSave.SetFloat("BJS_bestTimeSpent", bjsBestTime);

        LocalSave.Save();
    }

    // We store the asked list of actions, so multiple line can be parsed.
    // That is because the UI action is non blocking for the script, so asynchronous
    List<typeOfAction> pendingListOfAction = new List<typeOfAction>();

    private void askForNextAction(typeOfAction nextAction)
    {
        pendingListOfAction.Add(nextAction);

        Debug.Log("=== Added " + nextAction + " for pending confirmation ("+ pendingListOfAction.Count + ").");

        if (pendingListOfAction.Count == 1)
        {
            Debug.Log("=== First time: Calling  WarningWindows.");
            callWarningWindows(nextAction);
        }
    }

    private void callWarningWindows(typeOfAction typeOfAction)
    { 
        String textToDisplay = "Do you really want to restore ";
        String typeOfActionText = "";

        switch (typeOfAction)
        {
            case typeOfAction.NYAF_Prefs:
                textToDisplay = textToDisplay + " the preferences for NYAF? NYAF will need to restart.";
                typeOfActionText = "preferences.";
                break;

            case typeOfAction.NYAF_Progs:
                textToDisplay = textToDisplay + " the progression for NYAF? NYAF will need to restart.";
                typeOfActionText = "progression.";
                break;

            case typeOfAction.NYAF_TimeScore:
                textToDisplay = textToDisplay + " the time spent and the scores for NYAF? NYAF will need to restart.";
                typeOfActionText = "time spent and scores.";
                break;

            case typeOfAction.NYAF_BestTimeScore:
                textToDisplay = textToDisplay + " the best times and scores for NYAF?";
                typeOfActionText = "best times and scores.";
                break;

            case typeOfAction.NYAF_DogsAndCoins:
                textToDisplay = textToDisplay + " the total of coins earned in NYAF? NYAF will need to restart.";
                typeOfActionText = "total of coins earned and the dogs will need to be bought again.";
                break;

            case typeOfAction.YANYAF_Prefs:
                textToDisplay = textToDisplay + " the preferences for YANYAF?";
                typeOfActionText = "preferences.";
                break;

            case typeOfAction.YANYAF_Progs:
                textToDisplay = textToDisplay + " the progression for YANYAF?";
                typeOfActionText = "progression.";
                break;

            case typeOfAction.YANYAF_TimeScore:
                textToDisplay = textToDisplay + " the time spent for YANYAF?";
                typeOfActionText = "time spent.";
                break;

            case typeOfAction.YANYAF_BestTimeScore:
                textToDisplay = textToDisplay + " the best times for YANYAF?";
                typeOfActionText = "best times.";
                break;

            case typeOfAction.BJS_Prefs:
                textToDisplay = textToDisplay + " the preferences for BJS?";
                typeOfActionText = "preferences.";
                break;

            case typeOfAction.BJS_Progs:
                textToDisplay = textToDisplay + " the progression for BJS?";
                typeOfActionText = "progression.";
                break;

            case typeOfAction.BJS_TimeScore:
                textToDisplay = textToDisplay + " the time spent for BJS?";
                typeOfActionText = "time spent.";
                break;

            case typeOfAction.BJS_BestTimeScore:
                textToDisplay = textToDisplay + " the best time for BJS?";
                typeOfActionText = "best time.";
                break;

            case typeOfAction.MMPG_Prefs:
                textToDisplay = textToDisplay + " the preferences for MMPG?";
                typeOfActionText = "progression.";
                break;

            case typeOfAction.MMPG_Progs:
                textToDisplay = textToDisplay + " the progression for MMPG?";
                typeOfActionText = "preferences.";
                break;

            case typeOfAction.MMPG_TimeScore:
                textToDisplay = textToDisplay + " the time spent for MMPG?";
                typeOfActionText = "time spent.";
                break;

            case typeOfAction.MMPG_BestTime:
                textToDisplay = textToDisplay + " the best time for MMPG?";
                typeOfActionText = "best time.";
                break;
        }

        textToDisplay = textToDisplay + " It will replace your current " + typeOfActionText;

        warningText.text = textToDisplay;
        warningWindows.SetActive(true);
    }

    // For nyaf changes, we need to restart it so that they are taken into account
    bool needToRestartNyaf = false;

    public void confirmLastAction()
    {
        // The OK button of the Warning windows was pressed, so we can continue the current restore.
        nextActionAsked = pendingListOfAction[0];
        pendingListOfAction.RemoveAt(0);

        Debug.Log("=== Confirm last action "+nextActionAsked);

        switch (nextActionAsked)
        {
            case typeOfAction.NYAF_Prefs:
                applyNyafPreferences();
                needToRestartNyaf = true; // Restart after last action
                break;

            case typeOfAction.NYAF_Progs:
                applyNyafProgression();
                needToRestartNyaf = true; // Restart after last action
                break;

            case typeOfAction.NYAF_DogsAndCoins:
                applyNyafStoreCoins();
                needToRestartNyaf = true; // Restart after last action
                break;

            case typeOfAction.NYAF_TimeScore:
                applyNyafTimeScore();
                needToRestartNyaf = true; // Restart after last action
                break;

            case typeOfAction.NYAF_BestTimeScore:
                applyBestNyafTimeScore();
                break;

            case typeOfAction.YANYAF_Prefs:
                applyYanyafPreferences();
                break;

            case typeOfAction.YANYAF_Progs:
                applyYayafProgression();
                break;

            case typeOfAction.YANYAF_TimeScore:
                applyYanyafTimeScore();
                break;

            case typeOfAction.YANYAF_BestTimeScore:
                applyYanyafBestTimeScore();
                break;

            case typeOfAction.BJS_Prefs:
                applyBJSPrefs();
                break;

            case typeOfAction.BJS_Progs:
                applyBJSProgression();
                break;

            case typeOfAction.BJS_TimeScore:
                applyBJSTime();
                break;

            case typeOfAction.BJS_BestTimeScore:
                applyBJSBestTime();
                break;

            case typeOfAction.MMPG_Prefs:
                applyMmpgPreferences();
                break;

            case typeOfAction.MMPG_Progs:
                applyMmpgProgression();
                break;

            case typeOfAction.MMPG_TimeScore:
                applyMmpgTime();
                break;

            case typeOfAction.MMPG_BestTime:
                applyMmpgBestTime();
                break;
        }

        // And check if we need to do another check
        checkIfActionLeft();
    }

    public void cancelLastAction()
    {
        // The OK button of the Warning windows was pressed, so we can continue the current restore.
        pendingListOfAction.RemoveAt(0);

        checkIfActionLeft();
    }

    private void checkIfActionLeft()
    {
        Debug.Log("=== Check if action left " + pendingListOfAction.Count);

        if (pendingListOfAction.Count > 0)
        {
            nextActionAsked = pendingListOfAction[0];

            Debug.Log("=== Check if action calling windows with " + nextActionAsked);

            callWarningWindows(nextActionAsked);
        }
        else
        {
            if (needToRestartNyaf)
            {
                LaunchNYAF.loadNYAFGame();
            }
            warningWindows.SetActive(false);
        }
    }
}
