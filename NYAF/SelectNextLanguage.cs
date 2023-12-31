using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;
public class SelectNextLanguage : MonoBehaviour
{
    public const string LocaleChosenKey = "ChosenLocale";
    // Start is called before the first frame update
    IEnumerator Start()
    {
        yield return LocalizationSettings.InitializationOperation;

        if (LocalSave.HasIntKey(LocaleChosenKey))
        {
            int selected = LocalSave.GetInt(LocaleChosenKey);

            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[selected];
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void goToNextLanguage()
    {
        int selected = 0;
        int nextSelected = 0;
        for (int i = 0; i < LocalizationSettings.AvailableLocales.Locales.Count; ++i)
        {
            var locale = LocalizationSettings.AvailableLocales.Locales[i];
            if (LocalizationSettings.SelectedLocale == locale)
                selected = i;
        }
        if (selected < LocalizationSettings.AvailableLocales.Locales.Count - 1)
        {
            nextSelected = selected + 1;
        }
       
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[nextSelected];
        LocalSave.SetInt(LocaleChosenKey, nextSelected);
        LocalSave.Save();
    }
}
