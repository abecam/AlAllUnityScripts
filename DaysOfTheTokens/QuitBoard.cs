﻿using UnityEngine;
using System.Collections;

public class QuitBoard : MonoBehaviour
{
    private GameObject preferences; // To check whether we display the quit dialog or remove the preferences

    public GameObject quitBox;
    private GameObject quitBoxInstance = null;

    // Use this for initialization
    void Start()
    {
        GameObject preferencesParent = GameObject.FindGameObjectWithTag("PrefPanel");
        preferences = preferencesParent.transform.Find("PanelPrefs").gameObject;
    }

    int coolDown = 0;
    const int maxCoolDown = 20;

    // Update is called once per frame
    void Update()
    {
        if (coolDown == 0 && (Input.GetKeyDown("escape") || Input.GetKey(KeyCode.Escape)))
        {
            if (preferences.activeSelf)
            {
                preferences.SetActive(false);
            }
            else if (quitBoxInstance == null)
            {
                quitBoxInstance = Instantiate(quitBox);
            }
            else
            {
                Destroy(quitBoxInstance);
            }
            coolDown = maxCoolDown;
        }
        else
        {
            if (coolDown > 0)
            { 
                coolDown--;
            }
            else
            {
                coolDown = 0; // Stupidly just to be sure.
            }
        }
    }
}