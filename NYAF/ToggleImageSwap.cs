using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class ToggleImageSwap : MonoBehaviour
{
    //First, we get the target Toggle Component. 
    //Even if you don't select the target Toggle component here, the script get the one one the same object.
    public Toggle targetToggle;

    //This means that we want to actually "Swap" the target image which is set in Toggle component. 
    //By default, it is the "Background".
    //You may want to use this while "Checkmark" image set to "none" for traditional effect.
    public bool swapTogglesTargetGraphic = true;
    public Sprite swapSprite;

    //This means that we want to "Enable" a different image instead of "Checkmark" image when the toggle is unchecked.
    //You need to create a new image to use this, you can just duplicate Checkmark image and change it.
    public bool enableUncheckedGraphic = false;
    public Graphic uncheckedGraphic;

    void Start()
    {
        if (targetToggle == null)
            targetToggle = GetComponent<Toggle>();
        targetToggle.onValueChanged.AddListener(OnTargetToggleValueChanged);
        targetToggle.toggleTransition = Toggle.ToggleTransition.None;
        if (uncheckedGraphic != null)
            uncheckedGraphic.CrossFadeAlpha(targetToggle.isOn ? 0f : 1f, 0f, true);
    }

    void OnTargetToggleValueChanged(bool toggleValue)
    {
        if (swapTogglesTargetGraphic)
        {
            Image targetImage = targetToggle.targetGraphic as Image;
            if (targetImage != null)
            {
                if (toggleValue)
                    targetImage.overrideSprite = swapSprite;
                else
                    targetImage.overrideSprite = null;
            }
        }
        if (enableUncheckedGraphic)
        {
            uncheckedGraphic.CrossFadeAlpha(toggleValue ? 0f : 1f, 0f, true);
        }
    }
}