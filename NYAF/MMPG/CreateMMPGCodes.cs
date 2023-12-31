using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateMMPGCodes
{
    // Code the current values in a string
    string MMPG_Prefs;
    string MMPG_Progression;
    string MMPG_ScoreAndTime;
    private string MMPG_BestTime;

    CodeEncodeToString ourEncoder = new CodeEncodeToString();
    

    public string MMPGPrefs { get => MMPG_Prefs; }
    public string MMPGProgression { get => MMPG_Progression; }
    public string MMPGScoreAndTime { get => MMPG_ScoreAndTime; }
    public string MMPGBestTime { get => MMPG_BestTime; }

    public void getMMPGCodes()
    {
        int hasNature = 1;
        int hasMonster = 1;
        int isRestricted = 1;

        bool isHero1 = true;
        bool isHero2 = true;
        bool isHero3 = true;
        bool isHero4 = true;
        bool isHero5 = true;
        bool isHero6 = true;

        int nbOfClosed = 0;
        int nbOfRanged = 0;
        int nbOfMachines = 0;
        int nbOfMonks = 0;
        int nbOfSpies = 0;
        int nbOfBombs = 0;

        int maxLevel = 1;

        bool haskey = LocalSave.HasIntKey("nypb_levelReached");
        maxLevel = 1;
        if (haskey)
        {
            maxLevel = LocalSave.GetInt("nypb_levelReached");
        }

        haskey = LocalSave.HasIntKey("nypb_hasNature");

        if (haskey)
        {
            hasNature = LocalSave.GetInt("nypb_hasNature");
        }

        haskey = LocalSave.HasIntKey("nypb_hasMonster");

        if (haskey)
        {
            hasMonster = LocalSave.GetInt("nypb_hasMonster");
        }

        haskey = LocalSave.HasIntKey("nypb_hasRestrict");

        if (haskey)
        {
            isRestricted = LocalSave.GetInt("nypb_hasRestrict");
        }

        MMPGAddNewPlayerUnits.AllUnitsPlayer playersUnit = MMPGAddNewPlayerUnits.getNbOfUnits();
        nbOfClosed = playersUnit.nbOfCloseUnit;
        nbOfRanged = playersUnit.nbOfRangedUnit;
        nbOfMachines = playersUnit.nbOfMachineUnit;
        nbOfMonks = playersUnit.nbOfMonkUnit;
        nbOfSpies = playersUnit.nbOfSpyUnit;
        nbOfBombs = playersUnit.nbOfBomb;

        isHero1 = playersUnit.isHero1;
        isHero2 = playersUnit.isHero2;
        isHero3 = playersUnit.isHero3;
        isHero4 = playersUnit.isHero4;
        isHero5 = playersUnit.isHero5;
        isHero6 = playersUnit.isHero6;

        Debug.Log("MMPG PREFS: hasNature " + hasNature + " hasMonster " + hasMonster + " isRestricted " + isRestricted);

        int prefs = hasNature + (hasMonster << 1) + (isRestricted << 2);

        MMPG_Prefs = ourEncoder.ConvertToBase(prefs);

        Debug.Log("MMPG PROG: nbOfClosed " + nbOfClosed + " nbOfRanged " + nbOfRanged
            + " nbOfMachines " + nbOfMachines + " nbOfMonks " + nbOfMonks
            + " nbOfSpies " + nbOfSpies + " nbOfBombs " + nbOfBombs+ " heroes "+ isHero1 + isHero2 + isHero3 + isHero4 + isHero5 + isHero6);

        int heroes = (isHero1 ? 1 : 0) + ((isHero2 ? 1 : 0) << 2) + ((isHero3 ? 1 : 0) << 3)
            + ((isHero4 ? 1 : 0) << 4) + ((isHero5 ? 1 : 0) << 5) + ((isHero6 ? 1 : 0) << 6);

        MMPG_Progression = ourEncoder.ConvertToBase(nbOfClosed) + "." + ourEncoder.ConvertToBase(nbOfRanged)
            + "." + ourEncoder.ConvertToBase(nbOfMachines) + "." + ourEncoder.ConvertToBase(nbOfMonks)
            + "." + ourEncoder.ConvertToBase(nbOfSpies) + "." + ourEncoder.ConvertToBase(nbOfBombs) + "." + ourEncoder.ConvertToBase(heroes)+"."+ ourEncoder.ConvertToBase(maxLevel);

        float allTimeSpent = 0;

        haskey = LocalSave.HasFloatKey("nypb_allTimeSpent");

        if (haskey)
        {
            allTimeSpent = LocalSave.GetFloat("nypb_allTimeSpent");
        }

        Debug.Log("MMPG TIME: allTimeSpent " + allTimeSpent);
        MMPG_ScoreAndTime = ourEncoder.ConvertToBase((int)allTimeSpent);

        float bestTimeSpent = 1000000000;

        haskey = LocalSave.HasFloatKey("nypb_bestTime");

        if (haskey)
        {
            bestTimeSpent = LocalSave.GetFloat("nypb_bestTime");
        }

        Debug.Log("MMPG BEST TIME: bestTimeSpent " + bestTimeSpent);
        MMPG_BestTime = ourEncoder.ConvertToBase((int)bestTimeSpent);
    }
}
