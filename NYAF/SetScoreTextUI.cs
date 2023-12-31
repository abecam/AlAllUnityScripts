using System;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
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

        nbOfPiecesToFind = mainScript.NbOfPiecesToFind + "";

        LocalizationSettings.StringDatabase.GetLocalizedStringAsync(localizedLevel.TableReference, localizedLevel.TableEntryReference);
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
                string textToShow = localizedLevelText + " "+ currentLevel+" - " + scoreOfLevel + " " + localizedLeftText + smallFoundTxt;
                scoreText.text = textToShow;
                scoreBack1Text.text = textToShow;
                scoreBack2Text.text = textToShow;
            }
            else if (currentGameMode == 1)
            {
                int nbOfPiecesFound = mainScript.NbOfPiecesFound;

                string textToShowMode1 = localizedLevelText + " " + currentLevel + " - " + nbOfPiecesFound + " " + localizedOnText + " " + nbOfPiecesToFind + " (" + mainScript.NbOfElements + ") " + smallFoundTxt;
                scoreText.text = textToShowMode1;
                scoreBack1Text.text = textToShowMode1;
                scoreBack2Text.text = textToShowMode1;
            }
            else if (currentGameMode == 3)
            {
                int currentTimeAllocated = (int)mainScript.CurrentTimeAllocated;

                string textToShowMode3 = localizedLevelText + " " + currentLevel + " - " + scoreOfLevel + " " + localizedLeftWithText + " " + ((int)currentTimeAllocated) + smallFoundTxt;
                scoreText.text = textToShowMode3;
                scoreBack1Text.text = textToShowMode3;
                scoreBack2Text.text = textToShowMode3;
            }
        }
    }

    public LocalizedString localizedLevel;
    private string localizedLevelText = "Level";

    public LocalizedString localizedLeft;
    private string localizedLeftText = "left";

    public LocalizedString localizedOn;
    private string localizedOnText = "on";

    public LocalizedString localizedLeftWith;
    private string localizedLeftWithText = "left with";

    void OnEnable()
    {
        localizedLevel.StringChanged += UpdateStringLevel;
        localizedLeft.StringChanged += UpdateStringLeft;
        localizedOn.StringChanged += UpdateStringOn;
        localizedLeftWith.StringChanged += UpdateStringLeftWith;
    }

    void OnDisable()
    {
        localizedLevel.StringChanged -= UpdateStringLevel;
        localizedLeft.StringChanged -= UpdateStringLeft;
        localizedOn.StringChanged -= UpdateStringOn;
        localizedLeftWith.StringChanged -= UpdateStringLeftWith;
    }

    void UpdateStringLevel(string s)
    {
        localizedLevelText = s;
    }

    void UpdateStringLeft(string s)
    {
        localizedLeftText = s;
    }

    void UpdateStringOn(string s)
    {
        localizedOnText = s;
    }

    void UpdateStringLeftWith(string s)
    {
        localizedLeftWithText = s;
    }
}
