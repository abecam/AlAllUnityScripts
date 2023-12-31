using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class PlayerScriptFPWithGround : PlayScript
{
    private const int DIVIDER_CAMERA_SPEED = 4; // By how much we devide the cursor deplacement to move the camera. Higher means slower camera.
    private const float REDUCTION_OF_SPEED = 0.8f; // By how much we multiply the speed at each frame. Should always be below 1 :), and smaller mean faster deceleration.

    private float minImageX = -10;
    private float maxImageX = 10;
    private float minImageY = -10;
    private float maxImageY = 10;
    private float minImageZ = -10;
    private float maxImageZ = 5;

    private int coolDown = 0; // Do not immediately do something else after some action

    // Use this for initialization
    void Start()
    {
        Application.targetFrameRate = 60;
    }

    private bool dragMode = false;
    private bool rotateMode = false;
    private bool zoomMode = false;
    private Vector3 totalDrag;
    private Vector3 dragOldPosition;

    private Vector2 moveByKeyboard = new Vector2();
    private Vector2 upKeyboard = new Vector2(0, 0.05f);
    private Vector2 downKeyboard = new Vector2(0, -0.05f);
    private Vector2 leftKeyboard = new Vector2(-0.05f, 0);
    private Vector2 rightKeyboard = new Vector2(0.05f, 0);

    // Update is called once per frame
    void Update()
    {
        /*
        if (gameIsInPause)
        {
            // Nothing to do
            return;
        }
        */

        if (shakeCamera > 0)
        {
            shakeCamera--;
            float xRand = 0.2f * Random.value - 0.1f;
            float yRand = 0.2f * Random.value - 0.1f;
            transform.position = new Vector3(xRand, yRand, -7);
        }
        else if (shakeCamera == 0)
        {
            transform.position = new Vector3(0, 0, -7);
            shakeCamera = -1;
        }

        if (coolDown != 0)
        {

            coolDown--;
        }

        {
            Vector3 touchPos = Mouse.current.position.ReadValue();

            // use the input stuff
            if (Mouse.current.leftButton.isPressed)
            {
                moveCamera(touchPos);
            }
            else
            {
                dragMode = false;
            }

            if (Mouse.current.rightButton.isPressed)
            {
                rotateCamera(touchPos);
            }
            else
            {
                rotateMode = false;
            }


            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed)
            {
                moveByKeyboard += upKeyboard;
            }
            if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed)
            {
                moveByKeyboard += downKeyboard;
            }
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
            {
                moveByKeyboard += leftKeyboard;
            }
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
            {
                moveByKeyboard += rightKeyboard;
            }

            if (Mouse.current.scroll.ReadValue().y > 0f) // forward
            {
                zoomWheel(true);
            }
            else if (Mouse.current.scroll.ReadValue().y < 0f) // backwards
            {
                zoomWheel(false);
            }
        }

        // Move the camera if needed
        updateCameraPos();
    }

    private static readonly float normalWheelValue = 1.1f;
    float currentWheelValue = normalWheelValue;

    private void zoomWheel(bool zoomIn)
    {
        if (zoomIn)
        {
            currentWheelValue += 0.03f;

            if (currentWheelValue > 0.3f)
            {
                currentWheelValue = 0.3f;
            }
        }
        else
        {
            // Zoom in
            currentWheelValue -= 0.03f;

            if (currentWheelValue < -0.3f)
            {
                currentWheelValue = -0.3f;
            }
        }

        cameraSpeed = new Vector3(cameraSpeed.x, currentWheelValue, cameraSpeed.z);

        //Debug.Log("Camera speed is " + cameraSpeed);
    }

    Vector3 cameraSpeed = new Vector3(0, 0, 0);

    private void moveCamera(Vector3 mousePos)
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos + new Vector3(0, 0, 4f));
        Vector3 positionDelta = worldPos - Camera.main.ScreenToWorldPoint(dragOldPosition + new Vector3(0, 0, 4f));

        if (!dragMode)
        {
            totalDrag += positionDelta;
            if (Vector3.Distance(totalDrag, Vector3.zero) > 0)
            {
                dragMode = true;
            }
        }
        else
        {
            //Vector3 cameraPos = Camera.main.transform.position;

            //cameraPos -= positionDelta;
            Vector3 deltaZ = new Vector3(positionDelta.x, positionDelta.z, positionDelta.y);

            cameraSpeed -= deltaZ / DIVIDER_CAMERA_SPEED;

            //Camera.main.transform.position = RestrainCamera(cameraPos);
        }

        dragOldPosition = mousePos;
    }

    private float _sensitivity = 0.3f;
    private Vector3 _mouseReference;
    private Vector3 _mouseOffset;
    private Vector3 _rotation = Vector3.zero;
    //private bool _isRotating;
    private void rotateCamera(Vector3 mousePos)
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);

        if (!rotateMode)
        {
            rotateMode = true;

            _rotation = Vector3.zero;

            _mouseReference = mousePos;
        }
        else
        {
            //Vector3 cameraPos = Camera.main.transform.position;

            //cameraPos -= positionDelta;
            //cameraRotateSpeed -= Vector3.Angle(worldPos, transform.forward);

            //if (cameraRotateSpeed > 0.01f)
            //{
            //    cameraRotateSpeed = 0.01f;
            //}
            //else if (cameraRotateSpeed < -0.01f)
            //{
            //    cameraRotateSpeed = -0.01f;
            //}
            ////Camera.main.transform.position = RestrainCamera(cameraPos);
            //transform.Rotate(Vector3.forward, cameraRotateSpeed);
            _mouseOffset = (mousePos - _mouseReference);

            // apply rotation
            _rotation.z = -(_mouseOffset.x + _mouseOffset.y) * _sensitivity;

            // rotate
            transform.Rotate(_rotation);

            // store mouse
            _mouseReference = mousePos;
        }
    }

    float cameraRotateAcc = 0;
    float cameraRotateSpeed = 0;
    float damping = 0;
    float target = 1;
    float currentAngle = 0;
    Vector3 speedFromKeyboard;

    private void updateCameraPos()
    {
        Vector3 cameraPos = Camera.main.transform.position;
        speedFromKeyboard = new Vector3(moveByKeyboard.x, 0, moveByKeyboard.y);

        cameraPos += cameraSpeed + speedFromKeyboard;

        cameraSpeed = cameraSpeed * REDUCTION_OF_SPEED;
        moveByKeyboard = moveByKeyboard * REDUCTION_OF_SPEED;
        currentWheelValue = currentWheelValue * REDUCTION_OF_SPEED;

        if (cameraSpeed.magnitude < 0)
        {
            cameraSpeed = new Vector3(0, 0, 0);
        }

        Camera.main.transform.position = cameraPos; //  RestrainCamera(cameraPos);
    }

    private Vector3 RestrainCamera(Vector3 cameraPos)
    {
        // Need to check the background image boundaries.

        if (cameraPos.x > maxImageX)
        {
            cameraPos.x = maxImageX;
        }
        else if (cameraPos.x < minImageX)
        {
            cameraPos.x = minImageX;
        }
        if (cameraPos.y > maxImageY)
        {
            cameraPos.y = maxImageY;
            currentWheelValue = 0;
        }
        if (cameraPos.z > maxImageZ)
        {
            cameraPos.z = maxImageZ;
        }
        else if (cameraPos.z < minImageZ)
        {
            cameraPos.z = minImageZ;
        }

        // Limit y following the ground
        float xCurr, yCurr;

        float zOut; // Ignored here

        checkAtWhichLevel(cameraPos.x, 0, cameraPos.z + 4, out xCurr, out yCurr, out zOut);

        if (cameraPos.y < yCurr + 0.8f)
        {
            cameraPos.y = yCurr + 0.8f;
            currentWheelValue = 0;
        }

        return cameraPos;
    }

    private void checkAtWhichLevel(float x, int y, float z, out float xCurr, out float yCurr, out float zOut)
    {
        xCurr = x;
        yCurr = -2; // Should be where the ground is
        zOut = z;
    }
}
