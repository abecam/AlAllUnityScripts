using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MMPGPrefs : MonoBehaviour
{
    private ManageNYPB theMainScript;

    public Toggle isRestricted;
    public Toggle hasNature;
    public Toggle hasMonster;

    // Start is called before the first frame update
    void Start()
    {
        GameObject mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        theMainScript = mainCamera.GetComponent<ManageNYPB>();

        hasMonster.SetIsOnWithoutNotify(theMainScript.HasMonster);
        hasNature.SetIsOnWithoutNotify(theMainScript.HasNature);
        isRestricted.SetIsOnWithoutNotify(!theMainScript.IsRestricted);
    }

    public void setNature(bool isNature)
    {
        LocalSave.SetInt("nypb_hasNature", isNature ? 1 : 0);

        LocalSave.Save();

        theMainScript.HasNature = isNature;
    }

    public void setMonster(bool isMonster)
    {
        LocalSave.SetInt("nypb_hasMonster", isMonster ? 1 : 0);

        LocalSave.Save();

        theMainScript.HasMonster = isMonster;
    }

    public void setRestrict(bool isRestrict)
    {
        LocalSave.SetInt("nypb_hasRestrict", isRestrict ? 0 : 1);

        LocalSave.Save();

        theMainScript.IsRestricted = !isRestrict;
    }
}
