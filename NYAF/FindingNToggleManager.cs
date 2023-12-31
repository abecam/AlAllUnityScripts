using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class FindingNToggleManager : MonoBehaviour
{
    private ManageFindingNyafings mainScript;
    private Toggle ourToggle;

    public typeOfPref ourTypeOfPref;

    public enum typeOfPref
    {
        arrowsHelp, hintsHelp, transparence, isRotation
    }
    // Start is called before the first frame update
    void Start()
    {
        mainScript = Camera.main.GetComponent<ManageFindingNyafings>();

        ourToggle = GetComponent<Toggle>();

        ourToggle.onValueChanged.AddListener(OnTargetToggleValueChanged);

        bool haskey;

        // Check our value
        switch (ourTypeOfPref)
        {
            case typeOfPref.arrowsHelp:
                haskey = LocalSave.HasIntKey("FindingNYAF_showArrows");
                if (haskey)
                {
                    ourToggle.isOn = LocalSave.GetInt("FindingNYAF_showArrows") == 1;
                }
                break;
            case typeOfPref.hintsHelp:
                haskey = LocalSave.HasIntKey("FindingNYAF_showHints");
                if (haskey)
                {
                    ourToggle.isOn = LocalSave.GetInt("FindingNYAF_showHints") == 1;
                }
                break;
            case typeOfPref.transparence:
                haskey = LocalSave.HasIntKey("FindingNYAF_transparentChars");
                if (haskey)
                {
                    bool pieceTransparences = LocalSave.GetInt("FindingNYAF_transparentChars") == 1;
                    ourToggle.isOn = pieceTransparences;
                }
                break;
            case typeOfPref.isRotation:
                haskey = LocalSave.HasBoolKey("FindingNYAF_isRotation");
                if (haskey)
                {
                    ourToggle.isOn = LocalSave.GetBool("FindingNYAF_isRotation");
                }
                break;
        }
    }

    void OnTargetToggleValueChanged(bool toggleValue)
    {
        switch (ourTypeOfPref)
        {
            case typeOfPref.arrowsHelp:
                mainScript.setShowArrows(toggleValue);
                break;
            case typeOfPref.hintsHelp:
                mainScript.setShowHints(toggleValue);
                break;
            case typeOfPref.transparence:
                mainScript.setUseTransparence(toggleValue);
                break;

            case typeOfPref.isRotation:
                mainScript.setIsRotation(toggleValue);
                break;
        }
    }
}
