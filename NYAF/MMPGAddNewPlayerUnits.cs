using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MMPGAddNewPlayerUnits : MonoBehaviour
{
    /**
     * For all modes excepted 1
     */
    public static void nyafLevelEnded(int level, int mode, ShowResults theCurrentShowResults)
    {
        int nbOfCloseUnitsToAdd = level * 5;
        int nbOfRangedUnitsToAdd = (level - 1)* 2;

        if (mode == 2)
        {
            nbOfCloseUnitsToAdd = level * 10;
            nbOfRangedUnitsToAdd = (level - 1) * 4;
        }

        if (nbOfCloseUnitsToAdd < 0)
        {
            nbOfCloseUnitsToAdd = 0;
        }
        if (nbOfRangedUnitsToAdd < 0)
        {
            nbOfRangedUnitsToAdd = 0;
        }

        int currentNbOfCloseUnits = 0;

        bool haskey = LocalSave.HasIntKey("nypb_unitClosePlayer");
        if (haskey)
        {
            currentNbOfCloseUnits = LocalSave.GetInt("nypb_unitClosePlayer");
        }
        currentNbOfCloseUnits += nbOfCloseUnitsToAdd;
        LocalSave.SetInt("nypb_unitClosePlayer", currentNbOfCloseUnits);

        int currentNbOfRangedUnits = 0;

        haskey = LocalSave.HasIntKey("nypb_unitRangedPlayer");
        if (haskey)
        {
            currentNbOfRangedUnits = LocalSave.GetInt("nypb_unitRangedPlayer");
        }
        currentNbOfRangedUnits += nbOfRangedUnitsToAdd;
        LocalSave.SetInt("nypb_unitRangedPlayer", currentNbOfRangedUnits);

        LocalSave.Save();

        theCurrentShowResults.setNbOfEndLevelUnits(nbOfCloseUnitsToAdd, nbOfRangedUnitsToAdd, 0, 0, 0);
    }

    public static void nyafModeFinished(ShowResults theCurrentShowResults, bool inMode1)
    {
        int mode1Multiplier = inMode1 ? 4 : 1;
        int nbOfCloseUnitsToAdd = 200 * mode1Multiplier;
        int nbOfRangedUnitsToAdd = 100 * mode1Multiplier;
        int nbOfTanksToAdd = 20 * mode1Multiplier;

        int currentNbOfCloseUnits = 0;

        bool haskey = LocalSave.HasIntKey("nypb_unitClosePlayer");
        if (haskey)
        {
            currentNbOfCloseUnits = LocalSave.GetInt("nypb_unitClosePlayer");
        }
        currentNbOfCloseUnits += nbOfCloseUnitsToAdd;
        LocalSave.SetInt("nypb_unitClosePlayer", currentNbOfCloseUnits);

        int currentNbOfRangedUnits = 0;

        haskey = LocalSave.HasIntKey("nypb_unitRangedPlayer");
        if (haskey)
        {
            currentNbOfRangedUnits = LocalSave.GetInt("nypb_unitRangedPlayer");
        }
        currentNbOfRangedUnits += nbOfRangedUnitsToAdd;
        LocalSave.SetInt("nypb_unitRangedPlayer", currentNbOfRangedUnits);

        int currentNbOfTanks = 0;

        haskey = LocalSave.HasIntKey("nypb_unitTanksPlayer");
        if (haskey)
        {
            currentNbOfTanks = LocalSave.GetInt("nypb_unitTanksPlayer");
        }
        currentNbOfTanks += nbOfTanksToAdd;
        LocalSave.SetInt("nypb_unitTanksPlayer", currentNbOfTanks);

        LocalSave.Save();

        theCurrentShowResults.setNbOfEndLevelUnits(nbOfCloseUnitsToAdd, nbOfRangedUnitsToAdd, nbOfTanksToAdd, 0, 0);
    }

    public static void nyafBombExploded(int nbOfStepsBack, ShowResults theCurrentShowResults)
    {
        int nbOfBombsToAdd = nbOfStepsBack/10;
        if (nbOfBombsToAdd <= 0)
        {
            nbOfBombsToAdd = 1;
        }

        int currentNbOfBombs = 0;

        bool haskey = LocalSave.HasIntKey("nypb_bombsPlayer");
        if (haskey)
        {
            currentNbOfBombs = LocalSave.GetInt("nypb_bombsPlayer");
        }
        currentNbOfBombs += nbOfBombsToAdd;
        LocalSave.SetInt("nypb_bombsPlayer", currentNbOfBombs);
        LocalSave.Save();

        theCurrentShowResults.addBombs(nbOfBombsToAdd);
    }

    public static void bjsLevelEnded(int levelJohn, int levelBell, TextMesh nbOfMonksEarnedTM)
    {
        int nbOfMonksToAdd = (levelBell + 1) + levelJohn * 6;
        
        if (nbOfMonksToAdd < 0)
        {
            nbOfMonksToAdd = 0;
        }

        int currentNbOfMonks = 0;

        bool haskey = LocalSave.HasIntKey("nypb_unitMonkPlayer");
        if (haskey)
        {
            currentNbOfMonks = LocalSave.GetInt("nypb_unitMonkPlayer");
        }
        currentNbOfMonks += nbOfMonksToAdd;
        LocalSave.SetInt("nypb_unitMonkPlayer", currentNbOfMonks);

        LocalSave.Save();

        nbOfMonksEarnedTM.text = "+" + nbOfMonksToAdd + " Monks!";
    }

    public static void bjsGameEnded(TextMesh nbOfMonksEarnedTM)
    {
        int nbOfMonksToAdd = 50;

        if (nbOfMonksToAdd < 0)
        {
            nbOfMonksToAdd = 0;
        }

        int currentNbOfMonks = 0;

        bool haskey = LocalSave.HasIntKey("nypb_unitMonkPlayer");
        if (haskey)
        {
            currentNbOfMonks = LocalSave.GetInt("nypb_unitMonkPlayer");
        }
        currentNbOfMonks += nbOfMonksToAdd;
        LocalSave.SetInt("nypb_unitMonkPlayer", currentNbOfMonks);

        LocalSave.Save();

        nbOfMonksEarnedTM.text = "+" + nbOfMonksToAdd + " Monks!";
    }

    public static void yanyafLevelEnded(int forDifficulty, ShowResults theCurrentShowResults)
    {
        int nbOfSpyToAdd = (forDifficulty + 2)* 2;

        if (nbOfSpyToAdd < 0)
        {
            nbOfSpyToAdd = 0;
        }

        int currentNbOfSpy = 0;

        bool haskey = LocalSave.HasIntKey("nypb_unitSpyPlayer");
        if (haskey)
        {
            currentNbOfSpy = LocalSave.GetInt("nypb_unitSpyPlayer");
        }
        currentNbOfSpy += nbOfSpyToAdd;
        LocalSave.SetInt("nypb_unitSpyPlayer", currentNbOfSpy);

        LocalSave.Save();

        theCurrentShowResults.setNbOfEndLevelUnits(0, 0, 0, 0, nbOfSpyToAdd);
    }

    public static void yanyafMode1Ended(int forDifficulty, ShowResults theCurrentShowResults)
    {
        int nbOfSpyToAdd = (forDifficulty + 2) * 200;
        int nbOfTanksToAdd = (forDifficulty + 2) * 300;

        if (nbOfSpyToAdd < 0)
        {
            nbOfSpyToAdd = 0;
        }

        int currentNbOfSpy = 0;

        bool haskey = LocalSave.HasIntKey("nypb_unitSpyPlayer");
        if (haskey)
        {
            currentNbOfSpy = LocalSave.GetInt("nypb_unitSpyPlayer");
        }
        currentNbOfSpy += nbOfSpyToAdd;
        LocalSave.SetInt("nypb_unitSpyPlayer", currentNbOfSpy);

        int currentNbOfTanks = 0;

        haskey = LocalSave.HasIntKey("nypb_unitTanksPlayer");
        if (haskey)
        {
            currentNbOfTanks = LocalSave.GetInt("nypb_unitTanksPlayer");
        }
        currentNbOfTanks += nbOfTanksToAdd;
        LocalSave.SetInt("nypb_unitTanksPlayer", currentNbOfTanks);

        LocalSave.Save();

        theCurrentShowResults.setNbOfEndLevelUnits(0, 0, nbOfTanksToAdd, 0, nbOfSpyToAdd);
    }

    public class AllUnitsPlayer
    {
        public int nbOfCloseUnit = 0;
        public int nbOfRangedUnit = 0;
        public int nbOfMachineUnit = 0;
        public int nbOfMonkUnit = 0;
        public int nbOfSpyUnit = 0;
        public int nbOfBomb = 0;
        public bool isHero1 = false;
        public bool isHero2 = false;
        public bool isHero3 = false;
        public bool isHero4 = false;
        public bool isHero5 = false;
        public bool isHero6 = false;
    }

    public static AllUnitsPlayer getNbOfUnits()
    {
        int currentNbOfCloseUnits = 0;

        bool haskey = LocalSave.HasIntKey("nypb_unitClosePlayer");
        if (haskey)
        {
            currentNbOfCloseUnits = LocalSave.GetInt("nypb_unitClosePlayer");
        }

        int currentNbOfRangedUnits = 0;

        haskey = LocalSave.HasIntKey("nypb_unitRangedPlayer");
        if (haskey)
        {
            currentNbOfRangedUnits = LocalSave.GetInt("nypb_unitRangedPlayer");
        }

        int currentNbOfTanks = 0;

        haskey = LocalSave.HasIntKey("nypb_unitTanksPlayer");
        if (haskey)
        {
            currentNbOfTanks = LocalSave.GetInt("nypb_unitTanksPlayer");
        }

        int currentNbOfBombs = 0;

        haskey = LocalSave.HasIntKey("nypb_bombsPlayer");
        if (haskey)
        {
            currentNbOfBombs = LocalSave.GetInt("nypb_bombsPlayer");
        }

        int currentNbOfMonks = 0;

        haskey = LocalSave.HasIntKey("nypb_unitMonkPlayer");
        if (haskey)
        {
            currentNbOfMonks = LocalSave.GetInt("nypb_unitMonkPlayer");
        }

        int currentNbOfSpy = 0;

        haskey = LocalSave.HasIntKey("nypb_unitSpyPlayer");
        if (haskey)
        {
            currentNbOfSpy = LocalSave.GetInt("nypb_unitSpyPlayer");
        }

        bool isHero1 = false;

        haskey = LocalSave.HasBoolKey("nypb_unitHero1");
        if (haskey)
        {
            isHero1 = LocalSave.GetBool("nypb_unitHero1");
        }

        bool isHero2 = false;

        haskey = LocalSave.HasBoolKey("nypb_unitHero2");
        if (haskey)
        {
            isHero2 = LocalSave.GetBool("nypb_unitHero2");
        }

        bool isHero3 = false;

        haskey = LocalSave.HasBoolKey("nypb_unitHero3");
        if (haskey)
        {
            isHero3 = LocalSave.GetBool("nypb_unitHero3");
        }

        bool isHero4 = false;

        haskey = LocalSave.HasBoolKey("nypb_unitHero4");
        if (haskey)
        {
            isHero4 = LocalSave.GetBool("nypb_unitHero4");
        }

        bool isHero5 = false;

        haskey = LocalSave.HasBoolKey("nypb_unitHero5");
        if (haskey)
        {
            isHero5 = LocalSave.GetBool("nypb_unitHero5");
        }

        bool isHero6 = false;

        haskey = LocalSave.HasBoolKey("nypb_unitHero6");
        if (haskey)
        {
            isHero6 = LocalSave.GetBool("nypb_unitHero6");
        }

        AllUnitsPlayer allUnits = new AllUnitsPlayer();
        allUnits.nbOfCloseUnit = currentNbOfCloseUnits;
        allUnits.nbOfRangedUnit = currentNbOfRangedUnits;
        allUnits.nbOfMonkUnit = currentNbOfMonks;
        allUnits.nbOfSpyUnit = currentNbOfSpy;
        allUnits.nbOfMachineUnit = currentNbOfTanks;
        allUnits.nbOfBomb = currentNbOfBombs;
        allUnits.isHero1 = isHero1;
        allUnits.isHero2 = isHero2;
        allUnits.isHero3 = isHero3;
        allUnits.isHero4 = isHero4;
        allUnits.isHero5 = isHero5;
        allUnits.isHero6 = isHero6;

        return allUnits;
    }

    public static void resetAllUnits()
    {

            LocalSave.DeleteKey("nypb_unitClosePlayer");

            LocalSave.DeleteKey("nypb_unitRangedPlayer");

            LocalSave.DeleteKey("nypb_unitTanksPlayer");

            LocalSave.DeleteKey("nypb_bombsPlayer");

            LocalSave.DeleteKey("nypb_unitMonkPlayer");

            LocalSave.DeleteKey("nypb_unitSpyPlayer");

            LocalSave.DeleteKey("nypb_unitHero1");

            LocalSave.DeleteKey("nypb_unitHero2");

            LocalSave.DeleteKey("nypb_unitHero3");

            LocalSave.DeleteKey("nypb_unitHero4");

            LocalSave.DeleteKey("nypb_unitHero5");

            LocalSave.DeleteKey("nypb_unitHero6");
    }
}
