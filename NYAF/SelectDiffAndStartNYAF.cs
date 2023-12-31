using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectDiffAndStartNYAF : MonoBehaviour
{
    public enum Difficulty
    {
        veryEasy = 0,
        easy = 1,
        normal = 2,
        hard = 3,
        veryHard = 4,
        insane = 5,
        purgatory = 6,
        hell = 7
    }

    // Start is called before the first frame update
    void Start()
    {
    }


    public void SelectANewDifficulty(int newDifficulty)
    {
        Debug.Log("selecting difficulty " + newDifficulty);

        SelectDifficultyForLater((Difficulty)newDifficulty);
    }

    private void SelectDifficultyForLater(Difficulty newDifficulty)
    {
        // Save the difficulty and restart Nyaf
        Debug.Log("Setting initial difficulty to " + (int)newDifficulty);
        LocalSave.SetInt("difficulty", (int )newDifficulty);

        LaunchNYAF.loadNYAFGame();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
