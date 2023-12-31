using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestoreDefaultValueForAll : MonoBehaviour
{
    public void restoreDefaultForAllChildren()
    {
        foreach (Transform oneChildren in transform)
        {
            ShowControlSelector theShowControler = oneChildren.gameObject.GetComponent<ShowControlSelector>();

            theShowControler.restoreDefault();
        }
    }
}
