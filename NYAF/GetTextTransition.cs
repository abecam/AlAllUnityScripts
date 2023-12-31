using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetTextTransition : MonoBehaviour
{
    // Day 1 - 5 am

    private TextMesh level;
    private BJSMainLoop mainScript;

    // Start is called before the first frame update
    void Start()
    {
        GameObject mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        mainScript = mainCamera.GetComponent<BJSMainLoop>();
        level = GetComponent<TextMesh>();
    }

    // Update is called once per frame
    void Update()
    {
        level.text = "Day " + mainScript.getCurrentDay() + "\n5 am";
    }
}
