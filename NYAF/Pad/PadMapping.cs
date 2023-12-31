using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PadMapping
{
    // Define a mapping between an Action Set and another
    public Dictionary<PadActionSet.button, PadActionSet.button> mappingForButton;
    public Dictionary<PadActionSet.button, bool> buttonPressedUnpressed;
    public Dictionary<PadActionSet.button, IButtonReceiver> allButtonActionReceiver;

    public Dictionary<PadActionSet.axes, PadActionSet.axes> mappingForAxes;
    public Dictionary<PadActionSet.axes, PadButtonGridSelector> allAxesActionReceiver;

    public Dictionary<PadActionSet.floatValues, PadActionSet.floatValues> mappingForFloatValues;

    PadActionSet outputActionSet = new PadActionSet();

    public PadMapping()
    {
        mappingForButton = new Dictionary<PadActionSet.button, PadActionSet.button>();

        mappingForButton.Add(PadActionSet.button.a, PadActionSet.button.a);
        mappingForButton.Add(PadActionSet.button.b, PadActionSet.button.b);
        mappingForButton.Add(PadActionSet.button.x, PadActionSet.button.x);
        mappingForButton.Add(PadActionSet.button.y, PadActionSet.button.y);
        mappingForButton.Add(PadActionSet.button.rb, PadActionSet.button.rb);
        mappingForButton.Add(PadActionSet.button.lb, PadActionSet.button.lb);
        mappingForButton.Add(PadActionSet.button.back, PadActionSet.button.back);
        mappingForButton.Add(PadActionSet.button.start, PadActionSet.button.start);
        mappingForButton.Add(PadActionSet.button.leftStickButton, PadActionSet.button.leftStickButton);
        mappingForButton.Add(PadActionSet.button.rightStickButton, PadActionSet.button.rightStickButton);
        mappingForButton.Add(PadActionSet.button.cross_down, PadActionSet.button.cross_down);
        mappingForButton.Add(PadActionSet.button.cross_up, PadActionSet.button.cross_up);
        mappingForButton.Add(PadActionSet.button.cross_right, PadActionSet.button.cross_right);
        mappingForButton.Add(PadActionSet.button.cross_left, PadActionSet.button.cross_left);

        foreach (PadActionSet.button oneKey in mappingForButton.Keys)
        {
            Debug.Log("This button "+oneKey+" is mapped to "+ mappingForButton[oneKey]);
        }
        mappingForAxes = new Dictionary<PadActionSet.axes, PadActionSet.axes>();

        mappingForAxes.Add(PadActionSet.axes.left_stick, PadActionSet.axes.left_stick);
        mappingForAxes.Add(PadActionSet.axes.right_stick, PadActionSet.axes.right_stick);
        mappingForAxes.Add(PadActionSet.axes.dpad, PadActionSet.axes.dpad);

        mappingForFloatValues = new Dictionary<PadActionSet.floatValues, PadActionSet.floatValues>();

        mappingForFloatValues.Add(PadActionSet.floatValues.rt, PadActionSet.floatValues.rt);
        mappingForFloatValues.Add(PadActionSet.floatValues.lt, PadActionSet.floatValues.lt);

        buttonPressedUnpressed = new Dictionary<PadActionSet.button, bool>();
        allButtonActionReceiver = new Dictionary<PadActionSet.button, IButtonReceiver>();
        allAxesActionReceiver = new Dictionary<PadActionSet.axes, PadButtonGridSelector>();
    }

    /**
     * Register for the "raw" button action (i.e. after mapping, so shouldn't change)
    */
    internal void registerForButtonAction(PadActionSet.button forButton, IButtonReceiver getPadAction)
    {
        if (allButtonActionReceiver.ContainsKey(forButton))
        {
            Debug.LogError("The button " + forButton + " is already registered for"+allButtonActionReceiver[forButton].getOurName()+"!");
        }
        else
        { 
            allButtonActionReceiver.Add(forButton, getPadAction);
        }
    }

    internal void pushAButton(PadActionSet.button buttonPressed, bool isPressed)
    {
        //if (buttonPressed == PadActionSet.button.back)
        //    Debug.Log("A- Button " + buttonPressed + " has been pressed/unpressed " + isPressed);

        PadActionSet.button pressedButton = mappingForButton[buttonPressed];

        buttonPressedUnpressed[pressedButton] = isPressed;

        //if (buttonPressed == PadActionSet.button.back)
        //    Debug.Log("A- Button " + buttonPressed + " is mapped to " + pressedButton);

        if (allButtonActionReceiver.ContainsKey(pressedButton))
        {
            //foreach (PadActionSet.button oneKey in allButtonActionReceiver.Keys)
            //{
            //    Debug.Log("CC- This button " + oneKey + " is mapped to " + allButtonActionReceiver[oneKey].getOurName());
            //}
            //if (pressedButton == PadActionSet.button.back)
            //    Debug.Log("B- Button " + buttonPressed + " has been pressed/unpressed " + isPressed);

            allButtonActionReceiver[pressedButton].getButtonPressedInfo(pressedButton, isPressed);
        }
    }

    internal void registerForAxesAction(PadActionSet.axes forAxes, PadButtonGridSelector padButtonGridSelector)
    {
        if (allAxesActionReceiver.ContainsKey(forAxes))
        {
            Debug.LogError("The button " + padButtonGridSelector + " is already registered for" + allAxesActionReceiver[forAxes].getOurName() + "!");
        }
        else
        {
            allAxesActionReceiver.Add(forAxes, padButtonGridSelector);
        }
    }

    internal void moveAStick(PadActionSet.axes forAxes, Vector2 value)
    {
        //Debug.Log("Receiving stick input for " + forAxes + ": " + value);

        PadActionSet.axes usedAxes = mappingForAxes[forAxes];

        if (allAxesActionReceiver.ContainsKey(usedAxes))
        {
            //Debug.Log("Translating stick input for " + usedAxes + ": " + value);

            allAxesActionReceiver[forAxes].getAxeInfo(usedAxes, value);
        }
    }
}
