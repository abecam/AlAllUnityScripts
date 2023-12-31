using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class QuitGameFromTutorial : MonoBehaviour
{
    public GameObject quitBox;
    private GameObject quitBoxInstance = null;

    // Use this for initialization
    void Start()
    {
    }

    int coolDown = 0;
    const int maxCoolDown = 20;

    // Update is called once per frame
    void Update()
    {
        if (coolDown == 0 && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (quitBoxInstance == null)
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
