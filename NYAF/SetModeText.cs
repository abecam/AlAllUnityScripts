using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class SetModeText : MonoBehaviour
{
    public TextMesh ourDescText;
    TextMesh ourText;
    int currentGameMode = 0;

    // Start is called before the first frame update
    void Start()
    {
        bool haskey = LocalSave.HasIntKey("currentMode");
        
        if (haskey)
        {
            currentGameMode = LocalSave.GetInt("currentMode");
        }

        ourText = GetComponent<TextMesh>();

        LocalizationSettings.StringDatabase.GetLocalizedStringAsync(localizedDemo.TableReference, localizedDemo.TableEntryReference);

        SetText();
    }

    public LocalizedString localizedDemo;
    private string localizedDemoText = "Demo! Play the full game during 10 minutes!";

    public LocalizedString localizedModeA;
    private string localizedModeAText = "Mode A"; // A will be replaced by the current mode.

    public LocalizedString localizedModeAExpl;
    private string localizedModeAExplText = "Find the hidden characters in each level";

    public LocalizedString localizedModeBExpl;
    private string localizedModeBExplText = "Find a given number of characters across all levels";

    public LocalizedString localizedModeCExpl;
    private string localizedModeCExplTxt = "Like mode A but each 20s all characters are jumping around.";

    public LocalizedString localizedSick;
    private string localizedSickTxt = "(activate the rotation if it does not make you sick)";

    public LocalizedString localizedModeDExpl;
    private string localizedModeDExplTxt = "Try to keep your score negative in a world moving around.";

    public LocalizedString localizedModeEExpl;
    private string localizedModeEExplTxt = "Find the manually placed characters.No dog allowed!";

    public LocalizedString localizedOmega;
    private string localizedOmegaTxt = "I don't know where you are...\nContact the dev and shout at him!";

    void OnEnable()
    {
        ourText = GetComponent<TextMesh>();

        localizedDemo.StringChanged += UpdateStringDemo;
        localizedModeA.StringChanged += UpdateStringModeA;
        localizedModeAExpl.StringChanged += UpdateStringModeAExpl;
        localizedModeBExpl.StringChanged += UpdateStringModeBExpl;
        localizedModeCExpl.StringChanged += UpdateStringModeCExpl;
        localizedSick.StringChanged += UpdateStringSick;
        localizedModeDExpl.StringChanged += UpdateStringModeDExpl;
        localizedModeEExpl.StringChanged += UpdateStringModeEExpl;
        localizedOmega.StringChanged += UpdateStringOmega;
    }

    void OnDisable()
    {
        localizedDemo.StringChanged -= UpdateStringDemo;
        localizedModeA.StringChanged -= UpdateStringModeA;
        localizedModeAExpl.StringChanged -= UpdateStringModeAExpl;
        localizedModeBExpl.StringChanged -= UpdateStringModeBExpl;
        localizedModeCExpl.StringChanged -= UpdateStringModeCExpl;
        localizedSick.StringChanged -= UpdateStringSick;
        localizedModeDExpl.StringChanged -= UpdateStringModeDExpl;
        localizedModeEExpl.StringChanged -= UpdateStringModeEExpl;
        localizedOmega.StringChanged += UpdateStringOmega;
    }

    void UpdateStringDemo(string s)
    {
        localizedDemoText = s;
    }

    void UpdateStringModeA(string s)
    {
        localizedModeAText = s;
    }

    void UpdateStringModeAExpl(string s)
    {
        localizedModeAExplText = s;
    }

    void UpdateStringModeBExpl(string s)
    {
        localizedModeBExplText = s;
    }

    void UpdateStringModeCExpl(string s)
    {
        localizedModeCExplTxt = s;
    }

    void UpdateStringSick(string s)
    {
        localizedSickTxt = s;
    }

    void UpdateStringModeDExpl(string s)
    {
        localizedModeDExplTxt = s;
    }

    void UpdateStringModeEExpl(string s)
    {
        localizedModeEExplTxt = s;
    }

    void UpdateStringOmega(string s)
    {
        localizedOmegaTxt = s;

        SetText();
    }

    private void SetText()
    {
        string demoText = "";
        if (DemoManager.isDemo)
        {
            demoText = localizedDemoText;
        }
        switch (currentGameMode)
        {
            case 0:
                ourText.text = localizedModeAText;
                ourDescText.text = localizedModeAExplText + demoText;
                break;
            case 1:
                ourText.text = localizedModeAText.Replace("A", "B");
                ourDescText.text = localizedModeBExplText + demoText;
                break;
            case 2:
                ourText.text = localizedModeAText.Replace("A", "C");
                ourDescText.text = localizedModeCExplTxt+"\n"+ localizedSickTxt + demoText;
                if (DemoManager.isDemo)
                {
                    ourDescText.characterSize = 0.013f;
                }
                break;
            case 3:
                ourText.text = localizedModeAText.Replace("A", "D");
                ourDescText.text = localizedModeDExplTxt + "\n" + localizedSickTxt + demoText;
                if (DemoManager.isDemo)
                {
                    ourDescText.characterSize = 0.013f;
                }
                break;
            case 4:
                ourText.text = localizedModeAText.Replace("A", "E");
                ourDescText.text = localizedModeEExplTxt + demoText;
                break;
            default:
                ourText.text = localizedModeAText.Replace("A", "Omega");
                ourDescText.text = localizedOmegaTxt + demoText;
                break;
        }
    }
}
