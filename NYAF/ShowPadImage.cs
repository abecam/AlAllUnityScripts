using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowPadImage : MonoBehaviour
{
    public SelectNewControl.typeOfControl forType = SelectNewControl.typeOfControl.button;

    public PadActionSet.button typeOfControlButton;
    public PadActionSet.axes typeOfControlAxes;
    public PadActionSet.floatValues typeOfControlFloatValue;

    // Start is called before the first frame update
    void Start()
    {
        setUpRightImage();
    }

    internal void setUpRightImage()
    {
        // Check which image we show from our children
        foreach (Transform oneChildren in transform)
        {
            GivePadInfo onePadInfo = oneChildren.GetComponent<GivePadInfo>();
            if (forType == SelectNewControl.typeOfControl.button)
            {
                if (onePadInfo.typeOfControlButton == typeOfControlButton)
                {
                    oneChildren.gameObject.SetActive(true);
                }
                else
                {
                    oneChildren.gameObject.SetActive(false);
                }
            }
            else if (forType == SelectNewControl.typeOfControl.stick)
            {
                if (onePadInfo.typeOfControlAxes == typeOfControlAxes)
                {
                    oneChildren.gameObject.SetActive(true);
                }
                else
                {
                    oneChildren.gameObject.SetActive(false);
                }
            }
            else if (forType == SelectNewControl.typeOfControl.trigger)
            {
                if (onePadInfo.typeOfControlFloatValue == typeOfControlFloatValue)
                {
                    oneChildren.gameObject.SetActive(true);
                }
                else
                {
                    oneChildren.gameObject.SetActive(false);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
