using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PadMappingEditor : MonoBehaviour
{
    List<IButtonReceiver> allPadActions;
    List<PadButtonGridSelector> allAxesActions;

    internal PadMapping ourMapping = new PadMapping();

    public int forPlayer = 0;
    public string forGame = "";

    public PadMappingEditor()
    {
        allPadActions = new List<IButtonReceiver>();
        allAxesActions = new List<PadButtonGridSelector>();
    }

    private void Awake()
    {
        loadLastMappings();
    }
    // Start is called before the first frame update
    void Start()
    {    
    }

    internal void loadLastMappings()
    {
        if (LocalSave.HasIntKey(forGame+"-"+forPlayer+"-ButtonA"))
        {
            ourMapping.mappingForButton[PadActionSet.button.a] = (PadActionSet.button)LocalSave.GetInt(forGame + "-" + forPlayer + "-ButtonA");
        }
        if (LocalSave.HasIntKey(forGame + "-" + forPlayer + "-ButtonB"))
        {
            ourMapping.mappingForButton[PadActionSet.button.b] = (PadActionSet.button)LocalSave.GetInt(forGame + "-" + forPlayer + "-ButtonB");
        }
        if (LocalSave.HasIntKey(forGame + "-" + forPlayer + "-ButtonX"))
        {
            ourMapping.mappingForButton[PadActionSet.button.x] = (PadActionSet.button)LocalSave.GetInt(forGame + "-" + forPlayer + "-ButtonX");
        }
        if (LocalSave.HasIntKey(forGame + "-" + forPlayer + "-ButtonY"))
        {
            ourMapping.mappingForButton[PadActionSet.button.y] = (PadActionSet.button)LocalSave.GetInt(forGame + "-" + forPlayer + "-ButtonY");
        }
        if (LocalSave.HasIntKey(forGame + "-" + forPlayer + "-ButtonRB"))
        {
            ourMapping.mappingForButton[PadActionSet.button.rb] = (PadActionSet.button)LocalSave.GetInt(forGame + "-" + forPlayer + "-ButtonRB");
        }
        if (LocalSave.HasIntKey(forGame + "-" + forPlayer + "-ButtonLB"))
        {
            ourMapping.mappingForButton[PadActionSet.button.lb] = (PadActionSet.button)LocalSave.GetInt(forGame + "-" + forPlayer + "-ButtonLB");
        }
        if (LocalSave.HasIntKey(forGame + "-" + forPlayer + "-ButtonBack"))
        {
            ourMapping.mappingForButton[PadActionSet.button.back] = (PadActionSet.button)LocalSave.GetInt(forGame + "-" + forPlayer + "-ButtonBack");
        }
        if (LocalSave.HasIntKey(forGame + "-" + forPlayer + "-ButtonStart"))
        {
            ourMapping.mappingForButton[PadActionSet.button.start] = (PadActionSet.button)LocalSave.GetInt(forGame + "-" + forPlayer + "-ButtonStart");
        }
        if (LocalSave.HasIntKey(forGame + "-" + forPlayer + "-ButtonLeftStick"))
        {
            ourMapping.mappingForButton[PadActionSet.button.leftStickButton] = (PadActionSet.button)LocalSave.GetInt(forGame + "-" + forPlayer + "-ButtonLeftStick");
        }
        if (LocalSave.HasIntKey(forGame + "-" + forPlayer + "-ButtonRightStick"))
        {
            ourMapping.mappingForButton[PadActionSet.button.rightStickButton] = (PadActionSet.button)LocalSave.GetInt(forGame + "-" + forPlayer + "-ButtonRightStick");
        }
        if (LocalSave.HasIntKey(forGame + "-" + forPlayer + "-ButtonDPadUp"))
        {
            ourMapping.mappingForButton[PadActionSet.button.cross_up] = (PadActionSet.button)LocalSave.GetInt(forGame + "-" + forPlayer + "-ButtonDPadUp");
        }
        if (LocalSave.HasIntKey(forGame + "-" + forPlayer + "-ButtonDPadDown"))
        {
            ourMapping.mappingForButton[PadActionSet.button.cross_down] = (PadActionSet.button)LocalSave.GetInt(forGame + "-" + forPlayer + "-ButtonDPadDown");
        }
        if (LocalSave.HasIntKey(forGame + "-" + forPlayer + "-ButtonDPadLeft"))
        {
            ourMapping.mappingForButton[PadActionSet.button.cross_left] = (PadActionSet.button)LocalSave.GetInt(forGame + "-" + forPlayer + "-ButtonDPadLeft");
        }
        if (LocalSave.HasIntKey(forGame + "-" + forPlayer + "-ButtonDPadRight"))
        {
            ourMapping.mappingForButton[PadActionSet.button.cross_right] = (PadActionSet.button)LocalSave.GetInt(forGame + "-" + forPlayer + "-ButtonDPadRight");
        }
        if (LocalSave.HasIntKey(forGame + "-" + forPlayer + "-stickLeft"))
        {
            ourMapping.mappingForAxes[PadActionSet.axes.left_stick] = (PadActionSet.axes)LocalSave.GetInt(forGame + "-" + forPlayer + "-stickLeft");
        }
        if (LocalSave.HasIntKey(forGame + "-" + forPlayer + "-stickRight"))
        {
            ourMapping.mappingForAxes[PadActionSet.axes.right_stick] = (PadActionSet.axes)LocalSave.GetInt(forGame + "-" + forPlayer + "-stickRight");
        }
        if (LocalSave.HasIntKey(forGame + "-" + forPlayer + "-dpad"))
        {
            ourMapping.mappingForAxes[PadActionSet.axes.dpad] = (PadActionSet.axes)LocalSave.GetInt(forGame + "-" + forPlayer + "-dpad");
        }
        if (LocalSave.HasIntKey(forGame + "-" + forPlayer + "-leftTrigger"))
        {
            ourMapping.mappingForFloatValues[PadActionSet.floatValues.lt] = (PadActionSet.floatValues)LocalSave.GetInt(forGame + "-" + forPlayer + "-leftTrigger");
        }
        if (LocalSave.HasIntKey(forGame + "-" + forPlayer + "-rightTrigger"))
        {
            ourMapping.mappingForFloatValues[PadActionSet.floatValues.rt] = (PadActionSet.floatValues)LocalSave.GetInt(forGame + "-" + forPlayer + "-rightTrigger");
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public void selectMappingForButton(PadActionSet.button aNewButton, PadActionSet.button forButton)
    {
        ourMapping.mappingForButton[forButton] = aNewButton;

        switch (forButton)
        {
            case PadActionSet.button.a:
                selectMappingForA(aNewButton);
                break;
            case PadActionSet.button.b:
                selectMappingForB(aNewButton);
                break;
            case PadActionSet.button.x:
                selectMappingForX(aNewButton);
                break;
            case PadActionSet.button.y:
                selectMappingForY(aNewButton);
                break;
            case PadActionSet.button.lb:
                selectMappingForLB(aNewButton);
                break;
            case PadActionSet.button.rb:
                selectMappingForRB(aNewButton);
                break;
            case PadActionSet.button.back:
                selectMappingForBack(aNewButton);
                break;
            case PadActionSet.button.start:
                selectMappingForStart(aNewButton);
                break;
            case PadActionSet.button.cross_up:
                selectMappingForDPadUp(aNewButton);
                break;
            case PadActionSet.button.cross_down:
                selectMappingForDPadDown(aNewButton);
                break;
            case PadActionSet.button.cross_right:
                selectMappingForDPadRight(aNewButton);
                break;
            case PadActionSet.button.cross_left:
                selectMappingForDPadLeft(aNewButton);
                break;
            case PadActionSet.button.leftStickButton:
                selectMappingForLeftStickPush(aNewButton);
                break;
            case PadActionSet.button.rightStickButton:
                selectMappingForRightStickPush(aNewButton);
                break;
        }

        LocalSave.Save();

        updateButtonReceivers();
    }

    private void updateButtonReceivers()
    {
        foreach (IButtonReceiver oneButtonReceiver in allPadActions)
        {
            oneButtonReceiver.updateOurGfx();
        }
    }

    internal void padGotActivated()
    {
        foreach (IButtonReceiver oneButtonReceiver in allPadActions)
        {
            oneButtonReceiver.padActived();
        }
        foreach (PadButtonGridSelector oneAxeReceiver in allAxesActions)
        {
            oneAxeReceiver.padActived();
        }
    }  

    internal void padGotDeactivated()
    {
        foreach (IButtonReceiver oneButtonReceiver in allPadActions)
        {
            oneButtonReceiver.padDeactivated();
        }
        foreach (PadButtonGridSelector oneAxeReceiver in allAxesActions)
        {
            oneAxeReceiver.padDeactivated();
        }
    }

    public void selectMappingForAxes(PadActionSet.axes aNewAxe, PadActionSet.axes forAxe)
    {
        ourMapping.mappingForAxes[forAxe] = aNewAxe;

        switch (forAxe)
        {
            case PadActionSet.axes.left_stick:
                selectMappingForLeftStick(aNewAxe);
                break;
            case PadActionSet.axes.right_stick:
                selectMappingForRightStick(aNewAxe);
                break;
            case PadActionSet.axes.dpad:
                selectMappingForDPad(aNewAxe);
                break;
        }
        LocalSave.Save();
    }

    internal void registerForButtonAction(PadActionSet.button ourDefaultAction, GetPadAction getPadAction)
    {
        ourMapping.registerForButtonAction(ourDefaultAction, getPadAction);

        allPadActions.Add(getPadAction);
    }

    internal void registerForAxesAction(PadActionSet.axes ourDefaultAction, PadButtonGridSelector padButtonGridSelector)
    {
        ourMapping.registerForAxesAction(ourDefaultAction, padButtonGridSelector);

        allAxesActions.Add(padButtonGridSelector);
    }
    public void selectMappingForFloatValue(PadActionSet.floatValues aNewFV, PadActionSet.floatValues forFV)
    {
        ourMapping.mappingForFloatValues[forFV] = aNewFV;

        switch (forFV)
        {
            case PadActionSet.floatValues.lt:
                selectMappingForLT(aNewFV);
                break;
            case PadActionSet.floatValues.rt:
                selectMappingForRT(aNewFV);
                break;
        }
        LocalSave.Save();
    }

    public void selectMappingForA(PadActionSet.button buttonForA)
    {
        LocalSave.SetInt(forGame + "-" + forPlayer + "-ButtonA", (int )buttonForA);
    }

    public void selectMappingForB(PadActionSet.button buttonForB)
    {
        LocalSave.SetInt(forGame + "-" + forPlayer + "-ButtonB", (int)buttonForB);
    }

    public void selectMappingForX(PadActionSet.button buttonForX)
    {
        LocalSave.SetInt(forGame + "-" + forPlayer + "-ButtonX", (int)buttonForX);
    }

    public void selectMappingForY(PadActionSet.button buttonForY)
    {
        LocalSave.SetInt(forGame + "-" + forPlayer + "-ButtonY", (int)buttonForY);
    }

    public void selectMappingForRB(PadActionSet.button buttonForRB)
    {
        LocalSave.SetInt(forGame + "-" + forPlayer + "-ButtonRB", (int)buttonForRB);
    }

    public void selectMappingForLB(PadActionSet.button buttonForLB)
    {
        LocalSave.SetInt(forGame + "-" + forPlayer + "-ButtonLB", (int)buttonForLB);
    }

    public void selectMappingForBack(PadActionSet.button buttonForBack)
    {
        LocalSave.SetInt(forGame + "-" + forPlayer + "-ButtonBack", (int)buttonForBack);
    }

    public void selectMappingForStart(PadActionSet.button buttonForStart)
    {
        LocalSave.SetInt(forGame + "-" + forPlayer + "-ButtonStart", (int)buttonForStart);
    }

    public void selectMappingForLeftStickPush(PadActionSet.button buttonForLeftStickPush)
    {
        LocalSave.SetInt(forGame + "-" + forPlayer + "-ButtonLeftStick", (int)buttonForLeftStickPush);
    }

    public void selectMappingForRightStickPush(PadActionSet.button buttonForRightStickPush)
    {
        LocalSave.SetInt(forGame + "-" + forPlayer + "-ButtonRightStick", (int)buttonForRightStickPush);
    }

    public void selectMappingForDPadUp(PadActionSet.button buttonForDPadUp)
    {
        LocalSave.SetInt(forGame + "-" + forPlayer + "-ButtonDPadUp", (int)buttonForDPadUp);
    }

    public void selectMappingForDPadDown(PadActionSet.button buttonForDPadDown)
    {
        LocalSave.SetInt(forGame + "-" + forPlayer + "-ButtonDPadDown", (int)buttonForDPadDown);
    }

    public void selectMappingForDPadLeft(PadActionSet.button buttonForDPadLeft)
    {
        LocalSave.SetInt(forGame + "-" + forPlayer + "-ButtonDPadLeft", (int)buttonForDPadLeft);
    }

    public void selectMappingForDPadRight(PadActionSet.button buttonForDPadRight)
    {
        LocalSave.SetInt(forGame + "-" + forPlayer + "-ButtonDPadRight", (int)buttonForDPadRight);
    }

    public void selectMappingForLeftStick(PadActionSet.axes stickForLeftStick)
    {
        LocalSave.SetInt(forGame + "-" + forPlayer + "-stickLeft", (int)stickForLeftStick);
    }

    public void selectMappingForRightStick(PadActionSet.axes stickForRightStick)
    {
        LocalSave.SetInt(forGame + "-" + forPlayer + "-stickRight", (int)stickForRightStick);
    }

    public void selectMappingForDPad(PadActionSet.axes stickForDPad)
    {
        LocalSave.SetInt(forGame + "-" + forPlayer + "-dpad", (int)stickForDPad);
    }
    public void selectMappingForLT(PadActionSet.floatValues triggerForLeftTrigger)
    {
        LocalSave.SetInt(forGame + "-" + forPlayer + "-leftTrigger", (int)triggerForLeftTrigger);
    }

    public void selectMappingForRT(PadActionSet.floatValues triggerForRightTrigger)
    {
        LocalSave.SetInt(forGame + "-" + forPlayer + "-rightTrigger", (int)triggerForRightTrigger);
    }
}
