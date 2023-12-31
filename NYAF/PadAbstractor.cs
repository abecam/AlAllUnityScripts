using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine.InputSystem.Utilities;

public class PadAbstractor : MonoBehaviour
{
    public PadDetector thePadDetector;
    public PadMappingEditor forPadMapping;
    //private Gamepad[] allGamePads;
    //private InputUser[] allUsers;

    internal PadActionSet[] padActionsForUser = new PadActionSet[4];

    // Start is called before the first frame update
    void Awake()
    {
        //allGamePads = thePadDetector._gamepads;
        //allUsers = thePadDetector._users;
    }

    bool noPad = true;
    private ReadOnlyArray<Gamepad> allGamepads;
    private Gamepad oneGamepad;

    // Update is called once per frame
    void UpdateDeactivated()
    {
        //for (int iUser = 0; iUser < 4; iUser++)
        {
            //Gamepad oneGamepad = allGamePads[forPadMapping.forPlayer];
            allGamepads = Gamepad.all;

            if (allGamepads.Count > forPadMapping.forPlayer)
            {
                //Debug.Log("One pad found for player "+ forPadMapping.forPlayer + "! "+ allGamepads[forPadMapping.forPlayer].displayName);
                oneGamepad = allGamepads[forPadMapping.forPlayer];

                if (noPad)
                {
                    forPadMapping.padGotActivated();

                    noPad = false;

                    if (oneGamepad.enabled)
                    {
                        //Debug.Log("UUUUUUUUUUUUUUUUUUUUUUUUUUUUUUU");
                    }
                    else
                    {
                        InputSystem.EnableDevice(oneGamepad);
                    }
                }        

                // Manage mapping here?

                if (oneGamepad.aButton.wasPressedThisFrame)
                {
                    // Press
                    //forPadMapping.ltOutputAs
                    forPadMapping.ourMapping.pushAButton(PadActionSet.button.a, true);
                }
                else if (oneGamepad.aButton.wasReleasedThisFrame)
                {
                    // Unpress
                    forPadMapping.ourMapping.pushAButton(PadActionSet.button.a, false);
                }

                if (oneGamepad.bButton.wasPressedThisFrame)
                {
                    // Press
                    //forPadMapping.ltOutputAs
                    forPadMapping.ourMapping.pushAButton(PadActionSet.button.b, true);
                }
                else if (oneGamepad.bButton.wasReleasedThisFrame)
                {
                    // Unpress
                    forPadMapping.ourMapping.pushAButton(PadActionSet.button.b, false);
                }

                if (oneGamepad.xButton.wasPressedThisFrame)
                {
                    // Press
                    //forPadMapping.ltOutputAs
                    forPadMapping.ourMapping.pushAButton(PadActionSet.button.x, true);
                }
                else if (oneGamepad.xButton.wasReleasedThisFrame)
                {
                    // Unpress
                    forPadMapping.ourMapping.pushAButton(PadActionSet.button.x, false);
                }

                if (oneGamepad.yButton.wasPressedThisFrame)
                {
                    // Press
                    //forPadMapping.ltOutputAs
                    forPadMapping.ourMapping.pushAButton(PadActionSet.button.y, true);
                }
                else if (oneGamepad.yButton.wasReleasedThisFrame)
                {
                    // Unpress
                    forPadMapping.ourMapping.pushAButton(PadActionSet.button.y, false);
                }

                if (oneGamepad.leftStickButton.wasPressedThisFrame)
                {
                    // Press
                    //forPadMapping.ltOutputAs
                    forPadMapping.ourMapping.pushAButton(PadActionSet.button.leftStickButton, true);
                }
                else if (oneGamepad.leftStickButton.wasReleasedThisFrame)
                {
                    // Unpress
                    forPadMapping.ourMapping.pushAButton(PadActionSet.button.leftStickButton, false);
                }

                if (oneGamepad.rightStickButton.wasPressedThisFrame)
                {
                    // Press
                    //forPadMapping.ltOutputAs
                    forPadMapping.ourMapping.pushAButton(PadActionSet.button.rightStickButton, true);
                }
                else if (oneGamepad.rightStickButton.wasReleasedThisFrame)
                {
                    // Unpress
                    forPadMapping.ourMapping.pushAButton(PadActionSet.button.rightStickButton, false);
                }

                if (oneGamepad.leftShoulder.wasPressedThisFrame)
                {
                    // Press
                    //forPadMapping.ltOutputAs
                    forPadMapping.ourMapping.pushAButton(PadActionSet.button.lb, true);
                }
                else if (oneGamepad.leftShoulder.wasReleasedThisFrame)
                {
                    // Unpress
                    forPadMapping.ourMapping.pushAButton(PadActionSet.button.lb, false);
                }

                if (oneGamepad.rightShoulder.wasPressedThisFrame)
                {
                    // Press
                    //forPadMapping.ltOutputAs
                    forPadMapping.ourMapping.pushAButton(PadActionSet.button.rb, true);
                }
                else if (oneGamepad.rightShoulder.wasReleasedThisFrame)
                {
                    // Unpress
                    forPadMapping.ourMapping.pushAButton(PadActionSet.button.rb, false);
                }

                if (oneGamepad.dpad.up.wasPressedThisFrame)
                {
                    // Press
                    //forPadMapping.ltOutputAs
                    forPadMapping.ourMapping.pushAButton(PadActionSet.button.cross_up, true);
                }
                else if (oneGamepad.dpad.up.wasReleasedThisFrame)
                {
                    // Unpress
                    forPadMapping.ourMapping.pushAButton(PadActionSet.button.cross_up, false);
                }

                if (oneGamepad.dpad.right.wasPressedThisFrame)
                {
                    // Press
                    //forPadMapping.ltOutputAs
                    forPadMapping.ourMapping.pushAButton(PadActionSet.button.cross_right, true);
                }
                else if (oneGamepad.dpad.right.wasReleasedThisFrame)
                {
                    // Unpress
                    forPadMapping.ourMapping.pushAButton(PadActionSet.button.cross_right, false);
                }

                if (oneGamepad.dpad.left.wasPressedThisFrame)
                {
                    // Press
                    //forPadMapping.ltOutputAs
                    forPadMapping.ourMapping.pushAButton(PadActionSet.button.cross_left, true);
                }
                else if (oneGamepad.dpad.left.wasReleasedThisFrame)
                {
                    // Unpress
                    forPadMapping.ourMapping.pushAButton(PadActionSet.button.cross_left, false);
                }

                if (oneGamepad.dpad.down.wasPressedThisFrame)
                {
                    // Press
                    //forPadMapping.ltOutputAs
                    forPadMapping.ourMapping.pushAButton(PadActionSet.button.cross_down, true);
                }
                else if (oneGamepad.dpad.down.wasReleasedThisFrame)
                {
                    // Unpress
                    forPadMapping.ourMapping.pushAButton(PadActionSet.button.cross_down, false);
                }

                if (oneGamepad.selectButton.wasPressedThisFrame)
                {
                    Debug.Log("Button pressed!");
                    // Press
                    //forPadMapping.ltOutputAs
                    forPadMapping.ourMapping.pushAButton(PadActionSet.button.back, true);
                }
                else if (oneGamepad.selectButton.wasReleasedThisFrame)
                {
                    // Unpress
                    forPadMapping.ourMapping.pushAButton(PadActionSet.button.back, false);
                }

                if (oneGamepad.startButton.wasPressedThisFrame)
                {
                    // Press
                    //forPadMapping.ltOutputAs
                    forPadMapping.ourMapping.pushAButton(PadActionSet.button.start, true);
                }
                else if (oneGamepad.startButton.wasReleasedThisFrame)
                {
                    // Unpress
                    forPadMapping.ourMapping.pushAButton(PadActionSet.button.start, false);
                }

                // Get the axes values
                Vector2 leftStickValues = oneGamepad.leftStick.ReadValue();

                forPadMapping.ourMapping.moveAStick(PadActionSet.axes.left_stick, leftStickValues);

                Vector2 rightStickValues = oneGamepad.rightStick.ReadValue();

                forPadMapping.ourMapping.moveAStick(PadActionSet.axes.right_stick, rightStickValues);

                Vector2 dpadValues = oneGamepad.dpad.ReadValue();

                forPadMapping.ourMapping.moveAStick(PadActionSet.axes.dpad, dpadValues);
                // Also above ???

                // And the triggers values
            }
            else if (!noPad)
            {
                forPadMapping.padGotDeactivated();

                noPad = true;
            }
        }
    }

    internal void registerForAxesAction(PadActionSet.axes ourDefaultAction, PadButtonGridSelector padButtonGridSelector)
    {
        forPadMapping.registerForAxesAction(ourDefaultAction, padButtonGridSelector);
    }

    internal void registerForButtonAction(PadActionSet.button ourDefaultAction, GetPadAction getPadAction)
    {
        // And the pad mapping editor can inform of a change
        forPadMapping.registerForButtonAction(ourDefaultAction, getPadAction);
    }
}
