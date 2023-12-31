using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetXPLevel : MonoBehaviour
{
    private Image xpLevelImage;
    private BJSMainLoop mainScript;
    // Start is called before the first frame update
    void Start()
    {
        GameObject mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        mainScript = mainCamera.GetComponent<BJSMainLoop>();
        xpLevelImage = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        float xpNeeded = mainScript.getNeededXP();
        float currentXp = mainScript.getCurrentXP();

        xpLevelImage.rectTransform.localScale = new Vector3(currentXp/xpNeeded, 1);
    }
}
