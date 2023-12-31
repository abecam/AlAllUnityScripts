using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeThenFall : MonoBehaviour
{
    public GameObject title;
    private Rigidbody ourRigidBody;
    private Renderer ourRenderer;

    private void Awake()
    {
        ourRigidBody = GetComponent<Rigidbody>();
        ourRigidBody.isKinematic = true;

        ourRenderer = GetComponent<Renderer>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (currentStep < 3)
        {
            fadeText();
        }
        if (currentStep == 3)
        {
            ourRigidBody.isKinematic = false;

            zoomingTime = 0;
            currentStep = 4;
        }
        else if (currentStep == 4)
        {
            waitForFall();
        }
        else if (currentStep == 5)
        {
            elevateTitle();
        }
    }

    int zoomingTime = 0;
    int currentStep = 0;
    public int totalZoomingTime = 60;
    public int dimingTime = 60; // 200 frames

    void fadeText()
    {
        if (currentStep == 0)
        {
            // Zoom the box into view
            if (zoomingTime <= totalZoomingTime)
            {
                float fading = 1 - ((float)(totalZoomingTime - zoomingTime)) / ((float)totalZoomingTime);
                foreach (Transform childIntrobox in title.transform)
                {
                    //Debug.Log("Child: " + childIntrobox);
                    Renderer renderer1 = childIntrobox.GetComponent<Renderer>();
                    if (renderer1 != null)
                    {
                        renderer1.material.color = new Color(renderer1.material.color.r, renderer1.material.color.g, renderer1.material.color.b, fading);
                    }
                }

                // backgroundImage.transform.localScale = new Vector3(0.02f + currentZoomForIntroBox, 0.02f + currentZoomForIntroBox, 1);

                //introductionBox.transform.localScale = new Vector3(0.8f+currentZoomForIntroBox, 0.8f+currentZoomForIntroBox, 0.8f+currentZoomForIntroBox);
                zoomingTime++;
            }
            else
            {
                currentStep = 1;
                zoomingTime = 0;
            }
        }
        else if (currentStep == 1)
        {
            if (zoomingTime <= dimingTime - 10)
            {
                // Fading out of the box
                float fading = ((float)(dimingTime - zoomingTime)) / ((float)dimingTime);

                foreach (Transform childIntrobox in title.transform)
                {
                    //Debug.Log("Child: " + childIntrobox);
                    Renderer renderer1 = childIntrobox.GetComponent<Renderer>();
                    if (renderer1 != null)
                    {
                        renderer1.material.color = new Color(childIntrobox.GetComponent<Renderer>().material.color.r, childIntrobox.GetComponent<Renderer>().material.color.g, childIntrobox.GetComponent<Renderer>().material.color.b, fading);
                    }
                }

                zoomingTime++;
            }
            else
            {
                currentStep = 3;
            }
        }
    }

    private int fallingTime = 120;
    private int gettingUpTime = 120;

    private void waitForFall()
    {
        if (zoomingTime <= fallingTime)
        {
            // Fading out of the box
            zoomingTime++;
        }
        else
        {
            zoomingTime = 0;
            currentStep = 5;
        }
    }

    private void elevateTitle()
    {
        if (zoomingTime <= fallingTime)
        {
            ourRigidBody.AddForceAtPosition(20*Vector3.up, transform.position);

            zoomingTime++;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
