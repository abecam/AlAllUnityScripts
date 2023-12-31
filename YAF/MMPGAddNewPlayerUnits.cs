using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MMPGAddNewPlayerUnits : MonoBehaviour
{
    public class AllUnitsPlayer
    {
        public int nbOfHero = 1;
        public int nbOfSupport = 0;
        public int nbOfMachineUnit = 0;
    }

    public static AllUnitsPlayer getNbOfUnits()
    {
        int currentNbOfCloseUnits = 1;

        //bool haskey = LocalSave.HasIntKey("nypb_unitClosePlayer");
        //if (haskey)
        //{
        //    currentNbOfCloseUnits = LocalSave.GetInt("nypb_unitClosePlayer");
        //}

        int currentNbOfRangedUnits = 0;

        bool haskey = LocalSave.HasIntKey("nypb_unitRangedPlayer");
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

        AllUnitsPlayer allUnits = new AllUnitsPlayer();
        allUnits.nbOfHero = currentNbOfCloseUnits;
        allUnits.nbOfSupport = currentNbOfRangedUnits;
        allUnits.nbOfMachineUnit = currentNbOfTanks;

        return allUnits;
    }

    public static void resetAllUnits()
    {
        LocalSave.DeleteKey("nypb_unitClosePlayer");

        LocalSave.DeleteKey("nypb_unitRangedPlayer");

        LocalSave.DeleteKey("nypb_unitTanksPlayer");
    }
}
