using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;

public class SetExplicationText : MonoBehaviour
{
    public LocalizedString explanationA;
    public LocalizedString explanationB;
    public LocalizedString explanationC;
    public LocalizedString explanationD;
    public LocalizedString explanationE;
    private string localizedExplanationA = "";
    private string localizedExplanationB = "";
    private string localizedExplanationC = "";
    private string localizedExplanationD = "";
    private string localizedExplanationE = "";

    private Text ourText;
    private int nbOfPiecesToFind = 1500;
    private int currentDifficulty = SelectDifficulty.normal; // Default difficulty
    private int nbOfElements = 79;

    private int currentGameMode = 0;

    // Start is called before the first frame update
    void Start()
    {
        ourText = GetComponent<Text>();

        bool haskey = LocalSave.HasIntKey("currentMode");
        if (haskey)
        {
            currentGameMode = LocalSave.GetInt("currentMode");
        }

        findNbsToFind();

        setExplanationText(currentGameMode);

        explanationA.RefreshString();
        explanationB.RefreshString();
        explanationC.RefreshString();
        explanationD.RefreshString();
        explanationE.RefreshString();
    }

    void OnEnable()
    {
        explanationA.StringChanged += UpdateStringA;
        explanationB.StringChanged += UpdateStringB;
        explanationC.StringChanged += UpdateStringC;
        explanationD.StringChanged += UpdateStringD;
        explanationE.StringChanged += UpdateStringE;
    }

    void OnDisable()
    {
        explanationA.StringChanged -= UpdateStringA;
        explanationB.StringChanged -= UpdateStringB;
        explanationC.StringChanged -= UpdateStringC;
        explanationD.StringChanged -= UpdateStringD;
        explanationE.StringChanged -= UpdateStringE;
    }

    void UpdateStringA(string s)
    {
        localizedExplanationA = s;
    }
    void UpdateStringB(string s)
    {
        localizedExplanationB = s;
    }

    void UpdateStringC(string s)
    {
        localizedExplanationC = s;
    }

    void UpdateStringD(string s)
    {
        localizedExplanationD = s;
    }

    void UpdateStringE(string s)
    {
        localizedExplanationE = s;
        setExplanationText(currentGameMode);
    }


    private void findNbsToFind()
    {
        bool haskey = LocalSave.HasIntKey("difficulty");
        if (haskey)
        {
            currentDifficulty = LocalSave.GetInt("difficulty");
        }

        if (currentDifficulty == SelectDifficulty.veryEasy)
        {
            if (nbOfElements > 20)
            {
                nbOfElements = 20;
            }
            nbOfPiecesToFind = 300;
        }
        if (currentDifficulty == SelectDifficulty.easy)
        {
            if (nbOfElements > 40)
            {
                nbOfElements = 40;
            }
            nbOfPiecesToFind = 600;
        }
    }

    private void setExplanationText(int currentGameMode)
    {
        String text = localizedExplanationA;

        if (currentGameMode == 1)
        {
            text = localizedExplanationB.Replace("X", nbOfPiecesToFind+"");
        }
        if (currentGameMode == 2)
        {
            text = text + "\n"+ localizedExplanationC;
        }
        if (currentGameMode == 3)
        {
            text = text + "\n" + localizedExplanationD;
        }
        if (currentGameMode == 4)
        {
            text = localizedExplanationE;
        }
        if (text != null && ourText != null)
        { 
            ourText.text = text;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
