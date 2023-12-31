using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class ChangeToggleColor : MonoBehaviour
{
    private const string DogBoughtBaseKey = "dogBought";

    private Toggle toggle;
    public Text ourText;

    public DogScript forDog;

    bool bought = false;

    private void Start()
    {
        string keyBought = DogBoughtBaseKey + forDog.nameDog;

        toggle = GetComponent<Toggle>();
        toggle.onValueChanged.AddListener(OnToggleValueChanged);

        bool hasKey = LocalSave.HasBoolKey(keyBought);
        

        if (hasKey)
        {
            bought = LocalSave.GetBool(keyBought);
        }
        if (bought)
        {
            toggle.interactable = true;
        }
        else
        {
            toggle.interactable = false;
        }

        checkIfOff();

        LocalizationSettings.StringDatabase.GetLocalizedStringAsync(localizedEnable.TableReference, localizedEnable.TableEntryReference);
    }

    void Update()
    {
        if (!bought && forDog.bought)
        {
            bought = true;

            toggle.interactable = true;
        }
    }
    private void OnToggleValueChanged(bool isOn)
    {
        ColorBlock cb = toggle.colors;
        if (isOn)
        {
            cb.normalColor = Color.gray;
            cb.selectedColor = Color.gray;
            //cb.highlightedColor = Color.gray;
            ourText.text = localizedEnableText + " " + forDog.nameDog;  
        }
        else
        {
            cb.normalColor = Color.white;
            cb.selectedColor = Color.white;
            //cb.highlightedColor = Color.white;
            ourText.text = localizedDisableText + " " + forDog.nameDog;
        }
        toggle.colors = cb;
    }

    public void checkIfOff()
    {
        string keyOff = "DogOff" + forDog.nameDog;

        if (LocalSave.HasBoolKey(keyOff))
        {
            bool isOff = LocalSave.GetBool(keyOff);

            toggle.isOn = isOff;
            //toggle.SetIsOnWithoutNotify(isOff);
            //ColorBlock cb = toggle.colors;

            //if (isOff)
            //{
            //    cb.normalColor = Color.gray;
            //    cb.selectedColor = Color.gray;
            //    //cb.highlightedColor = Color.gray;
            //    ourText.text = localizedEnableText + " " + forDog.nameDog;
            //}
            //else
            //{
            //    cb.normalColor = Color.white;
            //    cb.selectedColor = Color.white;
            //    //cb.highlightedColor = Color.white;
            //    ourText.text = localizedDisableText + " " + forDog.nameDog;
            //}
        }
    }

    public LocalizedString localizedEnable;
    private string localizedEnableText = "Enable ";

    public LocalizedString localizedDisable;
    private string localizedDisableText = "Disable "; // A will be replaced by the current mode.

    void OnEnable()
    {
        toggle = GetComponent<Toggle>();

        localizedEnable.StringChanged += UpdateStringEnable;
        localizedDisable.StringChanged += UpdateStringDisable;
    }

    void OnDisable()
    {
        localizedEnable.StringChanged -= UpdateStringEnable;
        localizedDisable.StringChanged -= UpdateStringDisable;
    }

    void UpdateStringEnable(string s)
    {
        localizedEnableText = s;
    }

    void UpdateStringDisable(string s)
    {
        localizedDisableText = s;

        setupText();
    }

    void setupText()
    {
        string keyOff = "DogOff" + forDog.nameDog;

        if (LocalSave.HasBoolKey(keyOff))
        {
            bool isOff = LocalSave.GetBool(keyOff);

            if (isOff)
            {
                ourText.text = localizedEnableText + " " + forDog.nameDog;
            }
            else
            {
                ourText.text = localizedDisableText + " " + forDog.nameDog;
            }
        }
    }
}
