using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PadButtonGridSelector : MonoBehaviour
{
    //public ShowPadImage.currentControl withType = ShowPadImage.currentControl.full_cross;

    enum actionInGrid
    {
        up, down, right, left
    }

    public PadAbstractor thePadAbstractor;

    public string ourName = "";

    public ShowPadImage ourShower;

    private PadActionSet.axes ourDefaultAction;
    private PadActionSet.axes ourMappedAction;

    public Button startButton; // From this button, we will use the axes command to go to the adjacent ones.
    public Toggle startToggle;

    private bool isToggle = true;

    private Button nextButton;
    private Toggle nextToggle;

    // Find the right button to listen too

    // Listen to a PadActionSet from the current PadMapping!

    // Start is called before the first frame update
    void Start()
    {
        if (ourShower.forType != SelectNewControl.typeOfControl.stick)
        {
            Debug.LogError("PadButtonGridSelector is for axes only, check the Shower");
        }
        ourDefaultAction = ourShower.typeOfControlAxes;

        thePadAbstractor.registerForAxesAction(ourDefaultAction, this);

        updateOurGfx();

        // And hide the shower until it is activated
        ourShower.gameObject.SetActive(false);
    }

    public void getAxeInfo(PadActionSet.axes axesPressed, Vector2 lastValue)
    {
        //Debug.Log("Stick has moved : " + lastValue);
        // Our action is called here
        if (lastValue.x > 0.5)
        {
            nextToggle.isOn = false;
            // Go to right element
            nextToggle = (Toggle )nextToggle.FindSelectableOnRight();

            nextToggle.isOn = true;
            nextToggle.Select();

            Debug.Log("New toggle selected is " + nextToggle.name);
        }
        else if (lastValue.x < -0.5)
        {
            nextToggle.isOn = false;
            // Go to left element
            nextToggle = (Toggle)nextToggle.FindSelectableOnLeft();

            nextToggle.isOn = true;
            nextToggle.Select();

            Debug.Log("New toggle selected is " + nextToggle.name);
        }
        else if (lastValue.y > 0.5)
        {
            nextToggle.isOn = false;
            // Go to down element
            nextToggle = (Toggle)nextToggle.FindSelectableOnDown();

            nextToggle.isOn = true;
            nextToggle.Select();

            Debug.Log("New toggle selected is " + nextToggle.name);
        }
        else if (lastValue.y < -0.5)
        {
            nextToggle.isOn = false;
            // Go to up element
            nextToggle = (Toggle)nextToggle.FindSelectableOnUp();

            nextToggle.isOn = true;
            nextToggle.Select();

            Debug.Log("New toggle selected is " + nextToggle.name);
        }

       
    }

    public string getOurName()
    {
        return ourName;
    }

    public void updateOurGfx()
    {
        // Find from the shower which button we are, then update the shower from the mapping.
        ourMappedAction = thePadAbstractor.forPadMapping.ourMapping.mappingForAxes[ourDefaultAction];

        ourShower.typeOfControlAxes = ourMappedAction;

        ourShower.setUpRightImage();
    }

    public void padActived()
    {
        ourShower.gameObject.SetActive(true);

        nextToggle = startToggle;

        nextToggle.isOn = true;
    }

    public void padDeactivated()
    {
        ourShower.gameObject.SetActive(false);
    }
}
