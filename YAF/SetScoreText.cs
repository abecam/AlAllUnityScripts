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
        int scoreOfLevel = mainScript.ScoreOfLevel;

        {
           string textToShow = "" + scoreOfLevel + " left";
           scoreText.text = textToShow;
           scoreBack1Text.text = textToShow;
           scoreBack2Text.text = textToShow;
        }
    }
}
