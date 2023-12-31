using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdaptBJSLandscape : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        shiftAll();

        //backNightRenderers = new List<Renderer>();

        //foreach (Transform childCharacter in transform)
        //{
        //    backNightRenderers.Add(childCharacter.gameObject.GetComponent<Renderer>());
        //    foreach (Transform childChildCharacter in childCharacter)
        //    {
        //        backNightRenderers.Add(childChildCharacter.gameObject.GetComponent<Renderer>());
        //    }
        //}
    }

    float deltaX = 3.412471f; // 3.412501
    //float alpha = 1;
    //private List<Renderer> backNightRenderers;

    // Update is called once per frame
    //void Update()
    //{
    //    if (Input.GetKeyDown("right"))
    //    {
    //        deltaX -= 0.0001f;

    //        Debug.Log("Last value is " + deltaX);
    //        shiftAll();
    //    }
    //    if (Input.GetKeyDown("left"))
    //    {
    //        deltaX += 0.0001f;

    //        Debug.Log("Last value is " + deltaX);
    //        shiftAll();
    //    }
    //    if (Input.GetKeyDown("a"))
    //    {
    //        deltaX -= 0.00001f;

    //        Debug.Log("Last value is " + deltaX);
    //        shiftAll();
    //    }
    //    if (Input.GetKeyDown("d"))
    //    {
    //        deltaX += 0.00001f;

    //        Debug.Log("Last value is " + deltaX);
    //        shiftAll();
    //    }

    //    if (Input.GetKeyDown("up"))
    //    {
    //        alpha -= 0.01f;
    //        if (alpha < 0)
    //        {
    //            alpha = 0;
    //        }

    //        Debug.Log("Alpha is " + alpha);
    //        fadeAll();
    //    }
    //    if (Input.GetKeyDown("down"))
    //    {
    //        alpha += 0.01f;
    //        if (alpha > 1)
    //        {
    //            alpha = 1;
    //        }

    //        Debug.Log("Alpha is " + alpha);
    //        fadeAll();
    //    }
    //}

    //private void fadeAll()
    //{
    //    foreach (Renderer childRenderer in backNightRenderers)
    //    {
    //        childRenderer.material.color = new Color(childRenderer.material.color.r, childRenderer.material.color.g, childRenderer.material.color.b, alpha);
    //    }
    //}

    private void shiftAll()
    {
        float minX = -53.7457f;

        foreach (Transform extra in transform)
        {
            extra.localPosition = new Vector3(minX, extra.localPosition.y, extra.localPosition.z);

            minX += deltaX;
        }
    }
}
