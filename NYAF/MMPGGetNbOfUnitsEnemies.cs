using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class MMPGGetNbOfUnitsEnemies : MonoBehaviour
{
    private Text level;
    private ManageNYPB mainScript;

    // Start is called before the first frame update
    void Start()
    {
        GameObject mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        mainScript = mainCamera.GetComponent<ManageNYPB>();
        level = GetComponent<Text>();

        LocalizationSettings.StringDatabase.GetLocalizedStringAsync(localizedEnemies.TableReference, localizedEnemies.TableEntryReference);
    }

    // Update is called once per frame
    void Update()
    {
        level.text = localizedEnemiesText + "\n\n" + localizedClosedText + " " + mainScript.CurrentNbOfClosedEnemies + " / " + (mainScript.NbOfClosedEnemies>0? mainScript.NbOfClosedEnemies:0) + "\n" +
            localizedRangedText + " " + mainScript.CurrentNbOfRangedEnemies + " / " + (mainScript.NbOfRangedEnemies > 0 ? mainScript.NbOfRangedEnemies : 0) + "\n" +
            localizedTankTxt + " " + mainScript.CurrentNbOfMachinesEnemies + " / " + (mainScript.NbOfMachinesEnemies > 0 ? mainScript.NbOfMachinesEnemies : 0) + "\n";
    }


    public LocalizedString localizedEnemies;
    private string localizedEnemiesText = "Enemies";

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
        localizedEnemies.StringChanged += UpdateStringPlayer;
        localizedClosed.StringChanged += UpdateStringClosed;
        localizedRanged.StringChanged += UpdateStringRanged;
        localizedLocalizedTank.StringChanged += UpdateStringTank;
        localizedLocalizedMonk.StringChanged += UpdateStringMonk;
        localizedLocalizedSpy.StringChanged += UpdateStringSpy;
        localizedLocalizedBomb.StringChanged += UpdateStringBomb;
    }

    void OnDisable()
    {
        localizedEnemies.StringChanged -= UpdateStringPlayer;
        localizedClosed.StringChanged -= UpdateStringClosed;
        localizedRanged.StringChanged -= UpdateStringRanged;
        localizedLocalizedTank.StringChanged -= UpdateStringTank;
        localizedLocalizedMonk.StringChanged -= UpdateStringMonk;
        localizedLocalizedSpy.StringChanged -= UpdateStringSpy;
        localizedLocalizedBomb.StringChanged -= UpdateStringBomb;
    }

    void UpdateStringPlayer(string s)
    {
        localizedEnemiesText = s;
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
