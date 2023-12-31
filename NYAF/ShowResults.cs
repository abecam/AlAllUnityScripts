using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowResults : MonoBehaviour
{
    private MainApp mainScript;
    public Text textToShow;
    private Image ourImage;

    int iFade = 0;
    const int timeToFade = 120;
    const float stepFade = 1 / ((float)timeToFade);
    const int timeToStay = timeToFade+200;

    private string nextLevelAtEnd;

    int nbOfClosedUnits = 0;
    int nbOfRangedUnits = 0;
    int nbOfTanks = 0;
    int nbOfMonks = 0;
    int nbOfSpies = 0;
    int nbOfBombs = 0;

    bool inEndOfLevel = false;
    bool isShown = true;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Setting ourself in the main app");
        GameObject camera = GameObject.FindGameObjectWithTag("MainCamera");
        mainScript = camera.GetComponent<MainApp>();
        mainScript.setShowResult(this);

        ourImage = transform.GetComponent<Image>();

        transform.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (inEndOfLevel)
        {
            if (iFade < timeToFade)
            {
                float lastAlpha = ourImage.color.a;

                lastAlpha += stepFade;

                if (lastAlpha > 1)
                {
                    lastAlpha = 1;
                }
                ourImage.color = new Color(ourImage.color.r, ourImage.color.g, ourImage.color.b, lastAlpha);
            }
            if (iFade > timeToStay)
            {
                // Start the next level
                UnityEngine.SceneManagement.SceneManager.LoadScene(nextLevelAtEnd);
            }
        }
        else
        {
            if (iFade > timeToStay && isShown)
            {
                transform.gameObject.SetActive(false);

                isShown = false;
            }
        }
        iFade++;
    }

    public void showResultText(long nbOfHours, long nbOfMinutes, long nbOfSeconds, long nbOfHoursMode, long nbOfMinutesMode, long nbOfSecondsMode, string nextLevel)
    {
        string timeSpent = "You finished the level in " + ((nbOfHours > 0) ? nbOfHours + " hours " : "") +
            ((nbOfMinutes > 0) ? nbOfMinutes + " minutes " : "") +
            ((nbOfSeconds > 0) ? nbOfSeconds + " seconds " : "");


        string timeSpentMode = "You spent " + ((nbOfHoursMode > 0) ? nbOfHoursMode + " hours " : "") +
            ((nbOfMinutesMode > 0) ? nbOfMinutesMode + " minutes " : "") +
            ((nbOfSecondsMode > 0) ? nbOfSecondsMode + " seconds " : "")+"in this mode";

        string resultText = ((nbOfClosedUnits > 0) ? nbOfClosedUnits + " closed Units\n" : "") +
            ((nbOfRangedUnits > 0) ? nbOfRangedUnits + " ranged Units\n" : "") +
            ((nbOfTanks > 0) ? nbOfTanks + " tanks\n" : "") +
            ((nbOfMonks > 0) ? nbOfMonks + " monks\n" : "") +
            ((nbOfSpies > 0) ? nbOfSpies + " spies\n" : "") +
            ((nbOfBombs > 0) ? nbOfBombs + " bombs\n" : "");

        textToShow.text = "Congratulation!\n"+timeSpent+"\n\nAnd you earned:\n"+resultText+"\n\n"+ timeSpentMode;

        iFade = 0;

        ourImage.color = new Color(ourImage.color.r, ourImage.color.g, ourImage.color.b, 0);

        // Once the display is finished, launch the next level
        nextLevelAtEnd = nextLevel;

        transform.gameObject.SetActive(true);

        inEndOfLevel = true;
    }

    public void setNbOfEndLevelUnits(int nbOfClosedUnits, int nbOfRangedUnits, int nbOfTanks, int nbOfMonks, int nbOfSpies)
    {
        this.nbOfClosedUnits = nbOfClosedUnits;
        this.nbOfRangedUnits = nbOfRangedUnits;
        this.nbOfTanks = nbOfTanks;
        this.nbOfMonks = nbOfMonks;
        this.nbOfSpies = nbOfSpies;
    }

    public void addBombs(int nbOfBombs)
    {
        this.nbOfBombs += nbOfBombs;

        ourImage.color = new Color(ourImage.color.r, ourImage.color.g, ourImage.color.b, 0);

        transform.gameObject.SetActive(true);

        textToShow.text = "+" + nbOfBombs + " bombs!";

        iFade = timeToFade;

        isShown = true;
    }
}
