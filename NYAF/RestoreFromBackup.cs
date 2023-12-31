using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestoreFromBackup : MonoBehaviour
{
    private CreateBoard mainScript;
    // Start is called before the first frame update
    void Awake()
    {
        GameObject mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        mainScript = mainCamera.GetComponent<CreateBoard>();
    }

    public void DoTheRestore()
    {
        mainScript.restoreFromBackup();
    }
}
