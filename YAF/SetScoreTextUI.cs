using System;
using UnityEngine;
using UnityEngine.UI;

public class SetScoreTextUI : MonoBehaviour
{
    public GameObject score;

    public Text scoreText;
    public Text scoreBack1Text;
    public Text scoreBack2Text;

    private CreateBoard mainScript;
    private string nbOfPiecesToFind;

    bool shown = false;

    // Start is called before the first frame update
    void Start()
    {
        GameObject mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        mainScript = mainCamera.GetComponent<CreateBoard>();
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

        int scoreOfLevel = mainScript.ScoreOfLevel;
        int currentLevel = mainScript.currentLevel;

        {

                string textToShow = "Level "+ currentLevel+" - " + scoreOfLevel + " left";
                scoreText.text = textToShow;
                scoreBack1Text.text = textToShow;
                scoreBack2Text.text = textToShow;

        }
    }
}
