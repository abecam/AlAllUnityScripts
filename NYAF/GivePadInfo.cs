using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GivePadInfo : MonoBehaviour
{
    public SelectNewControl.typeOfControl forType = SelectNewControl.typeOfControl.button;

    public PadActionSet.button typeOfControlButton;
    public PadActionSet.axes typeOfControlAxes;
    public PadActionSet.floatValues typeOfControlFloatValue;
}
