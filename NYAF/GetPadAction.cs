using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GetPadAction : MonoBehaviour, IButtonReceiver
{
    public PadAbstractor thePadAbstractor;

    public string ourName = "";

    private ShowPadImage ourShower;

    private PadActionSet.button ourDefaultAction;
    private PadActionSet.button ourMappedAction;

    public Button forButton;
    public Toggle forToggle;
    // Find the right button to listen too

    // Listen to a PadActionSet from the current PadMapping!

    // Start is called before the first frame update
    void Start()
    {
        ourShower = GetComponent<ShowPadImage>();

        if (ourShower.forType != SelectNewControl.typeOfControl.button)
        {
            Debug.LogError("Get Pad Action is for button only, check the Shower");
        }
        ourDefaultAction = ourShower.typeOfControlButton;

        thePadAbstractor.registerForButtonAction(ourDefaultAction, this);

        updateOurGfx();

        // And hide the shower until it is activated
        ourShower.gameObject.SetActive(false);
    }

    public void getButtonPressedInfo(PadActionSet.button buttonPressed, bool isPressed)
    {
        if (isPressed)
        {
            Debug.Log("HHHH - Button for us is pressed " + isPressed);

            if (forButton != null)
            {
                forButton.onClick.Invoke();
            }
            if (forToggle != null)
            {
                forToggle.isOn = !forToggle.isOn;

                Debug.Log("HHHH - Triggering toggle " + forToggle.isOn);
            }
        }
    }

    public string getOurName()
    {
        return ourName;
    }

    public void updateOurGfx()
    {
        // Find from the shower which button we are, then update the shower from the mapping.
        ourMappedAction = thePadAbstractor.forPadMapping.ourMapping.mappingForButton[ourDefaultAction];

        ourShower.typeOfControlButton = ourMappedAction;

        ourShower.setUpRightImage();
    }

    public void padActived()
    {
        ourShower.gameObject.SetActive(true);
    }

    public void padDeactivated()
    {
        ourShower.gameObject.SetActive(false);
    }
}
