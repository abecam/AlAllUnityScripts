using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class MMPGGetNbOfUnitsPlayer : MonoBehaviour
{
    private Text level;
    private ManageNYPB mainScript;

    // Start is called before the first frame update
    void Start()
    {
        GameObject mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        mainScript = mainCamera.GetComponent<ManageNYPB>();
        level = GetComponent<Text>();

        LocalizationSettings.StringDatabase.GetLocalizedStringAsync(localizedPlayer.TableReference, localizedPlayer.TableEntryReference);
    }

    // Update is called once per frame
    void Update()
    {
        level.text = localizedPlayerText + "\n\n" + localizedClosedText + " " + mainScript.CurrentNbOfClosed+ " / " + mainScript.NbOfClosed + "\n"+
            localizedRangedText + " " + mainScript.CurrentNbOfRanged + " / " + mainScript.NbOfRanged + "\n" +
            localizedTankTxt + " " + mainScript.CurrentNbOfMachines + " / " + mainScript.NbOfMachines + "\n" +
            localizedMonkTxt + " " + mainScript.CurrentNbOfMonks + " / " + mainScript.NbOfMonks + "\n" +
            localizedSpyTxt + " " + mainScript.CurrentNbOfSpies + " / " + mainScript.NbOfSpies + "\n" +
            localizedBombTxt + "  " + mainScript.CurrentNbOfBombs + " / " + mainScript.NbOfBombs + "\n";
    }

    public LocalizedString localizedPlayer;
    private string localizedPlayerText = "Player";

    public LocalizedString localizedClosed;
    private string localizedClosedText = "Closed";

    public LocalizedString localizedRanged;
    private string localizedRangedText = "Ranged";

    public LocalizedString localizedLocalizedTank;
    private string localizedTankTxt = "Tank";

    public LocalizedString localizedLocalizedMonk;
    private string localizedMonkTxt = "Monk";

    public LocalizedString localizedLocalizedSpy;
    private string localizedSpyTxt = "Spy";

    public LocalizedString localizedLocalizedBomb;
    private string localizedBombTxt = "Bomb";

    void OnEnable()
    {
        localizedPlayer.StringChanged += UpdateStringPlayer;
        localizedClosed.StringChanged += UpdateStringClosed;
        localizedRanged.StringChanged += UpdateStringRanged;
        localizedLocalizedTank.StringChanged += UpdateStringTank;
        localizedLocalizedMonk.StringChanged += UpdateStringMonk;
        localizedLocalizedSpy.StringChanged += UpdateStringSpy;
        localizedLocalizedBomb.StringChanged += UpdateStringBomb;
    }

    void OnDisable()
    {
        localizedPlayer.StringChanged -= UpdateStringPlayer;
        localizedClosed.StringChanged -= UpdateStringClosed;
        localizedRanged.StringChanged -= UpdateStringRanged;
        localizedLocalizedTank.StringChanged -= UpdateStringTank;
        localizedLocalizedMonk.StringChanged -= UpdateStringMonk;
        localizedLocalizedSpy.StringChanged -= UpdateStringSpy;
        localizedLocalizedBomb.StringChanged -= UpdateStringBomb;
    }

    void UpdateStringPlayer(string s)
    {
        localizedPlayerText = s;
    }

    void UpdateStringClosed(string s)
    {
        localizedClosedText = s;
    }

    void UpdateStringRanged(string s)
    {
        localizedRangedText = s;
    }

    void UpdateStringTank(string s)
    {
        localizedTankTxt = s;
    }

    void UpdateStringMonk(string s)
    {
        localizedMonkTxt = s;
    }

    void UpdateStringSpy(string s)
    {
        localizedSpyTxt = s;
    }

    void UpdateStringBomb(string s)
    {
        localizedBombTxt = s;
    }
}
