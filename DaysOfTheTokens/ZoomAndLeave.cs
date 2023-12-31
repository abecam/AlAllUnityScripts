using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoomAndLeave : MonoBehaviour
{
    public float myWidth = 0.2f;

    int iZoom = maxZoom;
    private float sizeText = 1;
    const int maxZoom = 600;

    float zoomStep = (1.2f - 1) / maxZoom;

    // Start is called before the first frame update
    void Start()
    {
        float ourSize = transform.localScale.x;

        zoomStep = ((ourSize) - (0.8f * ourSize)) / maxZoom;

        sizeText = 0.8f * ourSize;

        GameObject ourCopy = Instantiate(gameObject);
        ourCopy.transform.SetParent(transform);
        ZoomAndLeave ourCopyZAL = ourCopy.GetComponent<ZoomAndLeave>();
        Destroy(ourCopyZAL);
        ourCopy.transform.localPosition = new Vector3(-0.01f, -0.01f, 0.2f);
        Renderer copyRenderer = ourCopy.GetComponent<Renderer>();
        copyRenderer.material.color = new Color(0, 0, 0);

        GameObject ourCopy2 = Instantiate(ourCopy);
        ourCopy2.transform.SetParent(ourCopyZAL.transform);
        ourCopy2.transform.localScale = new Vector3(1, 1, 1);
        ourCopy2.transform.localPosition = new Vector3(-0.005f, -0.005f, 0.05f);
        Renderer copyRenderer2 = ourCopy2.GetComponent<Renderer>();
        copyRenderer2.material.color = new Color(1, 1, 1);
        TextMesh copyTextMesh2 = ourCopy2.GetComponent<TextMesh>();
        copyTextMesh2.color = new Color(1, 1, 1);
    }

    // Update is called once per frame
    void Update()
    {
        if (iZoom > 0)
        {
            sizeText += zoomStep;
            iZoom--;
            transform.localScale = new Vector3(sizeText, sizeText, sizeText);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    internal void setPos(float nbOnScreen)
    {
        transform.position = new Vector3(0, 0.9f - myWidth * nbOnScreen, -6);
    }
}
