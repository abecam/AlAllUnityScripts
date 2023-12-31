using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    }

    int iTurn = 0;

    // Update is called once per frame
    void Update()
    {
        if (iTurn++ % 20 == 0)
        { 
            level.text = "Level " + mainScript.getCurrentLevel();
        }
    }

    public void manualUpdate()
    {
        level.text = "Level " + mainScript.getCurrentLevel();
    }
}
