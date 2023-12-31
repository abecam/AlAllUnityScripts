using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkFromString : MonoBehaviour
{  
    string codeInMorse = ".. --. --.. - -.. ..- .---";

    int placeInString = 0;

    int timeToWait = 0;

    private const int MaxTimeToWaitInBetween = 20;
    int timeToWaitInBetween = MaxTimeToWaitInBetween;

    char currentState;

    Renderer ourRenderer;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timeToWait--;

        if (timeToWait <= 0)
        {
            if (timeToWaitInBetween < 0)
            { 
                updateState();
            }
            else
            {
                if (timeToWaitInBetween == MaxTimeToWaitInBetween)
                {
                    ourRenderer.material.color = new Color(0, 0, 0);
                }
                timeToWaitInBetween--;
            }
        }
    }

    private void OnEnable()
    {
        timeToWait = 0;

        placeInString = 0;

        ourRenderer = GetComponent<Renderer>();
        updateState();
    }

    private void updateState()
    {
        timeToWaitInBetween = MaxTimeToWaitInBetween;
        currentState = codeInMorse[placeInString++];

        switch (currentState)
        {
            case '.':
                timeToWait = 10;
                ourRenderer.material.color = new Color(1, 1, 1);
                break;
            case '-':
                timeToWait = 60;
                ourRenderer.material.color = new Color(1, 1, 1);
                break;
            case ' ':
                timeToWait = 60;
                ourRenderer.material.color = new Color(0.3f, 0.3f, 0.3f);
                break;
        }

        if (placeInString == codeInMorse.Length)
        {
            gameObject.SetActive(false);
        }
    }
}
