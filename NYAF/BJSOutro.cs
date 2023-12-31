using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using Random = UnityEngine.Random;

public class BJSOutro : MonoBehaviour
{
    public GameObject johnWalk;
    public GameObject johnReturn;

    public Rigidbody2D johnWalkRB;
    public Rigidbody2D johnReturnRB;

    private GameObject title;

    public GameObject creditText;

    AudioSource musicSource;

    public GameObject curtain; // To fade the scene at the start/end
    Renderer curtainRenderer;

    public TextMesh transitionText;

    public LocalizedString outroLocalizedText;
    private string localizedOutroText = "";

    public LocalizedString outroLocalizedTimeSpentTxt;
    private string localizedTimeSpentTxt = "";

    public LocalizedString outroLocalizedTotalTimeSpentTxt;
    private string localizedTotalTimeSpentTxt = "";

    public LocalizedString outroLocalizedNewBestTimeTxt;
    private string localizedNewBestTimeTxt = "";

    public LocalizedString outroLocalizedBestTimeTxt;
    private string localizedBestTimeTxt = "";

    // Start is called before the first frame update
    void Start()
    {
        if (PlayAndKeepMusic.Instance != null)
        {
            PlayAndKeepMusic.Instance.Activated = false;
            PlayAndKeepMusic.Instance.stopMusic();
        }

        Application.targetFrameRate = 60;

        musicSource = GetComponent<AudioSource>();

        title = GameObject.FindWithTag("Title");

        creditText.transform.position = new Vector3(0, -22, -5);

        float newAlpha = 1;
        curtainRenderer = curtain.GetComponent<Renderer>();

        curtainRenderer.material.color = new Color(curtainRenderer.material.color.r, curtainRenderer.material.color.g, curtainRenderer.material.color.b, newAlpha);
        transitionText.color = new Color(transitionText.color.r, transitionText.color.g, transitionText.color.b, newAlpha);

        LocalizationSettings.StringDatabase.GetLocalizedStringAsync(outroLocalizedText.TableReference, outroLocalizedText.TableEntryReference);
        /*if (op.IsDone)
        {
            localizedOutroText = op.Result;
            SetCreditText();
        }
        else
        {
            op.Completed += (operation) =>
            {
                localizedOutroText = operation.Result;
                SetCreditText();
            };
        }*/
    }


    void OnEnable()
    {
        outroLocalizedText.StringChanged += UpdateStringA;
        outroLocalizedTimeSpentTxt.StringChanged += UpdateStringTimeSpent;
        outroLocalizedTotalTimeSpentTxt.StringChanged += UpdateStringTotalTimeSpent;
        outroLocalizedNewBestTimeTxt.StringChanged += UpdateStringNewBestTime;
        outroLocalizedBestTimeTxt.StringChanged += UpdateStringBestTime;
    }

    void OnDisable()
    {
        outroLocalizedText.StringChanged -= UpdateStringA;
        outroLocalizedTimeSpentTxt.StringChanged -= UpdateStringTimeSpent;
        outroLocalizedTotalTimeSpentTxt.StringChanged -= UpdateStringTotalTimeSpent;
        outroLocalizedNewBestTimeTxt.StringChanged -= UpdateStringNewBestTime;
        outroLocalizedBestTimeTxt.StringChanged -= UpdateStringBestTime;
    }

    void UpdateStringA(string s)
    {
        localizedOutroText = s;
    }

    void UpdateStringTimeSpent(string s)
    {
        localizedTimeSpentTxt = s;
    }

    void UpdateStringTotalTimeSpent(string s)
    {
        localizedTotalTimeSpentTxt = s;
    }

    void UpdateStringNewBestTime(string s)
    {
        localizedNewBestTimeTxt = s;
    }

    void UpdateStringBestTime(string s)
    {
        localizedBestTimeTxt = s;
        SetCreditText();
    }
    private void SetCreditText()
    {
        float allTimeSpent = 0;
        float lastTimeSpent = 0;

        TextMesh textToFill = creditText.GetComponent<TextMesh>();
       
        bool haskey = LocalSave.HasFloatKey("BJS_allTimeSpent");

        if (haskey)
        {
            allTimeSpent = LocalSave.GetFloat("BJS_allTimeSpent");
        }

        haskey = LocalSave.HasFloatKey("BJS_currentTimeSpent");

        if (haskey)
        {
            lastTimeSpent = LocalSave.GetFloat("BJS_currentTimeSpent");
        }

        long timeSpentLongMode = (long)allTimeSpent;

        long hoursSpentMode = timeSpentLongMode / 3600;
        if (hoursSpentMode > 0)
        {
            timeSpentLongMode -= hoursSpentMode * 3600;
        }

        long minutesSpentMode = timeSpentLongMode / 60;
        if (minutesSpentMode > 0)
        {
            timeSpentLongMode -= minutesSpentMode * 60;
        }

        long lastTimeSpentLongMode = (long)lastTimeSpent;

        long hoursLastSpentMode = lastTimeSpentLongMode / 3600;
        if (hoursLastSpentMode > 0)
        {
            lastTimeSpentLongMode -= hoursLastSpentMode * 3600;
        }

        long minutesLastSpentMode = lastTimeSpentLongMode / 60;
        if (minutesLastSpentMode > 0)
        {
            lastTimeSpentLongMode -= minutesLastSpentMode * 60;
        }
        String nbOfHours = ((hoursLastSpentMode > 0) ? hoursLastSpentMode + " hours\n" : "");
        String nbOfMins = ((hoursLastSpentMode > 0) || (minutesLastSpentMode > 0) ? minutesLastSpentMode + " minutes\n" : "");
        String nbOfSeconds = ((lastTimeSpentLongMode > 0) ? lastTimeSpentLongMode + " seconds\n" : "0 seconds");
        String timeText = localizedTimeSpentTxt.Replace("[HOURS]", "\n" + nbOfHours).Replace("[MINUTES]", "\n" + nbOfMins).Replace("[SECONDS]", "\n" + nbOfSeconds);
        /*
        String timeText = "You finished the game in\n"+((hoursLastSpentMode > 0) ? hoursLastSpentMode + " hours\n" : "") +
             ((hoursLastSpentMode > 0) || (minutesLastSpentMode > 0) ? minutesLastSpentMode + " minutes\n" : "") +
             ((lastTimeSpentLongMode > 0) ? lastTimeSpentLongMode + " seconds\n" : "0 seconds");
            */

        String nbOfTotalHours = ((hoursSpentMode > 0) ? hoursSpentMode + " hours\n" : "");
        String nbOfTotalMins = ((hoursSpentMode > 0) || (minutesSpentMode > 0) ? minutesSpentMode + " minutes\n" : "");
        String nbOfTotalSeconds = ((timeSpentLongMode > 0) ? timeSpentLongMode + " seconds\n" : "0 seconds");
        String allTimeText = localizedTotalTimeSpentTxt.Replace("[HOURS]", "\n" + nbOfTotalHours).Replace("[MINUTES]", "\n" + nbOfTotalMins).Replace("[SECONDS]", "\n" + nbOfTotalSeconds);

        /*
        String allTimeText = "You spent a total of\n" + ((hoursSpentMode > 0) ? hoursSpentMode + " hours\n" : "") +
             ((hoursSpentMode > 0) || (minutesSpentMode > 0) ? minutesSpentMode + " minutes\n" : "") +
             ((timeSpentLongMode > 0) ? timeSpentLongMode + " seconds" : "0 seconds") + "\nin BJS!";
        */
        // Best time
        float bestTimeSpent = 100000000000000000;
        haskey = LocalSave.HasFloatKey("BJS_bestTimeSpent");

        if (haskey)
        {
            bestTimeSpent = LocalSave.GetFloat("BJS_bestTimeSpent");
        }

        if (allTimeSpent < bestTimeSpent)
        {
            bestTimeSpent = allTimeSpent;

            timeText += "\n"+localizedNewBestTimeTxt;
            //timeText += "\nNew best time!";

            LocalSave.SetFloat("BJS_bestTimeSpent", bestTimeSpent);
            LocalSave.Save();
        }
        else
        {
            long bestTimeSpentLongMode = (long)bestTimeSpent;

            long hoursBestSpentMode = bestTimeSpentLongMode / 3600;
            if (hoursBestSpentMode > 0)
            {
                bestTimeSpentLongMode -= hoursBestSpentMode * 3600;
            }

            long minutesBestSpentMode = bestTimeSpentLongMode / 60;
            if (minutesBestSpentMode > 0)
            {
                bestTimeSpentLongMode -= minutesBestSpentMode * 60;
            }
            /*
            timeText += "\nBest time is " + ((hoursBestSpentMode > 0) ? hoursBestSpentMode + " hours\n" : "") +
            ((hoursBestSpentMode > 0) || (minutesBestSpentMode > 0) ? minutesBestSpentMode + " minutes\n" : "") +
            ((bestTimeSpentLongMode > 0) ? bestTimeSpentLongMode + " seconds " : "0 seconds");
            */

            String bestTimeNbOfHours = ((hoursBestSpentMode > 0) ? hoursBestSpentMode + " hours\n" : "");
            String bestTimeNbOfMins = ((hoursBestSpentMode > 0) || (minutesBestSpentMode > 0) ? minutesBestSpentMode + " minutes\n" : "");
            String bestTimeNbOfSeconds = ((bestTimeSpentLongMode > 0) ? bestTimeSpentLongMode + " seconds\n" : "0 seconds");
            timeText += "\n"+localizedBestTimeTxt.Replace("[HOURS]", "\n" + bestTimeNbOfHours).Replace("[MINUTES]", "\n" + bestTimeNbOfMins).Replace("[SECONDS]", "\n" + bestTimeNbOfSeconds);

        }

        //textToFill.text = "Thank you\nfor playing\nBrother John\nSimulator\n2.4.27\n"
        //   + "A very serious\nSimulation\nBy Alain Becam\n\nGraphics and ideas by\nSébastien Lesage\n\nMusic by\n"+
        //   "Jean-Philippe Rameau\n\nMusic by\nChris Collins\n\nFont by Gluk\nGlametrixBold\n\nDoor destruction:\nUnity-2D-Destruction\n(c) 2016 Matthew Holtzem\nunder MIT Licence\n\nBreaking sound from\nhttp://www.universal-soundbank.com/\n\n\n\n\n\n\n\n\n" + timeText +"\n\n"+ allTimeText;
        textToFill.text = localizedOutroText + "\n\n\n\n\n\n\n\n\n" + timeText + "\n\n" + allTimeText;
    }

    int iTurn = 0;
    int nextTurn = 10;
    int iFadingSteps = 0;
    const int nbOfFadingSteps = 120;
    const float stepFading = 1f / ((float)nbOfFadingSteps);

    // Update is called once per frame
    void Update()
    {
        if (iFadingSteps < nbOfFadingSteps)
        {
            float newAlpha = curtainRenderer.material.color.a - stepFading;
            curtainRenderer.material.color = new Color(curtainRenderer.material.color.r, curtainRenderer.material.color.g, curtainRenderer.material.color.b, newAlpha);
            transitionText.color = new Color(transitionText.color.r, transitionText.color.g, transitionText.color.b, newAlpha);

            iFadingSteps++;
        }
        if (title == null)
        {
            PlayMusic();

            creditText.transform.position = new Vector3(0, creditText.transform.position.y + 0.01f, -5);
            if (creditText.transform.position.y > 19)
            {
                creditText.transform.position = new Vector3(0, -19, -5);
            }
        }
        iTurn++;

        if (iTurn > nextTurn)
        {
            float newForcex = 4 *Random.value - 2;
            float newForcey = 4 * Random.value - 2;

            johnWalkRB.AddForce(new Vector2(newForcex, newForcey));

            newForcex = 4 * Random.value - 2;
            newForcey = 4 * Random.value - 2;

            johnReturnRB.AddForce(new Vector2(newForcex, newForcey));
            
            iTurn = 0;
            nextTurn = 120 * ((int )Random.value);
        }
    }

    private void PlayMusic()
    {
        if (musicSource != null)
        {
            if (!musicSource.isPlaying)
            {
                musicSource.Play();
            }
        }
    }
}
