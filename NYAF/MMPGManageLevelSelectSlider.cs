using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MMPGManageLevelSelectSlider : MonoBehaviour
{
    public Slider ourLevelSlide;
    public Text sliderText;

    private ManageNYPB mainScript;

    // Start is called before the first frame update
    void Start()
    {
        // Get the max level and set up the max in the slide from that
        GameObject mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        mainScript = mainCamera.GetComponent<ManageNYPB>();

        int maxLevel = mainScript.MaxLevel;

        ourLevelSlide.maxValue = maxLevel;
    }

    // Update is called once per frame
    void Update()
    {
        sliderText.text = ""+(int )ourLevelSlide.value;
    }
}
