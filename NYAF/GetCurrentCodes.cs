using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GetCurrentCodes : MonoBehaviour
{
    // 3 for each game + 3 reserved
    public const string nyafPrefsID = "A";
    public const string nyafProgressionID = "B";
    public const string nyafScoreAndTimeID = "C";
    public const string nyafBestScoreAndTimeID = "D";
    public const string nyafDogsAndCoinsID = "E";
    public const string yanyafPrefsID = "H";
    public const string yanyafProgressionID = "I";
    public const string yanyafScoreAndTimeID = "J";
    public const string yanyafBestScoreAndTimeID = "K";
    public const string bjsPrefsID = "N";
    public const string bjsProgressionID = "O";
    public const string bjsScoreAndTimeID = "P";
    public const string bjsBestScoreAndTimeID = "Q";
    public const string mmpgPrefsID = "T";
    public const string mmpgProgressionID = "U";
    public const string mmpgScoreAndTimeID = "V";
    public const string mmpgBestScoreAndTimeID = "W";
   

    // Code the current values in a string
    string nyafPrefs;
    string nyafProgression;
    string nyafScoreAndTime;
    string nyafBestScoreAndTime;
    string nyafDogsCoins;

    CodeEncodeToString ourEncoder = new CodeEncodeToString();

    public string NyafPrefs { get => nyafPrefs; }
    public string NyafProgression { get => nyafProgression; }
    public string NyafScoreAndTime { get => nyafScoreAndTime; }
    public string NyafBestScoreAndTime { get => nyafBestScoreAndTime; }

    public Text ourText = null;
    public InputField ourInputField = null;

    void getNyafCodes()
    {
        int currentDifficulty = 0;
        int pieceTransparences = 1; // 1 -> On, 0 -> Off
        int showHints = 1;
        int showArrows = 1;
        int maxGameMode = 0;
        int levelReachedInLastMode = 0;
        int allSmallFoundFlag = 0;
        int nbOfPiecesFound = 0;
        float[] lastTimeSpent = new float[5];
        float currentTimeAllocated = 0;
        float[] bestTimeSpent = new float[5];
        float bestScoreMode3 = 100000;

        int currentGameMode = 0;

        // 0 to 7
        bool haskey = LocalSave.HasIntKey("difficulty");
        if (haskey)
        {
            currentDifficulty = LocalSave.GetInt("difficulty");
        }

        haskey = LocalSave.HasIntKey("transparentChars");
        if (haskey)
        {
            pieceTransparences = LocalSave.GetInt("transparentChars");

            //Debug.Log("Transparence: " + pieceTransparences + " - " + pieceTransparencesToggle.isOn);
        }

        haskey = LocalSave.HasIntKey("showHints");
        if (haskey)
        {
            showHints = LocalSave.GetInt("showHints");
        }

        haskey = LocalSave.HasIntKey("showArrows");
        if (haskey)
        {
            showArrows = LocalSave.GetInt("showArrows");
        }

        // Check how far we are
        haskey = LocalSave.HasIntKey("modeFinished");
        if (haskey)
        {
            maxGameMode = LocalSave.GetInt("modeFinished");
        }

        haskey = LocalSave.HasIntKey("levelReached" + maxGameMode);
        
        if (haskey)
        {
            levelReachedInLastMode = LocalSave.GetInt("levelReached" + maxGameMode);
        }

        haskey = LocalSave.HasIntKey("smallFound");
        
        if (haskey)
        {
            allSmallFoundFlag = LocalSave.GetInt("smallFound");
        }

        haskey = LocalSave.HasIntKey("nbOfPiecesFound");

        if (haskey)
        {
            nbOfPiecesFound = LocalSave.GetInt("nbOfPiecesFound");
        }

        for (int iGameMode= 0; iGameMode < 5; iGameMode++)
        {
            haskey = LocalSave.HasFloatKey("allTimeSpent" + iGameMode);

            if (haskey)
            {
                lastTimeSpent[iGameMode] = LocalSave.GetFloat("allTimeSpent" + iGameMode);
            }
            else
            {
                lastTimeSpent[iGameMode] = 0;
            }

            haskey = LocalSave.HasFloatKey("bestTimeSpent" + iGameMode);

            if (haskey)
            {
                bestTimeSpent[iGameMode] = LocalSave.GetFloat("bestTimeSpent" + iGameMode);
            }
            else
            {
                bestTimeSpent[iGameMode] = 1000000000;
            }
        }
        haskey = LocalSave.HasFloatKey("currentTimeAllocated");

        if (haskey)
        {
            currentTimeAllocated = LocalSave.GetFloat("currentTimeAllocated");
        }

        haskey = LocalSave.HasFloatKey("bestScoreMode3");

        if (haskey)
        {
            bestScoreMode3 = LocalSave.GetFloat("bestScoreMode3");
        }

        // And which mode we are in
        haskey = LocalSave.HasIntKey("currentMode");
        if (haskey)
        {
            currentGameMode = LocalSave.GetInt("currentMode");
        }

        int preferences = currentDifficulty  // 0-7
            + (pieceTransparences << 4) // 0-1
            + (showHints << 5)   // 0-1
            + (showArrows << 6); // 0-1

        Debug.Log("NYAF PREFS: currentDifficulty " + currentDifficulty + " pieceTransparences " + pieceTransparences
            + " showHints " + showHints + " showArrows " + showArrows );

        int progression2 = levelReachedInLastMode + (maxGameMode<<8) + (currentGameMode << 16); // 0-11 now, reserved for more
        int progression3 = nbOfPiecesFound;

        int[] timeInt = new int[5];
        string timeInEachMode = "";
        bool first = true;

        for (int iMode = 0; iMode < 5; iMode++)
        {
            timeInt[iMode] = (int )lastTimeSpent[iMode];
            timeInEachMode+= (first?"":".")+ourEncoder.ConvertToBase(timeInt[iMode]);
            first = false;

            Debug.Log("NYAF TIME: time in mode [" + iMode + "] is " + timeInt[iMode] + " (" + timeInEachMode + ")");
        }

        long currentTimeMode3 = 10000000 + (long ) currentTimeAllocated; // currentTimeAllocated can be negative!

        nyafPrefs = ourEncoder.ConvertToBase(preferences);
        int returnedValue = ourEncoder.ConvertFromBase(nyafPrefs);

        Debug.Log("NYAF PREFS: Preference int should be " + preferences + " = "+ returnedValue+" ("+ nyafPrefs+")");
        nyafProgression = ourEncoder.ConvertToBase(allSmallFoundFlag) + "." +ourEncoder.ConvertToBase(progression2) + "." + ourEncoder.ConvertToBase(progression3);
        Debug.Log("NYAF PROG: levelReachedInLastMode  " + levelReachedInLastMode + ", maxGameMode " + maxGameMode + ", currentGameMode " + currentGameMode);
        Debug.Log("NYAF PROG: allSmallFoundFlag int should be " + allSmallFoundFlag + ", progression2 int should be " + progression2 + ", progression3 int should be " + progression3);
        nyafScoreAndTime = timeInEachMode + "." + ourEncoder.ConvertToBase(currentTimeMode3);


        int[] bestTimeInt = new int[5];
        string bestTimeInEachMode = "";
        bool firstBestTime = true;

        for (int iMode = 0; iMode < 5; iMode++)
        {
            bestTimeInt[iMode] = (int)bestTimeSpent[iMode];
            bestTimeInEachMode += (firstBestTime ? "" : ".") + ourEncoder.ConvertToBase(bestTimeInt[iMode]);
            firstBestTime = false;

            Debug.Log("NYAF BEST TIME: time in mode [" + iMode + "] is " + bestTimeInt[iMode] + " (" + bestTimeInEachMode + ")");
        }

        long currentBestTimeMode3 = 10000000 + (long)bestScoreMode3; // currentTimeAllocated can be negative!

        nyafBestScoreAndTime = bestTimeInEachMode + "." + ourEncoder.ConvertToBase(currentBestTimeMode3);

        int totalCoinsWon = 0;

        haskey = LocalSave.HasIntKey("TotalCoinsWon");

        if (haskey)
        {
            totalCoinsWon = LocalSave.GetInt("TotalCoinsWon");
        }

        nyafDogsCoins = ourEncoder.ConvertToBase(totalCoinsWon);
    }

    void OnEnable()
    {
        showNyaf();
    }

    public void showNyaf()
    {
        getNyafCodes();

        string nyafString = "    NYAF\n Preferences: *" + nyafPrefsID + nyafPrefs
                + "\n Progression: *" + nyafProgressionID + nyafProgression
                + "\n Scores and Times: *" + nyafScoreAndTimeID + nyafScoreAndTime
                + "\n Best Scores and Times: *" + nyafBestScoreAndTimeID + nyafBestScoreAndTime
                + "\n All coins earned: *" + nyafDogsAndCoinsID + nyafDogsCoins;

        //nyafString += " ***" + CompressString.StringCompressor.CompressString(LocalSave.returnAllAsString())+ "***";
        if (ourText != null)
        {
            ourText.text = nyafString;
        }
        if (ourInputField != null)
        {
            ourInputField.SetTextWithoutNotify(nyafString);
        }
    }

    public void showBJS()
    {
        bool BJSUnlocked = true;
        string bjsString = "";

        if (BJSUnlocked)
        {
            CreateBJSCodes ourCreateBJSCode = new CreateBJSCodes();

            ourCreateBJSCode.getBJSCodes();

            bjsString = "    BJS\n Preferences: *" + bjsPrefsID + ourCreateBJSCode.BJSPrefs
                + "\n Progression: *" + bjsProgressionID + ourCreateBJSCode.BJSProgression
                + "\n Scores and Times: *" + bjsScoreAndTimeID + ourCreateBJSCode.BJSScoreAndTime
                + "\n Best Time: *" + bjsBestScoreAndTimeID + ourCreateBJSCode.BJSBestTime;
        }

        if (ourText != null)
        {
            ourText.text = bjsString;
        }
        if (ourInputField != null)
        {
            ourInputField.SetTextWithoutNotify(bjsString);
        }
    }

    public void showYANYAF()
    {
        bool YANYAFUnlocked = true;
        string yanyafString = "";

        if (YANYAFUnlocked)
        {
            CreateYANYAFCodes ourCreateYANYAFCode = new CreateYANYAFCodes();

            ourCreateYANYAFCode.getYanyafCodes();

            yanyafString = "    YANYAF\n Preferences: *" + yanyafPrefsID + ourCreateYANYAFCode.YanyafPrefs
                + "\n Progression: *" + yanyafProgressionID + ourCreateYANYAFCode.YanyafProgression
                + "\n Scores and Times: *" + yanyafScoreAndTimeID + ourCreateYANYAFCode.YanyafScoreAndTime
                + "\n Best Time: *" + yanyafBestScoreAndTimeID + ourCreateYANYAFCode.YanyafBestTime;
        }

        if (ourText != null)
        {
            ourText.text = yanyafString;
        }
        if (ourInputField != null)
        {
            ourInputField.SetTextWithoutNotify(yanyafString);
        }
    }

    public void showMMPG()
    {
        bool MMPGUnlocked = true;
        string mmpgString = "";

        if (MMPGUnlocked)
        {
            CreateMMPGCodes ourCreateMMPGCode = new CreateMMPGCodes();

            ourCreateMMPGCode.getMMPGCodes();

            mmpgString = "    MMPG\n Preferences: *" + mmpgPrefsID + ourCreateMMPGCode.MMPGPrefs
                + "\n Progression: *" + mmpgProgressionID + ourCreateMMPGCode.MMPGProgression
                + "\n Scores and Times: *" + mmpgScoreAndTimeID + ourCreateMMPGCode.MMPGScoreAndTime
                + "\n Best Time: *" + mmpgBestScoreAndTimeID + ourCreateMMPGCode.MMPGBestTime;
        }

        if (ourText != null)
        {
            ourText.text = mmpgString;
        }
        if (ourInputField != null)
        {
            ourInputField.SetTextWithoutNotify(mmpgString);
        }
    }
}
