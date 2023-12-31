using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class MovingNToggleManager : MonoBehaviour
{
    private ManageMovingNyaf mainScript;
    private Toggle ourToggle;

    public typeOfPref ourTypeOfPref;

    public enum typeOfPref
    {
        arrowsHelp, hintsHelp, transparence, isRotation
    }
    // Start is called before the first frame update
    void Start()
    {
        mainScript = Camera.main.GetComponent<ManageMovingNyaf>();

        ourToggle = GetComponent<Toggle>();

        ourToggle.onValueChanged.AddListener(OnTargetToggleValueChanged);

        bool haskey;

        // Check our value
        switch (ourTypeOfPref)
        {
            case typeOfPref.arrowsHelp:
                haskey = LocalSave.HasIntKey("MovingNYAF_showArrows");
                if (haskey)
                {
                    ourToggle.isOn = LocalSave.GetInt("MovingNYAF_showArrows") == 1;
                }
                break;
            case typeOfPref.hintsHelp:
                haskey = LocalSave.HasIntKey("MovingNYAF_showHints");
                if (haskey)
                {
                    ourToggle.isOn = LocalSave.GetInt("MovingNYAF_showHints") == 1;
                }
                break;
            case typeOfPref.transparence:
                haskey = LocalSave.HasIntKey("MovingNYAF_transparentChars");
                if (haskey)
                {
                    bool pieceTransparences = LocalSave.GetInt("MovingNYAF_transparentChars") == 1;
                    ourToggle.isOn = pieceTransparences;
                }
                break;
            case typeOfPref.isRotation:
                haskey = LocalSave.HasBoolKey("MovingNYAF_isRotation");
                if (haskey)
                {
                    ourToggle.isOn = LocalSave.GetBool("MovingNYAF_isRotation");
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
