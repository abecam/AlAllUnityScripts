using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckIfAvailable : MonoBehaviour
{
    public whichGame thisGame;

    public enum whichGame
    {
        MMPG,
        BJS,
        YANYAF
    };

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        bool isAvailable = false;
        bool haskey = false;

        switch (thisGame)
        {
            case whichGame.MMPG:
                // First level has been passed, so there at at least some closed units
                int currentNbOfCloseUnits = 0;

                haskey = LocalSave.HasIntKey("nypb_unitClosePlayer");
                if (haskey)
                {
                    currentNbOfCloseUnits = LocalSave.GetInt("nypb_unitClosePlayer");
                }
                if (currentNbOfCloseUnits > 100)
                {
                    isAvailable = true;
                }
                break;

            case whichGame.YANYAF:
                // All mini shapes have been found.
                haskey = LocalSave.HasIntKey("smallFound");
                int levelReached = 0;
                if (haskey)
                {
                    levelReached = LocalSave.GetInt("smallFound");
                }

                if (levelReached == 0b1111111111100)
                {
                    isAvailable = true;
                }

                break;

            case whichGame.BJS:
                // First mode has been passed.
                int maxGameMode = 0;

                haskey = LocalSave.HasIntKey("modeFinished");
                if (haskey)
                {
                    maxGameMode = LocalSave.GetInt("modeFinished");
                }
                if (maxGameMode > 0)
                {
                    isAvailable = true;
                }
                break;
        }

        gameObject.SetActive(isAvailable);
    }
}
