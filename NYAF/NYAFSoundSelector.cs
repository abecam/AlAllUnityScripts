using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NYAFSoundSelector : MonoBehaviour
{
    private CreateBoard mainScript;
    private Dropdown ourToggle;

    // Start is called before the first frame update
    void Start()
    {
        mainScript = Camera.main.GetComponent<CreateBoard>();
        ourToggle = GetComponent<Dropdown>();

        ourToggle.onValueChanged.AddListener(OnTargetToggleValueChanged);

        bool haskey;

        // Check our value
        haskey = LocalSave.HasIntKey("typeOfSounds");
        if (haskey)
        {
            ourToggle.SetValueWithoutNotify(LocalSave.GetInt("typeOfSounds"));
        }
    }

    void OnTargetToggleValueChanged(int toggleValue)
    {
        mainScript.SetTypeOfSound(toggleValue);
    }
}
