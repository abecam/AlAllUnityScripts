using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetScoreText : MonoBehaviour
{
    public GameObject score;

    public TextMesh scoreText;
    public TextMesh scoreBack1Text;
    public TextMesh scoreBack2Text;

    private CreateBoard mainScript;
    private string nbOfPiecesToFind;

    bool shown = false;

    // Start is called before the first frame update
    void Start()
    {
        GameObject mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        mainScript = mainCamera.GetComponent<CreateBoard>();

        nbOfPiecesToFind = mainScript.NbOfPiecesToFind+"";
    }

    // Update is called once per frame
    void Update()
    {
        if (shown && mainScript.InTitle)
        {
            // Show the score
            score.transform.localPosition = new Vector3(100, score.transform.localPosition.y, 7);

            shown = false;
        }
        if (!shown && !mainScript.InTitle)
        {
            // Hide the score
            score.transform.localPosition = new Vector3(0, score.transform.localPosition.y, 7);

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

        {
            String smallFoundTxt = ourSmallFound ? "*" : "";

            if (currentGameMode != 1 && currentGameMode != 3)
            {
                string textToShow = "" + scoreOfLevel + " left" + smallFoundTxt;
                scoreText.text = textToShow;
                scoreBack1Text.text = textToShow;
                scoreBack2Text.text = textToShow;
            }
            else if (currentGameMode == 1)
            {
                int nbOfPiecesFound = mainScript.NbOfPiecesFound;

                string textToShowMode1 = "" + nbOfPiecesFound + " on " + nbOfPiecesToFind + " (" + mainScript.NbOfElements + " - " + scoreOfLevel + ") " + smallFoundTxt;
                scoreText.text = textToShowMode1;
                scoreBack1Text.text = textToShowMode1;
                scoreBack2Text.text = textToShowMode1;
            }
            else if (currentGameMode == 3)
            {
                int currentTimeAllocated = (int )mainScript.CurrentTimeAllocated;

                string textToShowMode3 = "" + scoreOfLevel + " left with " + ((int)currentTimeAllocated) + smallFoundTxt;
                scoreText.text = textToShowMode3;
                scoreBack1Text.text = textToShowMode3;
                scoreBack2Text.text = textToShowMode3;
            }
        }
    }
}
