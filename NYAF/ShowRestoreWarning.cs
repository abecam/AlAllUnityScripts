using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowRestoreWarning : MonoBehaviour
{
    public GameObject theRestoreCanvas;

    public void showWarning()
    {
        Instantiate(theRestoreCanvas);
    }
}
