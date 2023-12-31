using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine.UI;

public class SelectNewControl : MonoBehaviour
{
    public Text ourText;
    public ShowPadImage ourShower;
    public PadMappingEditor ourMappingEditor;
    public PadDetector thePadDetector;
    private ShowControlSelector forOurCaller;

    private Gamepad[] allGamePads;
    private InputUser[] allUsers;

    private bool isSetup = false;

    public enum typeOfControl
    {
        button, stick, trigger
    }

    private typeOfControl forWhichType = typeOfControl.button;
    private PadActionSet.button forWhichButton;
    private PadActionSet.axes forWhichAxes;
    private PadActionSet.floatValues forWhichFloatValues;

    private PadActionSet.button withButton;
    private PadActionSet.axes withAxes;
    private PadActionSet.floatValues withFloatValues;

    private bool hasBeenChosen = false;

    // Start is called before the first frame update
    void Start()
    {
        allGamePads = thePadDetector._gamepads;
        allUsers = thePadDetector._users;
    }

    // Update is called once per frame
    void Update()
    {
        if (isSetup)
        {
            //for (int iUser = 0; iUser < 4; iUser++)
            {
                Gamepad oneGamepad = allGamePads[ourMappingEditor.forPlayer];

                if (oneGamepad != null)
                {
                    // Manage mapping here?
                    if (forWhichType == typeOfControl.button)
                    {
                        Debug.Log("Listening for user " + ourMappingEditor.forPlayer + " and button");
                        mapAButton(oneGamepad);
                    }
                    else if (forWhichType == typeOfControl.stick)
                    {
                        Debug.Log("Listening for user " + ourMappingEditor.forPlayer + " and stick");
                        mapAnAxes(oneGamepad);
                    }
                    else if (forWhichType == typeOfControl.trigger)
                    {
                        Debug.Log("Listening for user " + ourMappingEditor.forPlayer + " and trigger");
                        mapAFloatValue(oneGamepad);
                    }
                }
            }

            if (hasBeenChosen)
            {
                forOurCaller.updateOurShower(forWhichType, withButton, withAxes, withFloatValues);

                gameObject.SetActive(false);
            }
        }
    }

    private void mapAFloatValue(Gamepad oneGamepad)
    {
        float leftTriggerValue = oneGamepad.leftTrigger.ReadValue();
        float rightTriggerValue = oneGamepad.rightTrigger.ReadValue();

        if (leftTriggerValue > 0.4f)
        {
            ourMappingEditor.selectMappingForFloatValue(PadActionSet.floatValues.lt, forWhichFloatValues);

            withFloatValues = PadActionSet.floatValues.lt;

            hasBeenChosen = true;
        }
        else if (rightTriggerValue > 0.4f)
        {
            ourMappingEditor.selectMappingForFloatValue(PadActionSet.floatValues.rt, forWhichFloatValues);

            withFloatValues = PadActionSet.floatValues.rt;

            hasBeenChosen = true;
        }
    }

    private void mapAnAxes(Gamepad oneGamepad)
    {
        Vector2 leftStickMove = oneGamepad.leftStick.ReadValue();
        Vector2 rightStickMove = oneGamepad.rightStick.ReadValue();
        Vector2 dpadStickMove = oneGamepad.dpad.ReadValue();

        if (leftStickMove.magnitude > 0.4f)
        {
            ourMappingEditor.selectMappingForAxes(PadActionSet.axes.left_stick, forWhichAxes);

            withAxes = PadActionSet.axes.left_stick;

            hasBeenChosen = true;
        }
        else if (rightStickMove.magnitude > 0.4f)
        {
            ourMappingEditor.selectMappingForAxes(PadActionSet.axes.right_stick, forWhichAxes);

            withAxes = PadActionSet.axes.right_stick;

            hasBeenChosen = true;
        }
        else if (dpadStickMove.magnitude > 0.4f)
        {
            ourMappingEditor.selectMappingForAxes(PadActionSet.axes.dpad, forWhichAxes);

            withAxes = PadActionSet.axes.dpad;

            hasBeenChosen = true;
        }
    }

    private void mapAButton(Gamepad oneGamepad)
    {
        if (oneGamepad.aButton.wasPressedThisFrame)
        {
            // In button selection, select the button
            assignNewButton(PadActionSet.button.a);
        }
        else if (oneGamepad.bButton.wasPressedThisFrame)
        {
            // In button selection, select the button
            assignNewButton(PadActionSet.button.b);
        }
        else if (oneGamepad.xButton.wasPressedThisFrame)
        {
            // In button selection, select the button
            assignNewButton(PadActionSet.button.x);
        }
        else if (oneGamepad.yButton.wasPressedThisFrame)
        {
            // In button selection, select the button
            assignNewButton(PadActionSet.button.y);
        }
        else if (oneGamepad.startButton.wasPressedThisFrame)
        {
            // In button selection, select the button
            assignNewButton(PadActionSet.button.start);
        }
        else if (oneGamepad.selectButton.wasPressedThisFrame)
        {
            // In button selection, select the button
            assignNewButton(PadActionSet.button.back);
        }
        else if (oneGamepad.dpad.up.wasPressedThisFrame)
        {
            // In button selection, select the button
            assignNewButton(PadActionSet.button.cross_up);
        }
        else if (oneGamepad.dpad.down.wasPressedThisFrame)
        {
            // In button selection, select the button
            assignNewButton(PadActionSet.button.cross_down);
        }
        else if (oneGamepad.dpad.left.wasPressedThisFrame)
        {
            // In button selection, select the button
            assignNewButton(PadActionSet.button.cross_left);
        }
        else if (oneGamepad.dpad.right.wasPressedThisFrame)
        {
            // In button selection, select the button
            assignNewButton(PadActionSet.button.cross_right);
        }
        else if (oneGamepad.leftShoulder.wasPressedThisFrame)
        {
            // In button selection, select the button
            assignNewButton(PadActionSet.button.lb);
        }
        else if (oneGamepad.rightShoulder.wasPressedThisFrame)
        {
            // In button selection, select the button
            assignNewButton(PadActionSet.button.rb);
        }
        else if (oneGamepad.leftStickButton.wasPressedThisFrame)
        {
            // In button selection, select the button
            assignNewButton(PadActionSet.button.leftStickButton);
        }
        else if (oneGamepad.rightStickButton.wasPressedThisFrame)
        {
            // In button selection, select the button
            assignNewButton(PadActionSet.button.rightStickButton);
        }
    }

    private void assignNewButton(PadActionSet.button withNewButton)
    {
        ourMappingEditor.selectMappingForButton(withNewButton, forWhichButton);

        withButton = withNewButton;

        hasBeenChosen = true;
    }

    public void setUsUp(typeOfControl forType, string withText, PadActionSet.button forButton, PadActionSet.axes forAxes, PadActionSet.floatValues forFloatValues, ShowControlSelector theCaller)
    {
        ourText.text = withText;

        forWhichType = forType;

        forWhichButton = forButton;
        forWhichAxes = forAxes;
        forWhichFloatValues = forFloatValues;

        isSetup = true;

        forOurCaller = theCaller;

        hasBeenChosen = false;

        updateOurShower(forWhichType, forWhichButton, forWhichAxes, forWhichFloatValues);
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
