using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetLevelText : MonoBehaviour
{
    private Text level;
    private BJSMainLoop mainScript;

    // Start is called before the first frame update
    void Start()
    {
        GameObject mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        mainScript = mainCamera.GetComponent<BJSMainLoop>();
        level = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        level.text = "BJ is Level "+mainScript.getCurrentLevel();
    }
}
