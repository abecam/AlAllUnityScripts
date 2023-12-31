using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RotateCamera : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.rightArrowKey.isPressed)
        {
            transform.Rotate(Vector3.forward, 2);
        }
        if (Keyboard.current.leftArrowKey.isPressed)
        {
            transform.Rotate(Vector3.forward, -2);
        }
        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            transform.rotation = Quaternion.identity;
        }
    }
}
