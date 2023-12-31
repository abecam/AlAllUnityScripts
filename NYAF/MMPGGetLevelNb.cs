using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class MMPGGetLevelNb : MonoBehaviour
{
    private Text level;
    private ManageNYPB mainScript;

    // Start is called before the first frame update
    void Start()
    {
        GameObject mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        mainScript = mainCamera.GetComponent<ManageNYPB>();
        level = GetComponent<Text>();

        LocalizationSettings.StringDatabase.GetLocalizedStringAsync(localizedLevel.TableReference, localizedLevel.TableEntryReference);
    }

    int iTurn = 0;

    // Update is called once per frame
    void Update()
    {
        if (iTurn++ % 20 == 0)
        { 
            level.text = localizedLevelText + " " + mainScript.getCurrentLevel();
        }
    }

    public void manualUpdate()
    {
        level.text = localizedLevelText + " " + mainScript.getCurrentLevel();
    }

    public LocalizedString localizedLevel;
    private string localizedLevelText = "Level";

    void OnEnable()
    {
        localizedLevel.StringChanged += UpdateStringPlayer;
    }

    void OnDisable()
    {
        localizedLevel.StringChanged -= UpdateStringPlayer;
    }

    void UpdateStringPlayer(string s)
    {
        localizedLevelText = s;
    }
}
