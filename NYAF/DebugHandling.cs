using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DebugHandling : MonoBehaviour
{
    const bool inDebug = true;

    public GameObject debugWindows;
    // Start is called before the first frame update
    void Start()
    {
        debugWindows.SetActive(false);
        if (!inDebug)
        {
            Destroy(this.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.f1Key.wasPressedThisFrame) // forward
        {
            debugWindows.SetActive(true);
        }
        if (Keyboard.current.f4Key.wasPressedThisFrame) // forward
        {
            debugWindows.SetActive(false);
        }
    }
}
