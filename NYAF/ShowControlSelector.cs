using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowControlSelector : MonoBehaviour
{
    public GameObject theSelector;
    public PadMappingEditor theEditor;

    private SelectNewControl theSelectorScript;

    public SelectNewControl.typeOfControl forType = SelectNewControl.typeOfControl.button;
    [TextArea]
    public string withText;
    public PadActionSet.button forButton;
    public PadActionSet.axes forAxes;
    public PadActionSet.floatValues forFloatValues;

    public ShowPadImage ourShower;

    private PadActionSet.button initialButton;
    private PadActionSet.axes initialAxes;
    private PadActionSet.floatValues initialFloatValues;

    public void restoreDefault()
    {
        forButton = initialButton;
        forAxes = initialAxes;
        forFloatValues = initialFloatValues;

        // Now to save the value again... :(
        if (forType == SelectNewControl.typeOfControl.button)
        {
            theEditor.selectMappingForButton(forButton, forButton);
        }
        else if (forType == SelectNewControl.typeOfControl.stick)
        {
            theEditor.selectMappingForAxes(forAxes, forAxes);
        }
        else if (forType == SelectNewControl.typeOfControl.trigger)
        {
            theEditor.selectMappingForFloatValue(forFloatValues, forFloatValues);
        }

        updateOurShower(forType, forButton, forAxes, forFloatValues);
    }
    private void Awake()
    {
        initialButton = forButton;
        initialAxes = forAxes;
        initialFloatValues = forFloatValues;

        //withText = "Press the button\nyou want to use";
        theEditor.loadLastMappings(); // Not ideal here, but we need to make sure it is loaded.

        if (forType == SelectNewControl.typeOfControl.button)
        {
            forButton = theEditor.ourMapping.mappingForButton[forButton];
        }
        else if (forType == SelectNewControl.typeOfControl.stick)
        {
            forAxes = theEditor.ourMapping.mappingForAxes[forAxes];
        }
        else if (forType == SelectNewControl.typeOfControl.trigger)
        {
            forFloatValues = theEditor.ourMapping.mappingForFloatValues[forFloatValues];
        }

        updateOurShower(forType, forButton, forAxes, forFloatValues);
    }

    public void showTheSelector()
    {
        Debug.Log("Selector should appear!");
        theSelector.SetActive(true);
        theSelectorScript = theSelector.GetComponent<SelectNewControl>();
        theSelectorScript.setUsUp(forType, withText, forButton, forAxes, forFloatValues, this);
    }

    internal void updateOurShower(SelectNewControl.typeOfControl forType, PadActionSet.button forButton, PadActionSet.axes forAxes, PadActionSet.floatValues forFloatValues)
    {
        if (forType == SelectNewControl.typeOfControl.button)
        {
            ourShower.typeOfControlButton = forButton;
        }
        else if (forType == SelectNewControl.typeOfControl.stick)
        {
            ourShower.typeOfControlAxes = forAxes;
        }
        else if (forType == SelectNewControl.typeOfControl.trigger)
        {
            ourShower.typeOfControlFloatValue = forFloatValues;
        }

        ourShower.setUpRightImage();
    }
}
