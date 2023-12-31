using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManageVault : MonoBehaviour
{
    //public ManageBox theBox;
    public GameObject ourGameObject;

    TextMesh ourText;
    int iZoom = 0;
    const int maxZoom = 20;

    const float zoomStep = (1f - 0.8f) / maxZoom;

    float sizeText = 0.8f;

    // Start is called before the first frame update
    void Start()
    {
        ourText = GetComponent<TextMesh>();
    }

    public void setANewNbOfEntries(int newNb)
    {
        ourText.text = newNb + "";

        iZoom = maxZoom;

        sizeText = 0.8f;

        transform.localScale = new Vector3(sizeText, 2*sizeText, sizeText);
    }

    // Update is called once per frame
    void Update()
    {
        if (iZoom > 0)
        {
            sizeText += zoomStep;
            iZoom--;
            transform.localScale = new Vector3(sizeText, 2*sizeText, sizeText);
        }
    }
}
