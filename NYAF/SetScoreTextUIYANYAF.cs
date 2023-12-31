using System;
using UnityEngine;
using UnityEngine.UI;

public class SetScoreTextUIYANYAF : MonoBehaviour
{
    public GameObject score;

    public Text scoreText;
    public Text scoreBack1Text;
    public Text scoreBack2Text;

    private ManageYANYAF mainScript;
    private string nbOfPiecesToFind;

    bool shown = false;

    // Start is called before the first frame update
    void Start()
    {
        GameObject mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        mainScript = mainCamera.GetComponent<ManageYANYAF>();

        nbOfPiecesToFind = mainScript.NbOfPiecesToFind + "";
    }

    // Update is called once per frame
    void Update()
    {
        if (shown && mainScript.InTitle)
        {
            // Hide the score
            score.SetActive(false);

            shown = false;
        }
        if (!shown && !mainScript.InTitle)
        {
            // Show the score
            score.SetActive(true);

            shown = true;
        }
        if (shown)
        {
            updateScore();
        }
    }

    void updateScore()
    {
        bool ourSmallFound = mainScript.wasCurrentSmallFound();
        int currentGameMode = mainScript.getCurrentModeNb();
        int scoreOfLevel = mainScript.ScoreOfLevel;
        int currentLevel = mainScript.currentLevel;

        {
            String smallFoundTxt = ourSmallFound ? "*" : "";

            if (currentGameMode != 1 && currentGameMode != 3)
            {
                string textToShow = scoreOfLevel + " left" + smallFoundTxt;
                scoreText.text = textToShow;
                scoreBack1Text.text = textToShow;
                scoreBack2Text.text = textToShow;
            }
            else if (currentGameMode == 1)
            {
                int nbOfPiecesFound = mainScript.NbOfPiecesFound;

                string textToShowMode1 = nbOfPiecesFound + " on " + nbOfPiecesToFind + " (" + mainScript.NbOfElements + ") " + smallFoundTxt;
                scoreText.text = textToShowMode1;
                scoreBack1Text.text = textToShowMode1;
                scoreBack2Text.text = textToShowMode1;
            }
            else if (currentGameMode == 3)
            {
                int currentTimeAllocated = (int)mainScript.CurrentTimeAllocated;

                string textToShowMode3 = scoreOfLevel + " left with " + ((int)currentTimeAllocated) + smallFoundTxt;
                scoreText.text = textToShowMode3;
                scoreBack1Text.text = textToShowMode3;
                scoreBack2Text.text = textToShowMode3;
            }
        }
    }
}
